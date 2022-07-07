import { NgModule } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";
import { MenuEnum } from "../core/enums/recaudacion.enum";
import { AuthGuard } from "../core/guards/auth.guard";
import { IndexComponent } from "./index/index.component";
import { PagesComponent } from "./pages.component";

const routes: Routes = [
  {
    path: "",
    component: PagesComponent,
    children: [
      {
        path: '', redirectTo: 'index', pathMatch: 'full'
      },
      {
        path: "index",
        component: IndexComponent,
        canActivate: [AuthGuard],
        data: {
          breadcrumb: '',
          codigoMenu: `${MenuEnum.INICIO}`
        }

      },
      {
        path: "tesoreria",
        loadChildren: () => import(`./tesoreria/tesoreria.module`).then((m) => m.TesoreriaModule),
        data: {
          breadcrumb: "Tesorería"
        },
      },

      {
        path: "comprobantes",
        loadChildren: () =>
          import(`./comprobante/comprobante.module`).then(
            (m) => m.ComprobanteModule
          ),
        data: { breadcrumb: "Comprobantes de Pago" },
      },

      {
        path: "almacen",
        loadChildren: () =>
          import(`./almacen/almacen.module`).then((m) => m.AlmacenModule),
        data: { breadcrumb: "Almacenes" },
      },

      {
        path: "comunes",
        loadChildren: () =>
          import(`./comunes/comunes.module`).then((m) => m.ComunesModule),
        data: { breadcrumb: "Maestras" },
      },

      {
        path: "configuracion",
        loadChildren: () => import(`./configuracion/configuracion.module`).then((m) => m.ConfiguracionModule),
        data: { breadcrumb: "Configuraciones" },
      },

      {
        path: "estadistica",
        loadChildren: () => import(`./estadistica/estadistica.module`).then((m) => m.EstadisticaModule),
        canActivate: [AuthGuard],
        data: {
          breadcrumb: "Cuadros estadísticos",
          codigoMenu: `${MenuEnum.CUADROS_ESTADISTICO}`
        },
      },
    ],
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class PageRoutingModule { }
