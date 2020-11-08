import { Injectable } from '@angular/core';
import { from, Observable } from 'rxjs';
import User from 'src/app/models/user';
import { UserService } from 'src/app/services/user.service';

@Injectable({
  providedIn: 'root'
})
export class UserContextService {
  private _userObservable;

  public get user$(): Observable<User> {
    if (!this._userObservable) {
      // Lazily initialize this request
      this._userObservable = from(this.getCurrentUser());
    }

    return this._userObservable;
  }

  constructor(private _userService: UserService) {
  }

  async getCurrentUser(): Promise<User> {
    if (await this._userService.isAuthenticated()) {
      return this._userService.me();
    }
  }
}
