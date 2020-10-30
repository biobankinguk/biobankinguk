$(function () {
    $.fn.dataTable.moment("DD/MM/YYYY HH:mm:ss");

    $("#adac-request-history").DataTable({
        "order": [[ 4, "desc" ]],
        "columnDefs": [{ "orderable": false, "targets": 6 }],
        "paging": false, //we might actually want this on this one! Depends if we keep the history indefinitely
        "info": false,
        "autoWidth": false,
        "language": {
            "search": "Filter: "
        }
    });

    $(".resend-confirm").click(function(e) {
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