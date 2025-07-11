import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, AbstractControl } from '@angular/forms';
import { Employee } from '../../../models/employee.model';
import { Department } from '../../../models/department.model';
import { DepartmentService } from '../../../services/department.service';

import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDialogModule, MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';

@Component({
  selector: 'app-employee-edit-dialog',
  standalone: true,
  templateUrl: './employee-edit-dialog.html',
  styleUrls: ['./employee-edit-dialog.css'],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatDialogModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
  ],
})
export class EmployeeEditDialog implements OnInit {
  form!: FormGroup;
  isEditMode = false;
  departments: Department[] = [];

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<EmployeeEditDialog>,
    @Inject(MAT_DIALOG_DATA) public data: { mode: 'add' | 'edit'; employee: Employee },
    private departmentService: DepartmentService
  ) { }

  ngOnInit(): void {
    this.isEditMode = !!this.data;

    this.form = this.fb.group({
      fullName: [this.data?.employee.fullName || '', Validators.required],
      birthDate: [this.data?.employee.birthDate || '', Validators.required],
      hireDate: [this.data?.employee.hireDate || '', Validators.required],
      salary: [this.data?.employee.salary ?? 0, [Validators.required, Validators.min(0)]],
      departmentId: [this.data?.employee.departmentId || '', Validators.required]
    });

    this.departmentService.getAll().subscribe(departments => {
      this.departments = departments;
    });
  }

  get fullName(): AbstractControl | null {
    return this.form.get('fullName');
  }

  get birthDate(): AbstractControl | null {
    return this.form.get('birthDate');
  }

  get hireDate(): AbstractControl | null {
    return this.form.get('hireDate');
  }

  get salary(): AbstractControl | null {
    return this.form.get('salary');
  }

  get departmentId(): AbstractControl | null {
    return this.form.get('departmentId');
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const result: Employee = {
      ...this.data.employee,
      ...this.form.value
    };

    this.dialogRef.close(result);
  }

  cancel(): void {
    this.dialogRef.close();
  }
}
