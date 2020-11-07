import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { DataApiService } from 'src/app/services/data-api.service';
import { Observable } from 'rxjs';
import Project from 'src/app/models/project';
import { ODataEntities } from 'angular-odata';
import { map } from 'rxjs/operators';
import {Document} from 'src/app/models/document'

@Component({
  selector: 'app-project-detail',
  templateUrl: './project-detail.component.html',
  styleUrls: ['./project-detail.component.scss']
})
export class ProjectDetailComponent implements OnInit {
  project$: Observable<Project>;
  documents$: Observable<Document[]>;

  constructor(
    private _route: ActivatedRoute,
    private _dataApi: DataApiService) { }

   projectId: number;

  ngOnInit(): void {
    this.projectId = Number(this._route.snapshot.params['id']);
    this.project$ = this._dataApi.projectService.entity(this.projectId).fetch()
    this.documents$ = this._dataApi.documentService.entities()
    .filter({ ProjectId: this.projectId })
    .get()
    .pipe(map((oe: ODataEntities<Document>) => oe.entities))
  }

}
