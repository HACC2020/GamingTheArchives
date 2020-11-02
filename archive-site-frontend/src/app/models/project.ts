import Model from './model';

export default class Project extends Model {
  constructor(
    id: number = 0,
    public Name: string = undefined,
    public Description: string = undefined,
    public SampleDocumentUrl: string = undefined,
    public Active: boolean = false) {
    super(id);
  }

  public static fromPayload(payload: any): Project {
    let result = new Project();

    result.Id = payload.Id;
    result.Name = payload.Name;
    result.Description = payload.Description;
    result.SampleDocumentUrl = payload.SampleDocumentUrl;
    result.Active = !!payload.Active;

    return result;
  }
}
