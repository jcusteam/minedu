import { AfterViewInit, Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { JwtHelperService } from '@auth0/angular-jwt';
import { AppSettings } from 'src/app/app.settings';
import { Settings } from 'src/app/app.settings.model';
import { Tools } from 'src/app/core/utils/tools';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-landing',
  templateUrl: './landing.component.html',
  styleUrls: ['./landing.component.scss']
})
export class LandingComponent implements OnInit, AfterViewInit {
  public settings: Settings;

  urlPassport = "";
  jwtHelper = new JwtHelperService();
  param: any;
  public parm = {
    CODIGO_SISTEMA: environment.codigoSistema,
    URL_RETORNO: environment.url.urlSistema,
  }

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    public appSettings: AppSettings) {
    this.settings = this.appSettings.settings;
  }

  ngOnInit() {
    this.settings.rtl = false;
    this.urlPassport = environment.url.urlPassport + "/iniciarSesion?param=" + btoa(JSON.stringify(this.parm));
    this.route.queryParams.subscribe(params => {
      if (params['param']) {
        this.param = params['param'];
        var token = JSON.parse(atob(this.param)).TOKEN_JWT;
        if (!this.jwtHelper.isTokenExpired(token)) {
          localStorage.setItem("token", token);
          this.router.navigateByUrl("/modulo");
        }
      }
    });
  }

  ngAfterViewInit() {
    setTimeout(() => { this.settings.loadingSpinner = false }, 300);
  }

  public scrollToDemos() {
    setTimeout(() => { window.scrollTo(0, 520) });
  }

  public changeLayout(menu, menuType, isRtl) {
    this.settings.menu = menu;
    this.settings.menuType = menuType;
    this.settings.rtl = isRtl;
    this.settings.theme = 'indigo-light';
  }

  public changeTheme(theme) {
    this.settings.theme = theme;
  }

}
