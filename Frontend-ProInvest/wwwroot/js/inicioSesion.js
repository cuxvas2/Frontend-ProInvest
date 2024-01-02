const mostrarContrasena = document.getElementById("togglePassword")
mostrarContrasena.addEventListener("click", function () {
    var passwordInput = document.getElementById("Password");
    if (passwordInput.type === "password") {
        passwordInput.type = "text";
    } else {
        passwordInput.type = "password";
    }
});

