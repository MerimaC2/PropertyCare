import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { DashboardStatsDto } from './dashboard-api.models';

@Injectable({ providedIn: 'root' })
export class DashboardApiService {
  private readonly apiUrl = `${environment.apiUrl}/api/dashboard`;

  constructor(private http: HttpClient) {}

  getStats(): Observable<DashboardStatsDto> {
    return this.http.get<DashboardStatsDto>(`${this.apiUrl}/stats`);
  }
}
