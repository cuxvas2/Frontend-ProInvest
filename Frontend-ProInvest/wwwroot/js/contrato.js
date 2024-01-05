const botonContrato = document.getElementById('descargar')
const folio = document.getElementById('folio').innerText;
var imagenFirma = document.getElementById('firmaImage');

// Accede al valor del atributo src
var urlFirma = imagenFirma.getAttribute('src');

window.obtenerImagen = () => {
    return urlFirma;
};

//impresion de contrato con su firma
botonContrato.addEventListener('click', (e) => {
    e.preventDefault();
    const ventana = window.open('ContratoDeInversion');
});

