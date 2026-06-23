import { ChangeDetectorRef, Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { HttpErrorResponse, HttpEventType } from '@angular/common/http';
import { MatDialog } from '@angular/material/dialog';
import { concatMap, from, last, tap } from 'rxjs';
import { ImageCropDialogComponent } from './image-crop-dialog/image-crop-dialog.component';
import { LookupsApiService } from '../../../api-services/lookups/lookups-api.service';
import {
  AssetLookupDto,
  RequestFormLookupsDto,
  UnitLookupDto
} from '../../../api-services/lookups/lookups-api.models';
import { MaintenanceRequestsApiService } from '../../../api-services/maintenance-requests/maintenance-requests-api.service';
import { ToasterService } from '../../../core/services/toaster.service';

/** Create a new fault report: description, priority, building, unit and asset. */
@Component({
  selector: 'app-request-create',
  templateUrl: './request-create.component.html',
  styleUrls: ['./request-create.component.scss'],
  standalone: false
})
export class RequestCreateComponent implements OnInit, OnDestroy {
  form: FormGroup;
  lookups: RequestFormLookupsDto | null = null;
  isLoading = false;
  apiError: string | null = null;

  /** Photos selected for upload, with a local preview URL each. */
  photos: { file: File; url: string }[] = [];
  isUploading = false;
  uploadProgress = 0;

  private readonly allowedTypes = ['image/jpeg', 'image/png', 'image/webp'];
  private readonly maxFileSizeBytes = 5 * 1024 * 1024; // 5 MB, mirrors the backend

  constructor(
    private formBuilder: FormBuilder,
    private lookupsApi: LookupsApiService,
    private requestsApi: MaintenanceRequestsApiService,
    private toaster: ToasterService,
    private router: Router,
    private dialog: MatDialog,
    private cdr: ChangeDetectorRef
  ) {
    // Frontend validation mirrors the backend CreateMaintenanceRequestCommandValidator.
    this.form = this.formBuilder.group({
      title: ['', [Validators.required, Validators.minLength(5), Validators.maxLength(150)]],
      description: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(2000)]],
      buildingId: [null, Validators.required],
      unitId: [null],
      assetId: [null],
      priorityId: [null, Validators.required]
    });
  }

  ngOnInit(): void {
    this.lookupsApi.getRequestFormLookups().subscribe({
      next: lookups => {
        this.lookups = lookups;
        this.cdr.markForCheck();
      },
      error: () => this.toaster.error('Failed to load form data.')
    });

    // Cascading dropdowns: changing the building resets unit and asset,
    // changing the unit resets the asset.
    this.form.get('buildingId')!.valueChanges.subscribe(() => {
      this.form.patchValue({ unitId: null, assetId: null }, { emitEvent: false });
    });
    this.form.get('unitId')!.valueChanges.subscribe(() => {
      this.form.patchValue({ assetId: null }, { emitEvent: false });
    });
  }

  get unitsForBuilding(): UnitLookupDto[] {
    const buildingId = this.form.value.buildingId;
    return this.lookups?.units.filter(u => u.buildingId === buildingId) ?? [];
  }

  get assetsForUnit(): AssetLookupDto[] {
    const unitId = this.form.value.unitId;
    return this.lookups?.assets.filter(a => a.unitId === unitId) ?? [];
  }

  /** Validates a picked image, lets the user crop it, then queues the cropped result with a preview. */
  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    input.value = ''; // reset so the same file can be picked again

    if (!file) {
      return;
    }
    if (!this.allowedTypes.includes(file.type)) {
      this.toaster.error(`${file.name}: only JPEG, PNG or WebP images are allowed.`);
      return;
    }
    if (file.size > this.maxFileSizeBytes) {
      this.toaster.error(`${file.name}: image must be 5 MB or smaller.`);
      return;
    }

    this.dialog
      .open(ImageCropDialogComponent, { data: { file }, width: '720px', maxWidth: '92vw' })
      .afterClosed()
      .subscribe((blob?: Blob) => {
        if (!blob) {
          return;
        }
        const cropped = new File([blob], this.toPngName(file.name), { type: 'image/png' });
        if (cropped.size > this.maxFileSizeBytes) {
          this.toaster.error('The cropped image is larger than 5 MB.');
          return;
        }
        this.photos.push({ file: cropped, url: URL.createObjectURL(cropped) });
        this.cdr.markForCheck();
      });
  }

  private toPngName(name: string): string {
    const dot = name.lastIndexOf('.');
    const base = dot > 0 ? name.substring(0, dot) : name;
    return `${base}.png`;
  }

  removePhoto(index: number): void {
    URL.revokeObjectURL(this.photos[index].url);
    this.photos.splice(index, 1);
    this.cdr.markForCheck();
  }

  onSubmit(): void {
    this.apiError = null;

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    this.requestsApi.create(this.form.value).subscribe({
      next: requestId => {
        if (this.photos.length === 0) {
          this.finishSuccess();
          return;
        }
        this.uploadPhotos(requestId);
      },
      error: (error: HttpErrorResponse) => {
        this.isLoading = false;
        this.apiError = error.error?.message ?? 'Failed to submit the request.';
        const fieldErrors = error.error?.errors as { field: string; message: string }[] | undefined;
        if (fieldErrors?.length) {
          this.apiError = fieldErrors.map(e => e.message).join(' ');
        }
        this.cdr.markForCheck();
      }
    });
  }

  /** Uploads queued photos one by one, tracking overall progress across all files. */
  private uploadPhotos(requestId: number): void {
    this.isUploading = true;
    this.uploadProgress = 0;
    const total = this.photos.length;
    let completed = 0;

    from(this.photos)
      .pipe(
        concatMap(photo =>
          this.requestsApi.uploadImage(requestId, photo.file).pipe(
            tap(httpEvent => {
              if (httpEvent.type === HttpEventType.UploadProgress && httpEvent.total) {
                const fileFraction = httpEvent.loaded / httpEvent.total;
                this.uploadProgress = Math.round(((completed + fileFraction) / total) * 100);
                this.cdr.markForCheck();
              }
            }),
            last()
          )
        )
      )
      .subscribe({
        next: () => {
          completed++;
          this.uploadProgress = Math.round((completed / total) * 100);
          this.cdr.markForCheck();
        },
        error: (error: HttpErrorResponse) => {
          this.isLoading = false;
          this.isUploading = false;
          this.apiError =
            error.error?.message ?? 'The request was created, but some photos could not be uploaded.';
          this.toaster.error('Some photos could not be uploaded.');
          this.cdr.markForCheck();
        },
        complete: () => this.finishSuccess()
      });
  }

  private finishSuccess(): void {
    this.isLoading = false;
    this.isUploading = false;
    this.clearPhotos();
    this.toaster.success('Your request was submitted.');
    this.router.navigate(['/reporter/my-requests']);
  }

  private clearPhotos(): void {
    this.photos.forEach(photo => URL.revokeObjectURL(photo.url));
    this.photos = [];
  }

  ngOnDestroy(): void {
    this.clearPhotos();
  }
}
