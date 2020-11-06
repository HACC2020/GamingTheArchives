/* This component creates a list of documents.
* the purpose of this component is to have a portable list of documents that can be
*   dropped into different pageviews.
* Eventually, this can be developed to add functionality, such as 
*   a list of documents edited by a user, a list of documents for a project, etc.
*/



import { Component, OnInit } from '@angular/core';
import { Document } from '../models/document';
import { DocumentService } from '../services/document.service';

@Component({
  selector: 'app-document',
  templateUrl: './document.component.html',
  styleUrls: ['./document.component.scss']
})
export class DocumentComponent implements OnInit {

  constructor(private documentService: DocumentService) { }

  ngOnInit(): void {
    this.getDocuments();
  }

  documentList: Document[];

  getDocuments(): void {
    this.documentService.getDocuments()
      .subscribe(document => this.documentList = document);
  }

}
