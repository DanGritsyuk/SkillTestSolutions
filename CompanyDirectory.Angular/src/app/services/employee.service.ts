import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Employee } from '../models/employee.model';

@Injectable({ providedIn: 'root' })
export class EmployeeService {
  private apiUrl = 'api/employees';

  constructor(private http: HttpClient) { }

  // Получение всех сотрудников
  getAll(): Observable<Employee[]> {
    return this.http.get<Employee[]>(this.apiUrl);
  }

  // Получение сотрудника по ID
  getById(id: number): Observable<Employee> {
    return this.http.get<Employee>(`${this.apiUrl}/${id}`);
  }

  // Создание сотрудника
  create(employee: Omit<Employee, 'id'>): Observable<Employee> {
    return this.http.post<Employee>(this.apiUrl, employee);
  }

  // Обновление сотрудника
  update(id: number, employee: Employee): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, employee);
  }

  // Удаление сотрудника (добавляем этот метод)
  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  // Дополнительные методы из ТЗ
  getHighSalary(): Observable<Employee[]> {
    return this.http.get<Employee[]>(`${this.apiUrl}/high-salary`);
  }

  deleteRetired(): Observable<{ count: number }> {
    return this.http.delete<{ count: number }>(`${this.apiUrl}/retired`);
  }

  adjustSalaries(): Observable<{ count: number }> {
    return this.http.put<{ count: number }>(`${this.apiUrl}/adjust-salaries`, null);
  }
}
