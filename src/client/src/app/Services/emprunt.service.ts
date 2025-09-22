import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CreateEmpRequest, EmppruntDTO, Statut_emp, UpdateEmppruntDTO } from '../model/emprunts.model';

@Injectable({
  providedIn: 'root'
})
export class EmpruntService {

  private apiUrl = 'http://localhost:5232/api/Emprunt';

  constructor(private http: HttpClient) { }

notifcation(): Observable<string> {
return this.http.get(`${this.apiUrl}/Notification`,{ responseType: 'text' });
}

  search(term: string): Observable<EmppruntDTO[]> {
    return this.http.get<EmppruntDTO[]>(`${this.apiUrl}/search/${term}`);
  }
  filtre(statut?: Statut_emp): Observable<EmppruntDTO[]> {
    return this.http.get<EmppruntDTO[]>(`${this.apiUrl}/FiltrEmp/${statut}`);
  }
  getAll(): Observable<EmppruntDTO[]> {
    return this.http.get<EmppruntDTO[]>(`${this.apiUrl}/Getall`);
  }
  getById(id: string): Observable<EmppruntDTO> {
    return this.http.get<EmppruntDTO>(`${this.apiUrl}/Get/${id}`);
  }

  create(id :string ,Emp: CreateEmpRequest): Observable<EmppruntDTO> {
    return this.http.post<EmppruntDTO>(`${this.apiUrl}/Create/${id}`, Emp);
  }

  update(id: string, Emp: UpdateEmppruntDTO): Observable<EmppruntDTO> {
    return this.http.patch<EmppruntDTO>(`${this.apiUrl}/Update/${id}`, Emp);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/Delete/${id}`);
  }
}
