import { ChangeDetectorRef, Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ImageCroppedEvent } from 'ngx-image-cropper';

export interface ImageCropDialogData {
  file: File;
}

/** Lets the reporter crop a picked photo before it is queued for upload. */
@Component({
  selector: 'app-image-crop-dialog',
  templateUrl: './image-crop-dialog.component.html',
  styleUrls: ['./image-crop-dialog.component.scss'],
  standalone: false
})
export class ImageCropDialogComponent {
  hasCrop = false;
  private croppedBlob: Blob | null = null;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: ImageCropDialogData,
    private dialogRef: MatDialogRef<ImageCropDialogComponent, Blob>,
    private cdr: ChangeDetectorRef
  ) {}

  onImageCropped(event: ImageCroppedEvent): void {
    this.croppedBlob = event.blob ?? null;
    this.hasCrop = this.croppedBlob !== null;
    this.cdr.markForCheck();
  }

  confirm(): void {
    if (this.croppedBlob) {
      this.dialogRef.close(this.croppedBlob);
    }
  }

  cancel(): void {
    this.dialogRef.close();
  }
}
