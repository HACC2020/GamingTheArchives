import { Model } from './model'

export class Document extends Model {
    constructor(
        id: number = 0,
        public ProjectId: number = undefined,
        public FileName: string = undefined,
        public DocumentImageUrl: string = undefined,
        public Status: string = "PendingTranscription") {
        super(id);
      }
}
