import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { CatalogoBien } from 'src/app/core/models/catalogobien';
import { ClasificadorIngreso } from 'src/app/core/models/clasificadoringreso';
import { UnidadMedida } from 'src/app/core/models/unidadmedida';
import { ClasificadorIngresoService } from 'src/app/core/services/clasificador-ingreso.service';
import { UnidadMedidaService } from 'src/app/core/services/unidad-medida.service';


@Component({
  selector: 'app-info-catalogo-bien',
  templateUrl: './info-catalogo-bien.component.html',
  styleUrls: ['./info-catalogo-bien.component.scss']
})
export class InfoCatalogoBienComponent implements OnInit {
  catalogoBienForm: FormGroup;
  catalogoBien = new CatalogoBien();

  clasigficadorIngresos: ClasificadorIngreso[] = [];
  unidadesMedidas:UnidadMedida [] = [];

  constructor(
    public dialogRef: MatDialogRef<InfoCatalogoBienComponent>,
    @Inject(MAT_DIALOG_DATA) public data: CatalogoBien,
    public fb: FormBuilder,
    private clasificadorIngresoService: ClasificadorIngresoService,
    private unidadMedidaService: UnidadMedidaService
  ) {
    this.catalogoBienForm = this.fb.group({
      clasificadorIngresoId: [data?.clasificadorIngresoId],
      unidadMedidaId: [data?.unidadMedidaId],
      codigo: [data?.codigo],
      descripcion: [data?.descripcion],
      stockMaximo: [data?.stockMaximo],
      stockMinimo: [data?.stockMinimo],
      puntoReorden: [data?.puntoReorden],
      estado: [data?.estado],
    });
  }
  ngOnInit() {
    this.clasificadorIngresoService.getClasificadorIngresos().subscribe(
      (response) => {
        if (response.success) {
          this.clasigficadorIngresos = response.data;
        }
      }
    );
    this.unidadMedidaService.getUnidadMedidas().subscribe(
      (response) => {
        if (response.success) {
          this.unidadesMedidas = response.data;
        }
      }
    );
  }

  onSubmit() {

  }

}
