import { Component } from '@angular/core';
import User from 'src/app/models/user';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  // TODO use events to update this when a login or logout event happens
  public user: User;
}
