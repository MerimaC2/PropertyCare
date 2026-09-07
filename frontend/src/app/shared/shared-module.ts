import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MaterialModule } from './material-module';
import { ConfirmDialogComponent } from './components/confirm-dialog/confirm-dialog.component';
import { RequestPhotosDialogComponent } from './components/request-photos-dialog/request-photos-dialog.component';

/** Common building blocks shared by all feature modules. */
@NgModule({
  declarations: [ConfirmDialogComponent, RequestPhotosDialogComponent],
  imports: [CommonModule, FormsModule, ReactiveFormsModule, MaterialModule],
  exports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MaterialModule,
    ConfirmDialogComponent,
    RequestPhotosDialogComponent
  ]
})
export class SharedModule {}
