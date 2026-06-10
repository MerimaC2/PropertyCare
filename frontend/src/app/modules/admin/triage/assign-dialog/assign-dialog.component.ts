import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { LookupItemDto } from '../../../../api-services/lookups/lookups-api.models';
import { TriageRequestDto } from '../../../../api-services/maintenance-requests/maintenance-requests-api.models';

export interface AssignDialogData {
  request: TriageRequestDto;
  technicians: LookupItemDto[];
}

export interface AssignDialogResult {
  technicianId: number;
  technicianName: string;
  note: string | null;
}

/** Dialog for picking the technician (and an optional note) for a request. */
@Component({
  selector: 'app-assign-dialog',
  templateUrl: './assign-dialog.component.html',
  styleUrls: ['./assign-dialog.component.scss'],
  standalone: false
})
export class AssignDialogComponent {
  form: FormGroup;

  constructor(
    public dialogRef: MatDialogRef<AssignDialogComponent, AssignDialogResult>,
    @Inject(MAT_DIALOG_DATA) public data: AssignDialogData,
    formBuilder: FormBuilder
  ) {
    this.form = formBuilder.group({
      technicianId: [null, Validators.required],
      note: ['', Validators.maxLength(1000)]
    });
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const technicianId = this.form.value.technicianId as number;
    const technician = this.data.technicians.find(t => t.id === technicianId);

    this.dialogRef.close({
      technicianId,
      technicianName: technician?.name ?? '',
      note: this.form.value.note || null
    });
  }
}
