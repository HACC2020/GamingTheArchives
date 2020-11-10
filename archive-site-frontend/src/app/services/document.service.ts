import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ODataEntities } from 'angular-odata';
import { from, Observable, of } from 'rxjs';
import { map, tap } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { DOCUMENTS } from '../mock-data/mock-documents';
import AzureTranscription from '../models/azure-transcription';
import BoundingBox from '../models/bounding-box';
import { Document } from '../models/document';
import { Transcription } from '../models/transcription';
import { DataApiService } from './data-api.service';

@Injectable({
  providedIn: 'root'
})
export class DocumentService {

  constructor(private dataApiService: DataApiService,
              private httpClient: HttpClient) { }

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
    const documents$ = this.dataApiService.documentService.entities()
      .filter({ProjectId: projectId})
      .orderBy("id asc")
      .get();

    return documents$.pipe(
      map((odata: ODataEntities<Document>) => odata.entities)
    );
  }

  getDocumentByDocumentId(documentId: number): Observable<Document> {
    return this.dataApiService.documentService.entity(documentId).fetch();
  }

  getNextDocument(projectId: number, documentId: number): Observable<Document> {
    const nextDocument$ = this.dataApiService.documentService.entities()
      .filter({ ProjectId: projectId,  Id: { gt: documentId } })
      .top(1).get();

    return this.mapSingleDocument(nextDocument$);
  }

  getPreviousDocument(projectId: number, documentId: number): Observable<Document> {
    const previousDocument$ = this.dataApiService.documentService.entities()
      .filter({ ProjectId: projectId,  Id: { lt: documentId } })
      .orderBy("id desc").top(1).get();

    return this.mapSingleDocument(previousDocument$);
  }

  private mapSingleDocument(document$: Observable<ODataEntities<Document>>): Observable<Document> {
    return document$.pipe(
      map((odata: ODataEntities<Document>) => {
        if (odata.entities.length > 0) {
          return odata.entities.pop();
        }
        return null;
      })
    );
  }

  setTranscriptionByDocumentId(transcription: Transcription): void {
    // TODO complete implementation
    this.dataApiService.transcriptionService
      .create(transcription)
      .subscribe((result) => console.log(result));
  }

  getAzureTranscription(documentId: number): Observable<Array<AzureTranscription>> {
    const apiUrl = `${environment.apiUrl}/CognitiveService`;
    const cacheKey = `azureTranscription:${documentId}`;

    const cache = localStorage.getItem(cacheKey);
    if (cache !== null) {
      const cacheObject = JSON.parse(cache);
      const azureTranscriptions = new Array<AzureTranscription>();

      cacheObject.forEach(element => {
        const boundingBox = element.BoundingBox;
        const azureTranscription = new AzureTranscription(
          new BoundingBox(boundingBox.Top, boundingBox.Left, boundingBox.Bottom, boundingBox.Right),
          element.Text
        );

        azureTranscriptions.push(azureTranscription);
      });

      return of<Array<AzureTranscription>>(azureTranscriptions);
    }

    return this.httpClient.post<Array<AzureTranscription>>(apiUrl, documentId)
      .pipe(tap(transcription => {
        localStorage.setItem(cacheKey, JSON.stringify(transcription));
      }));
  }

}
