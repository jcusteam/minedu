import { TransversalService } from 'src/app/core/services/transversal.service';
import { Combobox } from 'src/app/core/interfaces/combobox';
import { Component, OnInit } from "@angular/core";

import * as Highcharts from "highcharts";
import { Options } from "highcharts";
import { AppSettings } from "src/app/app.settings";
import { Settings } from "src/app/app.settings.model";
import { AuthService } from "src/app/core/services/auth.service";
import { MessageService } from "src/app/core/services/message.service";
import { ReciboIngresoService } from "src/app/core/services/recibo-ingreso.service";


class ChartTipoRecibo {
  tipoId: number;
  tipoName: string;
  charts: Chart[];
}

class Chart {
  id: number;
  tipoId: number;
  monthName: string;
  total: number;
}

@Component({
  selector: "app-estadistica-tipo-recibo-ingreso",
  templateUrl: "./estadistica-tipo-recibo-ingreso.component.html",
  styleUrls: ["./estadistica-tipo-recibo-ingreso.component.scss"],
})
export class EstadisticaTipoReciboIngresoComponent implements OnInit {

  decodeId = localStorage.getItem("unidadEjecutora");
  unidadEjecutoraId = 0;
  anio = 0;
  widthChart = 1200;
  sidenaWidth = 300;
  Highcharts1: typeof Highcharts = Highcharts;
  updateFlag = false;
  data = [];
  listAnios: Combobox[] = [];

  settings: Settings;
  chartTipoRecibo: ChartTipoRecibo[] = [];
  chartOptions1: Options = {
    title: {
      text: "Ingresos Mensuales por Tipo de Recibos de Ingresos",
    },
    xAxis: {
      type: "category",
    },
    yAxis: {
      title: {
        text: "Total",
      },
    },
    series: this.data,
  };

  constructor(
    private reciboIngresoService: ReciboIngresoService,
    private messageService: MessageService,
    private transversalService: TransversalService,
    private authService: AuthService,
    public appSettings: AppSettings
  ) {
    this.settings = this.appSettings.settings;
  }

  ngOnInit() {
    this.widthChart = window.innerWidth - this.sidenaWidth;
    
    this.anio = new Date().getFullYear();

    this.transversalService.getAnios().subscribe(
      response => {
        if (response.success) {
          this.listAnios = response.data;
        }
      }
    );

    this.onLoadCharts();
    this.onLoad();
  }

  sessionExpired() {
    this.messageService.msgSessionExpired("", () => { });
  }

  onLoad() {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    this.unidadEjecutoraId = +this.settings.unidadEjecutora;


    this.reciboIngresoService.getReciboIngresosChartsTipoRecibo(this.unidadEjecutoraId, this.anio)
      .subscribe((response) => {
        if (response.success) {
          this.chartTipoRecibo = response.data;
          this.addSeries();
        }
      });
  }

  onLoadCharts() {
    this.chartOptions1 = {
      title: {
        text: "Ingresos Mensuales por Tipo de Recibos de Ingresos"
      },
      subtitle: {
        text: "(" + this.anio + ")"
      },
      chart: {
        type: "column",
        width: this.widthChart,
      },
      xAxis: {
        categories: [
          "Ene.",
          "Febr.",
          "Mar.",
          "Abri.",
          "May.",
          "Juni.",
          "Juli.",
          "Agost.",
          "Septi.",
          "Octu.",
          "Novi.",
          "Dici.",
        ],
        crosshair: true,
      },

      yAxis: {
        min: 0,
        title: {
          text: "Total",
        },
      },
      tooltip: {
        headerFormat: '<span style="font-size:10px">{point.key}</span><table>',
        pointFormat:
          '<tr><td style="color:{series.color};padding:0">{series.name}: </td>' +
          '<td style="padding:0"><b>{point.y:.1f}</b></td></tr>',
        footerFormat: "</table>",
        shared: true,
        useHTML: true,
      },

      series: [
        {
          name: "Captación Recursos por Ventanilla",
          type: "column",
          data: [],
        },
        {
          name: "Habilitación Urbana",
          type: "column",
          data: [],
        },
        {
          name: "Penalidades",
          type: "column",
          data: [],
        },
        {
          name: "Retenciones de garantía",
          type: "column",
          data: [],
        },
        {
          name: "Sanciones administrativas",
          type: "column",
          data: [],
        },
        {
          name: "Ejecución de Carta Finanza",
          type: "column",
          data: [],
        },
        {
          name: "Devoluciones de saldo de viáticos y encargos otorgados",
          type: "column",
          data: [],
        },
        {
          name: "Depósitos indebidos",
          type: "column",
          data: [],
        },
        {
          name: "Costas procesales",
          type: "column",
          data: [],
        },
        {
          name: "Sentencias Judiciales",
          type: "column",
          data: [],
        },
      ],
      responsive: {
        rules: [
          {
            condition: {
              maxWidth: 500,
            },
            chartOptions: {
              legend: {
                align: 'center',
                verticalAlign: 'bottom',
                layout: 'horizontal'
              },
              yAxis: {
                labels: {
                  align: 'left',
                  x: 0,
                  y: -5
                },
                title: {
                  text: null
                }
              },
              subtitle: {
                text: null
              },
              credits: {
                enabled: false
              }
            }
          },
        ],
      },
    };
  }

