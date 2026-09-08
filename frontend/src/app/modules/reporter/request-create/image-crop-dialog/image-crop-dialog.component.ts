import { ChangeDetectorRef, Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ImageCroppedEvent } from 'ngx-image-cropper';

export interface ImageCropDialogData {
  file: File;
}

/**
 * A cropped image, or the marker that the browser could not decode the picked file. The marker keeps
 * a failed load apart from a plain cancel, which closes the dialog with no result at all.
 */
export type ImageCropDialogResult = Blob | 'load-failed';

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
    private dialogRef: MatDialogRef<ImageCropDialogComponent, ImageCropDialogResult>,
    private cdr: ChangeDetectorRef
  ) {}

  onImageCropped(event: ImageCroppedEvent): void {
    this.croppedBlob = event.blob ?? null;
    this.hasCrop = this.croppedBlob !== null;
    this.cdr.markForCheck();
  }

  /**
   * The cropper reports this when the browser cannot decode the file, which happens for anything
   * whose content does not match its extension. Without it the dialog would just sit there empty.
   */
  onLoadFailed(): void {
    this.dialogRef.close('load-failed');
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
