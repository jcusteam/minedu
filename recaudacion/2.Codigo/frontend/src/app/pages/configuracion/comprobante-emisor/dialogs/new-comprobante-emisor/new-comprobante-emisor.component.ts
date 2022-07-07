import { Sunat } from './../../../../../core/models/pide';
import { UnidadEjecutoraService } from './../../../../../core/services/unidad-ejecutora.service';
import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ComprobanteEmisor } from 'src/app/core/models/comprobante-emisor';
import { AuthService } from 'src/app/core/services/auth.service';
import { ComprobanteEmisorService } from 'src/app/core/services/comprobante-emisor.service';
import { MessageService } from 'src/app/core/services/message.service';
import { PideService } from 'src/app/core/services/pide.service';
import { MESSAGES, TYPE_MESSAGE } from 'src/app/core/utils/messages';
import { CodigoPaisEnum, TipoDocIdentidadSunatEnum } from 'src/app/core/enums/recaudacion.enum';

@Component({
  selector: 'app-new-comprobante-emisor',
  templateUrl: './new-comprobante-emisor.component.html',
  styleUrls: ['./new-comprobante-emisor.component.scss']
})
export class NewComprobanteEmisorComponent implements OnInit {

  // Enums
  tipoDocIdentidadSunatEnum = TipoDocIdentidadSunatEnum;
  codigoPaisEnum = CodigoPaisEnum;
  form: FormGroup;

  constructor(
    public dialogRef: MatDialogRef<NewComprobanteEmisorComponent>,
    @Inject(MAT_DIALOG_DATA) public data: ComprobanteEmisor,
    public fb: FormBuilder,
    private comprobanteEmisorService: ComprobanteEmisorService,
    private unidadEjecutoraService: UnidadEjecutoraService,
    private pideService: PideService,
    private messageService: MessageService,
    private authService: AuthService
  ) {
    this.form = this.fb.group({
      comprobanteEmisorId: [0],
      unidadEjecutoraId: [data?.unidadEjecutoraId],
      firmante: [null,
        Validators.compose([
          Validators.required,
          Validators.pattern('^[A-Za-zÀ-ÿñÑ0-9#°().,_ -]+$'),
          Validators.maxLength(50)
        ])
      ],
      numeroRuc: [null,
        Validators.compose([
          Validators.required,
          Validators.pattern("^[0-9]{11,11}$")
        ])
      ],
      tipoDocumento: [null,
        Validators.compose([
          Validators.required,
          Validators.pattern('^[0-9]+$'),
          Validators.maxLength(2)
        ])
      ],
      nombreComercial: [null,
        Validators.compose([
          Validators.required,
          Validators.pattern('^[A-Za-zÀ-ÿñÑ0-9#/\n(){}¿?¡!""°:;.,_ -]+$'),
          Validators.maxLength(200)
        ])
      ],
      razonSocial: [null,
        Validators.compose([
          Validators.required,
          Validators.pattern('^[A-Za-zÀ-ÿñÑ0-9#/\n(){}¿?¡!""°:;.,_ -]+$'),
          Validators.maxLength(200)
        ])
      ],
      ubigeo: [null,
        Validators.compose([
          Validators.required,
          Validators.pattern('^[0-9]+$'),
          Validators.maxLength(20)
        ])
      ],
      direccion: [null,
        Validators.compose([
          Validators.required,
          Validators.maxLength(300)
        ])
      ],
      urbanizacion: [null,
        Validators.compose([
          Validators.required,
          Validators.maxLength(300)
        ])
      ],
      departamento: [null,
        Validators.compose([
          Validators.required,
          Validators.pattern('^[A-Za-zÀ-ÿñÑ0-9#°().,_ -]+$'),
          Validators.maxLength(50)
        ])
      ],
      provincia: [null,
        Validators.compose([
          Validators.required,
          Validators.pattern('^[A-Za-zÀ-ÿñÑ0-9#°().,_ -]+$'),
          Validators.maxLength(50)
        ])
      ],
      distrito: [null,
        Validators.compose([
          Validators.required,
          Validators.pattern('^[A-Za-zÀ-ÿñÑ0-9#°().,_ -]+$'),
          Validators.maxLength(50)
        ])
      ],
      codigoPais: [null,
        Validators.compose([
          Validators.required,
          Validators.pattern('^[A-Za-z]+$'),
          Validators.maxLength(2)
        ])
      ],
      telefono: [null,
        Validators.compose([
          Validators.required,
          Validators.pattern('^[A-Za-z0-9-]+$'),
          Validators.maxLength(15)
        ])
      ],
      direccionAlternativa: [null,
        Validators.compose([
          Validators.required,
          Validators.maxLength(300)
        ])
      ],
      numeroResolucion: [null,
        Validators.compose([
          Validators.required,
          Validators.pattern('^[A-Za-zÀ-ÿñÑ0-9#/\n(){}¿?¡!""°:;.,_ -]+$'),
          Validators.maxLength(20)
        ])
      ],
      usuarioOSE: [null,
        Validators.compose([
          Validators.required,
          Validators.pattern('^[A-Za-zÀ-ÿñÑ0-9#/\n._@*-]+$'),
          Validators.maxLength(100)
        ])
      ],
      claveOSE: [null,
        Validators.compose([
          Validators.required,
          Validators.pattern('^[A-Za-zÀ-ÿñÑ0-9#/\n()._*-]+$'),
          Validators.maxLength(100)
        ])
      ],
      correoEnvio: [null,
        Validators.compose([
          Validators.required,
          Validators.email,
          Validators.maxLength(100)
        ])
      ],
      correoClave: [null,
        Validators.compose([
          Validators.required,
          Validators.pattern('^[A-Za-zÀ-ÿñÑ0-9#/\n()._-]+$'),
          Validators.maxLength(100)
        ])
      ],
      serverMail: [null,
        Validators.compose([
          Validators.required,
          Validators.pattern('^[A-Za-z0-9.]+$'),
          Validators.maxLength(50)
        ])
      ],
      serverPort: [null,
        Validators.compose([
          Validators.required,
          Validators.pattern('^[0-9]+$'),
          Validators.maxLength(10)
        ])
      ],
      nombreArchivoCer: [null,
        Validators.compose([
          Validators.required,
          Validators.pattern('^[A-Za-zÀ-ÿñÑ0-9#._-]+$'),
          Validators.maxLength(100)
        ])
      ],
      nombreArchivoKey: [null,
        Validators.compose([
          Validators.required,
          Validators.pattern('^[A-Za-zÀ-ÿñÑ0-9#._-]+$'),
          Validators.maxLength(100)
        ])
      ],
      estado: [data?.estado],
      usuarioCreador: [data?.usuarioCreador]
    });
  }
  ngOnInit() {
    // Unidad Ejecutora
    this.loadUnidadEjecutora();
  }

