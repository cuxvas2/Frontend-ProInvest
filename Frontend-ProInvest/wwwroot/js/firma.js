const canvas = document.querySelector('canvas');
const botonEnviar = document.getElementById('boton-enviar');
const botonLimpiar = document.querySelector('.boton-limpiar');
//contexto del canvas para dibujar en 2d
const ctx = canvas.getContext('2d');
//bandera para ver si ya comenzamos a presionar el botón sin soltarlo
let modoEscritura = false;
//variables para guardar la posición del cursor
let xAnterior = 0, yAnterior = 0, xActual = 0, yActual = 0;
//variables de estilo
const COLOR = 'blue';
const GROSOR = 2;

const limpiarPad = () => {
    ctx.fillStyle = 'white';
    ctx.fillRect(0, 0, canvas.width, canvas.height);
};
limpiarPad();

botonLimpiar.addEventListener('click', (e) => {
    e.preventDefault();
    limpiarPad();
});

//funcion accedida por ventana hijo
window.obtenerImagen = () => {
    return canvas.toDataURL();
};

botonEnviar.addEventListener('click', async function () {
    const urlImagen = canvas.toDataURL();
    try {
        var result = await $.ajax({
            url: '/Formulario/EnviarFirma',
            type: 'POST',
            data: { base64url: urlImagen }
        });
    } catch (error) {
        alert('Ocurrió un error al guardar la firma. Por favor, inténtalo de nuevo.');
    }
});



const obtenerPosicionCursor = (e) => {
    positionX = e.clientX - e.target.getBoundingClientRect().left;
    positionY = e.clientY - e.target.getBoundingClientRect().top;

    return [positionX, positionY];
}

//funcion cuando inicia el trazo
const OnClicOToqueIniciado = (e) => {
    modoEscritura = true;
    [xActual, yActual] = obtenerPosicionCursor(e);

    ctx.beginPath();
    ctx.fillStyle = COLOR;
    ctx.fillRect(xActual, yActual, GROSOR, GROSOR);
    ctx.closePath();
}

//al mover el dedo o mouse sin depejarlo dibujamos las líneas
const OnMouseODedoMovido = (e) => {
    if (!modoEscritura) return;

    let target = e;
    if (e.type.includes("touch")) {
        target = e.touches[0];
    }
    xAnterior = xActual;
    yAnterior = yActual;
    [xActual, yActual] = obtenerPosicionCursor(target);

    ctx.beginPath();
    ctx.lineWidth = GROSOR;
    ctx.strokeStyle = COLOR;
    ctx.moveTo(xAnterior, yAnterior);
    ctx.lineTo(xActual, yActual);
    ctx.stroke();
    ctx.closePath();
}

//al levantar el dedo
function OnClicODedoLevantado() {
    modoEscritura = false;
}

['mousedown', 'touchstart'].forEach(nombreEvento => {
    canvas.addEventListener(nombreEvento, OnClicOToqueIniciado, { passive: true });
});

['mousemove', 'touchmove'].forEach(nombreEvento => {
    canvas.addEventListener(nombreEvento, OnMouseODedoMovido, { passive: true });
});

['mouseup', 'touchend'].forEach(nombreEvento => {
    canvas.addEventListener(nombreEvento, OnClicODedoLevantado, { passive: true });
});

