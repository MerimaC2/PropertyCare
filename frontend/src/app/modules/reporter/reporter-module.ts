import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared-module';
import { ReporterRoutingModule } from './reporter-routing-module';
import { ReporterLayoutComponent } from './layout/reporter-layout.component';
import { MyRequestsComponent } from './my-requests/my-requests.component';
import { RequestCreateComponent } from './request-create/request-create.component';

@NgModule({
  declarations: [ReporterLayoutComponent, MyRequestsComponent, RequestCreateComponent],
  imports: [SharedModule, ReporterRoutingModule]
})
export class ReporterModule {}
