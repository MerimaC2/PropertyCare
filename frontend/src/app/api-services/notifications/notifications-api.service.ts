import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { buildHttpParams } from '../../core/models/build-http-params';
import { PageResult } from '../../core/models/paging/page-result';
import { ListMyNotificationsQuery, NotificationDto } from './notifications-api.models';

@Injectable({ providedIn: 'root' })
export class NotificationsApiService {
  private readonly apiUrl = `${environment.apiUrl}/api/notifications`;

  constructor(private http: HttpClient) {}

  listMy(query: ListMyNotificationsQuery): Observable<PageResult<NotificationDto>> {
    const params = buildHttpParams(query as unknown as Record<string, unknown>);
    return this.http.get<PageResult<NotificationDto>>(`${this.apiUrl}/my`, { params });
  }

  getUnreadCount(): Observable<number> {
    return this.http.get<number>(`${this.apiUrl}/unread-count`);
  }

  /** Marks one notification as read; returns the updated unread count. */
  markRead(id: number): Observable<number> {
    return this.http.post<number>(`${this.apiUrl}/${id}/read`, {});
  }

  /** Marks all notifications as read; returns the updated unread count (0). */
  markAllRead(): Observable<number> {
    return this.http.post<number>(`${this.apiUrl}/read-all`, {});
  }
}
