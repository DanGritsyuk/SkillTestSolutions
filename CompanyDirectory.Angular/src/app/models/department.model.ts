import { Employee } from "./employee.model";

export interface Department {
  id: number;
  name: string;
  employees?: Employee[];  // Опциональная связь с сотрудниками
}
