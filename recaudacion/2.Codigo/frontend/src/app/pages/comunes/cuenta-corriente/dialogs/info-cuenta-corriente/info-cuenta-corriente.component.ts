import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Banco } from 'src/app/core/models/banco';
import { CuentaCorriente } from 'src/app/core/models/cuentacorriente';
import { FuenteFinanciamiento } from 'src/app/core/models/fuentefinanciamiento';
import { BancoService } from 'src/app/core/services/banco.service';
import { FuenteFinanciamientoService } from 'src/app/core/services/fuente-financiamiento.service';


@Component({
  selector: 'app-info-cuenta-corriente',
  templateUrl: './info-cuenta-corriente.component.html',
  styleUrls: ['./info-cuenta-corriente.component.scss']
})
export class InfoCuentaCorrienteComponent implements OnInit {
  form: FormGroup;

  bancos: Banco[] = [];
  fuenteFinanciamientos: FuenteFinanciamiento[] = [];

  constructor(
    public dialogRef: MatDialogRef<InfoCuentaCorrienteComponent>,
    @Inject(MAT_DIALOG_DATA) public data: CuentaCorriente,
    public fb: FormBuilder,
    private bancoService: BancoService,
    private fuenteFinanciamientoService: FuenteFinanciamientoService
  ) {
    this.form = this.fb.group({
      bancoId: [data?.bancoId],
      fuenteFinanciamientoId: [data?.fuenteFinanciamientoId],
      codigo: [data?.codigo],
      numero: [data?.numero],
      denominacion: [data?.denominacion],
      estado: [data?.estado],
    });
  }
  ngOnInit() {
    this.bancoService.getBancos().subscribe(
      (response) => {
        if (response.success) {
          this.bancos = response.data;
        }
      }
    );
    this.fuenteFinanciamientoService.getFuenteFinanciamientos().subscribe(
      (response) => {
        if (response.success) {
          this.fuenteFinanciamientos = response.data;
        }
      }
    );
  }

  onSubmit() {

  }

}
