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
import { UnitsComponent } from './units/units.component';
import { UnitDialogComponent } from './units/unit-dialog/unit-dialog.component';
import { AssetTypesComponent } from './asset-types/asset-types.component';
import { AssetTypeDialogComponent } from './asset-types/asset-type-dialog/asset-type-dialog.component';
import { AssetsComponent } from './assets/assets.component';
import { AssetDialogComponent } from './assets/asset-dialog/asset-dialog.component';

@NgModule({
  declarations: [
    AdminLayoutComponent,
    TriageComponent,
    AssignDialogComponent,
    BuildingsComponent,
    BuildingDialogComponent,
    BuildingsMapComponent,
    DashboardComponent,
    UnitsComponent,
    UnitDialogComponent,
    AssetTypesComponent,
    AssetTypeDialogComponent,
    AssetsComponent,
    AssetDialogComponent
  ],
  imports: [SharedModule, AdminRoutingModule]
})
export class AdminModule {}
