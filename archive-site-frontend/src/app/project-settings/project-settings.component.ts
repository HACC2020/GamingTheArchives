import { Component, OnInit } from '@angular/core';
import { Field } from 'src/app/models/field';
import { Project } from 'src/app/models/project';
import { DataApiService } from 'src/app/services/data-api.service';
import { ActivatedRoute } from '@angular/router';
import * as _ from 'lodash';
import { map } from 'rxjs/operators';

@Component({
  selector: 'app-project-settings',
  templateUrl: './project-settings.component.html',
  styleUrls: ['./project-settings.component.scss']
})
export class ProjectSettingsComponent implements OnInit {
  originalProject: Project;
  originalFields: Field[];

  projectId: number;
  project: Project;
  fields: Field[];

  doneLoading: boolean;
  isSaving: boolean;

  constructor(
    private _route: ActivatedRoute,
    private _dataApi: DataApiService) {
  }

  async ngOnInit(): Promise<void> {
    this.projectId = Number(this._route.snapshot.params['id']);
    let [project, fields] = await Promise.all([
      this._dataApi.projectService.entity(this.projectId).fetch().toPromise(),
      this._dataApi.fieldService.entities().filter({ ProjectId: this.projectId }).all().toPromise()
    ]);

    this.originalProject = project;
    this.originalFields = _.orderBy(fields, ['Index']);

    this.project = Project.fromPayload(project);
    this.fields = this.originalFields.map(Field.fromPayload);

    this.doneLoading = true;
  }

  undo(): void {
    this.project = Project.fromPayload(this.originalProject);
    this.fields = this.originalFields.map(Field.fromPayload);
  }

  async save(): Promise<void> {
    this.originalProject =
      await this._dataApi.projectService
        .entity(this.project.Id)
        .put(this.project)
        .pipe(map(p => p.entity))
        .toPromise();
    this.project = Project.fromPayload(this.originalProject);

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
        field.ProjectId = this.project.Id;
        createFieldPromises.push(this._dataApi.fieldService.create(field).toPromise());
      }
    }

    let createdFields = await Promise.all(createFieldPromises);
    let updatedFields = await Promise.all(updateFieldPromises);
    this.originalFields = _.orderBy(createdFields.concat(updatedFields), 'Index');

    this.fields = this.originalFields.map(Field.fromPayload);
  }

  onProjectDetailsFormChange() {

  }

}
