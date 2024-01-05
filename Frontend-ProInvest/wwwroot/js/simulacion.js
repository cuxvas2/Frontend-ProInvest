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

var chart = new ApexCharts(document.getElementById("chartSimulacion"), options);
var chartComparacion = new ApexCharts(document.getElementById("chartComparacion"), options);
chart.render();
chartComparacion.render();

function calcularInversion(rendimiento, rendimientoAnterior) {
    rendimiento = parseFloat(rendimiento); 
    rendimientoAnterior = parseFloat(rendimientoAnterior);
    return rendimientoAnterior + (rendimientoAnterior * rendimiento);
}


const tiposInversionSelector = document.getElementById("tiposInversion");
const button = document.getElementById("btnMostrarGrafica");
const slider = document.getElementById("sliderSimulador");
const importeInput = document.getElementById("importe");
const form = document.getElementById("formulario-simulacion");

const buttonComparacion = document.getElementById("btnMostrarGraficaComparacion");
const sliderComparacion = document.getElementById("sliderComparacion");
const importeInputComparacion = document.getElementById("importeComparacion");
const formComparacion = document.getElementById("formulario-comparacion");
var selectedItems;
button.addEventListener('click', function (event) {

    event.preventDefault();
    actualizarGrafica();
})

buttonComparacion.addEventListener('click', function (event) {
    event.preventDefault();
    var checkboxes = document.querySelectorAll('.form-check-input');
    selectedItems = [];
    checkboxes.forEach(function (checkbox) {
        if (checkbox.checked) {
            selectedItems.push({
                value: checkbox.value,
                text: checkbox.id
            });
        }
    });
    actulizarGraficaComparativa();
})

slider.addEventListener('change', function () {
    actualizarGrafica();
})

sliderComparacion.addEventListener('change', function () {
    var checkboxes = document.querySelectorAll('.form-check-input');
    selectedItems = [];
    checkboxes.forEach(function (checkbox) {
        if (checkbox.checked) {
            selectedItems.push({
                value: checkbox.value,
                text: checkbox.id
            });
        }
    });
    actulizarGraficaComparativa();
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

function actulizarGraficaComparativa() {
    var labelExistente = document.getElementById('labelComparacion');
    if (labelExistente) {
        labelExistente.remove();
    }
    if (selectedItems.length < 2 || selectedItems.length > 4) {
        var labelElement = document.createElement('label');
        labelElement.id = 'labelComparacion';
        labelElement.className = 'text-danger'; // Clase para dar color rojo al texto
        labelElement.innerHTML = 'Debes seleccionar mínimo 2 y máximo 4 tipos de inversión para comparar';
        const seccionComparacion = document.getElementById("tiposDropdown");
        seccionComparacion.appendChild(labelElement);
    }
    else if ($(formComparacion).valid()) {
        var anios = sliderComparacion.value;
        var importe = importeInputComparacion.value;
        var rendimientosTotales = [];
        labels.length = 0;
        for (var i = 0; i < selectedItems.length; i++) {
            let rendimientoTipo = [];
            let porcentaje = selectedItems[i].value / 100;
            let rendimientoAnual = importe;
            for (var j = 0; j < anios; j++) {
                rendimientoAnual = calcularInversion(porcentaje, rendimientoAnual);
                rendimientoTipo.push(rendimientoAnual);
            }
            rendimientosTotales.push(rendimientoTipo);
        }
        for (var i = 0; i < anios; i++) {
            labels.push(`Año ${i + 1}`);
        }
        chart.updateOptions({
            xasis: {
                categories: labels
            }
        });
        let result = [];
        for (var i = 0; i < rendimientosTotales.length; i++) {
            var nombreSerie = selectedItems[i].text;
            result.push({
                name: nombreSerie,
                data: rendimientosTotales[i]
            });
        }
        chartComparacion.updateSeries(result);
        chartComparacion.update();
    }
}