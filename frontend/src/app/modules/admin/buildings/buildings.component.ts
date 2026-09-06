import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
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

/**
 * Admin management of buildings: paged list with create/edit/delete and 5 filter parameters
 * (search term, type, minimum and maximum unit count, and whether the building is on the map).
 */
@Component({
  selector: 'app-buildings',
  templateUrl: './buildings.component.html',
  styleUrls: ['./buildings.component.scss'],
  standalone: false
})
export class BuildingsComponent implements OnInit {
  readonly displayedColumns = ['name', 'type', 'address', 'units', 'location', 'actions'];

  filterForm: FormGroup;
  items: BuildingDto[] = [];
  buildingTypes: LookupItemDto[] = [];
  isLoading = false;

  /**
   * True while a create, update or delete call is in flight. The buttons that start one are
   * disabled meanwhile, so a second click cannot send the same write twice.
   */
  isSaving = false;

  totalItems = 0;
  pageSize = DEFAULT_PAGE_SIZE;
  currentPage = 1;

  constructor(
    private formBuilder: FormBuilder,
    private buildingsApi: BuildingsApiService,
    private lookupsApi: LookupsApiService,
    private dialog: MatDialog,
    private toaster: ToasterService,
    private cdr: ChangeDetectorRef
  ) {
    this.filterForm = this.formBuilder.group({
      search: [''],
      buildingTypeId: [null],
      minUnitCount: [null],
      maxUnitCount: [null],
      hasLocation: [null]
    });
  }

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
    const filters = this.filterForm.value;

    this.buildingsApi
      .list({
        paging: { page: this.currentPage, pageSize: this.pageSize },
        search: filters.search || null,
        buildingTypeId: filters.buildingTypeId,
        minUnitCount: filters.minUnitCount,
        maxUnitCount: filters.maxUnitCount,
        hasLocation: filters.hasLocation
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
        error: (err: HttpErrorResponse) => {
          this.isLoading = false;
          this.cdr.markForCheck();
          this.toaster.error(this.errorMessage(err, 'Failed to load buildings.'));
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
        this.isSaving = true;
        this.buildingsApi.create(command).subscribe({
          next: () => {
            this.isSaving = false;
            this.toaster.success('Building created.');
            this.loadData();
          },
          error: (err: HttpErrorResponse) => {
            this.isSaving = false;
            this.cdr.markForCheck();
            this.toaster.error(this.errorMessage(err, 'Failed to create building.'));
          }
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
        this.isSaving = true;
        this.buildingsApi.update(building.id, command).subscribe({
          next: () => {
            this.isSaving = false;
            this.toaster.success('Building updated.');
            this.loadData();
          },
          error: (err: HttpErrorResponse) => {
            this.isSaving = false;
            this.cdr.markForCheck();
            this.toaster.error(this.errorMessage(err, 'Failed to update building.'));
          }
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
        this.isSaving = true;
        this.buildingsApi.delete(building.id).subscribe({
          next: () => {
            this.isSaving = false;
            this.toaster.success('Building deleted.');
            this.loadData();
          },
          error: (err: HttpErrorResponse) => {
            this.isSaving = false;
            this.cdr.markForCheck();
            this.toaster.error(this.errorMessage(err, 'Failed to delete building.'));
          }
        });
      });
  }

  private errorMessage(err: HttpErrorResponse, fallback: string): string {
    const fieldErrors = err.error?.errors as { field: string; message: string }[] | undefined;
    if (fieldErrors?.length) {
      return fieldErrors.map(e => e.message).join(' ');
    }
    return err.error?.message ?? fallback;
  }
}
