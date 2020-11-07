import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { UserService } from 'src/app/services/user.service';
import User from 'src/app/models/user';
import { UserType } from 'src/app/models/user-type';
import { $enum } from 'ts-enum-util';
import { NotificationService } from 'src/app/services/notification-service';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {
  allUserTypes: UserType[] = Array.from($enum(UserType).values());

  isLoading: boolean = true;
  isSaving: boolean;
  isNewSignUp: boolean;
  adminMode: boolean;

  existingUser: User;

  email: string;
  displayName: string;
  userType: UserType;

  get hasChanges(): boolean {
    return !this.existingUser ||
      this.displayName !== this.existingUser.DisplayName ||
      this.userType !== this.existingUser.Type;
  }

  constructor(
    private _router: Router,
    private _route: ActivatedRoute,
    private _userService: UserService,
    private _notifications: NotificationService) { }

  async ngOnInit(): Promise<void> {
    let code = this._route.snapshot.queryParams['code'];
    // This is for demo purpose and will be removed in the production version.
    this.adminMode = code && code.toUpperCase() === 'IDDQD';

    let [ident, existing] = await Promise.all([
      this._userService.ident(),
      this._userService.me()
    ]);

    if (!ident || ! ident.Email) {
      // You need to at least authenticate first
      await this._router.navigate(['signup']);
      return;
    }

    this.existingUser = existing;
    this.email = ident.Email;
    if (existing) {
      this.displayName = existing.DisplayName;
      console.log(`loading existing type ${existing.Type}`);
      console.log(existing);
      this.userType = existing.Type;
    }

    this.isNewSignUp = !this.existingUser;
    this.isLoading = false;
  }

  async save(): Promise<void> {
    this.isSaving = true;

    if (this.existingUser) {
      this.existingUser.DisplayName = this.displayName;
      if (this.adminMode) {
        this.existingUser.Type = this.userType;
      }
      this.existingUser = await this._userService.saveProfile(this.existingUser);
      this.isSaving = false;
      this._notifications.success('User Profile Saved');
    } else {
      await this._userService.saveProfile(new User(
        undefined,
        this.email,
        this.displayName,
        this.adminMode ? this.userType : UserType.Rookie
      ));
      this._notifications.success('Welcome to Gaming the Archives');
      await this._router.navigate(['projects']);
    }
  }

  async cancel(): Promise<void> {
    if (this.existingUser != null) {
      this.displayName = this.existingUser.DisplayName;
    } else {
      await this._router.navigate(['projects']);
    }
  }

  getUserTypeString(type: UserType): string {
    return UserType[type];
  }
}
