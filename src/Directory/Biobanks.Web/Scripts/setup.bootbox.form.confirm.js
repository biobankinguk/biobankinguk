$(function() {
    $('.bootbox-confirm').click(function (e) {
        e.stopImmediatePropagation();
        e.preventDefault();

        var currentForm = $("#" + $(this).data("form-id"));
        var message = $(this).data("confirm-message");

        bootbox.confirm(message, function (result) {
            if (result) {
                currentForm.submit();
            }
        });
    });
});