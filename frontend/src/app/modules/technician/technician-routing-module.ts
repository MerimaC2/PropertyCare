import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TechnicianLayoutComponent } from './layout/technician-layout.component';
import { InterventionsComponent } from './interventions/interventions.component';
import { NotificationsComponent } from '../../shared/components/notifications/notifications.component';

const routes: Routes = [
  {
    path: '',
    component: TechnicianLayoutComponent,
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'interventions' },
      { path: 'interventions', component: InterventionsComponent },
      { path: 'notifications', component: NotificationsComponent }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TechnicianRoutingModule {}
