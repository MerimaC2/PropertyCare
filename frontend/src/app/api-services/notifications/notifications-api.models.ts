import { PageRequest } from '../../core/models/paging/page-request';

/** Mirrors the backend NotificationType enum (serialized as its string name). */
export type NotificationType = 'General' | 'RequestSubmitted' | 'Assigned' | 'Completed';

export interface NotificationDto {
  id: number;
  title: string;
  message: string;
  type: NotificationType;
  isRead: boolean;
  createdAtUtc: string;
}

export interface ListMyNotificationsQuery {
  paging: PageRequest;
  isRead?: boolean | null;
  type?: NotificationType | null;
}
