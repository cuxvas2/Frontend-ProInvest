$(document).ready(function() {
    $('#modalAgregar').on('show.bs.modal', function (event) {
        var url = '/admin/AgregarDocumentosExpediente'
        $.get(url, function(data) {
            $('#contenedorAgregar').html(data);
        });
    });

    $('#modalEditar').on('show.bs.modal', function (event) {
        var button = $(event.relatedTarget); 
        var idDocumento = button.closest('tr').data('documento-id'); 
        var nombreDocumento = button.closest('tr').data('documento-nombre');
        var url = '/admin/EditarDocumentosExpediente?idDocumento=' + encodeURIComponent(idDocumento) + '&nombreDocumento=' + encodeURIComponent(nombreDocumento);
        $.get(url, function(data) {
            $('#contenedorEditar').html(data);
        });
    });

    $('#modalEliminar').on('show.bs.modal', function (event) {
        var button = $(event.relatedTarget); 
        var idDocumento = button.closest('tr').data('documento-id'); 
        var nombreDocumento = button.closest('tr').data('documento-nombre');
        var url = '/admin/EliminarDocumentoExpediente?idDocumento=' + encodeURIComponent(idDocumento) + '&nombreDocumento=' + encodeURIComponent(nombreDocumento);
        $.get(url, function(data) {
            $('#contenedorEliminar').html(data);
        });
    });
});