import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ODataEntities } from 'angular-odata';
import { from, Observable, of } from 'rxjs';
import { map, mergeMap, tap } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { DOCUMENTS } from '../mock-data/mock-documents';
import AzureTranscription from '../models/azure-transcription';
import BoundingBox from '../models/bounding-box';
import { Document } from '../models/document';
import { Transcription } from '../models/transcription';
import { DataApiService } from './data-api.service';
import { UserContextService } from 'src/app/services/user-context-service';

@Injectable({
  providedIn: 'root'
})
export class DocumentService {
  constructor(
    private _dataApiService: DataApiService,
    private _userContext: UserContextService,
    private _httpClient: HttpClient) {
  }

  getDocumentsByProjectId(projectId: number): Observable<Document[]> {
    const documents$ = this._dataApiService.documentService.entities()
      .filter({ ProjectId: projectId })
      .orderBy("id asc")
      .get();

    return documents$.pipe(
      map((odata: ODataEntities<Document>) => odata.entities)
    );
  }

  getDocumentByDocumentId(documentId: number): Observable<Document> {
    return this._dataApiService.documentService.entity(documentId).fetch();
  }

  getNextDocument(projectId: number, documentId: number): Observable<Document> {
    const nextDocument$ = this._dataApiService.documentService.entities()
      .filter({ ProjectId: projectId, Id: { gt: documentId } })
      .top(1).get();

    return DocumentService.mapSingleDocument(nextDocument$);
  }

  getPreviousDocument(projectId: number, documentId: number): Observable<Document> {
    const previousDocument$ = this._dataApiService.documentService.entities()
      .filter({ ProjectId: projectId, Id: { lt: documentId } })
      .orderBy("id desc").top(1).get();

    return DocumentService.mapSingleDocument(previousDocument$);
  }

  getCurrentUserTranscription(documentId: number): Observable<Transcription> {
    return this._userContext.user$
      .pipe(
        mergeMap(user => {
          if (!user) {
            console.warn('not logged in');
            return;
            // throw new Error('User not logged in.');
          }

          console.log('Checking for existing transcriptions.');

          return this._dataApiService.transcriptionService.entities()
            .filter({ DocumentId: documentId, UserId: user.Id })
            .get()
        }),
        map(t => t.entities && t.entities[0]));
  }

  async saveTranscription(transcription: Transcription): Promise<Transcription> {
    let saved: Transcription;
    if (transcription.Id) {
      saved =
        await this._dataApiService.transcriptionService
          .entity(transcription.Id)
          .put(transcription)
          .pipe(map(t => t.entity))
          .toPromise();
    } else {
      saved =
        await this._dataApiService.transcriptionService
          .create(transcription)
          .toPromise();
    }

    console.log('Saved transcriptions: ' + JSON.stringify(saved));
    return saved;
  }

  private static mapSingleDocument(document$: Observable<ODataEntities<Document>>): Observable<Document> {
    return document$.pipe(
      map((odata: ODataEntities<Document>) => {
        if (odata.entities.length > 0) {
          return odata.entities.pop();
        }
        return null;
      })
    );
  }

  getAzureTranscription(documentId: number): Observable<Array<AzureTranscription>> {
    const apiUrl = `${environment.apiUrl}/CognitiveService`;
    const cacheKey = `azureTranscription:${documentId}`;

    const cacheObject = (data => data ? JSON.parse(data) : undefined)(localStorage.getItem(cacheKey));
    if (cacheObject) {
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

    return this._httpClient.post<Array<AzureTranscription>>(apiUrl, documentId)
      .pipe(tap(transcription => {
        localStorage.setItem(cacheKey, JSON.stringify(transcription));
      }));
  }
}
