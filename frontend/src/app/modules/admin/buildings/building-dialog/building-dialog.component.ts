import {
  AfterViewInit,
  ChangeDetectorRef,
  Component,
  ElementRef,
  Inject,
  OnDestroy,
  ViewChild
} from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import * as L from 'leaflet';
import { LookupItemDto } from '../../../../api-services/lookups/lookups-api.models';
import {
  BuildingDto,
  SaveBuildingCommand
} from '../../../../api-services/buildings/buildings-api.models';
import { BuildingsApiService } from '../../../../api-services/buildings/buildings-api.service';
import { buildingNameTakenValidator } from './building-name.validator';

export interface BuildingDialogData {
  building: BuildingDto | null;
  buildingTypes: LookupItemDto[];
}

/** Create/edit form for a building, with a map to pick its coordinates. */
@Component({
  selector: 'app-building-dialog',
  templateUrl: './building-dialog.component.html',
  styleUrls: ['./building-dialog.component.scss'],
  standalone: false
})
export class BuildingDialogComponent implements AfterViewInit, OnDestroy {
  @ViewChild('pickerContainer', { static: true })
  pickerContainer!: ElementRef<HTMLDivElement>;

  form: FormGroup;

  private map?: L.Map;
  private marker?: L.CircleMarker;

  constructor(
    public dialogRef: MatDialogRef<BuildingDialogComponent, SaveBuildingCommand>,
    @Inject(MAT_DIALOG_DATA) public data: BuildingDialogData,
    formBuilder: FormBuilder,
    private buildingsApi: BuildingsApiService,
    private cdr: ChangeDetectorRef
  ) {
    const b = data.building;
    this.form = formBuilder.group({
      name: [
        b?.name ?? '',
        [Validators.required, Validators.maxLength(120)],
        [buildingNameTakenValidator(this.buildingsApi, b?.id)]
      ],
      buildingTypeId: [b?.buildingTypeId ?? null, Validators.required],
      address: [b?.address ?? '', Validators.maxLength(200)],
      latitude: [b?.latitude ?? null, [Validators.min(-90), Validators.max(90)]],
      longitude: [b?.longitude ?? null, [Validators.min(-180), Validators.max(180)]]
    });
  }

  get isEdit(): boolean {
    return this.data.building !== null;
  }

  ngAfterViewInit(): void {
    const lat = this.form.value.latitude as number | null;
    const lng = this.form.value.longitude as number | null;
    const hasCoords = lat != null && lng != null;

    this.map = L.map(this.pickerContainer.nativeElement, {
      center: hasCoords ? [lat!, lng!] : [43.9, 17.7],
      zoom: hasCoords ? 14 : 7
    });
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      attribution: '© OpenStreetMap contributors',
      maxZoom: 19
    }).addTo(this.map);

    if (hasCoords) {
      this.setMarker(lat!, lng!);
    }

    this.map.on('click', (e: L.LeafletMouseEvent) => {
      const newLat = Math.round(e.latlng.lat * 1e6) / 1e6;
      const newLng = Math.round(e.latlng.lng * 1e6) / 1e6;
      this.form.patchValue({ latitude: newLat, longitude: newLng });
      this.setMarker(newLat, newLng);
      this.cdr.markForCheck();
    });

    // The dialog animates in, so the container has no size yet — recalc once open.
    this.dialogRef.afterOpened().subscribe(() => this.map?.invalidateSize());
  }

  private setMarker(lat: number, lng: number): void {
    if (this.marker) {
      this.marker.setLatLng([lat, lng]);
    } else {
      this.marker = L.circleMarker([lat, lng], {
        radius: 9,
        color: '#1565c0',
        weight: 2,
        fillColor: '#1565c0',
        fillOpacity: 0.6
      }).addTo(this.map!);
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const v = this.form.value;
    this.dialogRef.close({
      name: v.name,
      address: v.address || null,
      buildingTypeId: v.buildingTypeId,
      latitude: v.latitude ?? null,
      longitude: v.longitude ?? null
    });
  }

  ngOnDestroy(): void {
    this.map?.remove();
  }
}
