import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { DashboardResponse } from '../model/dashbord.model';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private apiUrl = 'http://localhost:5232/api/dashboard';

 constructor(private http: HttpClient) {}

  getDashboardData(biblioId: string): Observable<DashboardResponse> {
    return this.http.get<DashboardResponse>(`${this.apiUrl}/${biblioId}`);
  }

  refreshDashboard(biblioId: string) {
    return this.http.post(`${this.apiUrl}/${biblioId}/refresh`, {});
  }

  notifyDataChange(biblioId: string, changeType: string) {
    return this.http.post(`${this.apiUrl}/${biblioId}/notify/${changeType}`, {});
  }
}