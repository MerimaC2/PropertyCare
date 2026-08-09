import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared-module';
import { TechnicianRoutingModule } from './technician-routing-module';
import { TechnicianLayoutComponent } from './layout/technician-layout.component';
import { InterventionsComponent } from './interventions/interventions.component';
import { WorkLogDialogComponent } from './interventions/work-log-dialog/work-log-dialog.component';

@NgModule({
  declarations: [TechnicianLayoutComponent, InterventionsComponent, WorkLogDialogComponent],
  imports: [SharedModule, TechnicianRoutingModule]
})
export class TechnicianModule {}
