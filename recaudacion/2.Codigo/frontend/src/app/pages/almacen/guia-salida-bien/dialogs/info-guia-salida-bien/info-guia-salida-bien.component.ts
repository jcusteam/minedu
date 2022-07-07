import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { GuiaSalidaBien, GuiaSalidaBienDetalle } from 'src/app/core/models/guiasalidabien';
import { FormGroup, FormBuilder } from '@angular/forms';
import { GuiaSalidaBienService } from 'src/app/core/services/guia-salida-bien.service';
import { MatTableDataSource } from '@angular/material/table';


@Component({
  selector: 'app-info-guia-salida-bien',
  templateUrl: './info-guia-salida-bien.component.html',
  styleUrls: ['./info-guia-salida-bien.component.scss']
})
export class InfoGuiaSalidaBienComponent implements OnInit {
  displayedColumns: string[] =
    [
      'nro', 'codigoItem', 'nombreItem', 'cantidad', 'serieFormato', 'serieDel', 'serieAl'
    ];
  formGuiaSalidaBien: FormGroup;
  guiaSalidaBienDetalles: GuiaSalidaBienDetalle[] = [];
  dataSource = new MatTableDataSource(this.guiaSalidaBienDetalles);

  constructor(
    public dialogRef: MatDialogRef<InfoGuiaSalidaBienComponent>,
    private fb: FormBuilder,
    @Inject(MAT_DIALOG_DATA) public data: GuiaSalidaBien,
    private guiaSalidaBienService: GuiaSalidaBienService) {
    this.formGuiaSalidaBien = this.fb.group(
      {
        numero: this.data.numero,
        fechaRegistro: this.data.fechaRegistro,
        justificacion: this.data.justificacion
      });
  }

  ngOnInit() {
    this.guiaSalidaBienService.getGuiaSalidaBienById(this.data.guiaSalidaBienId)
      .subscribe((response) => {
        if (response.success) {
          this.dataSource.data = response.data.guiaSalidaBienDetalle;
        }
      });
  }

  onSubmit() {

  }

}
