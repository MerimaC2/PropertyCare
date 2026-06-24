import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { MatDialog } from '@angular/material/dialog';
import { PageEvent } from '@angular/material/paginator';
import { BuildingsApiService } from '../../../api-services/buildings/buildings-api.service';
import { BuildingDto } from '../../../api-services/buildings/buildings-api.models';
import { LookupsApiService } from '../../../api-services/lookups/lookups-api.service';
import { LookupItemDto } from '../../../api-services/lookups/lookups-api.models';
import { DEFAULT_PAGE_SIZE } from '../../../core/models/paging/page-request';
import { ToasterService } from '../../../core/services/toaster.service';
import {
  ConfirmDialogComponent,
  ConfirmDialogData
} from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { BuildingDialogComponent } from './building-dialog/building-dialog.component';

/** Admin management of buildings: searchable, paged list with create/edit/delete. */
@Component({
  selector: 'app-buildings',
  templateUrl: './buildings.component.html',
  styleUrls: ['./buildings.component.scss'],
  standalone: false
})
export class BuildingsComponent implements OnInit {
  readonly displayedColumns = ['name', 'type', 'address', 'units', 'location', 'actions'];

  searchControl = new FormControl<string>('', { nonNullable: true });
  items: BuildingDto[] = [];
  buildingTypes: LookupItemDto[] = [];
  isLoading = false;
  totalItems = 0;
  pageSize = DEFAULT_PAGE_SIZE;
  currentPage = 1;

  constructor(
    private buildingsApi: BuildingsApiService,
    private lookupsApi: LookupsApiService,
    private dialog: MatDialog,
    private toaster: ToasterService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.lookupsApi.getBuildingTypes().subscribe({
      next: types => {
        this.buildingTypes = types;
        this.cdr.markForCheck();
      },
      error: () => this.toaster.error('Failed to load building types.')
    });
    this.loadData();
  }

  loadData(): void {
    this.isLoading = true;
    this.buildingsApi
      .list({
        paging: { page: this.currentPage, pageSize: this.pageSize },
        search: this.searchControl.value || null
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
          this.toaster.error('Failed to load buildings.');
        }
      });
  }

  onSearch(): void {
    this.currentPage = 1;
    this.loadData();
  }

  onPageChange(event: PageEvent): void {
    this.currentPage = event.pageIndex + 1;
    this.pageSize = event.pageSize;
    this.loadData();
  }

  openCreate(): void {
    this.dialog
      .open(BuildingDialogComponent, {
        data: { building: null, buildingTypes: this.buildingTypes },
        width: '480px'
      })
      .afterClosed()
      .subscribe(command => {
        if (!command) {
          return;
        }
        this.buildingsApi.create(command).subscribe({
          next: () => {
            this.toaster.success('Building created.');
            this.loadData();
          },
          error: (err: HttpErrorResponse) =>
            this.toaster.error(this.errorMessage(err, 'Failed to create building.'))
        });
      });
  }

  openEdit(building: BuildingDto): void {
    this.dialog
      .open(BuildingDialogComponent, {
        data: { building, buildingTypes: this.buildingTypes },
        width: '480px'
      })
      .afterClosed()
      .subscribe(command => {
        if (!command) {
          return;
        }
        this.buildingsApi.update(building.id, command).subscribe({
          next: () => {
            this.toaster.success('Building updated.');
            this.loadData();
          },
          error: (err: HttpErrorResponse) =>
            this.toaster.error(this.errorMessage(err, 'Failed to update building.'))
        });
      });
  }

  confirmDelete(building: BuildingDto): void {
    const data: ConfirmDialogData = {
      title: 'Delete building',
      message: `Delete "${building.name}"? This cannot be undone.`,
      confirmLabel: 'Delete',
      cancelLabel: 'Cancel'
    };
    this.dialog
      .open(ConfirmDialogComponent, { data, width: '400px' })
      .afterClosed()
      .subscribe(confirmed => {
        if (!confirmed) {
          return;
        }
        this.buildingsApi.delete(building.id).subscribe({
          next: () => {
            this.toaster.success('Building deleted.');
            this.loadData();
          },
          error: (err: HttpErrorResponse) =>
            this.toaster.error(this.errorMessage(err, 'Failed to delete building.'))
        });
      });
  }

  private errorMessage(err: HttpErrorResponse, fallback: string): string {
    return err.error?.message ?? fallback;
  }
}
