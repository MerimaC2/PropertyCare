import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { PageEvent } from '@angular/material/paginator';
import { Sort } from '@angular/material/sort';
import { HttpErrorResponse } from '@angular/common/http';
import { LookupsApiService } from '../../../api-services/lookups/lookups-api.service';
import { TriageLookupsDto } from '../../../api-services/lookups/lookups-api.models';
import { MaintenanceRequestsApiService } from '../../../api-services/maintenance-requests/maintenance-requests-api.service';
import { TriageRequestDto } from '../../../api-services/maintenance-requests/maintenance-requests-api.models';
import { WorkOrdersApiService } from '../../../api-services/work-orders/work-orders-api.service';
import { DEFAULT_PAGE_SIZE } from '../../../core/models/paging/page-request';
import { ToasterService } from '../../../core/services/toaster.service';
import {
  ConfirmDialogComponent,
  ConfirmDialogData
} from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { AssignDialogComponent, AssignDialogResult } from './assign-dialog/assign-dialog.component';

/**
 * Admin triage screen: filterable, sortable, paged list of all
 * maintenance requests with a one-click "assign technician" action.
 */
@Component({
  selector: 'app-triage',
  templateUrl: './triage.component.html',
  styleUrls: ['./triage.component.scss'],
  standalone: false
})
export class TriageComponent implements OnInit {
  readonly displayedColumns = [
    'title', 'building', 'priority', 'status', 'createdBy', 'assignedTo', 'createdAtUtc', 'actions'
  ];

  filterForm: FormGroup;
  lookups: TriageLookupsDto | null = null;

  items: TriageRequestDto[] = [];
  isLoading = false;
  totalItems = 0;
  pageSize = DEFAULT_PAGE_SIZE;
  currentPage = 1;

  // Backend column sorting state
  sortBy: string | null = null;
  sortDesc = true;

  constructor(
    private formBuilder: FormBuilder,
    private requestsApi: MaintenanceRequestsApiService,
    private workOrdersApi: WorkOrdersApiService,
    private lookupsApi: LookupsApiService,
    private toaster: ToasterService,
    private dialog: MatDialog,
    private cdr: ChangeDetectorRef
  ) {
    this.filterForm = this.formBuilder.group({
      search: [''],
      statusId: [null],
      priorityId: [null],
      buildingId: [null],
      dateFrom: [null],
      dateTo: [null]
    });
  }

  ngOnInit(): void {
    this.lookupsApi.getTriageLookups().subscribe({
      next: lookups => {
        this.lookups = lookups;
        this.cdr.markForCheck();
      },
      error: () => this.toaster.error('Failed to load filter options.')
    });
    this.loadData();
  }

  loadData(): void {
    this.isLoading = true;
    const filters = this.filterForm.value;

    this.requestsApi
      .listForTriage({
        paging: { page: this.currentPage, pageSize: this.pageSize },
        search: filters.search,
        statusId: filters.statusId,
        priorityId: filters.priorityId,
        buildingId: filters.buildingId,
        dateFrom: filters.dateFrom,
        dateTo: filters.dateTo,
        sortBy: this.sortBy,
        sortDesc: this.sortDesc
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
          this.toaster.error('Failed to load requests.');
        }
      });
  }

  onApplyFilters(): void {
    this.currentPage = 1;
    this.loadData();
  }

  onResetFilters(): void {
    this.filterForm.reset({ search: '' });
    this.currentPage = 1;
    this.loadData();
  }

  onPageChange(event: PageEvent): void {
    this.currentPage = event.pageIndex + 1;
    this.pageSize = event.pageSize;
    this.loadData();
  }

  /** MatSort event mapped to backend sorting parameters. */
  onSortChange(sort: Sort): void {
    if (!sort.direction) {
      this.sortBy = null;
      this.sortDesc = true;
    } else {
      this.sortBy = sort.active;
      this.sortDesc = sort.direction === 'desc';
    }
    this.currentPage = 1;
    this.loadData();
  }

  canAssign(item: TriageRequestDto): boolean {
    return !item.statusIsTerminal && !item.assignedToName;
  }

  onAssign(item: TriageRequestDto): void {
    if (!this.lookups) {
      return;
    }

    const assignDialogRef = this.dialog.open<AssignDialogComponent, unknown, AssignDialogResult>(
      AssignDialogComponent,
      { width: '420px', data: { request: item, technicians: this.lookups.technicians } }
    );

    assignDialogRef.afterClosed().subscribe(result => {
      if (!result) {
        return;
      }

      // Assigning is confirmed explicitly because it changes the request status.
      const confirmRef = this.dialog.open<ConfirmDialogComponent, ConfirmDialogData, boolean>(
        ConfirmDialogComponent,
        {
          data: {
            title: 'Assign technician',
            message: `Assign "${item.title}" to ${result.technicianName}?`,
            confirmLabel: 'Assign'
          }
        }
      );

      confirmRef.afterClosed().subscribe(confirmed => {
        if (confirmed) {
          this.assign(item, result);
        }
      });
    });
  }

  private assign(item: TriageRequestDto, result: AssignDialogResult): void {
    this.workOrdersApi
      .assign({
        requestId: item.id,
        assignedToUserId: result.technicianId,
        note: result.note
      })
      .subscribe({
        next: () => {
          this.toaster.success(`Request assigned to ${result.technicianName}.`);
          this.loadData();
        },
        error: (error: HttpErrorResponse) => {
          this.toaster.error(error.error?.message ?? 'Failed to assign the request.');
        }
      });
  }

  statusClass(statusAbrv: string): string {
    return `status-chip status-${statusAbrv.toLowerCase()}`;
  }

  priorityClass(priorityAbrv: string): string {
    return `priority-chip priority-${priorityAbrv.toLowerCase()}`;
  }
}
