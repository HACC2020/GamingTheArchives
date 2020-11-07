/* This component is for building out the transcibing page.
* Need to add fields for text input and nav buttons.
*
*/


import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Location } from '@angular/common';

@Component({
  selector: 'app-doc-transcriber',
  templateUrl: './doc-transcriber.component.html',
  styleUrls: ['./doc-transcriber.component.scss']
})
export class DocTranscriberComponent implements OnInit {

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private location: Location
  ) { }

  ngOnInit(): void {
  }

}
