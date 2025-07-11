import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { EmployeeService } from '../../services/employee.service';
import { Employee } from '../../models/employee.model';
import { EmployeeEditDialog } from './employee-edit-dialog/employee-edit-dialog';
import { ConfirmDialog } from '../shared/confirm-dialog/confirm-dialog';
import { DepartmentService } from '../../services/department.service';
import { Department } from '../../models/department.model';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-employees',
  standalone: true,
  templateUrl: './employees.html',
  styleUrls: ['./employees.css'],
  imports: [
    CommonModule,
    MatTableModule,
    MatSortModule,
    MatDialogModule,
    MatButtonModule,
    MatIconModule,
    EmployeeEditDialog,
    ConfirmDialog
  ],
})
export class EmployeesComponent implements OnInit {
  displayedColumns: string[] = [
    'department',
    'fullName',
    'birthDate',
    'hireDate',
    'salary',
    'actions'
  ];

  dataSource = new MatTableDataSource<Employee>();
  departments: Department[] = [];

  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    private employeeService: EmployeeService,
    private departmentService: DepartmentService,
    private dialog: MatDialog
  ) { }

  ngOnInit(): void {
    this.loadEmployees();
    this.loadDepartments();
  }

  loadEmployees(): void {
    this.employeeService.getAll().subscribe(employees => {
      this.dataSource.data = employees;
      this.dataSource.sort = this.sort;
    });
  }

  loadDepartments(): void {
    this.departmentService.getAll().subscribe(departments => {
      this.departments = departments;
    });
  }

  openEditDialog(employee?: Employee): void {
    const dialogRef = this.dialog.open(EmployeeEditDialog, {
      width: '600px',
      data: {
        employee: employee || null,
        departments: this.departments
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) this.loadEmployees();
    });
  }

  deleteEmployee(id: number): void {
    const dialogRef = this.dialog.open(ConfirmDialog, {
      data: {
        title: 'Удаление сотрудника',
        message: 'Вы уверены, что хотите удалить этого сотрудника?'
      }
    });

    dialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        this.employeeService.delete(id).subscribe(() => {
          this.loadEmployees();
        });
      }
    });
  }

  applyFilter(event: Event, column: string): void {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filterPredicate = (data: Employee, filter: string) => {
      const columnValue = column === 'department'
        ? data.department?.name
        : (data as any)[column];
      return columnValue.toString().toLowerCase().includes(filter.toLowerCase());
    };
    this.dataSource.filter = filterValue.trim().toLowerCase();
  }
}
