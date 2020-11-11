import { Injectable } from '@angular/core';
import { NavigationExtras, Router } from '@angular/router';
import { Location } from '@angular/common';

@Injectable({
  providedIn: 'root'
})
export class UrlHelper {
  constructor(
    private _router: Router,
    private _location: Location) {
  }

  getRouteUrl(commands: any[], navigationExtras?: NavigationExtras): string {
    let url =
      this._location.prepareExternalUrl(
        this._router.createUrlTree(commands, navigationExtras).toString()
      );

    return url.startsWith('/') ? url : `/${url}`;
  }
}
