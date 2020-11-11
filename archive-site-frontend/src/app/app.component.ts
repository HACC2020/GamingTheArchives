import { Component, OnInit } from '@angular/core';
import { Location } from '@angular/common';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { User } from './models/user';
import { environment } from 'src/environments/environment';
import { UserContextService } from 'src/app/services/user-context-service';
import { map } from 'rxjs/operators';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  public user$: Observable<User>;
  public userFetched: boolean;

  constructor(
    private _userContext: UserContextService,
    private _location: Location,
    public router: Router) {
  }

  ngOnInit() {
    this.user$ =
      this._userContext.user$
        .pipe(map(u => {
          this.userFetched = true;
          return u;
        }));
  }

  async gotoSignup(): Promise<void> {
    await this.router.navigate(['signup']);
  }

  async gotoLogin(): Promise<void> {
    let returnUrl = this._location.prepareExternalUrl(this.router.url);
    if (!returnUrl.startsWith('/')) {
      returnUrl = `/${returnUrl}`;
    }

    await this.router.navigate(['login'], { queryParams: { returnUrl: returnUrl } });
  }

  logout() {
    let routeUrl = this._location.prepareExternalUrl(this.router.url);
    if (!routeUrl.startsWith('/')) {
      routeUrl = `/${routeUrl}`;
    }

    let returnUrl =
      environment.apiUrl.startsWith(window.origin) ?
        routeUrl :
        `${window.origin}${routeUrl}`;

    window.location.href =
      `${environment.apiUrl}/auth/logout?returnUrl=${encodeURIComponent(returnUrl)}`;
  }
}
