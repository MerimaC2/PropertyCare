import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ReporterLayoutComponent } from './layout/reporter-layout.component';
import { MyRequestsComponent } from './my-requests/my-requests.component';
import { RequestCreateComponent } from './request-create/request-create.component';
import { NotificationsComponent } from './notifications/notifications.component';

const routes: Routes = [
  {
    path: '',
    component: ReporterLayoutComponent,
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'my-requests' },
      { path: 'my-requests', component: MyRequestsComponent },
      { path: 'new-request', component: RequestCreateComponent },
      { path: 'notifications', component: NotificationsComponent }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ReporterRoutingModule {}
