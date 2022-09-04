let simplemde = new SimpleMDE({element: $("description")[0]});
$('.CodeMirror textarea').val(' ');
$(":file").filestyle({text: location.upload_image});

$("#topic").selectize({
    create: false,
    sortField: "text",
});

let fieldCounter = 0;

$("#add-field").click(function () {
    $(".custom-fields")
        .append('<div class="form-floating" id="custom-field-' + fieldCounter + '"></div>');

    $('#custom-field-' + fieldCounter).append('<input required type="text" name="CustomFieldsModel[' + fieldCounter + '].Key" placeholder="Enter field name"/>')
        .append('<select name="CustomFieldsModel[' + fieldCounter + '].Value">' +
            ' <option value="number">'+localization.int_field+'</option>' +
            '  <option value="text">'+localization.string_field+'</option>' +
            '  <option value="textarea">'+localization.text_field+'</option>' +
            '  <option value="checkbox">'+localization.bool_field+'</option>' +
            '   <option value="date">'+localization.date_field+'</option></select>')
        .append('<button id="' + fieldCounter + '" type="button" class="remove-field-button btn btn-danger">'+localization.remove_field+'</button>')
    fieldCounter++;
});


$('.custom-fields').on('click', '.remove-field-button', function () {
    $('.custom-fields').find("#custom-field-" + this.id).remove();
    fieldCounter--;
});
