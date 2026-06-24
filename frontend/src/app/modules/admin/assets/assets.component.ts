import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { MatDialog } from '@angular/material/dialog';
import { BuildingsApiService } from '../../../api-services/buildings/buildings-api.service';
import { BuildingDto } from '../../../api-services/buildings/buildings-api.models';
import { UnitsApiService } from '../../../api-services/units/units-api.service';
import { UnitDto } from '../../../api-services/units/units-api.models';
import { AssetsApiService } from '../../../api-services/assets/assets-api.service';
import { AssetDto } from '../../../api-services/assets/assets-api.models';
import { AssetTypesApiService } from '../../../api-services/asset-types/asset-types-api.service';
import { AssetTypeDto } from '../../../api-services/asset-types/asset-types-api.models';
import { ToasterService } from '../../../core/services/toaster.service';
import {
  ConfirmDialogComponent,
  ConfirmDialogData
} from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { AssetDialogComponent } from './asset-dialog/asset-dialog.component';

/** Admin master-detail: pick a building and unit, then manage that unit's assets. */
@Component({
  selector: 'app-assets',
  templateUrl: './assets.component.html',
  styleUrls: ['./assets.component.scss'],
  standalone: false
})
export class AssetsComponent implements OnInit {
  readonly displayedColumns = ['name', 'type', 'actions'];

  buildingControl = new FormControl<number | null>(null);
  unitControl = new FormControl<number | null>({ value: null, disabled: true });

  buildings: BuildingDto[] = [];
  units: UnitDto[] = [];
  assets: AssetDto[] = [];
  assetTypes: AssetTypeDto[] = [];
  isLoading = false;

  constructor(
    private buildingsApi: BuildingsApiService,
    private unitsApi: UnitsApiService,
    private assetsApi: AssetsApiService,
    private assetTypesApi: AssetTypesApiService,
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

    this.assetTypesApi.list().subscribe({
      next: types => {
        this.assetTypes = types;
        this.cdr.markForCheck();
      },
      error: () => this.toaster.error('Failed to load asset types.')
    });

    this.buildingControl.valueChanges.subscribe(buildingId => this.onBuildingChange(buildingId));
    this.unitControl.valueChanges.subscribe(() => this.loadAssets());
  }

  get selectedUnitId(): number | null {
    return this.unitControl.value;
  }

  private onBuildingChange(buildingId: number | null): void {
    this.units = [];
    this.assets = [];
    this.unitControl.setValue(null, { emitEvent: false });

    if (!buildingId) {
      this.unitControl.disable({ emitEvent: false });
      this.cdr.markForCheck();
      return;
    }

    this.unitsApi.list(buildingId).subscribe({
      next: units => {
        this.units = units;
        this.unitControl.enable({ emitEvent: false });
        this.cdr.markForCheck();
      },
      error: () => this.toaster.error('Failed to load units.')
    });
  }

  loadAssets(): void {
    const unitId = this.unitControl.value;
    if (!unitId) {
      this.assets = [];
      this.cdr.markForCheck();
      return;
    }
    this.isLoading = true;
    this.assetsApi.list(unitId).subscribe({
      next: assets => {
        this.assets = assets;
        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: () => {
        this.isLoading = false;
        this.cdr.markForCheck();
        this.toaster.error('Failed to load assets.');
      }
    });
  }

  openCreate(): void {
    const unitId = this.unitControl.value;
    if (!unitId) {
      return;
    }
    this.dialog
      .open(AssetDialogComponent, {
        data: { asset: null, assetTypes: this.assetTypes },
        width: '440px'
      })
      .afterClosed()
      .subscribe(result => {
        if (!result) {
          return;
        }
        this.assetsApi
          .create({ unitId, name: result.name, assetTypeId: result.assetTypeId })
          .subscribe({
            next: () => {
              this.toaster.success('Asset created.');
              this.loadAssets();
            },
            error: (err: HttpErrorResponse) =>
              this.toaster.error(this.errorMessage(err, 'Failed to create asset.'))
          });
      });
  }

  openEdit(asset: AssetDto): void {
    this.dialog
      .open(AssetDialogComponent, {
        data: { asset, assetTypes: this.assetTypes },
        width: '440px'
      })
      .afterClosed()
      .subscribe(result => {
        if (!result) {
          return;
        }
        this.assetsApi
          .update(asset.id, { name: result.name, assetTypeId: result.assetTypeId })
          .subscribe({
            next: () => {
              this.toaster.success('Asset updated.');
              this.loadAssets();
            },
            error: (err: HttpErrorResponse) =>
              this.toaster.error(this.errorMessage(err, 'Failed to update asset.'))
          });
      });
  }

  confirmDelete(asset: AssetDto): void {
    const data: ConfirmDialogData = {
      title: 'Delete asset',
      message: `Delete "${asset.name}"? This cannot be undone.`,
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
        this.assetsApi.delete(asset.id).subscribe({
          next: () => {
            this.toaster.success('Asset deleted.');
            this.loadAssets();
          },
          error: (err: HttpErrorResponse) =>
            this.toaster.error(this.errorMessage(err, 'Failed to delete asset.'))
        });
      });
  }

  private errorMessage(err: HttpErrorResponse, fallback: string): string {
    return err.error?.message ?? fallback;
  }
}
