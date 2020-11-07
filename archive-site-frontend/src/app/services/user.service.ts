//This is a service to generate an array of fake users.

import {Observable, of } from 'rxjs';
import { Injectable } from '@angular/core';
import { USERS } from '../mock-data/mock-users';
import User from '../models/user';
import { MessageService } from '../services/message.service';


@Injectable({
  providedIn: 'root'
})
export class UserService {


//this method builds an array of fake users and sends a message for each user to activity feed.
  getUsers(): Observable<User[]> {

    for (let user of USERS) {
      this.messageService.add(user.DisplayName + " has joined.");

    }
    return of(USERS);
  }
  constructor(private messageService: MessageService) { }
}
