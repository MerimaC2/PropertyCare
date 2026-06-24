import {
  AfterViewInit,
  ChangeDetectorRef,
  Component,
  ElementRef,
  OnDestroy,
  ViewChild
} from '@angular/core';
import * as L from 'leaflet';
import { BuildingsApiService } from '../../../api-services/buildings/buildings-api.service';
import { ToasterService } from '../../../core/services/toaster.service';

/** Interactive map plotting every building that has coordinates as a marker. */
@Component({
  selector: 'app-buildings-map',
  templateUrl: './buildings-map.component.html',
  styleUrls: ['./buildings-map.component.scss'],
  standalone: false
})
export class BuildingsMapComponent implements AfterViewInit, OnDestroy {
  @ViewChild('mapContainer', { static: true })
  mapContainer!: ElementRef<HTMLDivElement>;

  isLoading = false;
  count = 0;

  private map?: L.Map;

  constructor(
    private buildingsApi: BuildingsApiService,
    private toaster: ToasterService,
    private cdr: ChangeDetectorRef
  ) {}

  ngAfterViewInit(): void {
    // Default view roughly centered on Bosnia and Herzegovina.
    this.map = L.map(this.mapContainer.nativeElement, { center: [43.9, 17.7], zoom: 7 });
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      attribution: '© OpenStreetMap contributors',
      maxZoom: 19
    }).addTo(this.map);

    this.loadMarkers();
  }

  private loadMarkers(): void {
    this.isLoading = true;
    this.buildingsApi.listLocations().subscribe({
      next: locations => {
        this.count = locations.length;
        const markers: L.CircleMarker[] = [];

        for (const b of locations) {
          const marker = L.circleMarker([b.latitude, b.longitude], {
            radius: 9,
            color: '#1565c0',
            weight: 2,
            fillColor: '#1565c0',
            fillOpacity: 0.6
          }).bindPopup(`<strong>${b.name}</strong>${b.address ? '<br>' + b.address : ''}`);
          marker.addTo(this.map!);
          markers.push(marker);
        }

        if (markers.length > 0) {
          const group = L.featureGroup(markers);
          this.map!.fitBounds(group.getBounds().pad(0.2));
        }

        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: () => {
        this.isLoading = false;
        this.cdr.markForCheck();
        this.toaster.error('Failed to load building locations.');
      }
    });
  }

  ngOnDestroy(): void {
    this.map?.remove();
  }
}
