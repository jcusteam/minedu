import { UnidadEjecutoraService } from './../../../../../core/services/unidad-ejecutora.service';
import { Component, OnInit, Inject } from '@angular/core';
import { Validators, FormBuilder, FormGroup, FormArray } from '@angular/forms';
import { MatTableDataSource } from '@angular/material/table';
import { DatePipe } from '@angular/common';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { IngresoPecosa, IngresoPecosaDetalle } from 'src/app/core/models/ingresopecosa';
import { PedidoPecosa } from 'src/app/core/models/pedidopecosa';
import { PedidoPecosaService } from 'src/app/core/services/pedido-pecosa.service';
import { MAT_DATE_LOCALE, DateAdapter, MAT_DATE_FORMATS } from '@angular/material/core';
import { MomentDateAdapter, MAT_MOMENT_DATE_ADAPTER_OPTIONS } from '@angular/material-moment-adapter';
import { IngresoPecosaService } from 'src/app/core/services/ingreso-pecosa.service';
import { MessageService } from 'src/app/core/services/message.service';
import { MESSAGES, TYPE_MESSAGE } from 'src/app/core/utils/messages';
import { PeriodoPecosaEnum } from 'src/app/core/enums/recaudacion.enum';

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
  selector: 'app-new-ingreso-pecosa',
  templateUrl: './new-ingreso-pecosa.component.html',
  styleUrls: ['./new-ingreso-pecosa.component.scss'],
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
export class NewIngresoPecosaComponent implements OnInit {

  // Enums
  periodoPecosaEnum = PeriodoPecosaEnum;

  periodoMin = this.periodoPecosaEnum.ANIO_INICIO;
  periodoMax = new Date().getFullYear();

  form: FormGroup;

  ingresoPecosaDetalles: IngresoPecosaDetalle[] = [];
  displayedColumns: string[] = [
    'nro', 'codigoItem', 'nombreItem', 'nombreMarca', 'cantidad',
    'precioUnitario', 'valorTotal', 'serieFormato', 'serieDel', 'serieAl'
  ];

  dataSource = new MatTableDataSource(this.ingresoPecosaDetalles);

  // Constructor
  constructor(
    public dialogRef: MatDialogRef<NewIngresoPecosaComponent>,
    @Inject(MAT_DIALOG_DATA) public data: IngresoPecosa,
    private fb: FormBuilder,
    private ingresoPecosaService: IngresoPecosaService,
    private pedidoPecosaService: PedidoPecosaService,
    private unidadEjecutoraService: UnidadEjecutoraService,
    private messageService: MessageService
  ) {
    // form
    this.form = this.fb.group({
      numeroPecosa: [null,
        Validators.compose([
          Validators.required,
          Validators.pattern('^[0-9]+$'),
          Validators.maxLength(12)
        ])
      ],
      anioPecosa: [null,
        Validators.compose([
          Validators.required,
          Validators.pattern('^[0-9]+$'),
          Validators.min(this.periodoMin),
          Validators.max(this.periodoMax),
          Validators.maxLength(4)
        ])
      ],
      ejecutora: [null],
      fechaPecosa: [null, [Validators.required]],
      tipoBien: [""],
      nombreAlmacen: [""],
      motivoPedido: [""],
      tipoDocumentoId: [data.tipoDocumentoId],
      unidadEjecutoraId: [data.unidadEjecutoraId],
      estado: [data.estado],
      usuarioCreador: [data.usuarioCreador],
      detalles: this.fb.array([])
    });
  }

  ngOnInit() {
    // Unidad Ejecutora
    this.unidadEjecutoraService.getUnidadEjecutoraById(this.data.unidadEjecutoraId).subscribe(
      response => {
        if (response.success) {
          this.form.patchValue({ ejecutora: response.data?.codigo });
        }
      }
    );
    this.setDetallesForm();
  }

  private setDetallesForm() {
    const detalleCtrl = this.form.get('detalles') as FormArray;
    this.dataSource.data.forEach((detalle) => {
      detalleCtrl.push(this.setDetalleFormArray(detalle))
    })
  };

  private setDetalleFormArray(detalle) {


    return this.fb.group({
      codigoItem: [detalle?.codigoItem],
      nombreItem: [detalle?.nombreItem],
      nombreMarca: [detalle?.nombreMarca],
      cantidad: [detalle?.cantidad],
      precioUnitario: [detalle?.precioUnitario],
      valorTotal: [detalle?.valorTotal],
      serieFormato: [
        detalle?.serieFormato,
        Validators.compose([
          Validators.required,
          Validators.pattern("^[A-Za-z-]+$"),
          Validators.maxLength(4)
        ])
      ],
      serieDel: [
        detalle?.serieDel,
        Validators.compose([
          Validators.required,
          Validators.pattern("^[0-9]+$"),
          Validators.maxLength(10),
          Validators.min(1)
        ])
      ],
      serieAl: [
        detalle?.serieAl,
        Validators.compose([
          Validators.required,
          Validators.pattern("^[0-9]+$"),
          Validators.maxLength(10),
          Validators.min(1)
        ])]
    });
  }

