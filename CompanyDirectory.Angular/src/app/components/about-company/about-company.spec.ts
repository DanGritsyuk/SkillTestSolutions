import { Routes } from '@angular/router';
import { AboutCompanyComponent } from './about-company';
import { EmployeesComponent } from '../employees/employees';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'about',
    pathMatch: 'full'
  },
  {
    path: 'about',
    title: 'О компании',
    component: AboutCompanyComponent
  },
  {
    path: 'employees',
    title: 'Сотрудники',
    component: EmployeesComponent
  }
];
