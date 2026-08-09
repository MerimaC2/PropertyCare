import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CreateUnitCommand, UnitDto, UpdateUnitCommand } from './units-api.models';

@Injectable({ providedIn: 'root' })
export class UnitsApiService {
  private readonly apiUrl = `${environment.apiUrl}/api/units`;

  constructor(private http: HttpClient) {}

  list(buildingId: number): Observable<UnitDto[]> {
    const params = new HttpParams().set('buildingId', buildingId);
    return this.http.get<UnitDto[]>(this.apiUrl, { params });
  }

  create(command: CreateUnitCommand): Observable<number> {
    return this.http.post<number>(this.apiUrl, command);
  }

  update(id: number, command: UpdateUnitCommand): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, command);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
