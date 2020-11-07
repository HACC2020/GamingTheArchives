
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

@Component({
  selector: 'app-document',
  templateUrl: './document.component.html',
  styleUrls: ['./document.component.scss']
})
export class DocumentComponent implements OnInit {

  document$: Observable<Document>;
  fields = [
    { displayText: "Title", id:"title", type: "text" },
    { displayText: "Description", id:"description", type: "text" },
    { displayText: "Photo Credit", id:"photo-credit", type: "text" },
    { displayText: "From Collection", id:"from-collection", type: "text" },
    { displayText: "Gift of", id:"gift-of", type: "text" },
    { displayText: "Received Date", id:"recieved-negative", type: "date" },
    { displayText: "Negative Number", id:"negative-number", type: "text" },
    { displayText: "Negative Size", id:"negative-size", type: "text" },
    { displayText: "Display Date", id:"display-date", type: "date" },
    { displayText: "Start Date", id:"start-date", type: "date" },
    { displayText: "End Date", id:"end-date", type: "date" },
    { displayText: "Indexer Notes", id:"indexer-notes", type: "text" },
  ]
  constructor(
    private documentService: DocumentService,
    private route: ActivatedRoute,
    private location: Location,
    private _dataApi: DataApiService
  ) { }

  ngOnInit(): void {
    this.getDocument();
  }

  submit(): void {
    // TODO: Connect to actually send data to server
    console.log("submitted")
    console.log(this.getUserInput())
  }

  getUserInput(): any[] {
    return Array.from(document.querySelectorAll('input')).map(e =>{return{"id": e.id, "value": e.value}})
  }

  //Find the document to serve according to the URL.
  getDocument(): void {
    const id = +this.route.snapshot.paramMap.get('id');
    this.document$ = this._dataApi.documentService.entity(id).fetch();
  }
}
