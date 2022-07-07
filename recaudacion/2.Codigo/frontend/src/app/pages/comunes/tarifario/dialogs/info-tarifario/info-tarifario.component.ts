import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Combobox } from 'src/app/core/interfaces/combobox';
import { Tarifario } from 'src/app/core/models/tarifario';
import { ClasificadorIngresoService } from 'src/app/core/services/clasificador-ingreso.service';
import { GrupoRecaudacionService } from 'src/app/core/services/grupo-recaudacion.service';
import { TransversalService } from 'src/app/core/services/transversal.service';


@Component({
  selector: 'app-info-tarifario',
  templateUrl: './info-tarifario.component.html',
  styleUrls: ['./info-tarifario.component.scss']
})
export class InfoTarifarioComponent implements OnInit {

  tarifarioForm: FormGroup;

  clasigficadorIngresos = [];
  grupoRecaudaciones = [];
  preciosVariables: Combobox[] = [];

  constructor(
    public dialogRef: MatDialogRef<InfoTarifarioComponent>,
    @Inject(MAT_DIALOG_DATA) public data: Tarifario,
    public fb: FormBuilder,
    private clasificadorIngresoService: ClasificadorIngresoService,
    private grupoRecaudacionService: GrupoRecaudacionService,
    private transversalService: TransversalService,
  ) {
    this.tarifarioForm = this.fb.group({
      clasificadorIngresoId: [data.clasificadorIngresoId],
      grupoRecaudacionId: [data.grupoRecaudacionId],
      codigo: [data?.codigo.trim()],
      nombre: [data.nombre],
      porcentajeUit: [data.porcentajeUit],
      precio: [this.formatData(data.precio, 2)],
      precioVariable: [data.precioVariable],
      estado: [data.estado],
    });
  }
  ngOnInit() {
    this.clasificadorIngresoService.getClasificadorIngresos().subscribe((response) => {
      if (response.success) {
        this.clasigficadorIngresos = response.data;
      }
    }
    );
    this.grupoRecaudacionService.getGrupoRecaudacions().subscribe((response) => {
      if (response.success) {
        this.grupoRecaudaciones = response.data;
      }
    }
    );
    this.transversalService.getPreciosVariables().subscribe((response) => {
      if (response.success) {
        this.preciosVariables = response.data;
      }
    });
  }

  formatData(data, digit) {
    if (data == null) {
      return '';
    }
    return (Math.round(data * 100) / 100).toFixed(digit);
  }

  onSubmit() {

  }


}
