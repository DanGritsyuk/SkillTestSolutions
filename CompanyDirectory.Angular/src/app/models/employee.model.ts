import { Department } from "./department.model";

export interface Employee {
  id: number;
  fullName: string;
  birthDate: Date;
  hireDate: Date;
  salary: number;
  departmentId: number;
  department?: Department;
}

// Вспомогательные функции (аналог методов C#)
export function getEmployeeAge(birthDate: Date): number {
  const today = new Date();
  let age = today.getFullYear() - birthDate.getFullYear();
  if (birthDate > new Date(today.setFullYear(today.getFullYear() - age))) {
    age--;
  }
  return age;
}

export function getYearsOfWork(hireDate: Date): number {
  const today = new Date();
  let years = today.getFullYear() - hireDate.getFullYear();
  if (hireDate > new Date(today.setFullYear(today.getFullYear() - years))) {
    years--;
  }
  return years;
}
