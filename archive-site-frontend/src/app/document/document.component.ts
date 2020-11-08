
/* Changing the purpose of this component. It will 
* be to show the document as a single image.
*
*/

import { Component, OnInit } from '@angular/core';
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

@Component({
  selector: 'app-document',
  templateUrl: './document.component.html',
  styleUrls: ['./document.component.scss']
})
export class DocumentComponent implements OnInit {

  document$: Observable<Document>;
  id: number;
  projectId: number;

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
    { displayText: 'Indexer Notes', id: 'indexer-notes', type: 'text' },
  ];
  constructor(
    private documentService: DocumentService,
    private route: ActivatedRoute,
    private location: Location,
    private _dataApi: DataApiService,
    private messageService: MessageService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.getDocument();
  }

  submit(): void {
    const dataSubmitted = this.getUserInput();
    console.log(dataSubmitted);
    // TODO: get actual userID from somewhere
    const userId = 999;
    // TODO: currently we only support single record transcription, so we're just wrapping the data
    //  from the form in a one item array.
    const transcription = new Transcription(0, this.id, userId, JSON.stringify([dataSubmitted]));
    this._dataApi.transcriptionService
      .create(transcription)
      .subscribe((result) => console.log(result));

    this.messageService.add('A new transcription has been added.')
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
    /*
    TODO: This and the coresponding go to previous are the more horrific things I've ever written in my life.
    Please someone fix them to be more angular like.
    PS: The reason we don't just increment is because we have no guarantee that next id will be in same project
    */
    let found = false;

    // TODO: we should figure out how to use .filter to just get ids > this.id
    // Seems doable but idk how with our library. This explains how to do it with odata in general: https://www.odata.org/documentation/odata-version-3-0/url-conventions/
    const observableDocs = this._dataApi.documentService.entities().filter({ projectId: this.projectId }).get();

    observableDocs.pipe(map((oe: ODataEntities<Document>) => oe.entities))
      .forEach(documentArray => {
        documentArray.forEach(document => {
          if (!found && document.Id > this.id) {
            found = true;
            this.goToDocument(document.Id);
          }
        });
      });
  }

  goToPrevious(): void {
    let found = false;
    const observableDocs = this._dataApi.documentService.entities().filter({ projectId: this.projectId })
      .orderBy('Id desc')
      .get();

    observableDocs.pipe(map((oe: ODataEntities<Document>) => oe.entities))
      .forEach(documentArray => {
        documentArray.forEach(document => {
          if (!found && document.Id < this.id) {
            found = true;
            this.goToDocument(document.Id);
          }
        });
      });
  }

  goToDocument(id: number): void {
    // TODO: Also someone please fix this. It cannot be the right way to do it.
    this.router.navigate(['transcribe', id]).then(() => { this.ngOnInit(); });
  }

  // Find the document to serve according to the URL.
  getDocument(): void {
    this.id = +this.route.snapshot.paramMap.get('id');
    this.document$ = this._dataApi.documentService.entity(this.id).fetch();
    // TODO: I assume projectId should/could be passed in from a higher level component?
    this.document$.toPromise().then(doc => this.projectId = doc.ProjectId);
  }
}
