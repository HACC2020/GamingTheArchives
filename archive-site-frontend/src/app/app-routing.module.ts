import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ProjectsComponent } from 'src/app/projects/projects.component';
import { NotFoundPageComponent } from 'src/app/not-found-page/not-found-page.component';
import { ProjectDetailComponent } from 'src/app/project-detail/project-detail.component';
import { DocumentComponent } from './document/document.component';
import { LoginComponent } from 'src/app/login/login.component';
import { SignUpComponent } from 'src/app/sign-up/sign-up.component';
import { ProfileComponent } from 'src/app/profile/profile.component';


const routes: Routes = [
  { path: '',   redirectTo: '/projects?intro=true', pathMatch: 'full' },
  { path: 'projects', component: ProjectsComponent, pathMatch: 'full' },
  { path: 'project/:id', component: ProjectDetailComponent },
  { path: 'transcribe/:id', component: DocumentComponent},

  { path: 'login', component: LoginComponent },
  { path: 'signup', component: SignUpComponent },
  { path: 'profile', component: ProfileComponent },


//Add paths above this one. Any below this will not be found.
  { path: '**', component: NotFoundPageComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
