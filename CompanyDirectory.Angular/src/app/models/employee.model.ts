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
