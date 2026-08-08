import { NgModule } from '@angular/core';
import { ImageCropperComponent } from 'ngx-image-cropper';
import { SharedModule } from '../../shared/shared-module';
import { ReporterRoutingModule } from './reporter-routing-module';
import { ReporterLayoutComponent } from './layout/reporter-layout.component';
import { MyRequestsComponent } from './my-requests/my-requests.component';
import { RequestCreateComponent } from './request-create/request-create.component';
import { ImageCropDialogComponent } from './request-create/image-crop-dialog/image-crop-dialog.component';
import { NotificationsComponent } from './notifications/notifications.component';

@NgModule({
  declarations: [
    ReporterLayoutComponent,
    MyRequestsComponent,
    RequestCreateComponent,
    ImageCropDialogComponent,
    NotificationsComponent
  ],
  imports: [SharedModule, ReporterRoutingModule, ImageCropperComponent]
})
export class ReporterModule {}
