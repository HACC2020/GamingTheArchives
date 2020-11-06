/* This component creates a portable list of users
* that can be dropped into views.
* The goal is to be able to sort the viewers, such as by activity or 
* working on a project, etc.
*/


import { Component, OnInit } from '@angular/core';
import User from "../models/user";
import { UserService } from "../services/user.service";


@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.scss']
})
export class UsersComponent implements OnInit {

  constructor(private userService: UserService) { }

  ngOnInit(): void {
    this.getUsers();
  }

  userList: User[];

  getUsers(): void {
    this.userService.getUsers()
      .subscribe(user => this.userList = user);
  }

}
