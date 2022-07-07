import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AlmacenRouteModule } from './almacen.routing';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AngularEditorModule } from '@kolkov/angular-editor';
import { RxReactiveFormsModule } from '@rxweb/reactive-form-validators';
import { SweetAlert2Module } from '@sweetalert2/ngx-sweetalert2';
import { MatSelectInfiniteScrollModule } from 'ng-mat-select-infinite-scroll';
import { NgxMatSelectSearchModule } from 'ngx-mat-select-search';
import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';

import { SharedModule } from 'src/app/shared/shared.module';
import { AlmacenComponent } from './almacen.component';
import { InfoGuiaSalidaBienComponent } from './guia-salida-bien/dialogs/info-guia-salida-bien/info-guia-salida-bien.component';
import { ListSaldoIngresoComponent } from './guia-salida-bien/dialogs/list-saldo-ingreso/list-saldo-ingreso.component';
import { NewGuiaSalidaBienComponent } from './guia-salida-bien/dialogs/new-guia-salida-bien/new-guia-salida-bien.component';
import { GuiaSalidaBienComponent } from './guia-salida-bien/guia-salida-bien.component';
import { InfoIngresoPecosaComponent } from './ingreso-pecosa/dialogs/info-ingreso-pecosa/info-ingreso-pecosa.component';
import { NewIngresoPecosaComponent } from './ingreso-pecosa/dialogs/new-ingreso-pecosa/new-ingreso-pecosa.component';
import { IngresoPecosaComponent } from './ingreso-pecosa/ingreso-pecosa.component';
import { KardexComponent } from './kardex/kardex.component';
import { SaldoComponent } from './saldo/saldo.component';


@NgModule({
  imports: [
    CommonModule,
    AlmacenRouteModule,
    RxReactiveFormsModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule,
    SweetAlert2Module.forRoot(),
    PerfectScrollbarModule,
    NgxMatSelectSearchModule,
    MatSelectInfiniteScrollModule,
    AngularEditorModule,
  ],
  declarations: [
    AlmacenComponent,
    GuiaSalidaBienComponent,
    NewGuiaSalidaBienComponent,
    InfoGuiaSalidaBienComponent,
    ListSaldoIngresoComponent,
    KardexComponent,
    SaldoComponent,

    IngresoPecosaComponent,
    NewIngresoPecosaComponent,
    InfoIngresoPecosaComponent,
  ],
  entryComponents: [
    NewGuiaSalidaBienComponent,
    InfoGuiaSalidaBienComponent,
    ListSaldoIngresoComponent,

    NewIngresoPecosaComponent,
    InfoIngresoPecosaComponent,
  ]
})
export class AlmacenModule { }
