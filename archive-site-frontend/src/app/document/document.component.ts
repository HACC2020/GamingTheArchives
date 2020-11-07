
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
  constructor(
    private documentService: DocumentService,
    private route: ActivatedRoute,
    private location: Location,
    private _dataApi: DataApiService
    ) { }

  ngOnInit(): void {
    this.getDocument();
  }

  //Find the document to serve according to the URL.
  getDocument(): void {
    const id = +this.route.snapshot.paramMap.get('id');
    this.document$ = this._dataApi.documentService.entity(id).fetch();
  }
}
