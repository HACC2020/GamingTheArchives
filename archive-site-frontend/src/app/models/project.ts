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
}
