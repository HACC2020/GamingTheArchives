import {Observable, of } from 'rxjs';
import { Injectable } from '@angular/core';
import { USERS } from './mock-data/mock-users';
import User from './models/user';


@Injectable({
  providedIn: 'root'
})
export class UserService {


  getUsers(): User[] {
    return USERS;
  }
  constructor() { }
}
