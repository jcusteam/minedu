import { BrowserModule } from '@angular/platform-browser';
import { ErrorHandler, NgModule } from '@angular/core';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { OverlayContainer } from '@angular/cdk/overlay';
import { CustomOverlayContainer } from './theme/utils/custom-overlay-container';
import { JwtModule } from "@auth0/angular-jwt";

import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { PERFECT_SCROLLBAR_CONFIG } from 'ngx-perfect-scrollbar';
import { PerfectScrollbarConfigInterface } from 'ngx-perfect-scrollbar';

const DEFAULT_PERFECT_SCROLLBAR_CONFIG: PerfectScrollbarConfigInterface = {
  wheelPropagation: true,
  suppressScrollX: true
};

import { SharedModule } from './shared/shared.module';
import { PipesModule } from './theme/pipes/pipes.module';
import { routing } from './app.routing';

import { AppComponent } from './app.component';
import { AppSettings } from './app.settings';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { ModuloComponent } from './pages/modulo/modulo.component';
import { environment } from 'src/environments/environment';
import { UserMenuComponent } from './theme/components/user-menu/user-menu.component';
import { SweetAlert2Module } from '@sweetalert2/ngx-sweetalert2';
import { LandingComponent } from './pages/landing/landing.component';
import { RegisterComponent } from './pages/register/register.component';
import { NgSelectModule } from '@ng-select/ng-select';
import { MineduSharedModule } from 'src/@minedu/minedu-shared.module';
import { RecaptchaModule } from 'ng-recaptcha';
import { MatSelectFilterModule } from 'mat-select-filter';
import { ErrorComponent } from './pages/errors/error/error.component';
import { NotFoundComponent } from './pages/errors/not-found/not-found.component';
import { ErrInterceptor } from './core/interceptor/error.interceptor';
import { ErrHandler } from './core/handler/error.handler';

export function tokenGetter() {
  return localStorage.getItem("token");
}

@NgModule({
  imports: [
    HttpClientModule,
    BrowserModule,
    BrowserAnimationsModule,
    FormsModule,
    ReactiveFormsModule,
    PerfectScrollbarModule,
    SweetAlert2Module.forRoot(),
    NgSelectModule,
    MatSelectFilterModule,
    SharedModule,
    PipesModule,
    MineduSharedModule,
    RecaptchaModule,
    routing,
    JwtModule.forRoot({
      config: {
        tokenGetter: tokenGetter,
        whitelistedDomains: [environment.url.apiGatewayDomain],
        blacklistedRoutes: [environment.url.apiGateway],
      },
    }),
  ],
  declarations: [
    AppComponent,
    ModuloComponent,
    UserMenuComponent,
    LandingComponent,
    RegisterComponent,
    ErrorComponent,
    NotFoundComponent
  ],
  entryComponents: [

  ],
  providers: [
    AppSettings,
    {
      provide: PERFECT_SCROLLBAR_CONFIG,
      useValue: DEFAULT_PERFECT_SCROLLBAR_CONFIG
    },
    //{
    //  provide: LocationStrategy, useClass: HashLocationStrategy
    //},
    { provide: OverlayContainer, useClass: CustomOverlayContainer },
    { provide: HTTP_INTERCEPTORS, useClass: ErrInterceptor, multi: true },
    { provide: ErrorHandler, useClass: ErrHandler }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
