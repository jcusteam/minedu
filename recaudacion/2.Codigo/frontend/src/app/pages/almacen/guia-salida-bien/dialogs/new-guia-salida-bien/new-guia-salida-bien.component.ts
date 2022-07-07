import { Component, OnInit, Inject } from '@angular/core';
import { DatePipe } from '@angular/common';
import { MAT_DATE_LOCALE, DateAdapter, MAT_DATE_FORMATS } from '@angular/material/core';
import { MomentDateAdapter, MAT_MOMENT_DATE_ADAPTER_OPTIONS } from '@angular/material-moment-adapter';
import { MatTableDataSource } from '@angular/material/table';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { GuiaSalidaBien, GuiaSalidaBienDetalle } from 'src/app/core/models/guiasalidabien';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { GuiaSalidaBienService } from 'src/app/core/services/guia-salida-bien.service';
import { CatalogoBien } from 'src/app/core/models/catalogobien';
import { CatalogoBienesService } from 'src/app/core/services/catalogo-bienes.service';
import { ListSaldoIngresoComponent } from '../list-saldo-ingreso/list-saldo-ingreso.component';
import { MessageService } from 'src/app/core/services/message.service';
import { IngresoPecosaDetalle } from 'src/app/core/models/ingresopecosa';
import { MESSAGES, TYPE_MESSAGE } from 'src/app/core/utils/messages';

const DATE_MODE_FORMATS = {
  parse: {
    dateInput: "DD/MM/YYYY",
  },
  display: {
    dateInput: "DD/MM/YYYY",
    monthYearLabel: "MMM YYYY",
    dateA11yLabel: "LL",
    monthYearA11yLabel: "MMMM YYYY",
  },
};

@Component({
  selector: 'app-new-guia-salida-bien',
  templateUrl: './new-guia-salida-bien.component.html',
  styleUrls: ['./new-guia-salida-bien.component.scss'],
  providers: [
    { provide: DatePipe },
    { provide: MAT_DATE_LOCALE, useValue: "es-ES" },
    {
      provide: DateAdapter,
      useClass: MomentDateAdapter,
      deps: [MAT_DATE_LOCALE, MAT_MOMENT_DATE_ADAPTER_OPTIONS],
    },
    { provide: MAT_DATE_FORMATS, useValue: DATE_MODE_FORMATS },
  ],
})
export class NewGuiaSalidaBienComponent implements OnInit {

  displayedColumns: string[] =
    [
      'nro', 'codigoItem', 'nombreItem', 'anioPecosa', 'numeroPecosa', 'cantidad', 'serieFormato', 'serieDel', 'serieAl', 'actions'
    ];

  form: FormGroup;
  guiaSalidaBienDetalles: GuiaSalidaBienDetalle[] = [];
  dataSource = new MatTableDataSource(this.guiaSalidaBienDetalles);
  catalogoBienes: CatalogoBien[] = [];
  filteredCatalogoBienes;
  constructor(
    public dialog: MatDialog,
    public dialogRef: MatDialogRef<NewGuiaSalidaBienComponent>,
    @Inject(MAT_DIALOG_DATA) public data: GuiaSalidaBien,
    private fb: FormBuilder,
    private guiaSalidaBienService: GuiaSalidaBienService,
    private catalogoBienService: CatalogoBienesService,
    private messageService: MessageService
  ) {
    this.form = this.fb.group({
      numero: [null],
      fechaRegistro: [new Date()],
      justificacion: [null,
        Validators.compose([
          Validators.required,
          Validators.pattern('^[A-Za-zÀ-ÿñÑ0-9#/\n(){}¿?¡!""°:;.,_ -]+$'),
          Validators.maxLength(250)
        ])
      ],
      catalogoBien: [null],
      catalogoBienId: [null],
      tipoDocumentoId: [data.tipoDocumentoId],
      unidadEjecutoraId: [data.unidadEjecutoraId],
      estado: [data.estado],
      usuarioCreador: [data.usuarioCreador]
    });
  }

  ngOnInit() {
    this.loadMaster();

  }

  // Listar Catalogo de bienes
  loadMaster() {
    this.catalogoBienService.getCatalogoBienes().subscribe((response) => {
      if (response.success) {
        response.data.forEach(element => {
          element.codigoDescripcion = element.codigo + " - " + element.descripcion;
        });

        this.catalogoBienes = response.data;
        this.filteredCatalogoBienes = this.catalogoBienes.slice();
      }
    });
  }

