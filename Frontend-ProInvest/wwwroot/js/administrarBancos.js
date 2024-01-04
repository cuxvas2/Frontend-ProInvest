$(document).ready(function() {
    $('#modalAgregar').on('show.bs.modal', function (event) {
        var url = '/admin/AgregarBanco'
        $.get(url, function(data) {
            $('#contenedorAgregar').html(data);
        });
    });

    $('#modalEditar').on('show.bs.modal', function (event) {
        var button = $(event.relatedTarget); 
        var idBanco = button.closest('tr').data('banco-id'); 
        var nombreBanco = button.closest('tr').data('banco-nombre');
        var url = '/admin/EditarBanco?idBanco=' + encodeURIComponent(idBanco) + '&nombreBanco=' + encodeURIComponent(nombreBanco);
        $.get(url, function(data) {
            $('#contenedorEditar').html(data);
        });
    });

    $('#modalEliminar').on('show.bs.modal', function (event) {
        var button = $(event.relatedTarget); 
        var idBanco = button.closest('tr').data('banco-id'); 
        var nombreBanco = button.closest('tr').data('banco-nombre');
        var url = '/admin/EliminarBanco?idBanco=' + encodeURIComponent(idBanco) + '&nombreBanco=' + encodeURIComponent(nombreBanco);
        $.get(url, function(data) {
            $('#contenedorEliminar').html(data);
        });
    });
});