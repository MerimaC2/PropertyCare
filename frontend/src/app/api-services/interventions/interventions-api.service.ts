import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AddWorkLogCommand, InterventionDto } from './interventions-api.models';

@Injectable({ providedIn: 'root' })
export class InterventionsApiService {
  private readonly apiUrl = `${environment.apiUrl}/api/interventions`;

  constructor(private http: HttpClient) {}

  listMine(): Observable<InterventionDto[]> {
    return this.http.get<InterventionDto[]>(this.apiUrl);
  }

  addLog(workOrderId: number, command: AddWorkLogCommand): Observable<number> {
    return this.http.post<number>(`${this.apiUrl}/${workOrderId}/logs`, command);
  }
}
