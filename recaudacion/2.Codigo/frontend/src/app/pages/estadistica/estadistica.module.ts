import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { EstadisticaComponent } from './estadistica.component';
import { RouterModule, Routes } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from 'src/app/shared/shared.module';
import { HighchartsChartModule } from 'highcharts-angular';
import { EstadisticaTipoReciboIngresoComponent } from './estadistica-tipo-recibo-ingreso/estadistica-tipo-recibo-ingreso.component';
import { EstadisticaTipoComprobanteComponent } from './estadistica-tipo-comprobante/estadistica-tipo-comprobante.component';


const routes: Routes = [
  {
    path: '', component: EstadisticaComponent,
  }
];

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule,
    HighchartsChartModule,
    RouterModule.forChild(routes)
  ],
  declarations: [
    EstadisticaComponent,
    EstadisticaTipoReciboIngresoComponent,
    EstadisticaTipoComprobanteComponent
  ],
  providers: [

  ]

})
export class EstadisticaModule { }
