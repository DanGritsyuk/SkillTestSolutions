import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AboutCompanyComponent } from './components/about-company/about-company';
import { EmployeesComponent } from './components/employees/employees';

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

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
