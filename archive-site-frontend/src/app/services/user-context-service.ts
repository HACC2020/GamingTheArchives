import { Injectable } from '@angular/core';
import { from, Observable, Subject } from 'rxjs';
import User from 'src/app/models/user';
import { UserService } from 'src/app/services/user.service';

@Injectable({
  providedIn: 'root'
})
export class UserContextService {
  private _userSubject: Subject<User> = new Subject<User>();
  private _currentUserRequest: Promise<User>;

  public get user$(): Observable<User> {
    let observable = this._userSubject.asObservable();

    if (this._currentUserRequest) {
      this.emitCurrentUser();
    } else {
      this.invalidateUser();
    }

    return observable;
  }

  constructor(private _userService: UserService) {
  }

  public invalidateUser(): void {
    this._currentUserRequest = this.getCurrentUser();
    this.emitCurrentUser();
  }

  private emitCurrentUser() {
    from(this._currentUserRequest)
      .subscribe(
        result => {
          this._userSubject.next(result);
        },
        error => {
          console.warn(error);
          this._userSubject.error(error);
        }
      );
  }

  private async getCurrentUser(): Promise<User> {
    if (await this._userService.isAuthenticated()) {
      return this._userService.me();
    }
  }
}
