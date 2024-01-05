var labels = ['Año 1'];

var options = {
    series: [{
        name: 'Tú inversión',
        data: []
    },
    {
        name: 'Más rendimiento',
        data: []
    }],
    chart: {
        type: 'bar',
        width: "100%",
        height: 300
    },
    plotOptions: {
        bar: {
            horizontal: false,
            columnWidth: '55%',
            borderRadius: 10,
            endingShape: 'rounded'
        },
    },
    dataLabels: {
        enabled: false
    },
    stroke: {
        show: true,
        width: 2,
        colors: ['transparent']
    },
    xaxis: {
        categories: labels,
    },
    yaxis: {
        title: {
            text: '$'
        },
        labels: {
            formatter: function (value) {
                return value.toFixed(2); // Limitar a dos decimales
            },
        },
    },
    legend: {
        position: "top",
        containerMargin: {
            left: 0,
            rigth: 0
        }
    },
    fill: {
        opacity: 1
    },
    tooltip: {
        y: {
            formatter: function (val) {
                return "$ " + (val).toFixed(2);
            }
        }
    },
};

var chart = new ApexCharts(document.querySelector(".chart"), options);
chart.render();

function calcularInversion(rendimiento, rendimientoAnterior) {
    rendimiento = parseFloat(rendimiento); 
    rendimientoAnterior = parseFloat(rendimientoAnterior);
    return rendimientoAnterior + (rendimientoAnterior * rendimiento);
}


const tiposInversionSelector = document.getElementById("tiposInversion");
const button = document.getElementById("btnMostrarGrafica");
//const button = document.querySelector('.btn');
const slider = document.querySelector('.slider');
const importeInput = document.getElementById("importe");
const form = document.getElementById("formulario-simulacion");
button.addEventListener('click', function (event) {

    event.preventDefault();
    actualizarGrafica();
})

slider.addEventListener('change', function () {
    actualizarGrafica();
})

function actualizarGrafica() {
    if ($(form).valid()) {
        var anios = slider.value;
        var importe = importeInput.value;
        var rendimiento1 = tiposInversionSelector.value;
        var rendimiento = rendimiento1 / 100;
        var rendimientoAnterior = importe;
        var datos = [];
        var tuInversion = [];
        labels.length = 0;
        for (i = 0; i < anios; i++) {
            rendimientoAnterior = calcularInversion(rendimiento, rendimientoAnterior);
            datos.push(rendimientoAnterior);
            labels.push(`Año ${i + 1}`);
            tuInversion.push(importe);
        }
        chart.updateOptions({
            xasis: {
                categories: labels
            }
        });
        chart.updateSeries([{ data: tuInversion }, { data: datos }]);
        chart.update();
    }
}