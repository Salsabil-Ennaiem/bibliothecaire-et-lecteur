import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { MembreDto, UpdateMembreDto } from '../model/membre.model';

@Injectable({
  providedIn: 'root'
})

export class MembreService {
  private apiUrl = 'http://localhost:5232/api/Membre';

  constructor(private http: HttpClient) { }
  search(term: string): Observable<MembreDto[]> {
    return this.http.get<MembreDto[]>(`${this.apiUrl}/search/${term}`);
  }
  getAll(): Observable<MembreDto[]> {
    return this.http.get<MembreDto[]>(`${this.apiUrl}/Getall`);
  }
  getById(id: string): Observable<MembreDto> {
    return this.http.get<MembreDto>(`${this.apiUrl}/Get/${id}`);
  }
  update(id: string, membre: UpdateMembreDto): Observable<MembreDto> {
    return this.http.put<MembreDto>(`${this.apiUrl}/Update/${id}`, membre);
  }
  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/Delete/${id}`);
  }
}