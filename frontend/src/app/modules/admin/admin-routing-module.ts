import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AdminLayoutComponent } from './layout/admin-layout.component';
import { TriageComponent } from './triage/triage.component';
import { BuildingsComponent } from './buildings/buildings.component';
import { BuildingsMapComponent } from './map/buildings-map.component';

const routes: Routes = [
  {
    path: '',
    component: AdminLayoutComponent,
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'triage' },
      { path: 'triage', component: TriageComponent },
      { path: 'buildings', component: BuildingsComponent },
      { path: 'map', component: BuildingsMapComponent }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AdminRoutingModule {}
