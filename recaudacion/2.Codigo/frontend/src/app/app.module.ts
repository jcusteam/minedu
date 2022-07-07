import { BrowserModule } from "@angular/platform-browser";
import { ErrorHandler, NgModule } from "@angular/core";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { OverlayContainer } from "@angular/cdk/overlay";
import { CustomOverlayContainer } from "./theme/utils/custom-overlay-container";
import { JwtModule } from "@auth0/angular-jwt";

import { PerfectScrollbarModule } from "ngx-perfect-scrollbar";
import { PERFECT_SCROLLBAR_CONFIG } from "ngx-perfect-scrollbar";
import { PerfectScrollbarConfigInterface } from "ngx-perfect-scrollbar";
const DEFAULT_PERFECT_SCROLLBAR_CONFIG: PerfectScrollbarConfigInterface = {
  wheelPropagation: true,
  suppressScrollX: true,
};

import { SharedModule } from "./shared/shared.module";
import { PipesModule } from "./theme/pipes/pipes.module";
import { AppComponent } from "./app.component";
import { AppSettings } from "./app.settings";

import { HttpClientModule, HTTP_INTERCEPTORS } from "@angular/common/http";
import { LocationStrategy, HashLocationStrategy } from "@angular/common";
import { environment } from "src/environments/environment";
import { AppRoutingModule } from "./app.routing";
import { MineduSharedModule } from "src/@minedu/minedu.shared.module";
import { ErrInterceptor } from "./core/interceptors/error.interceptor";
import { NgxWebstorageModule } from 'ngx-webstorage';
import { ErrHandler } from "./core/handler/error.handler";
import { LandingComponent } from "./pages/landing/landing.component";

export function tokenGetter() {
  return localStorage.getItem("token");
}

@NgModule({
  imports: [
    HttpClientModule,
    BrowserModule,
    BrowserAnimationsModule,
    FormsModule,
    MineduSharedModule,
    ReactiveFormsModule,
    PerfectScrollbarModule,
    SharedModule,
    PipesModule,
    AppRoutingModule,
    NgxWebstorageModule.forRoot(),
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
    LandingComponent
  ],
  providers: [
    AppSettings,
    {
      provide: PERFECT_SCROLLBAR_CONFIG,
      useValue: DEFAULT_PERFECT_SCROLLBAR_CONFIG,
    },
    { provide: OverlayContainer, useClass: CustomOverlayContainer },
    {
      provide: LocationStrategy,
      useClass: HashLocationStrategy,
    },
    { provide: HTTP_INTERCEPTORS, useClass: ErrInterceptor, multi: true },
    { provide: ErrorHandler, useClass: ErrHandler }
  ],
  bootstrap: [AppComponent],
})
export class AppModule { }
