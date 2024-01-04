const codigoPostal = document.getElementById("codigoInput");
const codigoPostalSelect = document.getElementById('coloniaSelect');
const municipioSelect = document.getElementById('municipioSelect');
const estadoSelect = document.getElementById('estadoSelect');
codigoPostal.addEventListener("input", async function () {
    var codigoPostalIngresado = document.getElementById('codigoInput').value;
    var regex = /^[0-9]{5}$/;
    var isValid = regex.test(codigoPostalIngresado);

    if (isValid) {
        try {
            var result = await $.ajax({
                url: '/Formulario/HandleCodigoPostalChange', 
                type: 'POST',
                data: { codigoPostal: codigoPostalIngresado }
            });
            if (result.error !== undefined) {
                throw (error);
            }
            else {
                actualizarSelect(result.colonias);
                actualizarSelectEstado(result.colonias);
                actualizarSelectMunicipio(result.colonias);
            }
        } catch (error) {
            console.error('Error al manejar el cambio del código postal:', error);
            actualizarSelectError("coloniaSelect");
            actualizarSelectError("municipioSelect");
            actualizarSelectError("estadoSelect");
        }
    }
});

function actualizarSelectError(idElemento) {
    const elemento = document.getElementById(idElemento);
    elemento.innerHTML = '';
    var optionError = document.createElement('option');
    optionError.value = "";
    optionError.text = "Ocurrió un error al cargar";
    elemento.appendChild(optionError);
}

function actualizarSelect(opciones) {
    codigoPostalSelect.innerHTML = '';
    opciones = Array.isArray(opciones) ? opciones : [opciones];
    if (opciones.length > 0) {
        opciones.forEach(function (opcion) {
            var option = document.createElement('option');
            option.value = opcion.colonia;
            option.text = opcion.colonia;
            codigoPostalSelect.appendChild(option);
        });

    }
    else {
        var option = document.createElement('option');
        option.value = "";
        option.text = "No se encontraron colonias para ese CP";
        codigoPostalSelect.appendChild(option);
    }
}

function actualizarSelectEstado(opciones) {
    estadoSelect.innerHTML = '';
    opciones = Array.isArray(opciones) ? opciones : [opciones];
    if (opciones.length > 0) {
        var primeraColonia = opciones[0];
        var option = document.createElement('option');
        option.value = primeraColonia.estado;
        option.text = primeraColonia.estado;
        option.selected = true;
        estadoSelect.appendChild(option);
    }
    else {
        var option = document.createElement('option');
        option.value = "";
        option.text = "Verifique su código postal";
        estadoSelect.appendChild(option);
    }
}

function actualizarSelectMunicipio(opciones) {
    municipioSelect.innerHTML = '';
    opciones = Array.isArray(opciones) ? opciones : [opciones];
    if (opciones.length > 0) {
        var primeraColonia = opciones[0];
        var option = document.createElement('option');
        option.value = primeraColonia.municipio;
        option.text = primeraColonia.municipio;
        option.selected = true;
        municipioSelect.appendChild(option);
    }
    else {
        var option = document.createElement('option');
        option.value = "";
        option.text = "Verifique su código postal";
        municipioSelect.appendChild(option);
    }
}