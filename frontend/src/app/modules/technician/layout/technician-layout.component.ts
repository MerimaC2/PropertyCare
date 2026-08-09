import { Component } from '@angular/core';
import { AuthFacadeService, CurrentUser } from '../../../core/services/auth-facade.service';
import { ThemeService } from '../../../core/services/theme.service';
import { LayoutService } from '../../../core/services/layout.service';

/** Toolbar + router outlet shell for the technician area. */
@Component({
  selector: 'app-technician-layout',
  templateUrl: './technician-layout.component.html',
  styleUrls: ['./technician-layout.component.scss'],
  standalone: false
})
export class TechnicianLayoutComponent {
  constructor(
    private authFacade: AuthFacadeService,
    public theme: ThemeService,
    public layout: LayoutService
  ) {}

  get user(): CurrentUser | null {
    return this.authFacade.getCurrentUser();
  }

  onLogout(): void {
    this.authFacade.logout();
  }
}
