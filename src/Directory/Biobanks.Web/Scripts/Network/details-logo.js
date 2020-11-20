$(function () {
    var noLogo = '/Content/images/NoLogo.png';

    $('#Logo').hide();

    $('#NetworkLogoFileDialogueTrigger').click(function () {
        $('#Logo').click();
    });

    $('#Logo').change(function () {
        if ($('#Logo').val() != "") {
            var data = new FormData();
            var files = $("#Logo").get(0).files;

            if (files.length > 0) {
                data.append("TempLogo", files[0]);

                $.ajax({
                    url: "/Network/AddTempLogo",
                    type: "POST",
                    processData: false,
                    contentType: false,
                    data: data,
                    success: function (response) {
                        if (response.Key === true) {
                            var d = new Date();
                            $('#TempNetworkLogo').attr('src', response.Value + "/" + d.getTime());
                            $('#NetworkLogoUploadError').hide();
                            $('#RemoveLogo').val(false);
                        } else {
                            $('#NetworkLogoUploadError').html(response.Value);
                            $('#NetworkLogoUploadError').show();
                        }
                    }
                });
            } else {
                alert('Select a file!');
            }
        }
    });

    $('#RemoveNetworkLogoTrigger').click(function () {
        $.ajax({
            url: "/Network/RemoveTempLogo",
            type: "POST",
            success: function () {
                $('#Logo').val("");
                $('#RemoveLogo').val(true);
                $('#TempNetworkLogo').attr('src', noLogo);
            }
        });
    });
})