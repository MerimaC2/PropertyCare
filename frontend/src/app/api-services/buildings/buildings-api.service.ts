import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { buildHttpParams } from '../../core/models/build-http-params';
import { PageResult } from '../../core/models/paging/page-result';
import {
  BuildingDto,
  BuildingLocationDto,
  ListBuildingsQuery,
  SaveBuildingCommand
} from './buildings-api.models';

@Injectable({ providedIn: 'root' })
export class BuildingsApiService {
  private readonly apiUrl = `${environment.apiUrl}/api/buildings`;

  constructor(private http: HttpClient) {}

  list(query: ListBuildingsQuery): Observable<PageResult<BuildingDto>> {
    const params = buildHttpParams(query as unknown as Record<string, unknown>);
    return this.http.get<PageResult<BuildingDto>>(this.apiUrl, { params });
  }

  create(command: SaveBuildingCommand): Observable<number> {
    return this.http.post<number>(this.apiUrl, command);
  }

  update(id: number, command: SaveBuildingCommand): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, command);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  listLocations(): Observable<BuildingLocationDto[]> {
    return this.http.get<BuildingLocationDto[]>(`${this.apiUrl}/locations`);
  }

  /** Whether the given building name is already taken (excluding the building being edited). */
  nameExists(name: string, excludeId?: number): Observable<boolean> {
    let params = new HttpParams().set('name', name);
    if (excludeId != null) {
      params = params.set('excludeId', excludeId);
    }
    return this.http.get<boolean>(`${this.apiUrl}/name-exists`, { params });
  }
}
