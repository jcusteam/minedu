import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { MenuEnum } from 'src/app/core/enums/recaudacion.enum';
import { AuthGuard } from 'src/app/core/guards/auth.guard';
import { AlmacenComponent } from './almacen.component';
import { GuiaSalidaBienComponent } from './guia-salida-bien/guia-salida-bien.component';
import { IngresoPecosaComponent } from './ingreso-pecosa/ingreso-pecosa.component';
import { KardexComponent } from './kardex/kardex.component';
import { SaldoComponent } from './saldo/saldo.component';

const routes: Routes = [
  {
    path: '',
    component: AlmacenComponent,
    children: [
      {
        path: '', redirectTo: 'ingreso-pecosa', pathMatch: 'full'
      },
      {
        path: 'ingreso-pecosa', component: IngresoPecosaComponent,
        canActivate: [AuthGuard],
        data: {
          breadcrumb: 'Ingreso de Pecosas',
          codigoMenu: `${MenuEnum.INGRESO_PECOSA}`
        }
      },
      {
        path: 'salida-bienes', component: GuiaSalidaBienComponent,
        canActivate: [AuthGuard],
        data: {
          breadcrumb: 'Guia de Salida de Bienes',
          codigoMenu: `${MenuEnum.GUIA_SALIDA}`
        }
      },
      {
        path: 'kardex', component: KardexComponent,
        canActivate: [AuthGuard],
        data: {
          breadcrumb: 'Kardex de Almacén',
          codigoMenu: `${MenuEnum.KARDEX}`
        }
      },
      {
        path: 'saldos', component: SaldoComponent,
        canActivate: [AuthGuard],
        data: {
          breadcrumb: 'Saldos de Almacén',
          codigoMenu: `${MenuEnum.SALDO_ALMACEN}`
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
export class AlmacenRouteModule { }

