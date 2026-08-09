import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { APP_ROLES } from './api-services/auth/auth-api.model';
import { roleGuard } from './core/guards/role.guard';

// Feature modules are lazy-loaded; protected areas are guarded by role.
const routes: Routes = [
  {
    path: '',
    loadChildren: () => import('./modules/public/public-module').then(m => m.PublicModule)
  },
  {
    path: 'reporter',
    canActivate: [roleGuard],
    data: { roles: [APP_ROLES.reporter] },
    loadChildren: () => import('./modules/reporter/reporter-module').then(m => m.ReporterModule)
  },
  {
    path: 'admin',
    canActivate: [roleGuard],
    data: { roles: [APP_ROLES.administrator] },
    loadChildren: () => import('./modules/admin/admin-module').then(m => m.AdminModule)
  },
  {
    path: 'technician',
    canActivate: [roleGuard],
    data: { roles: [APP_ROLES.technician] },
    loadChildren: () => import('./modules/technician/technician-module').then(m => m.TechnicianModule)
  },
  { path: '**', redirectTo: '' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
