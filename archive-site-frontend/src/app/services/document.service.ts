import { Injectable } from '@angular/core';
import { ODataEntities } from 'angular-odata';
import { Observable, of } from 'rxjs';
import { map, tap } from 'rxjs/operators';
import { DOCUMENTS } from '../mock-data/mock-documents';
import { Document } from '../models/document';
import { DataApiService } from './data-api.service';
@Injectable({
  providedIn: 'root'
})
export class DocumentService {

  constructor(private _dataApiService: DataApiService) { }

  /**
   * @deprecated stub method to make ui work; drop and seed database to populate data
   */
  getAllDocuments(): Observable<Document[]> {
    return of(DOCUMENTS);
  }

  /**
   * @deprecated stub method to make ui work; drop and seed database to populate data
   * @param id 
   */
  getDocument(id: number): Observable<Document> {
    return of(DOCUMENTS.find(doc => doc.Id === id));
  }

  getDocumentsByProjectId(projectId: number): Observable<Document[]> {
    var documents$ = this._dataApiService.documentService.entities()
      .filter({ProjectId: projectId})
      .orderBy("id asc")
      .get();

    return documents$.pipe(
      map((odata: ODataEntities<Document>) => odata.entities)
    );
  }

  getDocumentByDocumentId(documentId: number): Observable<Document> {
    return this._dataApiService.documentService.entity(documentId).fetch();
  }

}
