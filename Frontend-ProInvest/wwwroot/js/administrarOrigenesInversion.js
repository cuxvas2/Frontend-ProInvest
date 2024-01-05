$(document).ready(function() {
    $('#modalAgregar').on('show.bs.modal', function (event) {
        var url = '/admin/AgregarOrigenInversion'
        $.get(url, function(data) {
            $('#contenedorAgregar').html(data);
        });
    });

    $('#modalEditar').on('show.bs.modal', function (event) {
        var button = $(event.relatedTarget); 
        var idOrigen = button.closest('tr').data('origen-id'); 
        var nombreOrigen = button.closest('tr').data('origen-nombre');
        var url = '/admin/EditarOrigenInversion?idOrigenInversion=' + encodeURIComponent(idOrigen) + '&nombreOrigen=' + encodeURIComponent(nombreOrigen);
        $.get(url, function(data) {
            $('#contenedorEditar').html(data);
        });
    });

    $('#modalEliminar').on('show.bs.modal', function (event) {
        var button = $(event.relatedTarget); 
        var idOrigen = button.closest('tr').data('origen-id'); 
        var nombreOrigen = button.closest('tr').data('origen-nombre');
        var url = '/admin/EliminarOrigenInversion?idOrigen=' + encodeURIComponent(idOrigen) + '&nombreOrigen=' + encodeURIComponent(nombreOrigen);
        $.get(url, function(data) {
            $('#contenedorEliminar').html(data);
        });
    });
});