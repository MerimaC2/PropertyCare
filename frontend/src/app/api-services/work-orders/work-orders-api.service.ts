import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AssignWorkOrderCommand } from './work-orders-api.models';

@Injectable({ providedIn: 'root' })
export class WorkOrdersApiService {
  private readonly apiUrl = `${environment.apiUrl}/api/work-orders`;

  constructor(private http: HttpClient) {}

  assign(command: AssignWorkOrderCommand): Observable<number> {
    return this.http.post<number>(`${this.apiUrl}/assign`, command);
  }
}
