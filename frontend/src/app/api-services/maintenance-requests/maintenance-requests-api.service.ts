import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { buildHttpParams } from '../../core/models/build-http-params';
import { PageResult } from '../../core/models/paging/page-result';
import {
  CreateMaintenanceRequestCommand,
  ListMyMaintenanceRequestsQuery,
  MyMaintenanceRequestDto
} from './maintenance-requests-api.models';

@Injectable({ providedIn: 'root' })
export class MaintenanceRequestsApiService {
  private readonly apiUrl = `${environment.apiUrl}/api/maintenance-requests`;

  constructor(private http: HttpClient) {}

  create(command: CreateMaintenanceRequestCommand): Observable<number> {
    return this.http.post<number>(this.apiUrl, command);
  }

  listMy(query: ListMyMaintenanceRequestsQuery): Observable<PageResult<MyMaintenanceRequestDto>> {
    const params = buildHttpParams(query as unknown as Record<string, unknown>);
    return this.http.get<PageResult<MyMaintenanceRequestDto>>(`${this.apiUrl}/my`, { params });
  }
}
