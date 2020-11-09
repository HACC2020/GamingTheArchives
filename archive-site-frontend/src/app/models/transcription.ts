import { Model } from './model';

export class Transcription extends Model {
  constructor(
    id: number = 0,
    public DocumentId: number = undefined,
    public UserId: number = undefined,
    public Data: string = undefined,
    public ValidationErrors: string = undefined,
    public IsSubmitted: boolean = false) {
    super(id);
  }
}
