import { Injectable, signal } from '@angular/core';
import { NotificationsApiService } from '../../../api-services/notifications/notifications-api.service';

/** Shared unread-count state so the toolbar badge and the notification center stay in sync. */
@Injectable({ providedIn: 'root' })
export class NotificationsStateService {
  /** Signal read in the toolbar template; updates the badge reactively (zoneless). */
  readonly unreadCount = signal(0);

  constructor(private api: NotificationsApiService) {}

  refresh(): void {
    this.api.getUnreadCount().subscribe({
      next: count => this.unreadCount.set(count),
      error: () => {}
    });
  }

  setUnreadCount(count: number): void {
    this.unreadCount.set(count);
  }
}
