import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { MatDialog } from '@angular/material/dialog';
import { AssetTypesApiService } from '../../../api-services/asset-types/asset-types-api.service';
import { AssetTypeDto } from '../../../api-services/asset-types/asset-types-api.models';
import { ToasterService } from '../../../core/services/toaster.service';
import {
  ConfirmDialogComponent,
  ConfirmDialogData
} from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { AssetTypeDialogComponent } from './asset-type-dialog/asset-type-dialog.component';

/** Admin management of asset types (flat CRUD list). */
@Component({
  selector: 'app-asset-types',
  templateUrl: './asset-types.component.html',
  styleUrls: ['./asset-types.component.scss'],
  standalone: false
})
export class AssetTypesComponent implements OnInit {
  readonly displayedColumns = ['name', 'sla', 'assets', 'actions'];

  items: AssetTypeDto[] = [];
  isLoading = false;

  constructor(
    private assetTypesApi: AssetTypesApiService,
    private dialog: MatDialog,
    private toaster: ToasterService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.isLoading = true;
    this.assetTypesApi.list().subscribe({
      next: items => {
        this.items = items;
        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: () => {
        this.isLoading = false;
        this.cdr.markForCheck();
        this.toaster.error('Failed to load asset types.');
      }
    });
  }

  openCreate(): void {
    this.dialog
      .open(AssetTypeDialogComponent, { data: { assetType: null }, width: '420px' })
      .afterClosed()
      .subscribe(command => {
        if (!command) {
          return;
        }
        this.assetTypesApi.create(command).subscribe({
          next: () => {
            this.toaster.success('Asset type created.');
            this.loadData();
          },
          error: (err: HttpErrorResponse) =>
            this.toaster.error(this.errorMessage(err, 'Failed to create asset type.'))
        });
      });
  }

  openEdit(assetType: AssetTypeDto): void {
    this.dialog
      .open(AssetTypeDialogComponent, { data: { assetType }, width: '420px' })
      .afterClosed()
      .subscribe(command => {
        if (!command) {
          return;
        }
        this.assetTypesApi.update(assetType.id, command).subscribe({
          next: () => {
            this.toaster.success('Asset type updated.');
            this.loadData();
          },
          error: (err: HttpErrorResponse) =>
            this.toaster.error(this.errorMessage(err, 'Failed to update asset type.'))
        });
      });
  }

  confirmDelete(assetType: AssetTypeDto): void {
    const data: ConfirmDialogData = {
      title: 'Delete asset type',
      message: `Delete "${assetType.name}"? This cannot be undone.`,
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
        this.assetTypesApi.delete(assetType.id).subscribe({
          next: () => {
            this.toaster.success('Asset type deleted.');
            this.loadData();
          },
          error: (err: HttpErrorResponse) =>
            this.toaster.error(this.errorMessage(err, 'Failed to delete asset type.'))
        });
      });
  }

  private errorMessage(err: HttpErrorResponse, fallback: string): string {
    return err.error?.message ?? fallback;
  }
}
