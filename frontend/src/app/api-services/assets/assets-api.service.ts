import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AssetDto, CreateAssetCommand, UpdateAssetCommand } from './assets-api.models';

@Injectable({ providedIn: 'root' })
export class AssetsApiService {
  private readonly apiUrl = `${environment.apiUrl}/api/assets`;

  constructor(private http: HttpClient) {}

  list(unitId: number): Observable<AssetDto[]> {
    const params = new HttpParams().set('unitId', unitId);
    return this.http.get<AssetDto[]>(this.apiUrl, { params });
  }

  create(command: CreateAssetCommand): Observable<number> {
    return this.http.post<number>(this.apiUrl, command);
  }

  update(id: number, command: UpdateAssetCommand): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, command);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
