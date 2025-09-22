import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CreateSanctionRequest, Raison_sanction, SanctionDTO } from '../model/sanction.model';

@Injectable({
  providedIn: 'root'
})
export class SanctionService {

  constructor(private http: HttpClient) { }
  private apiUrl = 'http://localhost:5232/api/Sanction';

  search(term: string): Observable<SanctionDTO[]> {
    return this.http.get<SanctionDTO[]>(`${this.apiUrl}/search/${term}`);
  }
  FiltrPay(pay?: boolean): Observable<SanctionDTO[]> {
    return this.http.get<SanctionDTO[]>(`${this.apiUrl}/FiltrPay/${pay}`);
  }
  FiltrRaison(raison?: Raison_sanction[]): Observable<SanctionDTO[]> {
    let params = new HttpParams();
    raison?.forEach(r => {
      params = params.append('raison', r.toString());
    });
    return this.http.get<SanctionDTO[]>(`${this.apiUrl}/FiltrRaison`, { params });
  }


  getAll(): Observable<SanctionDTO[]> {
    return this.http.get<SanctionDTO[]>(`${this.apiUrl}/Getall`);
  }
  create(Sanction: CreateSanctionRequest, id: string): Observable<SanctionDTO> {
    return this.http.post<SanctionDTO>(`${this.apiUrl}/Create/${id}`, Sanction);
  }
  modifier(id: string): Observable<any> {
    return this.http.patch(`${this.apiUrl}/modifier/${id}`, null);
  }
}
