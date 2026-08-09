import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { LookupItemDto, RequestFormLookupsDto, TriageLookupsDto } from './lookups-api.models';

@Injectable({ providedIn: 'root' })
export class LookupsApiService {
  private readonly apiUrl = `${environment.apiUrl}/api/lookups`;

  constructor(private http: HttpClient) {}

  getRequestFormLookups(): Observable<RequestFormLookupsDto> {
    return this.http.get<RequestFormLookupsDto>(`${this.apiUrl}/request-form`);
  }

  getTriageLookups(): Observable<TriageLookupsDto> {
    return this.http.get<TriageLookupsDto>(`${this.apiUrl}/triage`);
  }

  getBuildingTypes(): Observable<LookupItemDto[]> {
    return this.http.get<LookupItemDto[]>(`${this.apiUrl}/building-types`);
  }
}
