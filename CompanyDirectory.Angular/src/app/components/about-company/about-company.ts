import { Component } from '@angular/core';

@Component({
  selector: 'app-about-company',
  templateUrl: './about-company.html',
  styleUrls: ['./about-company.css']
})
export class AboutCompanyComponent {
  technologies = [
    { name: 'Backend', value: 'ASP.NET Core 8 (C#)' },
    { name: 'Frontend', value: 'Angular 20' },
    { name: 'Database', value: 'MS SQL Server' },
    { name: 'UI Framework', value: 'Angular Material' }
  ];

  features = [
    {
      title: 'Стартовая страница',
      description: 'Статичное описание проекта с техническими деталями',
      icon: 'info'
    },
    {
      title: 'Управление сотрудниками',
      description: 'Полнофункциональная таблица с CRUD-операциями',
      icon: 'group'
    }
  ];
}
