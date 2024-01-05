const botonContrato = document.getElementById('descargar')
const descargar = document.getElementById('descargarContrato')
var imagenFirma = document.getElementById('firmaImage');

// Accede al valor del atributo src
var urlFirma = imagenFirma.getAttribute('src');

window.obtenerImagen = () => {
    return urlFirma;
};

if (botonContrato != null) {
    botonContrato.addEventListener('click', (e) => {
        e.preventDefault();
        const ventana = window.open('ContratoDeInversion');
    });
}
//impresion de contrato con su firma
else {
    descargar.addEventListener('click', (e) => {
        e.preventDefault();
        const ventana = window.open('/Formulario/ContratoDeInversion');
    });
}

