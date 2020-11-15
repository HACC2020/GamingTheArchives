import { Model } from 'src/app/models/model';
import { ActivityType } from 'src/app/models/activity-type';
import { User } from 'src/app/models/user';

export class Activity extends Model {
  constructor(
    id: number = 0,
    public UserId: number = 0,
    public Message: string = undefined,
    public EntityType: string = undefined,
    public EntityId: number = undefined,
    public Type: ActivityType = undefined,
    public CreatedTime: Date = undefined,
    public User?: User) {
    super(id);
  }
}
