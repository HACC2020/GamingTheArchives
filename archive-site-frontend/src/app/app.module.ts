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

@NgModule({
  declarations: [
    AppComponent,
    ProjectsComponent,
    NotFoundPageComponent,
    ProjectDetailComponent,
    CreateProjectModal
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    NgbModule,
    FlexLayoutModule,
    FormsModule,
    ODataModule
  ],
  providers: [
    { provide: ODataSettings, useFactory: odataSettingsFactory }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
