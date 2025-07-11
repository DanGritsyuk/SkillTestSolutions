import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Department } from '../models/department.model';

@Injectable({
  providedIn: 'root'
})
export class DepartmentService {
  private apiUrl = 'api/departments';

  constructor(private http: HttpClient) { }

  // Получение всех отделов
  getAll(): Observable<Department[]> {
    return this.http.get<Department[]>(this.apiUrl);
  }

  // Получение отдела по ID
  getById(id: number): Observable<Department> {
    return this.http.get<Department>(`${this.apiUrl}/${id}`);
  }

  // Создание отдела
  create(department: Omit<Department, 'id'>): Observable<Department> {
    return this.http.post<Department>(this.apiUrl, department);
  }

  // Обновление отдела
  update(id: number, department: Department): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, department);
  }

  // Удаление отдела
  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  // Дополнительные методы по необходимости
  searchByName(name: string): Observable<Department[]> {
    return this.http.get<Department[]>(`${this.apiUrl}/search`, {
      params: { name }
    });
  }
}
