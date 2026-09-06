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
          }).bindPopup(this.buildPopup(b.name, b.address));
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

  /**
   * Builds the popup as DOM nodes rather than an HTML string. The name and the address are typed
   * in by an administrator and stored in the database, so concatenating them into markup would
   * let a building called `<img src=x onerror=...>` run script in every admin's browser.
   * textContent writes them as text, whatever they contain.
   */
  private buildPopup(name: string, address: string | null | undefined): HTMLElement {
    const container = document.createElement('div');

    const title = document.createElement('strong');
    title.textContent = name;
    container.appendChild(title);

    if (address) {
      container.appendChild(document.createElement('br'));
      container.appendChild(document.createTextNode(address));
    }

    return container;
  }

  ngOnDestroy(): void {
    this.map?.remove();
  }
}