  seletedChangeCatalogo(catalgoBien: CatalogoBien) {
    this.form.patchValue({ catalogoBienId: catalgoBien.catalogoBienId });
  }

  addRowData() {

    this.openDialogIngresoPecosa();
  }

  deleteRowData(data) {
    this.dataSource.data = this.dataSource.data.filter(obj => obj.ingresoPecosaDetalleId !== data.ingresoPecosaDetalleId);
  }

  openDialogIngresoPecosa() {

    const catalogoBien: CatalogoBien = this.form.get("catalogoBien").value;

    if (catalogoBien == null) {
      return;
    }

    let catalogos = this.dataSource.data.filter(x => x.catalogoBienId == catalogoBien.catalogoBienId);
    if (catalogos.length > 0) {
      this.messageService.msgWarning("El Catálogo de Bien ya se encuentra agregado", () => { });
      return;
    }

    const dialogRef = this.dialog.open(ListSaldoIngresoComponent, {
      width: '1000px',
      disableClose: true,
      data: catalogoBien
    });

    dialogRef.afterClosed().subscribe((response: IngresoPecosaDetalle[]) => {
      if (response) {
        response.filter(x => x.cantidadSalida > 0 && x.serieAlSalida > 0 && x.serieAlSalida > 0).forEach(item => {
          if (item.cantidadSalida > 0) {
            let guiaSalidaBienDetalle = new GuiaSalidaBienDetalle();
            guiaSalidaBienDetalle.ingresoPecosaDetalleId = item.ingresoPecosaDetalleId;
            guiaSalidaBienDetalle.ingresoPecosaDetalle = item;
            guiaSalidaBienDetalle.cantidad = item.cantidadSalida;
            guiaSalidaBienDetalle.serieFormato = item.serieFormato;
            guiaSalidaBienDetalle.serieDel = item.serieDelSalida;
            guiaSalidaBienDetalle.serieAl = item.serieAlSalida;
            guiaSalidaBienDetalle.anioPecosa = item.anioPecosa;
            guiaSalidaBienDetalle.numeroPecosa = item.numeroPecosa;
            guiaSalidaBienDetalle.catalogoBienId = item.catalogoBienId;
            const catalogoBien = this.form.get("catalogoBien").value;
            guiaSalidaBienDetalle.catalogoBien = catalogoBien;
            guiaSalidaBienDetalle.estado = "1";
            guiaSalidaBienDetalle.usuarioCreador = this.data.usuarioCreador;

            this.dataSource.data.push(guiaSalidaBienDetalle);
            this.dataSource._updateChangeSubscription();
          }
        });

        this.form.patchValue({
          catalogoBien: null
        });
      }
    });
  }

  onSubmit(form: GuiaSalidaBien) {
    let guiaSalidaBien = form;
    guiaSalidaBien.numero = "";
    guiaSalidaBien.fechaRegistro = new Date();
    guiaSalidaBien.guiaSalidaBienDetalle = this.dataSource.data;

    if (!this.form.valid) {
      this.messageService.msgWarning(MESSAGES.FORM.ERROR_VALIDATION, () => { });
      return;
    }

    if (this.dataSource.data.length == 0) {
      this.messageService.msgWarning('Ingrese el detalle de la guia de salida de bienes.', () => { });
      return;
    }

    this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_REGISTER,
      () => {
        this.messageService.msgLoad("Guardando...");
        this.guiaSalidaBienService.createGuiaSalidaBien(guiaSalidaBien).subscribe(
          (response) => {
            var message = response.messages.join(",");
            this.handleResponse(response.messageType, message, response.success);
          },
          (error) => this.handleError(error)
        );
      }, () => {
      });
  }

  handleResponse(type, message, success) {
    if (success) {
      this.messageService.msgSuccess(message, () => {
        this.onNoClick();
      });
    }
    else {
      if (type == TYPE_MESSAGE.WARNING) {
        this.messageService.msgWarning(message, () => { });
      }
      else if (type == TYPE_MESSAGE.INFO) {
        this.messageService.msgWarning(message, () => { });
      }
      else {
        this.messageService.msgError(message, () => { });
      }
    }
  }

  handleError(error) {
    throw error;
  }

  sessionExpired() {
    this.messageService.msgSessionExpired("", () => { });
  }

  onCloseDialog() {
    this.messageService.msgClose(MESSAGES.FORM.CLOSE_FORM, () => {
      this.onNoClick();
    });
  }

  onNoClick(): void {
    this.dialogRef.close(0);
  }

}
