import {
  AfterViewInit,
  ChangeDetectorRef,
  Component,
  ElementRef,
  OnDestroy,
  ViewChild
} from '@angular/core';
import { Chart, registerables } from 'chart.js';
import { DashboardApiService } from '../../../api-services/dashboard/dashboard-api.service';
import { ToasterService } from '../../../core/services/toaster.service';

Chart.register(...registerables);

/** Admin dashboard: request totals plus by-status and by-priority charts. */
@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
  standalone: false
})
export class DashboardComponent implements AfterViewInit, OnDestroy {
  @ViewChild('statusCanvas', { static: true })
  statusCanvas!: ElementRef<HTMLCanvasElement>;

  @ViewChild('priorityCanvas', { static: true })
  priorityCanvas!: ElementRef<HTMLCanvasElement>;

  total = 0;
  isLoading = false;

  private statusChart?: Chart;
  private priorityChart?: Chart;

  private readonly palette = [
    '#1565c0', '#5e35b1', '#b26a00', '#546e7a',
    '#2e7d32', '#c62828', '#00838f', '#6d4c41'
  ];

  constructor(
    private dashboardApi: DashboardApiService,
    private toaster: ToasterService,
    private cdr: ChangeDetectorRef
  ) {}

  ngAfterViewInit(): void {
    this.isLoading = true;
    this.dashboardApi.getStats().subscribe({
      next: stats => {
        this.total = stats.totalRequests;
        this.renderStatusChart(
          stats.byStatus.map(s => s.label),
          stats.byStatus.map(s => s.count)
        );
        this.renderPriorityChart(
          stats.byPriority.map(p => p.label),
          stats.byPriority.map(p => p.count)
        );
        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: () => {
        this.isLoading = false;
        this.cdr.markForCheck();
        this.toaster.error('Failed to load dashboard statistics.');
      }
    });
  }

  private renderStatusChart(labels: string[], data: number[]): void {
    this.statusChart = new Chart(this.statusCanvas.nativeElement, {
      type: 'doughnut',
      data: { labels, datasets: [{ data, backgroundColor: this.palette }] },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: { legend: { position: 'bottom' } }
      }
    });
  }

  private renderPriorityChart(labels: string[], data: number[]): void {
    this.priorityChart = new Chart(this.priorityCanvas.nativeElement, {
      type: 'bar',
      data: { labels, datasets: [{ label: 'Requests', data, backgroundColor: '#1565c0' }] },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: { legend: { display: false } },
        scales: { y: { beginAtZero: true, ticks: { precision: 0 } } }
      }
    });
  }

  ngOnDestroy(): void {
    this.statusChart?.destroy();
    this.priorityChart?.destroy();
  }
}
