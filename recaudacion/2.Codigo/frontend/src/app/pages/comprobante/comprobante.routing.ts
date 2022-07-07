import { NgModule } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";
import { MenuEnum } from "src/app/core/enums/recaudacion.enum";
import { AuthGuard } from "src/app/core/guards/auth.guard";
import { BoletaComponent } from "./boleta/container/boleta.component";
import { ComprobanteComponent } from "./comprobante.component";
import { FacturaComponent } from "./factura/container/factura.component";
import { NotaCreditoComponent } from "./nota-credito/container/nota-credito.component";
import { NotaDebitoComponent } from "./nota-debito/container/nota-debito.component";
import { RetencionComponent } from "./retencion/container/retencion.component";

const routes: Routes = [
  {
    path: "",
    component: ComprobanteComponent,
    children: [
      {
        path: "",
        redirectTo: "facturas",
        pathMatch: "full",
      },
      {
        path: "facturas",
        component: FacturaComponent,
        canActivate: [AuthGuard],
        data: {
          breadcrumb: "Facturas",
          codigoMenu: `${MenuEnum.FACTURA}`
        },
      },
      {
        path: "boletas",
        component: BoletaComponent,
        canActivate: [AuthGuard],
        data: {
          breadcrumb: "Boletas de Ventas",
          codigoMenu: `${MenuEnum.BOLETA}`
        },
      },
      {
        path: "nota-credito",
        component: NotaCreditoComponent,
        canActivate: [AuthGuard],
        data: {
          breadcrumb: "Notas de Crédito",
          codigoMenu: `${MenuEnum.NOTA_CREDITO}`
        },
      },
      {
        path: "nota-debito",
        component: NotaDebitoComponent,
        canActivate: [AuthGuard],
        data: {
          breadcrumb: "Notas de Débito",
          codigoMenu: `${MenuEnum.NOTA_DEBITO}`
        },
      },
      {
        path: "retencion",
        component: RetencionComponent,
        canActivate: [AuthGuard],
        data: {
          breadcrumb: "Comprobantes de Retención",
          codigoMenu: `${MenuEnum.RETENCION}`
        },
      }
    ]
  }
];
@NgModule({
  imports: [
    RouterModule.forChild(routes)
  ],
  exports: [RouterModule]
})
export class ComprobanteRouteModule { }
