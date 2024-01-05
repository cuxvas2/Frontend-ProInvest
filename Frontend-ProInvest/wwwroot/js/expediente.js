function updateCardText(index) {
    var fileInput = document.getElementById('file-input-' + index);
    var cardText = document.getElementById('card-text-' + index);
    var fileName = document.getElementById('file-name-' + index);
    if (fileInput.files.length > 0) {
        cardText.textContent = fileInput.files[0].name;
        fileName.value = fileInput.files[0].name;
    }
}
$(document).ready(function() {
    $('form').submit(function() {
        $('#loadingModal').show();
    });

    $(document).ajaxStop(function() {
        $('#loadingModal').hide();
    });
});