import { Routes, RouterModule, PreloadAllModules } from "@angular/router";
import { ModuleWithProviders } from "@angular/core";
import { ModuloComponent } from "./pages/modulo/modulo.component";
import { AuthGuard } from "./core/guards/auth.guard";
import { LandingComponent } from "./pages/landing/landing.component";
import { RegisterComponent } from "./pages/register/register.component";
import { ErrorComponent } from "./pages/errors/error/error.component";
import { NotFoundComponent } from "./pages/errors/not-found/not-found.component";

export const routes: Routes = [
  {
    path: "",
    component: LandingComponent,
  },
  {
    path: "modulo",
    component: ModuloComponent,
    runGuardsAndResolvers: "always",
    canActivate: [AuthGuard],
  },
  {
    path: "register",
    component: RegisterComponent,
  },
  { path: 'not-found', component: NotFoundComponent, data: { breadcrumb: 'not-found' } },
  { path: '**', pathMatch   : 'full', redirectTo:'not-found' }
];

export const routing: ModuleWithProviders = RouterModule.forRoot(routes, {
  preloadingStrategy: PreloadAllModules, // <- comment this line for enable lazy load
  // useHash: true
});
