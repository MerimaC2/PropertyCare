import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AssetTypeDto, SaveAssetTypeCommand } from './asset-types-api.models';

@Injectable({ providedIn: 'root' })
export class AssetTypesApiService {
  private readonly apiUrl = `${environment.apiUrl}/api/asset-types`;

  constructor(private http: HttpClient) {}

  list(): Observable<AssetTypeDto[]> {
    return this.http.get<AssetTypeDto[]>(this.apiUrl);
  }

  create(command: SaveAssetTypeCommand): Observable<number> {
    return this.http.post<number>(this.apiUrl, command);
  }

  update(id: number, command: SaveAssetTypeCommand): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, command);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
