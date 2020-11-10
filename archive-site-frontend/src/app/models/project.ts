import { Model } from './model';

export class Project extends Model {
  constructor(
    id: number = 0,
    public Name: string = undefined,
    public Description: string = undefined,
    public SampleDocumentUrl: string = undefined,
    public Active: boolean = false) {
    super(id);
  }

  static fromPayload(payload: any): Project {
    return new Project(
      payload.Id,
      payload.Name,
      payload.Description,
      payload.SampleDocumentUrl,
      payload.Active
    );
  }
}
