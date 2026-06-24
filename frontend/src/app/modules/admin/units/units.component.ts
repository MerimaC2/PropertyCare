import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { MatDialog } from '@angular/material/dialog';
import { BuildingsApiService } from '../../../api-services/buildings/buildings-api.service';
import { BuildingDto } from '../../../api-services/buildings/buildings-api.models';
import { UnitsApiService } from '../../../api-services/units/units-api.service';
import { UnitDto } from '../../../api-services/units/units-api.models';
import { ToasterService } from '../../../core/services/toaster.service';
import {
  ConfirmDialogComponent,
  ConfirmDialogData
} from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { UnitDialogComponent } from './unit-dialog/unit-dialog.component';

/** Admin master-detail: pick a building, then manage its units. */
@Component({
  selector: 'app-units',
  templateUrl: './units.component.html',
  styleUrls: ['./units.component.scss'],
  standalone: false
})
export class UnitsComponent implements OnInit {
  readonly displayedColumns = ['label', 'assets', 'actions'];

  buildingControl = new FormControl<number | null>(null);
  buildings: BuildingDto[] = [];
  units: UnitDto[] = [];
  isLoading = false;

  constructor(
    private buildingsApi: BuildingsApiService,
    private unitsApi: UnitsApiService,
    private dialog: MatDialog,
    private toaster: ToasterService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.buildingsApi.list({ paging: { page: 1, pageSize: 100 } }).subscribe({
      next: result => {
        this.buildings = result.items;
        this.cdr.markForCheck();
      },
      error: () => this.toaster.error('Failed to load buildings.')
    });

    this.buildingControl.valueChanges.subscribe(() => this.loadUnits());
  }

  get selectedBuildingId(): number | null {
    return this.buildingControl.value;
  }

  loadUnits(): void {
    const buildingId = this.buildingControl.value;
    if (!buildingId) {
      this.units = [];
      this.cdr.markForCheck();
      return;
    }
    this.isLoading = true;
    this.unitsApi.list(buildingId).subscribe({
      next: units => {
        this.units = units;
        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: () => {
        this.isLoading = false;
        this.cdr.markForCheck();
        this.toaster.error('Failed to load units.');
      }
    });
  }

  openCreate(): void {
    const buildingId = this.buildingControl.value;
    if (!buildingId) {
      return;
    }
    this.dialog
      .open(UnitDialogComponent, { data: { unit: null }, width: '420px' })
      .afterClosed()
      .subscribe(result => {
        if (!result) {
          return;
        }
        this.unitsApi.create({ buildingId, label: result.label }).subscribe({
          next: () => {
            this.toaster.success('Unit created.');
            this.loadUnits();
          },
          error: (err: HttpErrorResponse) =>
            this.toaster.error(this.errorMessage(err, 'Failed to create unit.'))
        });
      });
  }

  openEdit(unit: UnitDto): void {
    this.dialog
      .open(UnitDialogComponent, { data: { unit }, width: '420px' })
      .afterClosed()
      .subscribe(result => {
        if (!result) {
          return;
        }
        this.unitsApi.update(unit.id, { label: result.label }).subscribe({
          next: () => {
            this.toaster.success('Unit updated.');
            this.loadUnits();
          },
          error: (err: HttpErrorResponse) =>
            this.toaster.error(this.errorMessage(err, 'Failed to update unit.'))
        });
      });
  }

  confirmDelete(unit: UnitDto): void {
    const data: ConfirmDialogData = {
      title: 'Delete unit',
      message: `Delete "${unit.label}"? This cannot be undone.`,
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
        this.unitsApi.delete(unit.id).subscribe({
          next: () => {
            this.toaster.success('Unit deleted.');
            this.loadUnits();
          },
          error: (err: HttpErrorResponse) =>
            this.toaster.error(this.errorMessage(err, 'Failed to delete unit.'))
        });
      });
  }

  private errorMessage(err: HttpErrorResponse, fallback: string): string {
    return err.error?.message ?? fallback;
  }
}
