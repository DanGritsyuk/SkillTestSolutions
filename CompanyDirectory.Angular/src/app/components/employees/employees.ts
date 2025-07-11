import { Component, OnInit } from '@angular/core';
import { Employee } from '../../models/employee.model';
import { EmployeeService } from '../../services/employee.service';
import { CommonModule, DatePipe, DecimalPipe } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { EmployeeEditDialog } from '../employees/employee-edit-dialog/employee-edit-dialog';
import { ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatSelectModule } from '@angular/material/select';

@Component({
  selector: 'app-employees',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    DatePipe,
    DecimalPipe,
    MatButtonModule,
    MatIconModule,
    MatDialogModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatSelectModule,
    EmployeeEditDialog
  ],
  templateUrl: './employees.html',
})
export class EmployeesComponent implements OnInit {
  employees: Employee[] = [];

  constructor(
    public employeeService: EmployeeService,
    private dialog: MatDialog
  ) { }

  ngOnInit(): void {
    this.loadEmployees();
  }

  loadEmployees(): void {
    this.employeeService.getAll().subscribe({
      next: (employees: Employee[]) => this.employees = employees,
      error: (err: any) => console.error('Ошибка при загрузке сотрудников:', err)
    });
  }

  getHighSalary(): void {
    this.employeeService.getHighSalary().subscribe({
      next: (employees: Employee[]) => this.employees = employees,
      error: (err: any) => console.error('Ошибка при получении высоких зарплат:', err)
    });
  }

  deleteRetired(): void {
    this.employeeService.deleteRetired().subscribe({
      next: () => this.loadEmployees(),
      error: (err: any) => console.error('Ошибка при удалении пенсионеров:', err)
    });
  }

  adjustSalaries(): void {
    this.employeeService.adjustSalaries().subscribe({
      next: () => this.loadEmployees(),
      error: (err: any) => console.error('Ошибка при повышении зарплат:', err)
    });
  }

  openAddDialog(): void {
    const dialogRef = this.dialog.open(EmployeeEditDialog, {
      width: '400px',
      data: { mode: 'add' }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === 'updated') {
        this.loadEmployees();
      }
    });
  }

  openEditDialog(employee: Employee): void {
    const dialogRef = this.dialog.open(EmployeeEditDialog, {
      width: '400px',
      data: { mode: 'edit', employee }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === 'updated') {
        this.loadEmployees();
      }
    });
  }

  deleteEmployee(employee: Employee): void {
    if (confirm(`Удалить сотрудника ${employee.fullName}?`)) {
      this.employeeService.delete(employee.id!).subscribe(() => {
        this.loadEmployees();
      });
    }
  }
}
