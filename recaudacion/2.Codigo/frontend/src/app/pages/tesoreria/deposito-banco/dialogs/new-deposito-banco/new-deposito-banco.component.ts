import { Component, OnInit, Inject, DEFAULT_CURRENCY_CODE, ViewChild, ElementRef } from "@angular/core";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { FormBuilder, Validators, FormGroup } from "@angular/forms";
import { DatePipe } from "@angular/common";
import { MatTableDataSource } from "@angular/material/table";
import { MomentDateAdapter, MAT_MOMENT_DATE_ADAPTER_OPTIONS } from "@angular/material-moment-adapter";
import { MAT_DATE_LOCALE, DateAdapter, MAT_DATE_FORMATS } from "@angular/material/core";

import { CuentaCorriente } from "src/app/core/models/cuentacorriente";
import { DepositoBanco, DepositoBancoDetalle } from "src/app/core/models/depositobanco";
import { DepositoBancoService } from "src/app/core/services/deposito-banco.service";
import { CuentaCorrienteService } from "src/app/core/services/cuenta-corriente.service";

import { AuthService } from "src/app/core/services/auth.service";
import { MessageService } from "src/app/core/services/message.service";
import { MESSAGES, TYPE_MESSAGE } from "src/app/core/utils/messages";

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
  selector: "app-new-deposito-banco",
  templateUrl: "./new-deposito-banco.component.html",
  styleUrls: ["./new-deposito-banco.component.scss"],
  providers: [
    { provide: DatePipe },
    { provide: MAT_DATE_LOCALE, useValue: "es-ES" },
    {
      provide: DateAdapter,
      useClass: MomentDateAdapter,
      deps: [MAT_DATE_LOCALE, MAT_MOMENT_DATE_ADAPTER_OPTIONS],
    },
    { provide: MAT_DATE_FORMATS, useValue: DATE_MODE_FORMATS },
    { provide: DEFAULT_CURRENCY_CODE, useValue: "PEN" },
  ],
})
export class NewDepositoBancoComponent implements OnInit {
  @ViewChild('inputFile') inputFile: ElementRef;
  
  form: FormGroup;
  depositoBancoDetalles: DepositoBancoDetalle[] = [];
  cuentasCorrientes: CuentaCorriente[] = [];
  filteredcuentasCorrientes;

  displayedColumns: string[] = [
    "index",
    "numeroDeposito",
    "tipoDocumentoIdentidad",
    "numeroDocCliente",
    "nombreCliente",
    "importe",
    "fechaDeposito",
    "tipoDocumento",
    "serieDocumento",
    "utilizado",
  ];

  maxDate = new Date();
  minDate = new Date();

  dataSource = new MatTableDataSource(this.depositoBancoDetalles);
  constructor(
    public dialogRef: MatDialogRef<NewDepositoBancoComponent>,
    @Inject(MAT_DIALOG_DATA) public data: DepositoBanco,
    private depositoBancoService: DepositoBancoService,
    private cuentaCorrienteService: CuentaCorrienteService,
    public fb: FormBuilder,
    private messageService: MessageService,
    private authService: AuthService
  ) {
    this.form = this.fb.group({
      depositoBancoId: [0],
      unidadEjecutoraId: [data.unidadEjecutoraId],
      bancoId: [0],
      cuentaCorrienteId: [0],
      tipoDocumentoId: [data.tipoDocumentoId],
      numero: ["000"],
      importe: [0],
      fechaDeposito: [null,
        Validators.compose([Validators.required]),
      ],
      fechaRegistro: [new Date()],
      nombreArchivo: [""],
      cantidad: [0],
      estado: [data.estado],
      usuarioCreador: [data.usuarioCreador],

      //Extra
      cuentaCorriente: [null,
        Validators.compose([Validators.required]),
      ],
      fileDeposito:[null]
    });
  }

