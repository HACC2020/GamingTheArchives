//this component was created to display the activity feed, but its
//use has been eliminated by the message 


import { Component, OnInit } from '@angular/core';
import { DataApiService } from 'src/app/services/data-api.service';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import Project from 'src/app/models/project';
import { Document } from 'src/app/models/document';
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
  urls?: string;

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
      .subscribe(result => {
        this.createDocumentsFromUrls(result.Id)
          .then(() => {
            this.activeModal.close(result);
          });
      });
    this.messageService.add('A new project has been added.');
  }

  createDocumentsFromUrls(projectId: number): Promise<any> {
    const promises = [];
    this.urls.split('\n')
    .forEach(url => {
      url = url.trim();
      // This is to avoid empty strings or a stray character added
      if (url.length > 5) {
        promises.push(this.createDocumentFromUrl(projectId, url));
      }
    });
    return Promise.all(promises);
  }

  createDocumentFromUrl(projectId: number, url: string): Promise<any> {
    return this._dataApi.documentService.create(new Document(0, projectId, 'na', url)).toPromise();
  }
}
