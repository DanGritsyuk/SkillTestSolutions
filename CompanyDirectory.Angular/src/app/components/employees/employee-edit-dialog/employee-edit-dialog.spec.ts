import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EmployeeEditDialog } from './employee-edit-dialog';

describe('EmployeeEditDialog', () => {
  let component: EmployeeEditDialog;
  let fixture: ComponentFixture<EmployeeEditDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [EmployeeEditDialog]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EmployeeEditDialog);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
