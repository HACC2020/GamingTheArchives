import { Component, OnInit, Input } from '@angular/core';
import User from '../models/user';

@Component({
  selector: 'app-activity-feed',
  templateUrl: './activity-feed.component.html',
  styleUrls: ['./activity-feed.component.scss']
})
export class ActivityFeedComponent implements OnInit {

  @Input() individualUser: User;
  @Input() userList: User[];

  constructor() { }

  ngOnInit(): void {
  }

}