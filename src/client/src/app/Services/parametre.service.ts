import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ParametreDTO, UpdateParametreDTO } from '../model/parametre.model';

@Injectable({
  providedIn: 'root'
})
export class ParametreService {

   private apiUrl = 'http://localhost:5232/api/Parametre';

  constructor(private http: HttpClient) { }

  getById(): Observable<ParametreDTO> {
    return this.http.get<ParametreDTO>(`${this.apiUrl}/Get`);
  }
  modifier(Parametre: UpdateParametreDTO): Observable<ParametreDTO> {
    return this.http.post<ParametreDTO>(`${this.apiUrl}/Update`, Parametre);
  }
}
