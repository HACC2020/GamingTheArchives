import { Component, OnInit, Input } from '@angular/core';
import { User } from '../models/user';
import { DataApiService } from 'src/app/services/data-api.service';
import { Activity } from 'src/app/models/activity';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { UserContextService } from 'src/app/services/user-context-service';
import { ActivityType } from 'src/app/models/activity-type';
import * as Handlebars from 'handlebars'

@Component({
  selector: 'app-activity-feed',
  templateUrl: './activity-feed.component.html',
  styleUrls: ['./activity-feed.component.scss']
})
export class ActivityFeedComponent implements OnInit {

  private _currentUser: User;
  private _user: User;
  private _initialized: boolean;

  @Input() set user(value: User) {
    this._user = value;
    console.log(`User property set: ${(this._user ? this._user.EmailAddress : 'null')}`)
    if (this._initialized) {
      console.log(`Updating activity list for new user`);
      this.initializeActivityList();
    }
  }

  get user(): User {
    return this._user;
  }

  activityList$: Observable<Activity[]>

  constructor(
    private _userContext: UserContextService,
    private _dataApi: DataApiService) {
  }

  async ngOnInit(): Promise<void> {
    this._currentUser = await this._userContext.userPromise;
    this.initializeActivityList();
  }

  renderMessage(activity: Activity) {
    let DisplayName: string;
    switch (activity.Type) {
      case ActivityType.UserSignup:
        DisplayName = activity.User.DisplayName
        break;
      case ActivityType.TranscriptionSubmitted:
      case ActivityType.ProjectCreated:
      case ActivityType.ProjectCompleted:
      case ActivityType.TranscriptionApproved:
        if (this._currentUser && activity.UserId == this._currentUser.Id) {
          DisplayName = "You";
        } else {
          DisplayName = activity.User.DisplayName;
        }
        break;
    }

    let template = Handlebars.compile(activity.Message);
    return template({ DisplayName });
  }

  private initializeActivityList() {
    if (this._user) {
      console.log('Fetching activity list for user: ' + this._user.EmailAddress);
      let activitiesFunction =
        this._dataApi.userService.entity(this._user.Id)
          .function<void, Activity>('Activities');
      activitiesFunction.query.orderBy([['CreatedTime', 'desc']]);
      activitiesFunction.query.top(10);
      activitiesFunction.query.expand('User')
      this.activityList$ =
        activitiesFunction
          .get({ responseType: 'entities' })
          .pipe(map(wrapper => wrapper.entities));
    } else {
      console.log('Fetching activity list for all users.')
      this.activityList$ =
        this._dataApi.activityService.entities()
          .orderBy([['CreatedTime', 'desc']])
          .top(10)
          .expand('User')
          .get()
          .pipe(map(wrapper => wrapper.entities));
    }
  }
}
