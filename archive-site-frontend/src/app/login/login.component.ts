import { Component, OnInit } from '@angular/core';
import { environment } from 'src/environments/environment';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  private _returnUrl: string;

  constructor(
    private _route: ActivatedRoute) { }

  ngOnInit(): void {
    this._returnUrl = this._route.snapshot.queryParams['returnUrl'] || '/';
  }

  withFacebook(): void {
    let returnUrl =
      environment.apiUrl.startsWith(window.origin) ?
        this._returnUrl :
        `${window.origin}${this._returnUrl}`;

    window.location.href =
      `${environment.apiUrl}/auth/login-with-facebook?returnUrl=${encodeURIComponent(returnUrl)}`;
  }
}
