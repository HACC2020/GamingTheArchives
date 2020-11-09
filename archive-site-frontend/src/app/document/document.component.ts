
/* Changing the purpose of this component. It will 
* be to show the document as a single image.
*
*/

import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { Document } from '../models/document';
import { DocumentService } from '../services/document.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Location } from '@angular/common';
import { DataApiService } from '../services/data-api.service';
import { Observable } from 'rxjs';
import Transcription from '../models/transcription';
import { MessageService } from '../services/message.service';
import { ODataEntities } from 'angular-odata';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-document',
  templateUrl: './document.component.html',
  styleUrls: ['./document.component.scss']
})
export class DocumentComponent implements OnInit {

  document$: Observable<Document>;
  documentImageUrl: string;

  projectId: number;
  documentId: number;

  fields = [
    { displayText: 'Title', id: 'title', type: 'text' },
    { displayText: 'Description', id: 'description', type: 'text' },
    { displayText: 'Photo Credit', id: 'photo-credit', type: 'text' },
    { displayText: 'From Collection', id: 'from-collection', type: 'text' },
    { displayText: 'Gift of', id: 'gift-of', type: 'text' },
    { displayText: 'Received Date', id: 'received-negative', type: 'date' },
    { displayText: 'Negative Number', id: 'negative-number', type: 'text' },
    { displayText: 'Negative Size', id: 'negative-size', type: 'text' },
    { displayText: 'Display Date', id: 'display-date', type: 'date' },
    { displayText: 'Start Date', id: 'start-date', type: 'date' },
    { displayText: 'End Date', id: 'end-date', type: 'date' },
    { displayText: 'Indexer Notes', id: 'indexer-notes', type: 'textarea' },
  ];

  constructor(
    private documentService: DocumentService,
    private messageService: MessageService,
    private route: ActivatedRoute,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.getDocument();
  }

  // Find the document to serve according to the URL.
  getDocument(): void {
    this.route.params.subscribe(params => {
      this.projectId = +params.projectId;
      this.documentId = +params.documentId;

      this.document$ = this.documentService.getDocumentByDocumentId(this.documentId);
    });

    this.document$.subscribe(document => {
      this.documentImageUrl = `${environment.apiUrl}/DocumentImage/${document.Id}`;
    });
  }

  @ViewChild('documentImage') documentImage: ElementRef;

  onImageLoaded(event: Event): void {
    console.log(`image width ${this.documentImage.nativeElement.width}`);
    console.log(`image height ${this.documentImage.nativeElement.height}`);
    console.log(event);
  }

  submit(): void {
    const dataSubmitted = this.getUserInput();
    console.log(dataSubmitted);
    
    // TODO: get actual userID from somewhere
    const userId = 999;

    // TODO: currently we only support single record transcription, so we're just wrapping the data
    //  from the form in a one item array.
    const transcription = new Transcription(0, this.documentId, userId, JSON.stringify([dataSubmitted]));
    this.documentService.setTranscriptionByDocumentId(transcription);

    this.messageService.add('A new transcription has been added.');
  }

  getUserInput(): any {
    return Array.from(document.querySelectorAll('input'))
      .reduce(
        (acc, field) => {
          acc[field.id] = field.value;
          return acc;
        },
        {}
      );
  }

  goToNext(): void {
    const nextDocument$ = this.documentService.getNextDocument(this.projectId, this.documentId);
    nextDocument$.subscribe(document => this.goToDocument(document));
  }

  goToPrevious(): void {
    const previousDocument$ = this.documentService.getPreviousDocument(this.projectId, this.documentId);
    previousDocument$.subscribe(document => this.goToDocument(document));
  }

  goToDocument(document: Document): void {
    this.router.navigate(['/transcribe', document.ProjectId, document.Id]);
  }

}
