import { Component, OnInit } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Router } from '@angular/router';

@Component({
  selector: 'app-sign-up',
  templateUrl: './sign-up.component.html',
  styleUrls: ['./sign-up.component.scss']
})
export class SignUpComponent implements OnInit {

  constructor(private _router: Router) { }

  ngOnInit(): void {
  }

  withFacebook(): void {
    let profileUrl =
      this._router.createUrlTree(['profile'])
    let returnUrl =
      environment.apiUrl.startsWith(window.origin) ?
        profileUrl :
        `${window.origin}${profileUrl}`
    window.location.href =
      `${environment.apiUrl}/auth/login-with-facebook?returnUrl=${returnUrl}`;
  }

}
