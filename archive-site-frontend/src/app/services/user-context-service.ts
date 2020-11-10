import { Injectable } from '@angular/core';
import { from, Observable, Subject } from 'rxjs';
import User from 'src/app/models/user';
import { UserService } from 'src/app/services/user.service';

@Injectable({
  providedIn: 'root'
})
export class UserContextService {
  private _userSubject: Subject<User>;

  public get user$(): Observable<User> {
    if (!this._userSubject) {
      this.invalidateUser();
    }

    return this._userSubject.asObservable();
  }

  constructor(private _userService: UserService) {
  }

  public invalidateUser(): void {
    if (!this._userSubject) {
      // Lazily initialize this request
      this._userSubject = new Subject<User>();
    }

    from(this.getCurrentUser())
      .subscribe(
        result => {
          this._userSubject.next(result);
        },
        error => {
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
