import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { PageEvent } from '@angular/material/paginator';
import { NotificationsApiService } from '../../../api-services/notifications/notifications-api.service';
import {
  NotificationDto,
  NotificationType
} from '../../../api-services/notifications/notifications-api.models';
import { DEFAULT_PAGE_SIZE } from '../../../core/models/paging/page-request';
import { ToasterService } from '../../../core/services/toaster.service';
import { NotificationsStateService } from './notifications-state.service';

interface TypeOption {
  value: NotificationType;
  label: string;
  icon: string;
}

/** Notification center: paged list with read/unread + type filters and mark-as-read actions. */
@Component({
  selector: 'app-notifications',
  templateUrl: './notifications.component.html',
  styleUrls: ['./notifications.component.scss'],
  standalone: false
})
export class NotificationsComponent implements OnInit {
  filterForm: FormGroup;

  items: NotificationDto[] = [];
  isLoading = false;
  totalItems = 0;
  pageSize = DEFAULT_PAGE_SIZE;
  currentPage = 1;

  readonly typeOptions: TypeOption[] = [
    { value: 'RequestSubmitted', label: 'Submitted', icon: 'send' },
    { value: 'Assigned', label: 'Assigned', icon: 'engineering' },
    { value: 'Completed', label: 'Completed', icon: 'task_alt' },
    { value: 'General', label: 'General', icon: 'info' }
  ];

  constructor(
    private formBuilder: FormBuilder,
    private api: NotificationsApiService,
    private notifications: NotificationsStateService,
    private toaster: ToasterService,
    private cdr: ChangeDetectorRef
  ) {
    this.filterForm = this.formBuilder.group({
      isRead: [null],
      type: [null]
    });
  }

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.isLoading = true;
    const { isRead, type } = this.filterForm.value;

    this.api
      .listMy({
        paging: { page: this.currentPage, pageSize: this.pageSize },
        isRead,
        type
      })
      .subscribe({
        next: result => {
          this.items = result.items;
          this.totalItems = result.totalItems;
          this.currentPage = result.currentPage;
          this.pageSize = result.pageSize;
          this.isLoading = false;
          this.cdr.markForCheck();
        },
        error: () => {
          this.isLoading = false;
          this.cdr.markForCheck();
          this.toaster.error('Failed to load notifications.');
        }
      });

    this.notifications.refresh();
  }

  onApplyFilters(): void {
    this.currentPage = 1;
    this.loadData();
  }

  onResetFilters(): void {
    this.filterForm.reset();
    this.currentPage = 1;
    this.loadData();
  }

  onPageChange(event: PageEvent): void {
    this.currentPage = event.pageIndex + 1;
    this.pageSize = event.pageSize;
    this.loadData();
  }

  onNotificationClick(notification: NotificationDto): void {
    if (notification.isRead) {
      return;
    }
    this.api.markRead(notification.id).subscribe({
      next: unreadCount => {
        notification.isRead = true;
        this.notifications.setUnreadCount(unreadCount);
        this.cdr.markForCheck();
      },
      error: () => this.toaster.error('Failed to update the notification.')
    });
  }

  onMarkAllRead(): void {
    this.api.markAllRead().subscribe({
      next: unreadCount => {
        this.items.forEach(n => (n.isRead = true));
        this.notifications.setUnreadCount(unreadCount);
        this.cdr.markForCheck();
        this.toaster.success('All notifications marked as read.');
      },
      error: () => this.toaster.error('Failed to mark all as read.')
    });
  }

  iconFor(type: NotificationType): string {
    return this.typeOptions.find(o => o.value === type)?.icon ?? 'notifications';
  }
}
