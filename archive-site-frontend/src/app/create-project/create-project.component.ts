//this component was created to display the activity feed, but its
//use has been eliminated by the message 


import { Component, OnInit, ViewChild } from '@angular/core';
import { DataApiService } from 'src/app/services/data-api.service';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import Project from 'src/app/models/project';
import { MessageService } from '../services/message.service';

@Component({
  selector: 'app-create-project-modal',
  templateUrl: './create-project.component.html',
  styleUrls: ['./create-project.component.scss']
})
export class CreateProjectModal implements OnInit {
  isSaving = false;

  name: string;
  description?: string;

  constructor(
    public activeModal: NgbActiveModal,
    private _dataApi: DataApiService,
    private messageService: MessageService) {
  }

  ngOnInit(): void {
  }

  saveProject(): void {
    this.isSaving = true;
    this._dataApi.projectService
      .create(new Project(0, this.name, this.description, 'http://todo/', true))
      .subscribe((result) => this.activeModal.close(result));
    this.messageService.add("A new project has been added.")
  }

}
