import { Routes, RouterModule } from '@angular/router';
import { LandingComponent } from './landing.component';

export const routes = [
  { path: '', component: LandingComponent, pathMatch: 'full' }
];


export const LandingRoutes = RouterModule.forChild(routes);
