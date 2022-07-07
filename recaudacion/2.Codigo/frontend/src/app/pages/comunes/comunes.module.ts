import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { AngularEditorModule } from "@kolkov/angular-editor";
import { RxReactiveFormsModule } from "@rxweb/reactive-form-validators";
import { SweetAlert2Module } from "@sweetalert2/ngx-sweetalert2";
import { MatSelectInfiniteScrollModule } from "ng-mat-select-infinite-scroll";
import { NgxMatSelectSearchModule } from "ngx-mat-select-search";
import { PerfectScrollbarModule } from "ngx-perfect-scrollbar";
import { SharedModule } from "src/app/shared/shared.module";

import { ComunesRouteModule } from "./comunes.routing";
import { ComunesComponent } from "./comunes.component";
import { BancoComponent } from "./banco/banco.component";
import { EditBancoComponent } from "./banco/dialogs/edit-banco/edit-banco.component";
import { InfoBancoComponent } from "./banco/dialogs/info-banco/info-banco.component";
import { NewBancoComponent } from "./banco/dialogs/new-banco/new-banco.component";
import { CatalogoBienComponent } from "./catalogo-bien/catalogo-bien.component";
import { EditCatalogoBienComponent } from "./catalogo-bien/dialogs/edit-catalogo-bien/edit-catalogo-bien.component";
import { InfoCatalogoBienComponent } from "./catalogo-bien/dialogs/info-catalogo-bien/info-catalogo-bien.component";
import { NewCatalogoBienComponent } from "./catalogo-bien/dialogs/new-catalogo-bien/new-catalogo-bien.component";
import { ClasificadorIngresoComponent } from "./clasificador-ingreso/clasificador-ingreso.component";
import { EditClasificadorIngresoComponent } from "./clasificador-ingreso/dialogs/edit-clasificador-ingreso/edit-clasificador-ingreso.component";
import { InfoClasificadorIngresoComponent } from "./clasificador-ingreso/dialogs/info-clasificador-ingreso/info-clasificador-ingreso.component";
import { NewClasificadorIngresoComponent } from "./clasificador-ingreso/dialogs/new-clasificador-ingreso/new-clasificador-ingreso.component";
import { CuentaContableComponent } from "./cuenta-contable/cuenta-contable.component";
import { EditCuentaContableComponent } from "./cuenta-contable/dialogs/edit-cuenta-contable/edit-cuenta-contable.component";
import { InfoCuentaContableComponent } from "./cuenta-contable/dialogs/info-cuenta-contable/info-cuenta-contable.component";
import { NewCuentaContableComponent } from "./cuenta-contable/dialogs/new-cuenta-contable/new-cuenta-contable.component";
import { CuentaCorrienteComponent } from "./cuenta-corriente/cuenta-corriente.component";
import { EditCuentaCorrienteComponent } from "./cuenta-corriente/dialogs/edit-cuenta-corriente/edit-cuenta-corriente.component";
import { InfoCuentaCorrienteComponent } from "./cuenta-corriente/dialogs/info-cuenta-corriente/info-cuenta-corriente.component";
import { NewCuentaCorrienteComponent } from "./cuenta-corriente/dialogs/new-cuenta-corriente/new-cuenta-corriente.component";
import { EditFuenteFinanciamientoComponent } from "./fuente-financiamiento/dialogs/edit-fuente-financiamiento/edit-fuente-financiamiento.component";
import { InfoFuenteFinanciamientoComponent } from "./fuente-financiamiento/dialogs/info-fuente-financiamiento/info-fuente-financiamiento.component";
import { NewFuenteFinanciamientoComponent } from "./fuente-financiamiento/dialogs/new-fuente-financiamiento/new-fuente-financiamiento.component";
import { FuenteFinanciamientoComponent } from "./fuente-financiamiento/fuente-financiamiento.component";
import { EditGrupoRecaudacionComponent } from "./grupo-recaudacion/dialogs/edit-grupo-recaudacion/edit-grupo-recaudacion.component";
import { InfoGrupoRecaudacionComponent } from "./grupo-recaudacion/dialogs/info-grupo-recaudacion/info-grupo-recaudacion.component";
import { NewGrupoRecaudacionComponent } from "./grupo-recaudacion/dialogs/new-grupo-recaudacion/new-grupo-recaudacion.component";
import { GrupoRecaudacionComponent } from "./grupo-recaudacion/grupo-recaudacion.component";
import { EditTarifarioComponent } from "./tarifario/dialogs/edit-tarifario/edit-tarifario.component";
import { InfoTarifarioComponent } from "./tarifario/dialogs/info-tarifario/info-tarifario.component";
import { NewTarifarioComponent } from "./tarifario/dialogs/new-tarifario/new-tarifario.component";
import { TarifarioComponent } from "./tarifario/tarifario.component";
import { EditTipoCaptacionComponent } from "./tipo-captacion/dialogs/edit-tipo-captacion/edit-tipo-captacion.component";
import { InfoTipoCaptacionComponent } from "./tipo-captacion/dialogs/info-tipo-captacion/info-tipo-captacion.component";
import { NewTipoCaptacionComponent } from "./tipo-captacion/dialogs/new-tipo-captacion/new-tipo-captacion.component";
import { TipoCaptacionComponent } from "./tipo-captacion/tipo-captacion.component";
import { EditTipoComprobantePagoComponent } from "./tipo-comprobante-pago/dialogs/edit-tipo-comprobante-pago/edit-tipo-comprobante-pago.component";
import { InfoTipoComprobantePagoComponent } from "./tipo-comprobante-pago/dialogs/info-tipo-comprobante-pago/info-tipo-comprobante-pago.component";
import { NewTipoComprobantePagoComponent } from "./tipo-comprobante-pago/dialogs/new-tipo-comprobante-pago/new-tipo-comprobante-pago.component";
import { TipoComprobantePagoComponent } from "./tipo-comprobante-pago/tipo-comprobante-pago.component";
import { EditTipoDocumentoIdentidadComponent } from "./tipo-documento-identidad/dialogs/edit-tipo-documento-identidad/edit-tipo-documento-identidad.component";
import { InfoTipoDocumentoIdentidadComponent } from "./tipo-documento-identidad/dialogs/info-tipo-documento-identidad/info-tipo-documento-identidad.component";
import { NewTipoDocumentoIdentidadComponent } from "./tipo-documento-identidad/dialogs/new-tipo-documento-identidad/new-tipo-documento-identidad.component";
import { TipoDocumentoIdentidadComponent } from "./tipo-documento-identidad/tipo-documento-identidad.component";
import { EditTipoReciboIngresoComponent } from "./tipo-recibo-ingreso/dialogs/edit-tipo-recibo-ingreso/edit-tipo-recibo-ingreso.component";
import { InfoTipoReciboIngresoComponent } from "./tipo-recibo-ingreso/dialogs/info-tipo-recibo-ingreso/info-tipo-recibo-ingreso.component";
import { NewTipoReciboIngresoComponent } from "./tipo-recibo-ingreso/dialogs/new-tipo-recibo-ingreso/new-tipo-recibo-ingreso.component";
import { TipoReciboIngresoComponent } from "./tipo-recibo-ingreso/tipo-recibo-ingreso.component";
import { EditUitComponent } from "./uit/dialogs/edit-uit/edit-uit.component";
import { InfoUitComponent } from "./uit/dialogs/info-uit/info-uit.component";
import { NewUitComponent } from "./uit/dialogs/new-uit/new-uit.component";
import { UitComponent } from "./uit/uit.component";
import { EditUnidadEjecutoraComponent } from "./unidad-ejecutora/dialogs/edit-unidad-ejecutora/edit-unidad-ejecutora.component";
import { InfoUnidadEjecutoraComponent } from "./unidad-ejecutora/dialogs/info-unidad-ejecutora/info-unidad-ejecutora.component";
import { NewUnidadEjecutoraComponent } from "./unidad-ejecutora/dialogs/new-unidad-ejecutora/new-unidad-ejecutora.component";
import { UnidadEjecutoraComponent } from "./unidad-ejecutora/unidad-ejecutora.component";
import { EditUnidadMedidaComponent } from "./unidad-medida/dialogs/edit-unidad-medida/edit-unidad-medida.component";
import { InfoUnidadMedidaComponent } from "./unidad-medida/dialogs/info-unidad-medida/info-unidad-medida.component";
import { NewUnidadMedidaComponent } from "./unidad-medida/dialogs/new-unidad-medida/new-unidad-medida.component";
import { UnidadMedidaComponent } from "./unidad-medida/unidad-medida.component";
import { ClienteComponent } from "./cliente/cliente.component";
import { EditClienteComponent } from "./cliente/dialogs/edit-cliente/edit-cliente.component";
import { InfoClienteComponent } from "./cliente/dialogs/info-cliente/info-cliente.component";
import { NewClienteComponent } from "./cliente/dialogs/new-cliente/new-cliente.component";
import { MineduSharedModule } from "src/@minedu/minedu.shared.module";


