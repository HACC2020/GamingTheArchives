import { Component, Input, OnInit, Output, ViewChild } from '@angular/core';
import { EventEmitter } from '@angular/core';
import { NgForm } from '@angular/forms';

@Component({
  selector: 'app-project-details-form',
  templateUrl: './project-details-form.component.html',
  styleUrls: ['./project-details-form.component.scss']
})
export class ProjectDetailsFormComponent implements OnInit {
  @Input() name: string;
  @Output() nameChange = new EventEmitter<string>();

  @Input() description?: string;
  @Output() descriptionChange = new EventEmitter<string>();

  @Input() disabled: boolean = false;

  @ViewChild('projectForm', { static: true }) protected projectForm: NgForm;

  @Output() change = new EventEmitter();

  get invalid(): boolean {
    return this.projectForm.form.invalid;
  }

  constructor() { }

  ngOnInit(): void {
  }

  onFormChange(value: any) {
    this.change.emit(value);
  }
}
