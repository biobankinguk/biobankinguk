$(function () {
  var noLogo = "/images/NoLogo.png";
  var form = $("#Logo");

  form.hide();

  $("#NetworkLogoFileDialogueTrigger").click(function () {
    form.click();
  });

  form.change(function () {
    if (form.val() != "") {
      var data = new FormData();
      var files = form.get(0).files;

      if (files.length > 0) {
        data.append("TempLogo", files[0]);

        $.ajax({
          url: form.data("resource-url"),
          type: "POST",
          processData: false,
          contentType: false,
          data: data,
          success: function (response) {
            if (response.key === true) {
              var d = new Date();
              $("#TempNetworkLogo").attr(
                "src",
                response.value + "/" + d.getTime()
              );
              $("#NetworkLogoUploadError").hide();
              $("#RemoveLogo").val(false);
            } else {
              $("#NetworkLogoUploadError").html(response.value);
              $("#NetworkLogoUploadError").show();
            }
          },
        });
      } else {
        alert("Select a file!");
      }
    }
  });

  $("#RemoveNetworkLogoTrigger").click(function (e) {
    var form = $(e.target);

    $.ajax({
      url: form.data("resource-url"),
      type: "POST",
      success: function () {
        $("#Logo").val("");
        $("#RemoveLogo").val(true);
        $("#TempNetworkLogo").attr("src", noLogo);
      },
    });
  });
});
