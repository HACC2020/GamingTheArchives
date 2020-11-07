import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DocTranscriberComponent } from './doc-transcriber.component';

describe('DocTranscriberComponent', () => {
  let component: DocTranscriberComponent;
  let fixture: ComponentFixture<DocTranscriberComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DocTranscriberComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DocTranscriberComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
