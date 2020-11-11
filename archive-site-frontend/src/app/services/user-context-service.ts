import { Injectable } from '@angular/core';
import { from, Observable, Observer, Subscription, SubscriptionLike, Unsubscribable } from 'rxjs';
import { User } from 'src/app/models/user';
import { UserService } from 'src/app/services/user.service';

@Injectable({
  providedIn: 'root'
})
export class UserContextService {
  private _userSubject: ReplayLatestSubject<User> = new ReplayLatestSubject<User>();
  private _currentUserRequest: Promise<User>;

  public get user$(): Observable<User> {
    let observable = this._userSubject.asObservable();

    if (!this._currentUserRequest) {
      this.invalidateUser();
    }

    return observable;
  }

  public get userPromise(): Promise<User> {
    return new Promise<User>((resolve, error) => {
      this.user$.subscribe(
        next => resolve(next),
        err => error(err)
      );
    });
  }

  constructor(private _userService: UserService) {
  }

  public invalidateUser(): void {
    this._currentUserRequest = this.getCurrentUser();
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

class ReplayLatestSubject<T> extends Observable<T> implements SubscriptionLike {
  observers: Observer<T>[] = [];

  hasLatest: boolean;
  latest: T;
  closed: boolean;
  isStopped: boolean;
  hasError: boolean;
  thrownError: any = null;

  constructor() {
    super((subscriber) => {
      if (this.closed) {
        throw Error('Unable to subscribe, this ReplayLatestSubject is closed.');
      } else if (this.hasError) {
        subscriber.error(this.thrownError);
        return;
      } else if (this.isStopped) {
        subscriber.complete();
        return;
      } else {
        this.observers.push(subscriber);
        if (this.hasLatest) {
          subscriber.next(this.latest);
        }
        return new ReplayLatestSubjectSubscription(this, subscriber);
      }
    });
  }

  next(value?: T) {
    if (this.closed) {
      throw Error('Unable to dispatch next, this ReplayLatestSubject is closed.');
    }

    if (!this.isStopped) {
      this.latest = value;
      this.hasLatest = true;
      const { observers } = this;
      const len = observers.length;
      const copy = observers.slice();
      for (let i = 0; i < len; i++) {
        copy[i].next(value);
      }
    }
  }

  error(err: any) {
    if (this.closed) {
      throw new Error('Unable to dispatch error, this ReplayLatestSubject is closed.');
    }

    this.hasError = true;
    this.thrownError = err;
    this.isStopped = true;
    const { observers } = this;
    const len = observers.length;
    const copy = observers.slice();
    for (let i = 0; i < len; i++) {
      copy[i].error(err);
    }

    this.observers.length = 0;
  }

  complete() {
    if (this.closed) {
      throw new Error('Unable to complete, this ReplayLatestSubject is closed.');
    }

    this.isStopped = true;
    const { observers } = this;
    const len = observers.length;
    const copy = observers.slice();
    for (let i = 0; i < len; i++) {
      copy[i].complete();
    }

    this.observers.length = 0;
  }

  unsubscribe() {
    this.isStopped = true;
    this.closed = true;
    this.observers = null;
  }

  asObservable(): Observable<T> {
    const observable = new Observable<T>();
    (<any> observable).source = this;
    return observable;
  }
}

class ReplayLatestSubjectSubscription<T> extends Subscription implements Unsubscribable {
  closed: boolean = false;

  constructor(public subject: ReplayLatestSubject<T>, public subscriber: Observer<T>) {
    super();
  }

  unsubscribe() {
    if (this.closed) {
      return;
    }

    this.closed = true;

    const subject = this.subject;
    const observers = subject.observers;

    this.subject = null;

    if (!observers || observers.length === 0 || subject.isStopped || subject.closed) {
      return;
    }

    const subscriberIndex = observers.indexOf(this.subscriber);

    if (subscriberIndex !== -1) {
      observers.splice(subscriberIndex, 1);
    }
  }
}