  ngOnInit() {
    this.cuentaCorrienteService.getCuentaCorrienteByEjecutora(this.data.unidadEjecutoraId)
      .subscribe((response) => {
        if (response.success) {
          response.data.forEach((element) => {
            element.numeroDenominacion = element?.numero + " - " + element?.denominacion;
          });
          this.cuentasCorrientes = response.data;
          this.filteredcuentasCorrientes = this.cuentasCorrientes.slice();
        }
      });
  }


  // Cuenta Corriente
  seletedChangeCuentaCorriente(cuentaCorriente: CuentaCorriente) {
    this.form.patchValue({ cuentaCorrienteId: cuentaCorriente.cuentaCorrienteId, bancoId: cuentaCorriente.bancoId })
  }

  onChange(ev): void {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }
    this.messageService.msgLoad("Cargando Archivo...");
    const file = ev.target.files[0];
    this.dataSource.data = [];
    this.dataSource._updateChangeSubscription();

    if (file == null || file === undefined) {
      this.messageService.msgWarning("Seleccione correctamente el archivo", () => {
      });
      return;
    } else {
      if (file.type == "text/plain") {
        let fileName = ev.target.files[0].name;
        let usuario = this.data.usuarioCreador;
        this.depositoBancoService.getDepositoBancoFiles(file, usuario).subscribe(
          response => {
            var message = response.messages.join(" , ");
            if (response.success) {
              this.onLoadDepositoFileCliente(response.data);
              this.form.patchValue({
                nombreArchivo: fileName,
                importe: response.data.importeTotal,
                cantidad: response.data.cantidad
              });
            }
            else {
              if (response.messageType == TYPE_MESSAGE.INFO) {
                this.messageService.msgInfo(message, () => { });
              }
              else if (response.messageType == TYPE_MESSAGE.WARNING) {
                this.messageService.msgWarning(message, () => { });
              }
              else {
                this.messageService.msgError(message, () => { });
              }
            }
          },
          (error) => this.handleError(error)
        );
      } else {
        this.messageService.msgWarning("La extensión del archivo no corresponde.", () => { });
      }
    }
  }

  onLoadDepositoFileCliente(data) {
    this.depositoBancoService.fileDepositoBanco(data).subscribe(
      response => {
        this.messageService.msgAutoClose();
        var message = response.messages.join(" , ");
        if (response.success) {
          this.dataSource.data = response.data;
          this.dataSource._updateChangeSubscription();
        }
        else {
          if (response.messageType == TYPE_MESSAGE.INFO) {
            this.messageService.msgInfo(message, () => {
            });
          }
          else if (response.messageType == TYPE_MESSAGE.WARNING) {
            this.messageService.msgWarning(message, () => {
            });
          }
          else {
            this.messageService.msgError(message, () => {
            });
          }
        }
      }
    );
  }

  onClean(){
    this.form.reset();
    this.dataSource.data =[];
    this.dataSource._updateChangeSubscription();
    this.inputFile.nativeElement.value = '';
  }

  onSubmit(form: DepositoBanco) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    if (!this.form.valid) {
      this.messageService.msgWarning(MESSAGES.FORM.ERROR_VALIDATION, () => { });
    }

    if (this.dataSource.data.length == 0) {
      this.messageService.msgWarning("Ingrese el detalle del depóstio en cuenta corriente", () => {
      });
      return;
    }

    let depositoBanco = form;
    depositoBanco.cantidad = +form.cantidad;
    depositoBanco.importe = +form.importe;
    depositoBanco.depositoBancoDetalle = this.dataSource.data;
    this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_REGISTER,
      () => {
        this.messageService.msgLoad("Guardando...");
        this.depositoBancoService.createDepositoBanco(depositoBanco).subscribe(
          (response) => {
            var message = response.messages.join(",");
            this.handleResponse(response.messageType, message, response.success);
          },
          (error) => this.handleError(error)
        );
      }, () => {

      });

  }

  //Response
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
