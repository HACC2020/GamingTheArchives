import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { from, Observable } from 'rxjs';
import User from './models/user';
import { UserService } from './services/user.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  public user$: Observable<User>;

  constructor(
    private _userService: UserService,
    public router: Router) {
  }

  ngOnInit(): void {
    this.user$ = from(this.getCurrentUser());
  }

  async getCurrentUser(): Promise<User> {
    if (await this._userService.isAuthenticated()) {
      return this._userService.me();
    }
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
