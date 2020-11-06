import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { DOCUMENTS } from '../mock-data/mock-documents';
import { Document } from '../models/document';
@Injectable({
  providedIn: 'root'
})
export class DocumentService {

  getDocuments(): Observable<Document[]> {
    return of(DOCUMENTS);
  }

  constructor() { }
}
