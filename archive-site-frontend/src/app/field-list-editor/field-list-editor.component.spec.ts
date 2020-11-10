import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FieldListEditorComponent } from './field-list-editor.component';

describe('FieldListEditorComponent', () => {
  let component: FieldListEditorComponent;
  let fixture: ComponentFixture<FieldListEditorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ FieldListEditorComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(FieldListEditorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
