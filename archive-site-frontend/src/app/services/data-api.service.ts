import { Injectable } from '@angular/core';
import { ODataEntityService, ODataServiceFactory, ODataSettings } from 'angular-odata';
import { environment } from 'src/environments/environment';
import User from 'src/app/models/user';
import Project from 'src/app/models/project';
import { Document } from 'src/app/models/document';
import Transcription from '../models/transcription';

export function odataSettingsFactory() {
  return new ODataSettings({
    serviceRootUrl: `${environment.apiUrl}/odata`,
    withCredentials: true
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

  get documentService(): ODataEntityService<Document> {
    return this._factory.entity<Document>('Documents', 'ArchiveSite.Data.Document');
  }

  get transcriptionService(): ODataEntityService<Transcription> {
    return this._factory.entity<Transcription>('Transcriptions', 'ArchiveSite.Data.Transcription');
  }
}
