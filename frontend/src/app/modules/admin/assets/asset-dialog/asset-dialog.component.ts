import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { AssetDto } from '../../../../api-services/assets/assets-api.models';
import { AssetTypeDto } from '../../../../api-services/asset-types/asset-types-api.models';

export interface AssetDialogData {
  asset: AssetDto | null;
  assetTypes: AssetTypeDto[];
}

export interface AssetDialogResult {
  name: string;
  assetTypeId: number;
}

/** Create/edit form for an asset (name + asset type). */
@Component({
  selector: 'app-asset-dialog',
  templateUrl: './asset-dialog.component.html',
  styleUrls: ['./asset-dialog.component.scss'],
  standalone: false
})
export class AssetDialogComponent {
  form: FormGroup;

  constructor(
    public dialogRef: MatDialogRef<AssetDialogComponent, AssetDialogResult>,
    @Inject(MAT_DIALOG_DATA) public data: AssetDialogData,
    formBuilder: FormBuilder
  ) {
    const a = data.asset;
    this.form = formBuilder.group({
      name: [a?.name ?? '', [Validators.required, Validators.maxLength(150)]],
      assetTypeId: [a?.assetTypeId ?? null, Validators.required]
    });
  }

  get isEdit(): boolean {
    return this.data.asset !== null;
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
    this.dialogRef.close({ name: v.name, assetTypeId: v.assetTypeId });
  }
}
