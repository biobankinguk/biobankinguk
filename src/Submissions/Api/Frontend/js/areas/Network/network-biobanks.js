$(function() {
    $("#network-biobanks").DataTable({
        "columnDefs": [{ "orderable": false, "targets": 1 }],
        "paging": false,
        "info": false,
        "autoWidth": false,
        "language": {
            "search": "Filter: "

        }
    });

    //wire up bootbox confirmation
    $(".confirm-delete").click(function(e) {
        e.preventDefault();
        var $link = $(this);
        bootbox.confirm("Are you sure you wish to remove " + $link.data("biobank-name") + " from your network?", function (confirmation) {
            confirmation && window.location.assign($link.attr("href"));
        });
    });
});