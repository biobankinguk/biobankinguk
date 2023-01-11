$(function () {
    $(".delete-confirm").click(function (e) {
        e.preventDefault();

        var $link = $(this);
        var linkData = $link.data("refdata-model")
        var url = $link.data("resource-url") + "/" + linkData.Id;

        bootbox.confirm("Are you sure you want to delete " + linkData.Title + "?",
            function (confirmation) {
                if (confirmation) {
                    deleteRefData(url, $link.data("success-redirect"), $link.data("refdata-type"));
                }
            }
        );
    });
})