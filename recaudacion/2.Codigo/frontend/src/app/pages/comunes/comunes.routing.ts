import { NgModule } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";
import { MenuEnum } from "src/app/core/enums/recaudacion.enum";
import { AuthGuard } from "src/app/core/guards/auth.guard";
import { BancoComponent } from "./banco/banco.component";
import { CatalogoBienComponent } from "./catalogo-bien/catalogo-bien.component";
import { ClasificadorIngresoComponent } from "./clasificador-ingreso/clasificador-ingreso.component";
import { ClienteComponent } from "./cliente/cliente.component";
import { ComunesComponent } from "./comunes.component";
import { CuentaContableComponent } from "./cuenta-contable/cuenta-contable.component";
import { CuentaCorrienteComponent } from "./cuenta-corriente/cuenta-corriente.component";
import { FuenteFinanciamientoComponent } from "./fuente-financiamiento/fuente-financiamiento.component";
import { GrupoRecaudacionComponent } from "./grupo-recaudacion/grupo-recaudacion.component";
import { TarifarioComponent } from "./tarifario/tarifario.component";
import { TipoCaptacionComponent } from "./tipo-captacion/tipo-captacion.component";
import { TipoComprobantePagoComponent } from "./tipo-comprobante-pago/tipo-comprobante-pago.component";
import { TipoDocumentoIdentidadComponent } from "./tipo-documento-identidad/tipo-documento-identidad.component";
import { TipoReciboIngresoComponent } from "./tipo-recibo-ingreso/tipo-recibo-ingreso.component";
import { UitComponent } from "./uit/uit.component";
import { UnidadEjecutoraComponent } from "./unidad-ejecutora/unidad-ejecutora.component";
import { UnidadMedidaComponent } from "./unidad-medida/unidad-medida.component";

const routes: Routes = [
  {
    path: "",
    component: ComunesComponent,
    children: [
      {
        path: "",
        redirectTo: "tarifario",
        pathMatch: "full",
      },
      {
        path: "tarifario",
        component: TarifarioComponent,
        canActivate: [AuthGuard],
        data: {
          breadcrumb: "Tarifario Institucional",
          codigoMenu: `${MenuEnum.TARIFARIO}`
        },
      },
      {
        path: "cuentas-corrientes",
        component: CuentaCorrienteComponent,
        canActivate: [AuthGuard],
        data: {
          breadcrumb: "Cuentas Corrientes",
          codigoMenu: `${MenuEnum.CUENTA_CORRIENTE}`
        },
      },
      {
        path: "clasificador-ingresos",
        component: ClasificadorIngresoComponent,
        canActivate: [AuthGuard],
        data: {
          breadcrumb: "Clasificador de Ingresos",
          codigoMenu: `${MenuEnum.CLASIFICADOR_INGRESO}`
        },
      },
      {
        path: "catalogo-bienes",
        component: CatalogoBienComponent,
        canActivate: [AuthGuard],
        data: {
          breadcrumb: "Catálogo de Bienes",
          codigoMenu: `${MenuEnum.CATALOGO_BIEN}`
        },
      },
      {
        path: "grupo-recaudacion",
        component: GrupoRecaudacionComponent,
        canActivate: [AuthGuard],
        data: {
          breadcrumb: "Grupo de Recaudación",
          codigoMenu: `${MenuEnum.GRUPO_RECAUDACION}`
        },
      },
      {
        path: "unidad-ejecutora",
        component: UnidadEjecutoraComponent,
        canActivate: [AuthGuard],
        data: {
          breadcrumb: "Unidades Ejecutoras",
          codigoMenu: `${MenuEnum.UNIDA_EJECUTORA}`
        },
      },
      {
        path: "banco",
        component: BancoComponent,
        canActivate: [AuthGuard],
        data: {
          breadcrumb: "Bancos",
          codigoMenu: `${MenuEnum.BANCO}`
        },
      },
      {
        path: "cuentas-contables",
        component: CuentaContableComponent,
        canActivate: [AuthGuard],
        data: {
          breadcrumb: "Cuentas Contables",
          codigoMenu: `${MenuEnum.CUENTA_CONTABLE}`
        },
      },
      {
        path: "tipo-recibo-ingreso",
        component: TipoReciboIngresoComponent,
        canActivate: [AuthGuard],
        data: {
          breadcrumb: "Tipos de Recibos de Ingresos",
          codigoMenu: `${MenuEnum.TIPO_RECIBO_INGRESO}`
        },
      },
      {
        path: "fuente-financiamiento",
        component: FuenteFinanciamientoComponent,
        canActivate: [AuthGuard],
        data: {
          breadcrumb: "Fuentes de Financiamiento",
          codigoMenu: `${MenuEnum.FUENTE_FINANCIAMIENTO}`
        },
      },
      {
        path: "clientes",
        component: ClienteComponent,
        canActivate: [AuthGuard],
        data: {
          breadcrumb: "Clientes",
          codigoMenu: `${MenuEnum.CLIENTE}`
        },
      },
      {
        path: "uit",
        component: UitComponent,
        canActivate: [AuthGuard],
        data: {
          breadcrumb: "UIT",
          codigoMenu: `${MenuEnum.UIT}`
        },
      },
      {
        path: "unidad-medida",
        component: UnidadMedidaComponent,
        canActivate: [AuthGuard],
        data: {
          breadcrumb: "Unidad de Medida",
          codigoMenu: `${MenuEnum.UNIDAD_MEDIDA}`
        },
      },
      {
        path: "tipo-comprobante-pago",
        component: TipoComprobantePagoComponent,
        canActivate: [AuthGuard],
        data: {
          breadcrumb: "Tipo de Comprobante de Pago",
          codigoMenu: `${MenuEnum.TIPO_COMPROBANTE}`
        },
      },
      {
        path: "tipo-captacion",
        component: TipoCaptacionComponent,
        canActivate: [AuthGuard],
        data: {
          breadcrumb: "Tipo de Captación",
          codigoMenu: `${MenuEnum.TIPO_CAPTACION}`
        },
      },
      {
        path: "tipo-documento-identidad",
        component: TipoDocumentoIdentidadComponent,
        canActivate: [AuthGuard],
        data: {
          breadcrumb: "Tipo de Documento de Identidad",
          codigoMenu: `${MenuEnum.TIPO_DOC_IDENTIDAD}`
        },
      },
    ],
  },
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ComunesRouteModule { }
