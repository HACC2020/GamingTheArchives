import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ProjectsComponent } from 'src/app/projects/projects.component';
import { NotFoundPageComponent } from 'src/app/not-found-page/not-found-page.component';
import { ProjectDetailComponent } from 'src/app/project-detail/project-detail.component';
import { DocTranscriberComponent } from './doc-transcriber/doc-transcriber.component';

const routes: Routes = [
  { path: '',   redirectTo: '/projects?intro=true', pathMatch: 'full' },
  { path: 'projects', component: ProjectsComponent, pathMatch: 'full' },
  { path: 'project/:id', component: ProjectDetailComponent },
  { path: 'transcribe', component: DocTranscriberComponent, pathMatch: 'full' },
  { path: '**', component: NotFoundPageComponent },
 
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
