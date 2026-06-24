import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared-module';
import { AdminRoutingModule } from './admin-routing-module';
import { AdminLayoutComponent } from './layout/admin-layout.component';
import { TriageComponent } from './triage/triage.component';
import { AssignDialogComponent } from './triage/assign-dialog/assign-dialog.component';
import { BuildingsComponent } from './buildings/buildings.component';
import { BuildingDialogComponent } from './buildings/building-dialog/building-dialog.component';
import { BuildingsMapComponent } from './map/buildings-map.component';
import { DashboardComponent } from './dashboard/dashboard.component';

@NgModule({
  declarations: [
    AdminLayoutComponent,
    TriageComponent,
    AssignDialogComponent,
    BuildingsComponent,
    BuildingDialogComponent,
    BuildingsMapComponent,
    DashboardComponent
  ],
  imports: [SharedModule, AdminRoutingModule]
})
export class AdminModule {}