@NgModule({
  imports: [
    CommonModule,
    ComunesRouteModule,
    RxReactiveFormsModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule,
    SweetAlert2Module.forRoot(),
    PerfectScrollbarModule,
    NgxMatSelectSearchModule,
    MatSelectInfiniteScrollModule,
    AngularEditorModule,
    MineduSharedModule,
  ],
  declarations: [
    ComunesComponent,
    BancoComponent,
    NewBancoComponent,
    EditBancoComponent,
    InfoBancoComponent,

    CatalogoBienComponent,
    NewCatalogoBienComponent,
    EditCatalogoBienComponent,
    InfoCatalogoBienComponent,

    ClasificadorIngresoComponent,
    NewClasificadorIngresoComponent,
    EditClasificadorIngresoComponent,
    InfoClasificadorIngresoComponent,

    ClienteComponent,
    NewClienteComponent,
    InfoClienteComponent,
    EditClienteComponent,

    CuentaContableComponent,
    NewCuentaContableComponent,
    EditCuentaContableComponent,
    InfoCuentaContableComponent,

    CuentaCorrienteComponent,
    NewCuentaCorrienteComponent,
    EditCuentaCorrienteComponent,
    InfoCuentaCorrienteComponent,

    FuenteFinanciamientoComponent,
    NewFuenteFinanciamientoComponent,
    EditFuenteFinanciamientoComponent,
    InfoFuenteFinanciamientoComponent,

    GrupoRecaudacionComponent,
    NewGrupoRecaudacionComponent,
    EditGrupoRecaudacionComponent,
    InfoGrupoRecaudacionComponent,

    TarifarioComponent,
    NewTarifarioComponent,
    EditTarifarioComponent,
    InfoTarifarioComponent,

    TipoCaptacionComponent,
    NewTipoCaptacionComponent,
    EditTipoCaptacionComponent,
    InfoTipoCaptacionComponent,

    TipoComprobantePagoComponent,
    NewTipoComprobantePagoComponent,
    EditTipoComprobantePagoComponent,
    InfoTipoComprobantePagoComponent,

    TipoDocumentoIdentidadComponent,
    NewTipoDocumentoIdentidadComponent,
    EditTipoDocumentoIdentidadComponent,
    InfoTipoDocumentoIdentidadComponent,

    TipoReciboIngresoComponent,
    NewTipoReciboIngresoComponent,
    EditTipoReciboIngresoComponent,
    InfoTipoReciboIngresoComponent,

    UitComponent,
    NewUitComponent,
    EditUitComponent,
    InfoUitComponent,

    UnidadEjecutoraComponent,
    NewUnidadEjecutoraComponent,
    EditUnidadEjecutoraComponent,
    InfoUnidadEjecutoraComponent,

    UnidadMedidaComponent,
    NewUnidadMedidaComponent,
    EditUnidadMedidaComponent,
    InfoUnidadMedidaComponent,
  ],
  entryComponents: [
    NewBancoComponent,
    EditBancoComponent,
    InfoBancoComponent,

    NewCatalogoBienComponent,
    EditCatalogoBienComponent,
    InfoCatalogoBienComponent,

    NewClasificadorIngresoComponent,
    EditClasificadorIngresoComponent,
    InfoClasificadorIngresoComponent,

    NewClienteComponent,
    InfoClienteComponent,
    EditClienteComponent,

    NewCuentaContableComponent,
    EditCuentaContableComponent,
    InfoCuentaContableComponent,

    NewCuentaCorrienteComponent,
    EditCuentaCorrienteComponent,
    InfoCuentaCorrienteComponent,

    NewFuenteFinanciamientoComponent,
    EditFuenteFinanciamientoComponent,
    InfoFuenteFinanciamientoComponent,

    NewGrupoRecaudacionComponent,
    EditGrupoRecaudacionComponent,
    InfoGrupoRecaudacionComponent,

    NewTarifarioComponent,
    EditTarifarioComponent,
    InfoTarifarioComponent,

    NewTipoCaptacionComponent,
    EditTipoCaptacionComponent,
    InfoTipoCaptacionComponent,

    NewTipoComprobantePagoComponent,
    EditTipoComprobantePagoComponent,
    InfoTipoComprobantePagoComponent,

    NewTipoDocumentoIdentidadComponent,
    EditTipoDocumentoIdentidadComponent,
    InfoTipoDocumentoIdentidadComponent,

    NewTipoReciboIngresoComponent,
    EditTipoReciboIngresoComponent,
    InfoTipoReciboIngresoComponent,

    NewUitComponent,
    EditUitComponent,
    InfoUitComponent,

    NewUnidadEjecutoraComponent,
    EditUnidadEjecutoraComponent,
    InfoUnidadEjecutoraComponent,

    NewUnidadMedidaComponent,
    EditUnidadMedidaComponent,
    InfoUnidadMedidaComponent,
  ]
})
export class ComunesModule {}
