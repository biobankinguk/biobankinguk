$(function () {
    $("#accept-network").DataTable({
        "columnDefs": [{ "orderable": false, "targets": 1 }],
        "paging": false,
        "info": false,
        "autoWidth": false,
        "language": {
            "search": "Filter: ",
            "emptyTable": "There are no network requests requiring action at the moment."
        }
    });

    //wire up bootbox confirmation
    $(".confirm-accept").click(function (e) {
        e.preventDefault();
        var $link = $(this);
        bootbox.confirm("Are you sure you wish to accept the invitation to join " + $link.data("network-name") + " ?", function (confirmation) {
            confirmation && window.location.assign($link.attr("href"));
        });
    });
});