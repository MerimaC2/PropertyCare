import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
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
export class RequestCreateComponent implements OnInit {
  form: FormGroup;
  lookups: RequestFormLookupsDto | null = null;
  isLoading = false;
  apiError: string | null = null;

  constructor(
    private formBuilder: FormBuilder,
    private lookupsApi: LookupsApiService,
    private requestsApi: MaintenanceRequestsApiService,
    private toaster: ToasterService,
    private router: Router
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
      next: lookups => (this.lookups = lookups),
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

  onSubmit(): void {
    this.apiError = null;

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    this.requestsApi.create(this.form.value).subscribe({
      next: () => {
        this.isLoading = false;
        this.toaster.success('Your request was submitted.');
        this.router.navigate(['/reporter/my-requests']);
      },
      error: (error: HttpErrorResponse) => {
        this.isLoading = false;
        this.apiError = error.error?.message ?? 'Failed to submit the request.';
        const fieldErrors = error.error?.errors as { field: string; message: string }[] | undefined;
        if (fieldErrors?.length) {
          this.apiError = fieldErrors.map(e => e.message).join(' ');
        }
      }
    });
  }
}
