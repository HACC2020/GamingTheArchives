
/* Changing the purpose of this component. It will 
* be to show the document as a single image.
*
*/

import { Component, OnInit } from '@angular/core';
import { Document } from '../models/document';
import { DocumentService } from '../services/document.service';
import { ActivatedRoute } from '@angular/router';
import { Location } from '@angular/common';

@Component({
  selector: 'app-document',
  templateUrl: './document.component.html',
  styleUrls: ['./document.component.scss']
})
export class DocumentComponent implements OnInit {

  constructor(
    private documentService: DocumentService,
    private route: ActivatedRoute,
    private location: Location) { }

 

  ngOnInit(): void {
    
    this.getDocument();
  }

  document: Document;

  //Find the document to serve according to the URL.
  getDocument(): void {
    const id = +this.route.snapshot.paramMap.get('id');
    this.documentService.getDoc(id)
      .subscribe(doc => this.document = doc);
  }


}
