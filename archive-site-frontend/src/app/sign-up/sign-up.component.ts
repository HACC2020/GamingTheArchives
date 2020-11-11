import { Component, OnInit } from '@angular/core';
import { environment } from 'src/environments/environment';
import { UrlHelper } from 'src/app/services/url-helper';

@Component({
  selector: 'app-sign-up',
  templateUrl: './sign-up.component.html',
  styleUrls: ['./sign-up.component.scss']
})
export class SignUpComponent implements OnInit {

  constructor(public urlHelper: UrlHelper) { }

  ngOnInit(): void {
  }

  withFacebook(): void {
    let profileUrl = this.urlHelper.getRouteUrl(['/profile'])
    let returnUrl =
      environment.apiUrl.startsWith(window.origin) ?
        profileUrl :
        `${window.origin}${profileUrl}`;
    window.location.href =
      `${environment.apiUrl}/auth/login-with-facebook?returnUrl=${encodeURIComponent(returnUrl)}`;
  }

}
