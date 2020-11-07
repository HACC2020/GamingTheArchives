import { Injectable } from '@angular/core';
import User from '../models/user';
import { DataApiService } from 'src/app/services/data-api.service';
import { DataEntityServiceWrapper } from 'src/app/services/data-entity-service-wrapper';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class UserService extends DataEntityServiceWrapper<User> {
  constructor(api: DataApiService) {
    super(api.userService);
  }

  isAuthenticated(): Promise<boolean> {
    return this.ident().then(ident => ident && !!ident.Email);
  }

  async ident(): Promise<{ Email: string }> {
    let response = await fetch(
      this.getActionUrl('ident'),
      { credentials: 'include' }
    );

    if (response.ok) {
      return response.json();
    } else {
      return;
    }
  }

  async me(): Promise<User> {
    let response = await fetch(this.getActionUrl('me'), { credentials: 'include' });
    if (response.ok) {
      return User.fromPayload(await response.json());
    } else if (response.status == 404) {
      return undefined;
    } else {
      // TODO figure out the right approach here to make this behave like other ODataEntityService methods.
      console.log(response);
      throw new Error(`Unable to fetch the current user's profile. ${response.status} ${response.statusText}`);
    }
  }

  async saveProfile(user: User): Promise<User> {
    let response = await fetch(
      `${this.apiConfig.serviceRootUrl}Users/saveprofile`,
      {
        credentials: environment.apiCredentialMode,
        method: 'post',
        body: JSON.stringify(user),
        headers: { 'Content-Type': 'application/json' }
      }
    );
    if (response.ok) {
      return User.fromPayload(await response.json());
    } else {
      // TODO figure out the right approach here to make this behave like other ODataEntityService methods.
      console.log(response);
      throw new Error(`Unable to save the current user's profile. ${response.status} ${response.statusText}`);
    }
  }

  private getActionUrl(actionName: string) {
    return `${this.apiConfig.serviceRootUrl}Users/${actionName}`;
  }

}
