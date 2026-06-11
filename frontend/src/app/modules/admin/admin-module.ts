import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared-module';
import { AdminRoutingModule } from './admin-routing-module';
import { AdminLayoutComponent } from './layout/admin-layout.component';
import { TriageComponent } from './triage/triage.component';
import { AssignDialogComponent } from './triage/assign-dialog/assign-dialog.component';

@NgModule({
  declarations: [AdminLayoutComponent, TriageComponent, AssignDialogComponent],
  imports: [SharedModule, AdminRoutingModule]
})
export class AdminModule {}
