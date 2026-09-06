import { ChangeDetectorRef, Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { forkJoin, of, switchMap } from 'rxjs';
import { MaintenanceRequestsApiService } from '../../../api-services/maintenance-requests/maintenance-requests-api.service';
import { RequestImageDto } from '../../../api-services/maintenance-requests/maintenance-requests-api.models';

export interface RequestPhotosDialogData {
  requestId: number;
  requestTitle: string;
}

interface LoadedPhoto {
  image: RequestImageDto;
  objectUrl: string;
}

/**
 * Shows the photos attached to a maintenance request. Attachments are served by an authorized API
 * action rather than as static files, so each one is fetched as a blob and turned into an object
 * URL before it can be rendered or saved.
 */
@Component({
  selector: 'app-request-photos-dialog',
  templateUrl: './request-photos-dialog.component.html',
  styleUrls: ['./request-photos-dialog.component.scss'],
  standalone: false
})
export class RequestPhotosDialogComponent implements OnInit, OnDestroy {
  photos: LoadedPhoto[] = [];
  isLoading = false;
  loadError: string | null = null;

  constructor(
    public dialogRef: MatDialogRef<RequestPhotosDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: RequestPhotosDialogData,
    private requestsApi: MaintenanceRequestsApiService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.isLoading = true;

    this.requestsApi
      .listImages(this.data.requestId)
      .pipe(
        switchMap(images =>
          images.length === 0
            ? of([] as LoadedPhoto[])
            : forkJoin(
                images.map(image =>
                  this.requestsApi
                    .getImageContent(this.data.requestId, image.id)
                    .pipe(
                      switchMap(blob =>
                        of({ image, objectUrl: URL.createObjectURL(blob) } as LoadedPhoto)
                      )
                    )
                )
              )
        )
      )
      .subscribe({
        next: photos => {
          this.photos = photos;
          this.isLoading = false;
          this.cdr.markForCheck();
        },
        error: () => {
          this.isLoading = false;
          this.loadError = 'Failed to load the photos of this request.';
          this.cdr.markForCheck();
        }
      });
  }

  /** Saves one photo under its original file name. */
  download(photo: LoadedPhoto): void {
    const link = document.createElement('a');
    link.href = photo.objectUrl;
    link.download = photo.image.fileName;
    link.click();
  }

  sizeInKb(sizeBytes: number): number {
    return Math.max(1, Math.round(sizeBytes / 1024));
  }

  ngOnDestroy(): void {
    this.photos.forEach(photo => URL.revokeObjectURL(photo.objectUrl));
  }
}
