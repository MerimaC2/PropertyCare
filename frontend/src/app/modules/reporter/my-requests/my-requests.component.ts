import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { PageEvent } from '@angular/material/paginator';
import { LookupsApiService } from '../../../api-services/lookups/lookups-api.service';
import { RequestFormLookupsDto } from '../../../api-services/lookups/lookups-api.models';
import { MaintenanceRequestsApiService } from '../../../api-services/maintenance-requests/maintenance-requests-api.service';
import { MyMaintenanceRequestDto } from '../../../api-services/maintenance-requests/maintenance-requests-api.models';
import { DEFAULT_PAGE_SIZE } from '../../../core/models/paging/page-request';
import { ToasterService } from '../../../core/services/toaster.service';

/**
 * "My requests" list with 5 filter parameters
 * (status, priority, building, date from, date to) and backend paging.
 */
@Component({
  selector: 'app-my-requests',
  templateUrl: './my-requests.component.html',
  styleUrls: ['./my-requests.component.scss'],
  standalone: false
})
export class MyRequestsComponent implements OnInit {
  readonly displayedColumns = ['title', 'building', 'asset', 'priority', 'status', 'createdAtUtc'];

  filterForm: FormGroup;
  lookups: RequestFormLookupsDto | null = null;

  items: MyMaintenanceRequestDto[] = [];
  isLoading = false;
  totalItems = 0;
  pageSize = DEFAULT_PAGE_SIZE;
  currentPage = 1;

  constructor(
    private formBuilder: FormBuilder,
    private requestsApi: MaintenanceRequestsApiService,
    private lookupsApi: LookupsApiService,
    private toaster: ToasterService,
    private cdr: ChangeDetectorRef
  ) {
    this.filterForm = this.formBuilder.group({
      statusId: [null],
      priorityId: [null],
      buildingId: [null],
      dateFrom: [null],
      dateTo: [null]
    });
  }

  ngOnInit(): void {
    this.lookupsApi.getRequestFormLookups().subscribe({
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
      .listMy({
        paging: { page: this.currentPage, pageSize: this.pageSize },
        statusId: filters.statusId,
        priorityId: filters.priorityId,
        buildingId: filters.buildingId,
        dateFrom: filters.dateFrom,
        dateTo: filters.dateTo
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
          this.toaster.error('Failed to load your requests.');
        }
      });
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

  statusClass(statusAbrv: string): string {
    return `status-chip status-${statusAbrv.toLowerCase()}`;
  }

  priorityClass(priorityAbrv: string): string {
    return `priority-chip priority-${priorityAbrv.toLowerCase()}`;
  }
}
