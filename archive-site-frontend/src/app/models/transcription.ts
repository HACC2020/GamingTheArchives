import { Model } from './model';
import { Document } from 'src/app/models/document';
import { User } from 'src/app/models/user';

export class Transcription extends Model {
  constructor(
    id: number = 0,
    public DocumentId: number = undefined,
    public UserId: number = undefined,
    public Data: string = undefined,
    public ValidationErrors: string = undefined,
    public IsSubmitted: boolean = false,
    public Document?: Document,
    public User?: User) {
    super(id);
  }
}
