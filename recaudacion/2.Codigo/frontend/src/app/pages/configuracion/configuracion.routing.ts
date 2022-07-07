import { NgModule } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";
import { MenuEnum } from "src/app/core/enums/recaudacion.enum";
import { AuthGuard } from "src/app/core/guards/auth.guard";
import { ComprobanteEmisorComponent } from "./comprobante-emisor/comprobante-emisor.component";
import { ConfiguracionComponent } from "./configuracion.component";
import { TipoDocumentoComponent } from "./tipo-documento/tipo-documento.component";

const routes: Routes = [
  {
    path: "",
    component: ConfiguracionComponent,
    children: [
      {
        path: "",
        redirectTo: "comprobante-emisor",
        pathMatch: "full",
      },
      {
        path: "tipo-documento",
        component: TipoDocumentoComponent,
        canActivate: [AuthGuard],
        data: {
          breadcrumb: "Tipo de Documentos",
          codigoMenu: `${MenuEnum.TIPO_DOCUMENTO}`
        },
      },

      {
        path: "comprobante-emisor",
        component: ComprobanteEmisorComponent,
        canActivate: [AuthGuard],
        data: {
          breadcrumb: "Comprobante Emisor",
          codigoMenu: `${MenuEnum.COMPROBANTE_EMISOR}`
        },
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ConfiguracionRouteModule { }
