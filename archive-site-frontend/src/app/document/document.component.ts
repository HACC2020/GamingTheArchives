/* Changing the purpose of this component. It will
* be to show the document as a single image.
*
*/

import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable, ReplaySubject } from 'rxjs';

import { HighlightMarker, MarkerArea } from 'markerjs';

import { environment } from 'src/environments/environment';

import { DocumentService } from '../services/document.service';
import { MessageService } from '../services/message.service';
import { MarkerAreaState } from 'markerjs/typings/MarkerAreaState';
import { Document } from '../models/document';
import { Transcription } from '../models/transcription';
import AzureTranscription from '../models/azure-transcription';
import { Field } from 'src/app/models/field';

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
  ) {
  }

  ngOnInit(): void {
    this.getDocument();
  }

  // Find the document to serve according to the URL.
  getDocument(): void {
    this.route.params.subscribe(params => {
      this.projectId = +params.projectId;
      this.documentId = +params.documentId;

      this.azureTranscriptions$ = this.documentService.getAzureTranscription(this.documentId);
      this.document$ = this.documentService.getDocumentByDocumentId(this.documentId);
      this.document$.subscribe(document => {
        this.documentImageUrl = `${environment.apiUrl}/DocumentImage/${document.Id}`;
      });
    });
  }

  @ViewChild('documentImage') documentImage: ElementRef;

  @ViewChild('documentMarker') documentMarker: ElementRef;

  private markerArea: MarkerArea;
  private renderedImage: string;
  private markerAreaState: MarkerAreaState;

  private azureTranscriptions$: Observable<Array<AzureTranscription>>;
  private azureTranscriptions: Array<AzureTranscription>;
  private widthRatio: number;
  private heightRatio: number;

  onImageLoaded(event: Event): void {
    /*
    console.log(`image width ${this.documentImage.nativeElement.width}`);
    console.log(`image height ${this.documentImage.nativeElement.height}`);
    console.log(event);
    */
    console.log('onImageLoaded');

    let naturalWidth = this.documentImage.nativeElement.naturalWidth;
    let naturalHeight = this.documentImage.nativeElement.naturalHeight;
    let imageWidth = this.documentImage.nativeElement.width;
    let imageHeight = this.documentImage.nativeElement.height;

    this.widthRatio = imageWidth / naturalWidth;
    this.heightRatio = imageHeight / naturalHeight;

    ///*
    if (this.markerArea != null) {
      this.markerArea.resetState();
    }

    // must be re-instantiated because the image size MIGHT change
    this.markerArea = new MarkerArea(this.documentImage.nativeElement,
      { targetRoot: this.documentMarker.nativeElement, showUi: false });
    this.markerArea.show((dataUrl, state) => {
      this.renderedImage = dataUrl;
      this.markerAreaState = state;
    });

    //this.markerArea.close();
    //*/

    this.resetImagePosition();

    this.azureTranscriptions$.subscribe((azureTranscripts) => {
      console.log(azureTranscripts);
      this.azureTranscriptions = azureTranscripts;

      azureTranscripts.forEach(transcript => {
        const boundingBox = transcript.BoundingBox;
        this.markerArea.addMarker(HighlightMarker,
          {
            translateX: boundingBox.Left * this.widthRatio,
            translateY: boundingBox.Top * this.heightRatio,
            width: (boundingBox.Right - boundingBox.Left) * this.widthRatio,
            height: (boundingBox.Bottom - boundingBox.Top) * this.heightRatio,
            markerId: '',
            markerType: 'HighlightMarker'
          });
      });

      this.renderImage();
    });
  }

  showRendered: boolean = false;

  showHideHighlights(): void {
    this.showRendered = !this.showRendered;
  }

  resetImagePosition(): void {
    // reset mouse related coordinates
    this.isImageMoving = false;
    this.mouseDown = { x: 0, y: 0, left: 0, top: 0 };

    // reset documentImage position
    this.documentImage.nativeElement.style.left = 0;
    this.documentImage.nativeElement.style.top = 0;
    this.documentImage.nativeElement.style.transform = 'scale(1)';
  }

  addMarker(): void {
    console.log("addM");
    this.markerArea.open();
    this.markerArea.addMarker(HighlightMarker);
  }

  @ViewChild('documentRender') documentRender: HTMLImageElement;

  public image$: ReplaySubject<string> = new ReplaySubject(1);

  renderImage(): void {
    this.markerArea.render((dataUrl) => {
      this.image$.next(dataUrl);
      this.markerArea.resetState();
      this.documentMarker.nativeElement.style.display = 'none';
    });
  }

  private isImageMoving = false;
  private mouseDown = { x: 0, y: 0, left: 0, top: 0 };

  onImageMouseDown(event: MouseEvent): void {
    event.preventDefault();

    const left = parseInt(this.documentImage.nativeElement.style.left);
    this.mouseDown.left = isNaN(left) ? 0 : left;

    const top = parseInt(this.documentImage.nativeElement.style.top);
    this.mouseDown.top = isNaN(top) ? 0 : top;

    this.mouseDown.x = event.clientX;
    this.mouseDown.y = event.clientY;

    if (this.isImageMoving) {
      this.isImageMoving = !this.isImageMoving;
      return;
    }

    this.isImageMoving = !this.isImageMoving;
  }

  onImageMouseMove(mouseEvent: MouseEvent): void {
    if (!this.isImageMoving) {
      return;
    }

    var left = mouseEvent.clientX - this.mouseDown.x;
    var top = mouseEvent.clientY - this.mouseDown.y;

    //console.log(`event x: ${mouseEvent.clientX} - event y: ${mouseEvent.clientY}`);
    //console.log(`down x: ${this.mouseDown.x}, down y: ${this.mouseDown.y}`);

    this.documentImage.nativeElement.style.left = (this.mouseDown.left + left) + "px";
    this.documentImage.nativeElement.style.top = (this.mouseDown.top + top) + "px";
  }

  onImageMouseUp(): void {
    this.isImageMoving = false;
  }

  onImageMouseWheel(event: WheelEvent): void {
    event.preventDefault();
    event.stopPropagation();

    let scale = this.getImageTransformScale();

    if (event.deltaY > 0) { // then zoom out
      scale = scale - 0.025;
    } else if (event.deltaY < 0) { // then zoom in
      scale = scale + 0.025;
    }

    this.documentImage.nativeElement.style.transform = `scale(${scale})`;
  }

  onImageDragOver(event: DragEvent): void {
    console.log('onImageDragOver');
    event.preventDefault();
    event.stopPropagation();

    this.showRendered = true;
  }

  startAutoTranscribe(event: DragEvent, field: { id: string }): void {
    event.dataTransfer.setData('application/json', JSON.stringify({ id: field.id }))
  }

  onImageDrop(event: DragEvent): void {
    console.log(event);
    let x = event.offsetX;
    let y = event.offsetY;

    let trueX = x / this.widthRatio;
    let trueY = y / this.heightRatio;

    console.log({ x, y, trueX, trueY });

    if (this.azureTranscriptions) {
      let match: string;
      for (const transcription of this.azureTranscriptions) {
        if (trueX >= transcription.BoundingBox.Left && trueX <= transcription.BoundingBox.Right &&
          trueY >= transcription.BoundingBox.Top && trueY <= transcription.BoundingBox.Bottom) {
          match = transcription.Text;
          break;
        }
      }

      if (match) {
        console.log(`Boom! ${match}`);

        let fieldInfo = JSON.parse(event.dataTransfer.getData('application/json'))

        if (fieldInfo.id) {
          let transcribeTo =
            Array.from(document.querySelectorAll('input'))
              .filter(input => input.id === fieldInfo.id)[0];

          if (transcribeTo) {
            transcribeTo.value = match;
          }
        }
      }
    }

    this.showRendered = false;
  }

  private getImageTransformScale(): number {
    const transform = this.documentImage.nativeElement.style.transform;
    if (transform === '') {
      return 1;
    }

    const regExp = /[-+]?[0-9]*\.?[0-9]+/;

    return parseFloat(regExp.exec(transform)[0]);
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
