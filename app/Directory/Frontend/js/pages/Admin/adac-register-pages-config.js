$(function () {
    $('input[type=checkbox]').each(function () {
        $(this).prop("checked", $(this).val() == "true");
    });
    $('input[type=checkbox]').change(function () {
        $(this).val($(this).prop("checked"));
    });
}); 
