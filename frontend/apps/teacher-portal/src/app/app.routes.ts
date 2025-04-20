import { Routes } from '@angular/router';
import { DashboardComponent } from './dashboard/dashboard.component';
import { ClassroomComponent } from './classroom/classroom.component';
import { CoursesComponent } from './courses/courses.component';
import { AssessmentsComponent } from './assessments/assessments.component';
import { DiscussionsComponent } from './discussions/discussions.component';
import { ResourcesComponent } from './resources/resources.component';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'dashboard',
    pathMatch: 'full'
  },
  {
    path: 'dashboard',
    component: DashboardComponent
  },
  {
    path: 'classroom/:id',
    component: ClassroomComponent
  },
  {
    path: 'courses',
    component: CoursesComponent
  },
  {
    path: 'assessments',
    component: AssessmentsComponent
  },
  {
    path: 'discussions',
    component: DiscussionsComponent
  },
  {
    path: 'resources/:type',
    component: ResourcesComponent
  },
  {
    path: '**',
    redirectTo: 'dashboard'
  }
];