  selectedAnio(anio: number) {
    this.anio = anio;
    this.onLoad();
  }

  addSeries() {
    setTimeout(() => {
      var data1 = [];
      var data2 = [];
      var data3 = [];
      var data4 = [];
      var data5 = [];
      var data6 = [];
      var data7 = [];
      var data8 = [];
      var data9 = [];
      var data10 = [];

      this.chartTipoRecibo[0].charts.forEach((item) => {
        data1.push(item.total);
      });

      this.chartTipoRecibo[1].charts.forEach((item) => {
        data2.push(item.total);
      });

      this.chartTipoRecibo[2].charts.forEach((item) => {
        data3.push(item.total);
      });

      this.chartTipoRecibo[3].charts.forEach((item) => {
        data4.push(item.total);
      });

      this.chartTipoRecibo[4].charts.forEach((item) => {
        data5.push(item.total);
      });

      this.chartTipoRecibo[5].charts.forEach((item) => {
        data6.push(item.total);
      });

      this.chartTipoRecibo[6].charts.forEach((item) => {
        data7.push(item.total);
      });

      this.chartTipoRecibo[7].charts.forEach((item) => {
        data8.push(item.total);
      });

      this.chartTipoRecibo[8].charts.forEach((item) => {
        data9.push(item.total);
      });

      this.chartTipoRecibo[9].charts.forEach((item) => {
        data10.push(item.total);
      });

      this.chartOptions1.subtitle.text = "(" + this.anio + ")";
      this.chartOptions1.series[0] = {
        name: this.chartTipoRecibo[0].tipoName,
        type: "column",
        data: data1,
      };

      this.chartOptions1.series[1] = {
        name: this.chartTipoRecibo[1].tipoName,
        type: "column",
        data: data2,
      };

      this.chartOptions1.series[2] = {
        name: this.chartTipoRecibo[2].tipoName,
        type: "column",
        data: data3,
      };

      this.chartOptions1.series[3] = {
        name: this.chartTipoRecibo[3].tipoName,
        type: "column",
        data: data4,
      };

      this.chartOptions1.series[4] = {
        name: this.chartTipoRecibo[4].tipoName,
        type: "column",
        data: data5,
      };

      this.chartOptions1.series[5] = {
        name: this.chartTipoRecibo[5].tipoName,
        type: "column",
        data: data6,
      };

      this.chartOptions1.series[6] = {
        name: this.chartTipoRecibo[6].tipoName,
        type: "column",
        data: data7,
      };

      this.chartOptions1.series[7] = {
        name: this.chartTipoRecibo[7].tipoName,
        type: "column",
        data: data8,
      };

      this.chartOptions1.series[8] = {
        name: this.chartTipoRecibo[8].tipoName,
        type: "column",
        data: data9,
      };

      this.chartOptions1.series[9] = {
        name: this.chartTipoRecibo[9].tipoName,
        type: "column",
        data: data9,
      };

      this.updateFlag = true;
    }, 500);
  }
}
