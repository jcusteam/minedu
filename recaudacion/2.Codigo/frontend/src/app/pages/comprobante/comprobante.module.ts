import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { ComprobanteRouteModule } from "./comprobante.routing";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { AngularEditorModule } from "@kolkov/angular-editor";
import { RxReactiveFormsModule } from "@rxweb/reactive-form-validators";
import { SweetAlert2Module } from "@sweetalert2/ngx-sweetalert2";
import { MatSelectInfiniteScrollModule } from "ng-mat-select-infinite-scroll";
import { NgxMatSelectSearchModule } from "ngx-mat-select-search";
import { PerfectScrollbarModule } from "ngx-perfect-scrollbar";
import { SharedModule } from "src/app/shared/shared.module";
import { MineduSharedModule } from "src/@minedu/minedu.shared.module";

import { BoletaComponent } from "./boleta/container/boleta.component";
import { InfoBoletaComponent } from "./boleta/dialogs/info-boleta/info-boleta.component";
import { ListSaldoIngresoBoletaComponent } from "./boleta/dialogs/list-saldo-ingreso/list-saldo-ingreso.component";
import { NewBoletaClienteComponent } from "./boleta/dialogs/new-boleta-cliente/new-boleta-cliente.component";
import { NewBoletaComponent } from "./boleta/dialogs/new-boleta/new-boleta.component";
import { ComprobanteComponent } from "./comprobante.component";

import { InfoFacturaComponent } from "./factura/dialogs/info-factura/info-factura.component";
import { NewFacturaClienteComponent } from "./factura/dialogs/new-factura-cliente/new-factura-cliente.component";
import { NewFacturaComponent } from "./factura/dialogs/new-factura/new-factura.component";
import { FacturaComponent } from "./factura/container/factura.component";

import { InfoNotaCreditoComponent } from "./nota-credito/dialogs/info-nota-credito/info-nota-credito.component";
import { NewNotaCreditoComponent } from "./nota-credito/dialogs/new-nota-credito/new-nota-credito.component";
import { NotaCreditoComponent } from "./nota-credito/container/nota-credito.component";
import { InfoNotaDebitoComponent } from "./nota-debito/dialogs/info-nota-debito/info-nota-debito.component";
import { NewNotaDebitoComponent } from "./nota-debito/dialogs/new-nota-debito/new-nota-debito.component";
import { NotaDebitoComponent } from "./nota-debito/container/nota-debito.component";
import { InfoRetencionComponent } from "./retencion/dialogs/info-retencion/info-retencion.component";
import { NewRetencionDetalleComponent } from "./retencion/dialogs/new-retencion-detalle/new-retencion-detalle.component";
import { NewRetencionComponent } from "./retencion/dialogs/new-retencion/new-retencion.component";
import { RetencionComponent } from "./retencion/container/retencion.component";
import { ListSaldoIngresoFacturaComponent } from "./factura/dialogs/list-saldo-ingreso/list-saldo-ingreso.component";
import { ListSaldoIngresoNotaCreditoComponent } from "./nota-credito/dialogs/list-saldo-ingreso/list-saldo-ingreso.component";
import { ListSaldoIngresoNotaDebitoComponent } from "./nota-debito/dialogs/list-saldo-ingreso/list-saldo-ingreso.component";
import { NewNotaCreditoClienteComponent } from "./nota-credito/dialogs/new-nota-credito-cliente/new-nota-credito-cliente.component";
import { NewRetencionClienteComponent } from './retencion/dialogs/new-retencion-cliente/new-retencion-cliente.component';
import { NewNotaDebitoClienteComponent } from './nota-debito/dialogs/new-nota-debito-cliente/new-nota-debito-cliente.component';


@NgModule({
  imports: [
    CommonModule,
    ComprobanteRouteModule,
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
    ComprobanteComponent,
    BoletaComponent,
    NewBoletaComponent,
    InfoBoletaComponent,
    ListSaldoIngresoBoletaComponent,
    NewBoletaClienteComponent,
  
    FacturaComponent,
    NewFacturaComponent,
    InfoFacturaComponent,
    ListSaldoIngresoFacturaComponent,
    NewFacturaClienteComponent,

    NotaCreditoComponent, 
    NewNotaCreditoComponent,
    InfoNotaCreditoComponent,
    ListSaldoIngresoNotaCreditoComponent,
    NewNotaCreditoClienteComponent,

    NotaDebitoComponent, 
    NewNotaDebitoComponent,
    InfoNotaDebitoComponent,
    ListSaldoIngresoNotaDebitoComponent,
    NewNotaDebitoClienteComponent,

    RetencionComponent,
    NewRetencionComponent,
    InfoRetencionComponent,
    NewRetencionDetalleComponent,
    NewRetencionClienteComponent,
  ],
  entryComponents: [
    InfoBoletaComponent,
    NewBoletaComponent,
    ListSaldoIngresoBoletaComponent,
    NewBoletaClienteComponent,
    
    NewFacturaComponent,
    InfoFacturaComponent,
    ListSaldoIngresoFacturaComponent,
    NewFacturaClienteComponent,
    
    NewNotaCreditoComponent,
    InfoNotaCreditoComponent,
    ListSaldoIngresoNotaCreditoComponent,
    NewNotaCreditoClienteComponent,

    NewNotaDebitoComponent,
    InfoNotaDebitoComponent,
    ListSaldoIngresoNotaDebitoComponent,
    NewNotaDebitoClienteComponent,

    NewRetencionComponent,
    InfoRetencionComponent,
    NewRetencionDetalleComponent,
    NewRetencionClienteComponent

  ]
})
export class ComprobanteModule {}