  // Importar pedido pecosa
  inportarPecosa() {

    if (!this.form.get("anioPecosa").valid ||
      !this.form.get("numeroPecosa").valid ||
      !this.form.get("ejecutora").valid) {
      return;
    }

    this.dataSource.data = [];
    this.dataSource._updateChangeSubscription();

    const detalleCtrl = this.form.get('detalles') as FormArray;
    detalleCtrl.clear();

    const anioPecosa = this.form.get("anioPecosa").value;
    const numeroPecosa = this.form.get("numeroPecosa").value;
    const ejecutora = this.form.get("ejecutora").value;

    this.messageService.msgConfirm(MESSAGES.FORM.INGRESO_PECOSA.CONFIRM_IMPORT,
      () => {
        this.messageService.msgLoad("Importando...");
        this.pedidoPecosaService.getPedidoPecosas(ejecutora, anioPecosa, numeroPecosa).subscribe(
          (response) => {
            this.messageService.msgAutoClose();
            var message = response.messages.join(",");
            this.handleResponsePecosa(response.messageType, message, response.success, response.data);
          },
          (error) => {
            this.handleError(error);
          }
        );
      }, () => {
      });
  }

  // handle response pecosa
  handleResponsePecosa(type, message, success, data: PedidoPecosa[]) {
    if (success) {
      let pedidoPecosa = data[0];
      this.form.patchValue(
        {
          fechaPecosa: pedidoPecosa?.fechaPecosa,
          tipoBien: pedidoPecosa?.tipoBien,
          nombreAlmacen: pedidoPecosa?.nombreAlmacen,
          motivoPedido: pedidoPecosa?.motivoPedido,
        }
      );
      data.forEach((item, index) => {
        let ingresoPecosaDetalle = new IngresoPecosaDetalle();
        ingresoPecosaDetalle.ingresoPecosaDetalleId = index + 1;
        ingresoPecosaDetalle.unidadMedida = item.nombreUnidad;
        ingresoPecosaDetalle.codigoItem = item.codigoItem;
        ingresoPecosaDetalle.nombreItem = item.nombreItem;
        ingresoPecosaDetalle.nombreMarca = item.nombreMarca;
        ingresoPecosaDetalle.cantidad = +item.cantidadAprobada;
        ingresoPecosaDetalle.cantidadSalida = 0;
        ingresoPecosaDetalle.precioUnitario = item.precioUnitario;
        ingresoPecosaDetalle.valorTotal = item.valorTotal;
        ingresoPecosaDetalle.serieFormato = "";
        ingresoPecosaDetalle.estado = "1";
        ingresoPecosaDetalle.usuarioCreador = this.data.usuarioCreador;
        this.dataSource.data.push(ingresoPecosaDetalle);
      });
      this.dataSource._updateChangeSubscription();

      this.setDetallesForm();
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

  // serie Formato
  applySerieFormato(event: Event, item) {
    var serie = (event.target as HTMLInputElement).value;
    var letters = /^[A-Za-z-]+$/;

    if (!serie.match(letters)) {
      (event.target as HTMLInputElement).value = '';
      serie = '';
    }

    this.dataSource.data.forEach(row => {
      if (row.ingresoPecosaDetalleId == item.ingresoPecosaDetalleId) {
        row.serieFormato = serie;
      }
    });
  }

  // serie Del
  applySerieDel(event: Event, item: IngresoPecosaDetalle) {
    var serieDel = (event.target as HTMLInputElement).value;
    var letters = /^[0-9]+$/;

    if (!serieDel.match(letters)) {
      (event.target as HTMLInputElement).value = '';
      serieDel = '0';
    }

    this.dataSource.data.forEach(row => {
      if (row.ingresoPecosaDetalleId == item.ingresoPecosaDetalleId) {
        row.serieDel = +serieDel;
      }
    });
    this.dataSource._updateChangeSubscription();
  }

  // serie Al
  applySerieAl(event: Event, item) {
    var serieAl = (event.target as HTMLInputElement).value;

    var letters = /^[0-9]+$/;

    if (!serieAl.match(letters)) {
      (event.target as HTMLInputElement).value = '';
      serieAl = '0';
    }

    this.dataSource.data.forEach(row => {
      if (row.ingresoPecosaDetalleId == item.ingresoPecosaDetalleId) {
        row.serieAl = +serieAl;
      }
    });
    this.dataSource._updateChangeSubscription();
  }


  onSubmit(form: IngresoPecosa) {

    if (this.dataSource.data.length == 0) {
      this.messageService.msgWarning('Ingrese el detalle del ingreso de pecosa.', () => { });
      return;
    }

    if (!this.form.valid) {
      this.messageService.msgWarning(MESSAGES.FORM.ERROR_VALIDATION, () => { });
      return;
    }

    let ingresoPecosa = form;
    ingresoPecosa.numeroPecosa = +form.numeroPecosa;
    ingresoPecosa.anioPecosa = +form.anioPecosa;
    ingresoPecosa.fechaRegistro = new Date();
    ingresoPecosa.ingresoPecosaDetalle = this.dataSource.data;

    this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_REGISTER,
      () => {
        this.messageService.msgLoad("Guardando...");
        this.ingresoPecosaService.createIngresoPecosa(ingresoPecosa).subscribe(
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
    this.messageService.msgError(MESSAGES.API.ERR_SERVICE, () => { });
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
