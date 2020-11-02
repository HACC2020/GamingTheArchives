import { Injectable } from '@angular/core';
import { ODataEntityService, ODataServiceFactory, ODataSettings } from 'angular-odata';
import { environment } from 'src/environments/environment';
import User from 'src/app/models/user';
import Project from 'src/app/models/project';

export function odataSettingsFactory() {
  return new ODataSettings({
    serviceRootUrl: `${environment.apiUrl}/odata`
  });
}

@Injectable({
  providedIn: 'root'
})
export class DataApiService {
  constructor(private _factory: ODataServiceFactory) { }

  get userService(): ODataEntityService<User> {
    return this._factory.entity<User>('Users', 'ArchiveSite.Data.User');
  }

  get projectService(): ODataEntityService<Project> {
    return this._factory.entity<Project>('Projects', 'ArchiveSite.Data.Project');
  }
}
