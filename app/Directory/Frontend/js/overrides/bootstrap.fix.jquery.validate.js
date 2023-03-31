/**
 * Bootstrap 3.x and JQuery Validate interfere with each other; this fixes that
 * for any fields with relevant (otherwise broken) classes applied.
 */

(function ($) {
  var defaultOptions = {
    errorClass: "has-error",
    validClass: "has-success",
    highlight: function (element, errorClass, validClass) {
      $(element)
        .closest(".form-group")
        .addClass(errorClass)
        .removeClass(validClass);
    },
    unhighlight: function (element, errorClass, validClass) {
      $(element)
        .closest(".form-group")
        .removeClass(errorClass)
        .addClass(validClass);
    },
  };

  $.validator.setDefaults(defaultOptions);

  $.validator.unobtrusive.options = {
    errorClass: defaultOptions.errorClass,
    validClass: defaultOptions.validClass,
  };
})(jQuery);

$(document).ready(function () {
  $(".input-validation-error").parents(".form-group").addClass("has-error");
  $(".field-validation-error").addClass("text-danger");
});
