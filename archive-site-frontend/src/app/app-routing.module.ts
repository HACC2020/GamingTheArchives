import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ProjectsComponent } from 'src/app/projects/projects.component';
import { NotFoundPageComponent } from 'src/app/not-found-page/not-found-page.component';
import { ProjectDetailComponent } from 'src/app/project-detail/project-detail.component';
import { DocumentComponent } from './document/document.component';
import { LoginComponent } from 'src/app/login/login.component';
import { SignUpComponent } from 'src/app/sign-up/sign-up.component';
import { ProfileComponent } from 'src/app/profile/profile.component';
import { ProjectSettingsComponent } from 'src/app/project-settings/project-settings.component';
import { environment } from 'src/environments/environment';


const routes: Routes = [
  { path: '',   redirectTo: '/projects?intro=true', pathMatch: 'full' },
  { path: 'projects', component: ProjectsComponent, pathMatch: 'full' },
  { path: 'project/:id', component: ProjectDetailComponent },
  { path: 'project/:id/settings', component: ProjectSettingsComponent },
  { path: 'transcribe/:projectId/:documentId', component: DocumentComponent},

  { path: 'login', component: LoginComponent },
  { path: 'signup', component: SignUpComponent },
  { path: 'profile', component: ProfileComponent },


  // Add paths above this one. Any below this will not be found.
  { path: '**', component: NotFoundPageComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { useHash: environment.useHash })],
  exports: [RouterModule]
})
export class AppRoutingModule { }
