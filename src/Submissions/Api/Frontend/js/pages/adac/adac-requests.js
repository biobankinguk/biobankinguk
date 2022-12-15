$(function () {
    $("#adac-biobanks").DataTable({
        "columnDefs": [{ "searchable": false, "orderable": false, "targets": 3 }],
        "paging": false,
        "info": false,
        "autoWidth": false,
        "language": {
            "search": "Filter: "
        }
    });

    $("#adac-networks").DataTable({
        "columnDefs": [{ "searchable": false, "orderable": false, "targets": 3 }],
        "paging": false,
        "info": false,
        "autoWidth": false,
        "language": {
            "search": "Filter: "
        }
    });

    //wire up bootbox confirmations
    $(".confirm-decline").click(function (e) {
        e.preventDefault();
        var $link = $(this);
        bootbox.confirm("Are you sure you wish to decline the request for " + $link.data("entity-name") + "?", function (confirmation) {
            confirmation && window.location.assign($link.attr("href"));
        });
    });

    $(".confirm-accept").click(function (e) {
        e.preventDefault();
        var $link = $(this);
        bootbox.confirm("Are you sure you wish to accept the request for " + $link.data("entity-name") + "?", function (confirmation) {
            confirmation && window.location.assign($link.attr("href"));
        });
    });
});