import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { MenuEnum } from 'src/app/core/enums/recaudacion.enum';
import { AuthGuard } from 'src/app/core/guards/auth.guard';
import { DepositoBancoComponent } from './deposito-banco/container/deposito-banco.component';
import { LiquidacionIngresoComponent } from './liquidacion-ingreso/container/liquidacion-ingreso.component';
import { PapeletaDepositoComponent } from './papeleta-deposito/container/papeleta-deposito.component';
import { ReciboIngresoComponent } from './recibo-ingreso/container/recibo-ingreso.component';
import { RegistroLineaComponent } from './registro-linea/container/registro-linea.component';
import { TesoreriaComponent } from './tesoreria.component';

const routes: Routes = [
  {
    path: '',
    component: TesoreriaComponent,
    children: [
      {
        path: '', redirectTo: 'recibo-ingreso', pathMatch: 'full'
      },
      {
        path: 'deposito-cuentas-corrientes', component: DepositoBancoComponent,
        canActivate: [AuthGuard],
        data: {
          breadcrumb: 'Depósito en Cuentas Corrientes',
          codigoMenu: `${MenuEnum.DEPOSITO_BANCO}`
        }
      },
      {
        path: 'liquidacion-ingreso', component: LiquidacionIngresoComponent,
        canActivate: [AuthGuard],
        data: {
          breadcrumb: 'Liquidación de Ingresos',
          codigoMenu: `${MenuEnum.LIQUIDACION_INGRESO}`
        }
      },
      {
        path: 'papeleta-deposito', component: PapeletaDepositoComponent,
        canActivate: [AuthGuard],
        data: {
          breadcrumb: 'Papeleta de Depósitos',
          codigoMenu: `${MenuEnum.PAPELETA_DEPOSITO}`
        }
      },
      {
        path: 'recibo-ingreso', component: ReciboIngresoComponent,
        canActivate: [AuthGuard],
        data: {
          breadcrumb: 'Recibo de Ingresos',
          codigoMenu: `${MenuEnum.RECIBO_INGRESO}`
        }
      },
      {
        path: 'registro-linea', component: RegistroLineaComponent,
        canActivate: [AuthGuard],
        data: {
          breadcrumb: 'Registro en Línea',
          codigoMenu: `${MenuEnum.REGISTRO_LINEA}`
        }
      },
    ]
  },

];

@NgModule({
  imports: [
    RouterModule.forChild(routes)
  ],
  exports: [RouterModule]
})
export class TesoreriaRouteModule { }
