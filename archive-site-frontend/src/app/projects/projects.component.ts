import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ODataEntities } from 'angular-odata';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { CreateProjectModal } from 'src/app/create-project/create-project.component';
import Project from 'src/app/models/project';
import { DataApiService } from 'src/app/services/data-api.service';


@Component({
  selector: 'app-projects',
  templateUrl: './projects.component.html',
  styleUrls: ['./projects.component.scss']
})
export class ProjectsComponent implements OnInit {
  showIntro: boolean;
  projects$: Observable<Project[]>;

  constructor(
    private _router: Router,
    private _route: ActivatedRoute,
    private _modalService: NgbModal,
    private _dataApi: DataApiService) { }

  ngOnInit(): void {
    this.showIntro = !!this._route.snapshot.queryParams['intro'];
    this.refreshProjects();
  }

  hideIntro(): void {
    this.showIntro = false;
    history.replaceState(history.state, document.title, '/projects');
  }

  async createProject(): Promise<void> {
    const modalRef =
      this._modalService.open(
        CreateProjectModal,
        {
          centered: true,
          size: 'lg'
        }
      );
    const result = await modalRef.result.catch(err => undefined);

    if (result) {
      this.refreshProjects();
    }
  }

  private refreshProjects() {
    this.projects$ =
      this._dataApi.projectService.entities()
        .filter({ Active: true })
        .get()
        .pipe(map((oe: ODataEntities<Project>) => oe.entities));
  }
}
