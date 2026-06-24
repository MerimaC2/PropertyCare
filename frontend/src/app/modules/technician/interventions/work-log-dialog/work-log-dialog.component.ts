import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { AddWorkLogCommand } from '../../../../api-services/interventions/interventions-api.models';

export interface WorkLogDialogData {
  requestTitle: string;
}

/** Form to log work (note + minutes) on a work order. */
@Component({
  selector: 'app-work-log-dialog',
  templateUrl: './work-log-dialog.component.html',
  styleUrls: ['./work-log-dialog.component.scss'],
  standalone: false
})
export class WorkLogDialogComponent {
  form: FormGroup;

  constructor(
    public dialogRef: MatDialogRef<WorkLogDialogComponent, AddWorkLogCommand>,
    @Inject(MAT_DIALOG_DATA) public data: WorkLogDialogData,
    formBuilder: FormBuilder
  ) {
    this.form = formBuilder.group({
      note: ['', [Validators.required, Validators.maxLength(1000)]],
      minutesSpent: [null, [Validators.required, Validators.min(1)]]
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
    const v = this.form.value;
    this.dialogRef.close({ note: v.note, minutesSpent: v.minutesSpent });
  }
}
