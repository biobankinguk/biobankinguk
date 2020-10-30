$(function () {
    $(".clickable-row").click(function() {
        window.location.href = $(this).data("href");
    });

    $("#biobank-capabilities").DataTable({
        "paging": false,
        "info": false,
        "autoWidth": false,
        "language": {
            "search": "Filter: "
        }
    });
});