/* This component creates a list of documents.
* the purpose of this component is to have a portable list of documents that can be
*   dropped into different pageviews.
* Eventually, this can be developed to add functionality, such as 
*   a list of documents edited by a user, a list of documents for a project, etc.
*/


import { Component, OnInit, Input } from '@angular/core';
import { Document } from '../models/document';
import { DocumentService } from '../services/document.service';
import { ActivatedRoute } from '@angular/router';
import { Location } from '@angular/common';

@Component({
  selector: 'app-doc-list',
  templateUrl: './doc-list.component.html',
  styleUrls: ['./doc-list.component.scss']
})
export class DocListComponent implements OnInit {


  //Need this @Input to call project number in project details.
  @Input () projectId: Number;


  constructor(
    private documentService: DocumentService,
    private route: ActivatedRoute,
    private location: Location) { }

  ngOnInit(): void {
    this.getDocuments();
  }



  documentList: Document[]; //array to hold mock file data.

  // fill array with mock file data.
  getDocuments(): void {
    this.documentService.getDocuments()
      .subscribe(document => this.documentList = document);
  }
}
