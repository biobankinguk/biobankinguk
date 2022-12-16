$(function () {
    var noLogo = '/Content/images/NoLogo.png';

    $('#Logo').hide();

    $('#BiobankLogoFileDialogueTrigger').click(function () {
        $('#Logo').click();
    });

    $('#Logo').change(function () {
        if ($('#Logo').val() != "") {
            var data = new FormData();
            var files = $("#Logo").get(0).files;

            if (files.length > 0) {
                data.append("TempLogo", files[0]);

                $.ajax({
                    url: "/Biobank/AddTempLogo",
                    type: "POST",
                    processData: false,
                    contentType: false,
                    data: data,
                    success: function (response) {
                        if (response.Key === true) {
                            var d = new Date();
                            $('#TempBiobankLogo').attr('src', response.Value + "/" + d.getTime());
                            $('#BiobankLogoUploadError').hide();
                            $('#RemoveLogo').val(false);
                        } else {
                            $('#BiobankLogoUploadError').html(response.Value);
                            $('#BiobankLogoUploadError').show();
                        }
                    }
                });
            } else {
                alert('Select a file!');
            }
        }
    });

    $('#RemoveBiobankLogoTrigger').click(function () {
        $.ajax({
            url: "/Biobank/RemoveTempLogo",
            type: "POST",
            success: function () {
                $('#Logo').val("");
                $('#RemoveLogo').val(true);
                $('#TempBiobankLogo').attr('src', noLogo);
            }
        });
    });
})