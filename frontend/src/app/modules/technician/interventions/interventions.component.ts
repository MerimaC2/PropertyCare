import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { MatDialog } from '@angular/material/dialog';
import { InterventionsApiService } from '../../../api-services/interventions/interventions-api.service';
import { InterventionDto } from '../../../api-services/interventions/interventions-api.models';
import { ToasterService } from '../../../core/services/toaster.service';
import { WorkLogDialogComponent } from './work-log-dialog/work-log-dialog.component';

/** Technician's work orders with their logged work, plus logging new work. */
@Component({
  selector: 'app-interventions',
  templateUrl: './interventions.component.html',
  styleUrls: ['./interventions.component.scss'],
  standalone: false
})
export class InterventionsComponent implements OnInit {
  items: InterventionDto[] = [];
  isLoading = false;

  constructor(
    private api: InterventionsApiService,
    private dialog: MatDialog,
    private toaster: ToasterService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.isLoading = true;
    this.api.listMine().subscribe({
      next: items => {
        this.items = items;
        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: () => {
        this.isLoading = false;
        this.cdr.markForCheck();
        this.toaster.error('Failed to load your interventions.');
      }
    });
  }

  addLog(item: InterventionDto): void {
    this.dialog
      .open(WorkLogDialogComponent, {
        data: { requestTitle: item.requestTitle },
        width: '440px'
      })
      .afterClosed()
      .subscribe(command => {
        if (!command) {
          return;
        }
        this.api.addLog(item.workOrderId, command).subscribe({
          next: () => {
            this.toaster.success('Work logged.');
            this.loadData();
          },
          error: (err: HttpErrorResponse) =>
            this.toaster.error(err.error?.message ?? 'Failed to log work.')
        });
      });
  }
}
