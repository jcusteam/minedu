import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ConfiguracionRouteModule } from './configuracion.routing';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AngularEditorModule } from '@kolkov/angular-editor';
import { RxReactiveFormsModule } from '@rxweb/reactive-form-validators';
import { SweetAlert2Module } from '@sweetalert2/ngx-sweetalert2';
import { MatSelectInfiniteScrollModule } from 'ng-mat-select-infinite-scroll';
import { NgxMatSelectSearchModule } from 'ngx-mat-select-search';
import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { SharedModule } from 'src/app/shared/shared.module';
import { ConfiguracionComponent } from './configuracion.component';
import { MatPaginatorIntl } from '@angular/material/paginator';
import { ComprobanteEmisorComponent } from './comprobante-emisor/comprobante-emisor.component';
import { EditComprobanteEmisorComponent } from './comprobante-emisor/dialogs/edit-comprobante-emisor/edit-comprobante-emisor.component';
import { InfoComprobanteEmisorComponent } from './comprobante-emisor/dialogs/info-comprobante-emisor/info-comprobante-emisor.component';
import { NewComprobanteEmisorComponent } from './comprobante-emisor/dialogs/new-comprobante-emisor/new-comprobante-emisor.component';
import { EditTipoDocumentoEstadoComponent } from './tipo-documento/dialogs/edit-tipo-documento-estado/edit-tipo-documento-estado.component';
import { EditTipoDocumentoParametroComponent } from './tipo-documento/dialogs/edit-tipo-documento-parametro/edit-tipo-documento-parametro.component';
import { EditTipoDocumentoComponent } from './tipo-documento/dialogs/edit-tipo-documento/edit-tipo-documento.component';
import { InfoTipoDocumentoComponent } from './tipo-documento/dialogs/info-tipo-documento/info-tipo-documento.component';
import { ListTipoDocumentoEstadoComponent } from './tipo-documento/dialogs/list-tipo-documento-estado/list-tipo-documento-estado.component';
import { NewTipoDocumentoComponent } from './tipo-documento/dialogs/new-tipo-documento/new-tipo-documento.component';
import { TipoDocumentoComponent } from './tipo-documento/tipo-documento.component';

@NgModule({
  imports: [
    CommonModule,
    ConfiguracionRouteModule,
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
    ConfiguracionComponent,
    ComprobanteEmisorComponent,
    NewComprobanteEmisorComponent,
    EditComprobanteEmisorComponent,
    InfoComprobanteEmisorComponent,
    
    TipoDocumentoComponent,
    NewTipoDocumentoComponent,
    EditTipoDocumentoComponent,
    InfoTipoDocumentoComponent,
    ListTipoDocumentoEstadoComponent,
    EditTipoDocumentoEstadoComponent,
    EditTipoDocumentoParametroComponent

  ],
  entryComponents: [
    NewComprobanteEmisorComponent,
    EditComprobanteEmisorComponent,
    InfoComprobanteEmisorComponent,

    NewTipoDocumentoComponent,
    EditTipoDocumentoComponent,
    InfoTipoDocumentoComponent,
    ListTipoDocumentoEstadoComponent,
    EditTipoDocumentoEstadoComponent,
    EditTipoDocumentoParametroComponent
  ],
})
export class ConfiguracionModule { }
