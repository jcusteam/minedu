import { Component, OnInit } from '@angular/core';


import * as Highcharts from "highcharts";
import { Options } from "highcharts";
import { AppSettings } from 'src/app/app.settings';
import { Settings } from 'src/app/app.settings.model';
import { Combobox } from 'src/app/core/interfaces/combobox';
import { AuthService } from 'src/app/core/services/auth.service';
import { ComprobantePagoService } from 'src/app/core/services/comprobante-pago.service';
import { MessageService } from 'src/app/core/services/message.service';
import { TransversalService } from 'src/app/core/services/transversal.service';


class ChartTipoRecibo {
  tipoId: number;
  tipoName: string;
  charts: Chart[]
}

class Chart {
  id: number;
  tipoId: number;
  monthName: string;
  total: number;
}

@Component({
  selector: 'app-estadistica-tipo-comprobante',
  templateUrl: './estadistica-tipo-comprobante.component.html',
  styleUrls: ['./estadistica-tipo-comprobante.component.scss']
})
export class EstadisticaTipoComprobanteComponent implements OnInit {
  decodeId = localStorage.getItem("unidadEjecutora");
  unidadEjecutoraId = 0;
  anio = 0;
  Highcharts2: typeof Highcharts = Highcharts;
  updateFlag = false;
  data = [];
  listAnios: Combobox[] = [];

  settings: Settings;
  chartTipoRecibo: ChartTipoRecibo[] = [];
  chartOptions2: Options = {
    title: {
      text: "Comprobantes Emitidos por mes según Tipo",
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

  widthChart = 1200;
  sidenaWidth = 300;

  constructor(
    private comprobantePagoService: ComprobantePagoService,
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

    this.comprobantePagoService.getComprobantePagoChartByTipo(this.unidadEjecutoraId, this.anio).subscribe(
      (response) => {
        if (response.success) {
          this.chartTipoRecibo = response.data;
          this.addSeries();
        }
      }
    );


  }

  onLoadCharts() {


    this.chartOptions2 = {
      title: {
        text: "Comprobantes Emitidos por mes según Tipo",
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
        crosshair: true
      },

      yAxis: {
        min: 0,
        title: {
          text: 'Total'
        }
      },
      tooltip: {
        headerFormat: '<span style="font-size:10px">{point.key}</span><table>',
        pointFormat: '<tr><td style="color:{series.color};padding:0">{series.name}: </td>' +
          '<td style="padding:0"><b>{point.y:.1f}</b></td></tr>',
        footerFormat: '</table>',
        shared: true,
        useHTML: true
      },

      series: [
        {
          name: 'Factura',
          type: 'column',
          data: []
        },
        {
          name: 'Boleta de Venta',
          type: 'column',
          data: []
        },
        {
          name: 'Nota de Crédito',
          type: 'column',
          data: []
        },
        {
          name: 'Nota de Débito',
          type: 'column',
          data: []
        },
        {
          name: 'Comprobante Retención',
          type: 'column',
          data: []
        }
      ],
      responsive: {
        rules: [{
          condition: {
            maxWidth: 500
          },

        }]
      }
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

      this.chartTipoRecibo[0].charts.forEach(item => {
        data1.push(item.total);
      });

      this.chartTipoRecibo[1].charts.forEach(item => {
        data2.push(item.total);
      });

      this.chartTipoRecibo[2].charts.forEach(item => {
        data3.push(item.total);
      });

      this.chartTipoRecibo[3].charts.forEach(item => {
        data4.push(item.total);
      });

      this.chartTipoRecibo[4].charts.forEach(item => {
        data5.push(item.total);
      });

      this.chartOptions2.subtitle.text = "(" + this.anio + ")";

      this.chartOptions2.series[0] =
      {
        name: this.chartTipoRecibo[0].tipoName,
        type: 'column',
        data: data1
      };

      this.chartOptions2.series[1] =
      {
        name: this.chartTipoRecibo[1].tipoName,
        type: 'column',
        data: data2
      };

      this.chartOptions2.series[2] =
      {
        name: this.chartTipoRecibo[2].tipoName,
        type: 'column',
        data: data3
      };

      this.chartOptions2.series[3] =
      {
        name: this.chartTipoRecibo[3].tipoName,
        type: 'column',
        data: data4
      };

      this.chartOptions2.series[4] =
      {
        name: this.chartTipoRecibo[4].tipoName,
        type: 'column',
        data: data5
      };

      this.updateFlag = true;
    }, 500);

  }

}