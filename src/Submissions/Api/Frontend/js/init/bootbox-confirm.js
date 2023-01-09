/**
 * Initialises a default Confirmation dialog using Bootbox
 * on any element with the class `bootbox-confirm`
 */

$(function () {
  $(".bootbox-confirm").click(function (e) {
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