  loadUnidadEjecutora() {
    this.unidadEjecutoraService.getUnidadEjecutoraById(this.data.unidadEjecutoraId).subscribe(
      response => {
        if (response.success) {
          var unidaEjecutora = response.data;
          this.getSunatData(unidaEjecutora.numeroRuc);
        }
      }
    )
  }

  // Limpiar formulario
  clearForm() {
    this.form.reset();
  }

  // Key Up Ruc
  onKeyUpRuc(nro) {
    this.clearForm();
  }

  // Buscar RUC
  searchRuc() {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    this.clearForm();

    if (!this.form.get("numeroRuc").valid) {
      return;
    }

    let numeroRuc = this.form.get("numeroRuc").value;
    this.getSunatData(numeroRuc);
  }

  // Consulta SUNAT
  getSunatData(numeroRuc) {
    this.messageService.msgLoad("Consultado SUNAT...");
    this.pideService.getSunatByRuc(numeroRuc).subscribe(
      (response) => {
        this.messageService.msgAutoClose();
        if (response.success) {
          this.addForm(response.data);
        }
        else {
          var message = response.messages.join(",");
          let type = response.messageType;
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
      },
      (error) => {
        this.handleError(error);
      }
    );
  }

  addForm(data: Sunat) {
    this.form.patchValue({
      firmante: "",
      numeroRuc: data.ddp_numruc?.trim(),
      tipoDocumento: this.tipoDocIdentidadSunatEnum.RUC,
      nombreComercial: data.ddp_nombre?.trim(),
      razonSocial: data.ddp_nombre?.trim(),
      ubigeo: data.ddp_ubigeo?.trim(),
      direccion: data.desc_domi_fiscal?.trim(),
      urbanizacion: data.ddp_nomzon?.trim(),
      departamento: data.desc_dep?.trim(),
      provincia: data.desc_prov?.trim(),
      distrito: data.desc_dist?.trim(),
      codigoPais: this.codigoPaisEnum.PE,
      telefono: "",
      direccionAlternativa: "",
      numeroResolucion: "",
    });
  }

  onSubmit(form: ComprobanteEmisor) {

    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    if (!this.form.valid) {
      this.messageService.msgWarning(MESSAGES.FORM.ERROR_VALIDATION, () => { });
      return;
    }

    let comprobanteEmisisor = form;
    this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_REGISTER,
      () => {
        this.messageService.msgLoad("Guardando...");
        this.comprobanteEmisorService.createComprobanteEmisor(comprobanteEmisisor).subscribe(
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
