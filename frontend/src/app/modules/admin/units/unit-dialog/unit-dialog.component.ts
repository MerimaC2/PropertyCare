import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { UnitDto } from '../../../../api-services/units/units-api.models';

export interface UnitDialogData {
  unit: UnitDto | null;
}

export interface UnitDialogResult {
  label: string;
}

/** Create/edit form for a unit (label only). */
@Component({
  selector: 'app-unit-dialog',
  templateUrl: './unit-dialog.component.html',
  styleUrls: ['./unit-dialog.component.scss'],
  standalone: false
})
export class UnitDialogComponent {
  form: FormGroup;

  constructor(
    public dialogRef: MatDialogRef<UnitDialogComponent, UnitDialogResult>,
    @Inject(MAT_DIALOG_DATA) public data: UnitDialogData,
    formBuilder: FormBuilder
  ) {
    this.form = formBuilder.group({
      label: [data.unit?.label ?? '', [Validators.required, Validators.maxLength(120)]]
    });
  }

  get isEdit(): boolean {
    return this.data.unit !== null;
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.dialogRef.close({ label: this.form.value.label });
  }
}
