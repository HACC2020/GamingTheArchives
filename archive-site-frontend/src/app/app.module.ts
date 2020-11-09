import { NgModule } from '@angular/core';
import { FlexLayoutModule } from '@angular/flex-layout';
import { FormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { ODataModule, ODataSettings } from 'angular-odata';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ProjectsComponent } from './projects/projects.component';
import { NotFoundPageComponent } from './not-found-page/not-found-page.component';
import { odataSettingsFactory } from './services/data-api.service';
import { ProjectDetailComponent } from './project-detail/project-detail.component';
import { CreateProjectModal } from './create-project/create-project.component';
import { ActivityFeedComponent } from './activity-feed/activity-feed.component';
import { MessagesComponent } from './messages/messages.component';
import { DocumentComponent } from './document/document.component';
import { UsersComponent } from './users/users.component';
import { DocListComponent } from './doc-list/doc-list.component';
import { LoginComponent } from './login/login.component';
import { SignUpComponent } from './sign-up/sign-up.component';
import { ProfileComponent } from './profile/profile.component';
import { NotificationsContainerComponent } from 'src/app/notifications-container.component';
import { FieldListEditorComponent } from './field-list-editor/field-list-editor.component';
import { ProjectDetailsFormComponent } from './project-details-form/project-details-form.component';
import { ProjectSettingsComponent } from './project-settings/project-settings.component';

@NgModule({
  declarations: [
    AppComponent,
    ProjectsComponent,
    NotFoundPageComponent,
    ProjectDetailComponent,
    CreateProjectModal,
    ActivityFeedComponent,
    MessagesComponent,
    DocumentComponent,
    UsersComponent,
    DocListComponent,
    LoginComponent,
    SignUpComponent,
    ProfileComponent,
    NotificationsContainerComponent,
    FieldListEditorComponent,
    ProjectDetailsFormComponent,
    ProjectSettingsComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    NgbModule,
    FlexLayoutModule,
    FormsModule,
    ODataModule,

  ],
  providers: [
    { provide: ODataSettings, useFactory: odataSettingsFactory }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
