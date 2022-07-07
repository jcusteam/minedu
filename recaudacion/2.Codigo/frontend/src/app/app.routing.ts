import { Routes, RouterModule } from "@angular/router";
import { NgModule } from "@angular/core";

import { LandingComponent } from "./pages/landing/landing.component";

export const routes: Routes = [
  {
    path: "",
    loadChildren: () =>import("./pages/pages.module").then((m) => m.PagesModule),
  },
  { path: 'landing', component: LandingComponent },
  { path: "**", redirectTo: "landing", pathMatch: "full" },

];

@NgModule({
  imports: [RouterModule.forRoot(routes, { enableTracing: false })],
  exports: [RouterModule],
})
export class AppRoutingModule { }
