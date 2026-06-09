import { Component } from '@angular/core';
import { AuthFacadeService, CurrentUser } from '../../../core/services/auth-facade.service';

/** Toolbar + router outlet shell for the reporter area. */
@Component({
  selector: 'app-reporter-layout',
  templateUrl: './reporter-layout.component.html',
  styleUrls: ['./reporter-layout.component.scss'],
  standalone: false
})
export class ReporterLayoutComponent {
  constructor(private authFacade: AuthFacadeService) {}

  get user(): CurrentUser | null {
    return this.authFacade.getCurrentUser();
  }

  onLogout(): void {
    this.authFacade.logout();
  }
}
