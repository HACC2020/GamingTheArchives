import { Component, OnInit, ViewChild } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { DataApiService } from 'src/app/services/data-api.service';
import { Project } from 'src/app/models/project';
import { Document } from 'src/app/models/document';
import { MessageService } from 'src/app/services/message.service';
import { ProjectDetailsFormComponent } from 'src/app/project-details-form/project-details-form.component';
import { Field } from 'src/app/models/field';
import { environment } from 'src/environments/environment';
import * as _ from 'lodash';
import { A } from '@angular/cdk/keycodes';
import { map } from 'rxjs/operators';

export enum CreateProjectModalStep {
  ProjectDetails,
  UploadRtpXml,
  EditFieldList,
  UploadProjectDocuments,
}

@Component({
  selector: 'app-create-project-modal',
  templateUrl: './create-project.component.html',
  styleUrls: ['./create-project.component.scss']
})
export class CreateProjectModal implements OnInit {
  // Make our enum type accessible to the template
  CreateProjectModalStep = CreateProjectModalStep;

  isSaving = false;

  name: string;
  description?: string;
  fields: Field[];
  urls?: string;

  step: CreateProjectModalStep = CreateProjectModalStep.ProjectDetails;

  isReady: boolean = false;
  errorMessage: string;

  @ViewChild('projectDetailsForm') protected projectDetailsForm: ProjectDetailsFormComponent;

  private _rtpXmlFile: File;
  private _project: Project;

  get nextButtonText(): string {
    switch (this.step) {
      case CreateProjectModalStep.ProjectDetails:
        return "Next";
      case CreateProjectModalStep.UploadRtpXml:
        if (this._rtpXmlFile) {
          return "Next";
        } else {
          return "Skip";
        }
      case CreateProjectModalStep.EditFieldList:
        return "Next";
      case CreateProjectModalStep.UploadProjectDocuments:
        return "Save";
    }
  }

  get backButtonText(): string {
    switch (this.step) {
      case CreateProjectModalStep.ProjectDetails:
        return 'Cancel';
      default:
        return 'Back';
    }
  }

  constructor(
    public activeModal: NgbActiveModal,
    private _dataApi: DataApiService,
    private messageService: MessageService) {
  }

  ngOnInit(): void {
  }

  back(): void {
    this.errorMessage = '';
    switch (this.step) {
      case CreateProjectModalStep.ProjectDetails:
        this.activeModal.dismiss('Cancel');
        break;
      case CreateProjectModalStep.UploadRtpXml:
        this.step = CreateProjectModalStep.ProjectDetails;
        break;
      case CreateProjectModalStep.EditFieldList:
        this.step = CreateProjectModalStep.UploadRtpXml;
        break;
      case CreateProjectModalStep.UploadProjectDocuments:
        this.step = CreateProjectModalStep.EditFieldList;
        break;
    }
  }

  async next(): Promise<void> {
    this.errorMessage = '';
    this.isSaving = true;
    try {
      switch (this.step) {
        case CreateProjectModalStep.ProjectDetails:
          if (!this._project) {
            this._project = await this._dataApi.projectService
              .create(new Project(0, this.name, this.description, undefined, false))
              .toPromise();
            this.messageService.add(`A new project has been added: ${this.name}`);
          } else {
            this._project =
              await this._dataApi.projectService
                .entity(this._project.Id)
                .put(this._project)
                .pipe(map(p => p.entity))
                .toPromise();
          }
          this.step = CreateProjectModalStep.UploadRtpXml;
          break;

        case CreateProjectModalStep.UploadRtpXml:
          if (this._rtpXmlFile) {
            let formData = new FormData();
            formData.append('rtpXml', this._rtpXmlFile);

            let response =
              await fetch(
                `${environment.apiUrl}/odata/Fields/parse-rtp-xml`,
                {
                  method: 'POST',
                  body: formData,
                  credentials: environment.apiCredentialMode
                }
              );

            if (response.ok) {
              let fieldParserResults = await response.json();
              for (let w of fieldParserResults.Warnings) {
                console.warn(w);
              }

              this.fields =
                _.orderBy(fieldParserResults.Fields.map(Field.fromPayload), 'Index')
                  .map((f: Field, ix) => {
                    // Make sure all fields have a contiguous set of indices.
                    f.Index = ix;
                    return f;
                  });

            } else {
              if (response.status === 400 && response.headers.get('Content-Length')) {
                let problem = await response.json();
                this.errorMessage = problem.Title || 'Unable to import field list.';
                console.warn(problem);
              } else {
                this.errorMessage = 'Unable to import field list.';
              }
            }
          } else if (!this.fields) {
            this.fields = [];
          }

          this.step = CreateProjectModalStep.EditFieldList;
          break;

        case CreateProjectModalStep.EditFieldList:
          let createFieldPromises: Promise<Field>[] = [];
          let updateFieldPromises: Promise<Field>[] = [];
          for (let field of this.fields) {
            if (field.Id) {
              updateFieldPromises.push(
                this._dataApi.fieldService
                  .entity(field.Id)
                  .put(field)
                  .pipe(map(f => f.entity))
                  .toPromise()
              );
            } else {
              field.ProjectId = this._project.Id;
              createFieldPromises.push(this._dataApi.fieldService.create(field).toPromise());
            }
          }

          let createdFields = await Promise.all(createFieldPromises);
          let updatedFields = await Promise.all(updateFieldPromises);
          this.fields = _.orderBy(createdFields.concat(updatedFields), 'Index');
          this.step = CreateProjectModalStep.UploadProjectDocuments;
          break;

        case CreateProjectModalStep.UploadProjectDocuments:
          await this.createDocumentsFromUrls(this._project.Id);
          this.activeModal.close({ project: this._project, fields: this.fields });
          break;
      }
    } finally {
      this.isSaving = false;
    }
  }

  createDocumentsFromUrls(projectId: number): Promise<Document[]> {
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

  createDocumentFromUrl(projectId: number, url: string): Promise<Document> {
    return (
      this._dataApi.documentService
        .create(new Document(0, projectId, 'na', url))
        .toPromise());
  }

  onFileChange(event: Event) {
    const target = <HTMLInputElement> event.target;
    if (target.files && target.files.length) {
      this._rtpXmlFile = target.files[0];
    } else {
      this._rtpXmlFile = undefined;
    }
  }

  onProjectDetailsFormChange() {
    if (this.step === CreateProjectModalStep.ProjectDetails) {
      this.isReady = !this.projectDetailsForm.invalid;
    }
  }
}
