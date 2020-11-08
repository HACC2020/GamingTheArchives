import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import User from './models/user';
import { environment } from 'src/environments/environment';
import { UserContextService } from 'src/app/services/user-context-service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  public user$: Observable<User>;

  constructor(
    private _userContext: UserContextService,
    public router: Router) {

  }

  ngOnInit() {
    this.user$ = this._userContext.user$;
  }

  async gotoSignup(): Promise<void> {
    await this.router.navigate(['signup']);
  }

  async gotoLogin(): Promise<void> {
    await this.router.navigate(['login'], { queryParams: { returnUrl: this.router.url } });
  }

  logout() {
    let returnUrl =
      environment.apiUrl.startsWith(window.origin) ?
        this.router.url :
        `${window.origin}${this.router.url}`;

    window.location.href =
      `${environment.apiUrl}/auth/logout?returnUrl=${returnUrl}`;
  }
}
