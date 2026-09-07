import { Injectable } from '@angular/core';
import { HttpClient, HttpEvent } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { buildHttpParams } from '../../core/models/build-http-params';
import { PageResult } from '../../core/models/paging/page-result';
import {
  CreateMaintenanceRequestCommand,
  ListMyMaintenanceRequestsQuery,
  ListTriageRequestsQuery,
  MyMaintenanceRequestDto,
  RequestImageDto,
  TriageRequestDto
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

  listForTriage(query: ListTriageRequestsQuery): Observable<PageResult<TriageRequestDto>> {
    const params = buildHttpParams(query as unknown as Record<string, unknown>);
    return this.http.get<PageResult<TriageRequestDto>>(`${this.apiUrl}/triage`, { params });
  }

  /** Uploads one photo to a request; emits HTTP events so the caller can track upload progress. */
  uploadImage(requestId: number, file: File): Observable<HttpEvent<RequestImageDto>> {
    const formData = new FormData();
    formData.append('file', file, file.name);

    return this.http.post<RequestImageDto>(`${this.apiUrl}/${requestId}/images`, formData, {
      reportProgress: true,
      observe: 'events'
    });
  }

  listImages(requestId: number): Observable<RequestImageDto[]> {
    return this.http.get<RequestImageDto[]>(`${this.apiUrl}/${requestId}/images`);
  }

  /**
   * Downloads one attachment. Attachments are not static files any more, so the request carries the
   * bearer token and the result has to be turned into an object URL before it can be displayed.
   */
  getImageContent(requestId: number, imageId: number): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/${requestId}/images/${imageId}/content`, {
      responseType: 'blob'
    });
  }
}
