import { Model } from './model'
import { Project } from 'src/app/models/project';

export class Document extends Model {
    constructor(
        id: number = 0,
        public ProjectId: number = undefined,
        public FileName: string = undefined,
        public DocumentImageUrl: string = undefined,
        public Status: string = "PendingTranscription",
        public Project?: Project) {
        super(id);
      }
}
