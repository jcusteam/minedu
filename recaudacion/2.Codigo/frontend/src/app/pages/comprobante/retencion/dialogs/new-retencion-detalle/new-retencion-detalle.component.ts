import { Component, OnInit, Inject } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { DatePipe } from '@angular/common';
import { MomentDateAdapter, MAT_MOMENT_DATE_ADAPTER_OPTIONS } from "@angular/material-moment-adapter";
import { MAT_DATE_LOCALE, DateAdapter, MAT_DATE_FORMATS } from "@angular/material/core";
import { ComprobanteRetencionDetalle } from 'src/app/core/models/comprobanteretencion';
import { TransversalService } from 'src/app/core/services/transversal.service';
import { TipoComprobantePagoService } from 'src/app/core/services/tipo-comprobante-pago.service';
import { Combobox } from 'src/app/core/interfaces/combobox';
import { TipoComprobanteEnum, TipoMonedaEnum } from 'src/app/core/enums/recaudacion.enum';

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
  selector: 'app-new-retencion-detalle',
  templateUrl: './new-retencion-detalle.component.html',
  styleUrls: ['./new-retencion-detalle.component.scss'],
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
export class NewRetencionDetalleComponent implements OnInit {

  form: FormGroup;
  tipoDocs: Combobox[] = [];
  tipoMonedas: Combobox[] = [];
  maxDate = new Date();
  minDate = new Date();

  // Enums
  tipoComprobanteEnum = TipoComprobanteEnum;
  tipoMonedaEnum = TipoMonedaEnum;

  constructor(
    private dialogRef: MatDialogRef<NewRetencionDetalleComponent>,
    @Inject(MAT_DIALOG_DATA) public data: ComprobanteRetencionDetalle,
    private transversalService: TransversalService,
    private tipoComprobantePagoService: TipoComprobantePagoService,
    private fb: FormBuilder,
  ) {
    this.form = this.fb.group(
      {
        comprobanteRetencionDetalleId: [0],
        comprobanteRetencionId: [0],
        comprobantePagoId: [null],
        tipoDocumento: [null,
          Validators.compose([Validators.required])],
        serie: [null,
          Validators.compose([Validators.required])],
        correlativo: [null,
          Validators.compose([Validators.required])],
        fechaEmision: [null,
          Validators.compose([Validators.required])],
        importeTotal: [0,
          Validators.compose([Validators.required, Validators.pattern(/^[.\d]+$/), Validators.maxLength(12), Validators.min(0)])],
        ModificaNotaCredito: [false],
        tipoModena: [this.tipoMonedaEnum.NUEVO_SOL,
        Validators.compose([Validators.required])],
        importeOperacion: [0,
          Validators.compose([Validators.required, Validators.pattern(/^[.\d]+$/), Validators.maxLength(12), Validators.min(0)])],
        fechaPago: [
          new Date(),
          Validators.compose([Validators.required])],
        tipoCambio: [1],
        numeroCorrelativoPago: [null,
          Validators.compose([Validators.required])],
        importePago: [0,
          Validators.compose([Validators.required, Validators.pattern(/^[.\d]+$/), Validators.maxLength(12), Validators.min(0)])],
        tasa: [data?.tasa],
        regimenRetencion: [null],
        importeRetenido: [0,
          Validators.compose([Validators.required, Validators.pattern(/^[.\d]+$/), Validators.maxLength(12), Validators.min(0)])],
        fechaRetencion: [
          new Date(),
          Validators.compose([Validators.required])],
        importeNetoPagado: [0,
          Validators.compose([Validators.required, Validators.pattern(/^[.\d]+$/), Validators.maxLength(12), Validators.min(0)])],
        estado: [data?.estado],
        usuarioCreador: [data?.usuarioCreador],
        // Extra
        regimenRetencionDesc: [
          data?.regimenRetencionDesc,
          Validators.compose([Validators.required])],
        tipoDocumentoNombre: [null]
      });
  }

  ngOnInit() {
    this.tipoComprobantePagoService.getTipoComprobantePagos().subscribe(
      response => {
        if (response.success) {
          let tipoDoc = response.data.filter(x => x.tipoComprobantePagoId == this.tipoComprobanteEnum.FACTURA)[0];
          if (tipoDoc != null) {
            this.tipoDocs.push({ label: tipoDoc?.nombre, value: tipoDoc?.codigo });
            this.form.patchValue({ tipoDocumento: tipoDoc?.codigo, tipoDocumentoNombre: tipoDoc?.nombre });
          }

        }
      }
    );

    // tipo operaciones
    this.transversalService.getTipoMonedas().subscribe((response) => {
      if (response.success) {
        this.tipoMonedas = response.data;
      }
    });
  }

  applyTotalComprobante(event: Event) {
    const totalComprobante = +(event.target as HTMLInputElement).value;

    if (isNaN(totalComprobante)) {
      this.form.patchValue({
        importeOperacion: 0,
        importePago: 0,
        importeRetenido: 0,
        importeNetoPagado: 0
      });
      return;

    }

    const importeRetenido = (totalComprobante * this.data.tasa) / 100.00;
    const importeNetoPagado = totalComprobante - importeRetenido;
    this.form.patchValue({
      importeOperacion: this.formatData(totalComprobante),
      importePago: this.formatData(totalComprobante),
      importeRetenido: this.formatData(importeRetenido),
      importeNetoPagado: this.formatData(importeNetoPagado)
    })
  }

  formatData(data) {
    if (data == null) {
      return 0;
    }
    return (Math.round(data * 100) / 100).toFixed(2);
  }

  onSubmit(form: ComprobanteRetencionDetalle) {
    if (this.form.valid) {
      this.data = form;
      this.data.importeTotal = +form.importeTotal;
      this.data.importeOperacion = +form.importeOperacion;
      this.data.tipoCambio = +form.tipoCambio;
      this.data.importePago = +form.importePago;
      this.data.tasa = +form.tasa;
      this.data.importeRetenido = +form.importeRetenido;
      this.data.importeNetoPagado = +form.importeNetoPagado;
      this.closeDialog();
    }

  }

  onNoClick(): void {
    this.dialogRef.close(null);
  }

  closeDialog(): void {
    this.dialogRef.close(this.data);
  }

}
