import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import {
  AssetTypeDto,
  SaveAssetTypeCommand
} from '../../../../api-services/asset-types/asset-types-api.models';

export interface AssetTypeDialogData {
  assetType: AssetTypeDto | null;
}

/** Create/edit form for an asset type (name + optional default SLA hours). */
@Component({
  selector: 'app-asset-type-dialog',
  templateUrl: './asset-type-dialog.component.html',
  styleUrls: ['./asset-type-dialog.component.scss'],
  standalone: false
})
export class AssetTypeDialogComponent {
  form: FormGroup;

  constructor(
    public dialogRef: MatDialogRef<AssetTypeDialogComponent, SaveAssetTypeCommand>,
    @Inject(MAT_DIALOG_DATA) public data: AssetTypeDialogData,
    formBuilder: FormBuilder
  ) {
    const t = data.assetType;
    this.form = formBuilder.group({
      name: [t?.name ?? '', [Validators.required, Validators.maxLength(120)]],
      defaultSlaHours: [t?.defaultSlaHours ?? null, [Validators.min(1)]]
    });
  }

  get isEdit(): boolean {
    return this.data.assetType !== null;
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
    this.dialogRef.close({ name: v.name, defaultSlaHours: v.defaultSlaHours ?? null });
  }
}
