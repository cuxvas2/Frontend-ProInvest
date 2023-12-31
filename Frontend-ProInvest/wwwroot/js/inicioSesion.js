const mostrarContrasena = document.getElementById("togglePassword")
mostrarContrasena.addEventListener("click", function () {
    var passwordInput = document.getElementById("Password");
    if (passwordInput.type === "password") {
        passwordInput.type = "text";
    } else {
        passwordInput.type = "password";
    }
});
document.addEventListener('DOMContentLoaded', (event) => {
    const modalBienvenida = document.getElementById('modalBienvenida');
    const mostrarModalBienvenida = modalBienvenida.dataset.mostrar;

    if (mostrarModalBienvenida === 'true') {
        const nuevoModal = new bootstrap.Modal(modalBienvenida);
        nuevoModal.show();
    }
});