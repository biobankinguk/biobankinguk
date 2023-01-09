$(function () {
    $("#adac-networks").DataTable({
        "columnDefs": [{ "orderable": false, "targets": 1 }],
        "paging": false,
        "info": false,
        "autoWidth": false,
        "language": {
            "search": "Filter: "
        }
    });

    $(".resend-confirm").click(function (e) {
        e.preventDefault();
        var $link = $(this);
        bootbox.confirm(
            $link.data("admin-name") +
            " (" + $link.data("admin-email") + ") " +
            "has not yet confirmed their account.<br/>Do you want to resend their confirmation link?",
            function (confirmation) {
                confirmation && window.location.assign($link.attr("href"));
            });
    });
});