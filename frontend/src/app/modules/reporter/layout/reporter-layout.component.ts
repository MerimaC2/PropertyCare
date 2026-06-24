import { Component, OnInit } from '@angular/core';
import { AuthFacadeService, CurrentUser } from '../../../core/services/auth-facade.service';
import { ThemeService } from '../../../core/services/theme.service';
import { NotificationsStateService } from '../notifications/notifications-state.service';

/** Toolbar + router outlet shell for the reporter area. */
@Component({
  selector: 'app-reporter-layout',
  templateUrl: './reporter-layout.component.html',
  styleUrls: ['./reporter-layout.component.scss'],
  standalone: false
})
export class ReporterLayoutComponent implements OnInit {
  constructor(
    private authFacade: AuthFacadeService,
    public notifications: NotificationsStateService,
    public theme: ThemeService
  ) {}

  ngOnInit(): void {
    this.notifications.refresh();
  }

  get user(): CurrentUser | null {
    return this.authFacade.getCurrentUser();
  }

  onLogout(): void {
    this.authFacade.logout();
  }
}
