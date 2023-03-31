function copyText(fieldId) {
  /* Get the text field */
  var source = document.getElementById(fieldId);

  /* Select the text field */
  source.select();
  source.setSelectionRange(0, 99999); /* For mobile devices */

  /* Copy the text inside the text field */
  document.execCommand("copy");
}

//copy text
$(".copy-text").click(function (e) {
  e.preventDefault();
  var $btn = $(this);
  var txtbox = $btn.data("target");
  copyText(txtbox);
});

//generate client id
$("#generatekey").click(function (e) {
  var $btn = $(this);
  $.post(
    $btn.data("generate-url"),
    {
      biobankId: $btn.data("biobank-id"),
    },
    function (data) {
      $("#clientId").val(data.clientId);
      $("#clientSecret").val(data.clientSecret);
      $("#clientSecretWrapper").removeAttr("hidden");
    }
  );
});
