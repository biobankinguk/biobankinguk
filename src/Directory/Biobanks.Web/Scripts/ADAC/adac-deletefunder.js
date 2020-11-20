$(function() {
    $(".delete-confirm-input").on("change keyup paste", function() {
        if ($(this).val().toLowerCase() === $(this).data("confirmtext").toLowerCase()) {
            $(".confirm-delete-button").prop("disabled", false);
        } else {
            $(".confirm-delete-button").prop("disabled", true);
        }
    });
});