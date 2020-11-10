import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable, ReplaySubject } from 'rxjs';
import { map } from 'rxjs/operators';

import { HighlightMarker, MarkerArea } from 'markerjs';

import { environment } from 'src/environments/environment';
import { Field, FieldType } from 'src/app/models/field';
import { UserContextService } from 'src/app/services/user-context-service';
import { NgForm } from '@angular/forms';

import { DocumentService } from '../services/document.service';
import { MessageService } from '../services/message.service';
import { MarkerAreaState } from 'markerjs/typings/MarkerAreaState';
import { Document } from '../models/document';
import { Transcription } from '../models/transcription';
import AzureTranscription from '../models/azure-transcription';
import { DataApiService } from 'src/app/services/data-api.service';

@Component({
  selector: 'app-document',
  templateUrl: './document.component.html',
  styleUrls: ['./document.component.scss']
})
export class DocumentComponent implements OnInit {

  document$: Observable<Document>;
  fields$: Observable<Field[]>;
  data: { [key: string]: string } = {};
  documentImageUrl: string;

  projectId: number;
  documentId: number;

  isLoading: boolean = true;

  @ViewChild('transcribeForm') protected transcribeForm: NgForm;

  transcription: Transcription;
  private _saving: Promise<void>;

  constructor(
    private dataApi: DataApiService,
    private documentService: DocumentService,
    private messageService: MessageService,
    private userContext: UserContextService,
    private route: ActivatedRoute,
    private router: Router
  ) {
  }

  ngOnInit(): void {
    console.log('DocumentComponent.ngOnInit');

    this.route.params.subscribe(params => {
      console.log('DocumentComponent route params changed');
      this.isLoading = true;
      this.projectId = Number(params.projectId);
      this.documentId = Number(params.documentId);

      this.transcription = undefined;
      this.data = {};

      this.azureTranscriptions$ = this.documentService.getAzureTranscription(this.documentId);

      this.document$ = this.documentService.getDocumentByDocumentId(this.documentId);
      this.document$.subscribe(document => {
        this.documentImageUrl = `${environment.apiUrl}/DocumentImage/${document.Id}`;
      });

      this.fields$ =
        this.dataApi.fieldService.entities()
          .filter({ ProjectId: this.projectId })
          .get()
          .pipe(map(f => f.entities));

      this.documentService.getCurrentUserTranscription(this.documentId)
        .subscribe(transcription => {
          if (transcription) {
            this.transcription = transcription;
            let allTranscriptions: any[] = JSON.parse(transcription.Data);
            console.log('Existing transcription data loaded');
            console.log(allTranscriptions);
            if (allTranscriptions && allTranscriptions.length > 0) {
              this.data = allTranscriptions[0];
            }
          }

          this.isLoading = false;
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
    event.preventDefault();
    event.stopPropagation();

    this.showRendered = true;
  }

  startAutoTranscribe(event: DragEvent, field: Field): void {
    event.dataTransfer.setData('application/json', JSON.stringify(field))
  }

  async onImageDrop(event: DragEvent): Promise<void> {
    console.log(event);
    let x = event.offsetX;
    let y = event.offsetY;

    let trueX = x / this.widthRatio;
    let trueY = y / this.heightRatio;

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

        let fieldInfo =
          Field.fromPayload(JSON.parse(event.dataTransfer.getData('application/json')));

        if (fieldInfo.Name) {
          this.data[fieldInfo.Name] = match;
          await this.saveTranscription();
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

  async submit(): Promise<void> {
    await this.saveTranscription(true);
    this.messageService.add('A new transcription has been added.');
  }

  formChanged(): Promise<void> {
    console.log('formChanged');
    return this.saveTranscription();
  }

  async saveTranscription(submit: boolean = false): Promise<void> {
    if (this._saving) {
      // Make sure any previous saving is done.
      await this._saving;
    }
    this._saving = this.saveTranscriptionHelper(submit);
    return this._saving;
  }

  async saveTranscriptionHelper(submit: boolean): Promise<void> {
    console.log('Saving transcription data: ' + JSON.stringify(this.data));
    if (this.transcription) {
      // Update
      this.transcription.Data = JSON.stringify([this.data]);
      if (submit) {
        this.transcription.IsSubmitted = true;
      }
      this.transcription =
        await this.documentService.saveTranscription(this.transcription);
    } else {
      // Save New
      console.log('fetch user');
      let user = await this.userContext.userPromise;
      console.log('got user');

      // TODO: currently we only support single record transcription, so we're just wrapping the
      //  data from the form in a one item array.
      this.transcription =
        await this.documentService.saveTranscription(
          new Transcription(
            0,
            this.documentId,
            user.Id,
            JSON.stringify([this.data]),
            undefined,
            submit
          )
        );
    }
  }

  async goToNext(): Promise<void> {
    const next =
      await this.documentService.getNextDocument(this.projectId, this.documentId).toPromise();
    await this.goToDocument(next);
  }

  async goToPrevious(): Promise<void> {
    const previous =
      await this.documentService.getPreviousDocument(this.projectId, this.documentId).toPromise();
    await this.goToDocument(previous);
  }

  async goToDocument(document: Document): Promise<void> {
    await this.router.navigate(['/transcribe', document.ProjectId, document.Id]);
  }

  getHtmlInputType(type: FieldType) {
    switch (type) {
      case FieldType.Boolean:
        return 'checkbox';
      case FieldType.Integer:
        return 'number';
      case FieldType.String:
        return 'text';
      case FieldType.Date:
        return 'date';
    }
  }
}
