import { Model } from './model';
import { toUserType, UserType } from './user-type';

export class User extends Model {
  constructor(
    id: number = 0,
    public EmailAddress: string = undefined,
    public DisplayName: string = undefined,
    public Type: UserType = undefined,
    public LastLogin: Date = undefined,
    public SignUpDate: Date = undefined) {
    super(id);
}

  public static fromPayload(payload: any): User {
    let result = new User();

    result.Id = payload.Id;
    result.EmailAddress = payload.EmailAddress;
    result.DisplayName = payload.DisplayName;
    result.Type = toUserType(payload.Type);
    result.LastLogin = payload.LastLogin;
    result.SignUpDate = payload.SignUpDate;

    return result;
  }
}
