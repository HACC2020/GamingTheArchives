
/* Changing the purpose of this component. It will 
* be to show the document as a single image.
*
*/

import { Component, OnInit } from '@angular/core';
import { Document } from '../models/document';
import { DocumentService } from '../services/document.service';
import { ActivatedRoute } from '@angular/router';
import { Location } from '@angular/common';
import { DataApiService } from '../services/data-api.service';
import { Observable } from 'rxjs';
import Transcription from '../models/transcription';
import { MessageService } from '../services/message.service';

@Component({
  selector: 'app-document',
  templateUrl: './document.component.html',
  styleUrls: ['./document.component.scss']
})
export class DocumentComponent implements OnInit {

  document$: Observable<Document>;
  id: number;

  fields = [
    { displayText: 'Title', id: 'title', type: 'text' },
    { displayText: 'Description', id: 'description', type: 'text' },
    { displayText: 'Photo Credit', id: 'photo-credit', type: 'text' },
    { displayText: 'From Collection', id: 'from-collection', type: 'text' },
    { displayText: 'Gift of', id: 'gift-of', type: 'text' },
    { displayText: 'Received Date', id: 'recieved-negative', type: 'date' },
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
    private messageService: MessageService
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

  // Find the document to serve according to the URL.
  getDocument(): void {
    this.id = +this.route.snapshot.paramMap.get('id');
    this.document$ = this._dataApi.documentService.entity(this.id).fetch();
  }
}
