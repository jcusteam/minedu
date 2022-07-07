import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RxReactiveFormsModule } from '@rxweb/reactive-form-validators';
import { SweetAlert2Module } from '@sweetalert2/ngx-sweetalert2';
import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { SharedModule } from 'src/app/shared/shared.module';
import { MatSelectInfiniteScrollModule } from 'ng-mat-select-infinite-scroll';
import { NgxMatSelectSearchModule } from 'ngx-mat-select-search';
import { AngularEditorModule } from '@kolkov/angular-editor';
import { MineduSharedModule } from 'src/@minedu/minedu.shared.module';

import { TesoreriaRouteModule } from './tesoreria.routing';
import { TesoreriaComponent } from './tesoreria.component';
import { ReciboIngresoComponent } from './recibo-ingreso/container/recibo-ingreso.component';
import { EditReciboIngresoComponent } from './recibo-ingreso/dialogs/edit-recibo-ingreso/edit-recibo-ingreso.component';
import { InfoReciboIngresoComponent } from './recibo-ingreso/dialogs/info-recibo-ingreso/info-recibo-ingreso.component';
import { NewReciboIngresoClienteComponent } from './recibo-ingreso/dialogs/new-recibo-ingreso-cliente/new-recibo-ingreso-cliente.component';
import { NewReciboIngresoComponent } from './recibo-ingreso/dialogs/new-recibo-ingreso/new-recibo-ingreso.component';

import { DepositoBancoComponent } from './deposito-banco/container/deposito-banco.component';
import { EditDepositoBancoComponent } from './deposito-banco/dialogs/edit-deposito-banco/edit-deposito-banco.component';
import { NewDepositoBancoComponent } from './deposito-banco/dialogs/new-deposito-banco/new-deposito-banco.component';

import { EditLiquidacionIngresoComponent } from './liquidacion-ingreso/dialogs/edit-liquidacion-ingreso/edit-liquidacion-ingreso.component';
import { InfoLiquidacionIngresoComponent } from './liquidacion-ingreso/dialogs/info-liquidacion-ingreso/info-liquidacion-ingreso.component';
import { NewLiquidacionIngresoComponent } from './liquidacion-ingreso/dialogs/new-liquidacion-ingreso/new-liquidacion-ingreso.component';
import { LiquidacionIngresoComponent } from './liquidacion-ingreso/container/liquidacion-ingreso.component';
import { EditPapeletaDepositoComponent } from './papeleta-deposito/dialogs/edit-papeleta-deposito/edit-papeleta-deposito.component';
import { InfoPapeletaDepositoComponent } from './papeleta-deposito/dialogs/info-papeleta-deposito/info-papeleta-deposito.component';
import { NewPapeletaDepositoComponent } from './papeleta-deposito/dialogs/new-papeleta-deposito/new-papeleta-deposito.component';
import { PapeletaDepositoComponent } from './papeleta-deposito/container/papeleta-deposito.component';
import { EditRegistroLineaComponent } from './registro-linea/dialogs/edit-registro-linea/edit-registro-linea.component';
import { InfoRegistroLineaComponent } from './registro-linea/dialogs/info-registro-linea/info-registro-linea.component';
import { NewRegistroLineaComponent } from './registro-linea/dialogs/new-registro-linea/new-registro-linea.component';
import { ObsRegistroLineaComponent } from './registro-linea/dialogs/obs-registro-linea/obs-registro-linea.component';
import { RegistroLineaComponent } from './registro-linea/container/registro-linea.component';


@NgModule({
  imports: [
    CommonModule,
    TesoreriaRouteModule,
    RxReactiveFormsModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule,
    SweetAlert2Module.forRoot(),
    PerfectScrollbarModule,
    NgxMatSelectSearchModule,
    MatSelectInfiniteScrollModule,
    AngularEditorModule,
    MineduSharedModule
  ],
  declarations: [
    TesoreriaComponent,
    DepositoBancoComponent,
    NewDepositoBancoComponent,
    EditDepositoBancoComponent,
    LiquidacionIngresoComponent,
    NewLiquidacionIngresoComponent,
    EditLiquidacionIngresoComponent,
    InfoLiquidacionIngresoComponent,

    PapeletaDepositoComponent,
    NewPapeletaDepositoComponent,
    EditPapeletaDepositoComponent,
    InfoPapeletaDepositoComponent,

    ReciboIngresoComponent,
    NewReciboIngresoComponent,
    EditReciboIngresoComponent,
    InfoReciboIngresoComponent,
    NewReciboIngresoClienteComponent,

    RegistroLineaComponent,
    NewRegistroLineaComponent,
    EditRegistroLineaComponent,
    InfoRegistroLineaComponent,
    ObsRegistroLineaComponent,


  ],
  entryComponents: [
    NewDepositoBancoComponent,
    EditDepositoBancoComponent,

    NewLiquidacionIngresoComponent,
    EditLiquidacionIngresoComponent,
    InfoLiquidacionIngresoComponent,

    NewPapeletaDepositoComponent,
    EditPapeletaDepositoComponent,
    InfoPapeletaDepositoComponent,

    NewReciboIngresoComponent,
    EditReciboIngresoComponent,
    InfoReciboIngresoComponent,
    NewReciboIngresoClienteComponent,

    NewRegistroLineaComponent,
    EditRegistroLineaComponent,
    InfoRegistroLineaComponent,
    ObsRegistroLineaComponent,
  ]
})
export class TesoreriaModule { }
