/* This component creates a portable list of users
* that can be dropped into views.
* The goal is to be able to sort the viewers, such as by activity or
* working on a project, etc.
*/


import { Component, OnInit } from '@angular/core';
import User from "../models/user";
import { UserService } from "../services/user.service";
import { Observable } from 'rxjs';


@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.scss']
})
export class UsersComponent implements OnInit {

  userList$: Observable<User[]>;

  constructor(private _userService: UserService) { }

  ngOnInit(): void {
    this.getUsers();
  }

  getUsers(): void {
    this.userList$ = this._userService.entities().all();
    this.userList$.subscribe(
      result => { console.log('got users'); console.log(result); },
      err => { console.log('derp'); console.log(err); });
  }

}
