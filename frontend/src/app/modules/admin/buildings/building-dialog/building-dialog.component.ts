import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { LookupItemDto } from '../../../../api-services/lookups/lookups-api.models';
import {
  BuildingDto,
  SaveBuildingCommand
} from '../../../../api-services/buildings/buildings-api.models';

export interface BuildingDialogData {
  building: BuildingDto | null;
  buildingTypes: LookupItemDto[];
}

/** Create/edit form for a building, including its optional map coordinates. */
@Component({
  selector: 'app-building-dialog',
  templateUrl: './building-dialog.component.html',
  styleUrls: ['./building-dialog.component.scss'],
  standalone: false
})
export class BuildingDialogComponent {
  form: FormGroup;

  constructor(
    public dialogRef: MatDialogRef<BuildingDialogComponent, SaveBuildingCommand>,
    @Inject(MAT_DIALOG_DATA) public data: BuildingDialogData,
    formBuilder: FormBuilder
  ) {
    const b = data.building;
    this.form = formBuilder.group({
      name: [b?.name ?? '', [Validators.required, Validators.maxLength(120)]],
      buildingTypeId: [b?.buildingTypeId ?? null, Validators.required],
      address: [b?.address ?? '', Validators.maxLength(200)],
      latitude: [b?.latitude ?? null, [Validators.min(-90), Validators.max(90)]],
      longitude: [b?.longitude ?? null, [Validators.min(-180), Validators.max(180)]]
    });
  }

  get isEdit(): boolean {
    return this.data.building !== null;
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const v = this.form.value;
    this.dialogRef.close({
      name: v.name,
      address: v.address || null,
      buildingTypeId: v.buildingTypeId,
      latitude: v.latitude ?? null,
      longitude: v.longitude ?? null
    });
  }
}
