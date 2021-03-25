$(function () {

    $("#CountyId").change(function () {

            //AJAX Update
            $.ajax({
                url: '/api/' + 'county' + "/" + $("#CountyId").val() + "/country",
                type: 'GET',
                success: function (data) {
                    $("#CountryId").val(data);
                }
            });

    });
});