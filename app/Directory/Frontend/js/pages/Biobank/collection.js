$(function () {
    $(".clickable-row").click(function () {
        window.location.href = $(this).data("href");
    });

    $("#collection-samplesets").DataTable({
        "paging": false,
        "info": false,
        "language": {
            "search": "Filter: "
        }
    });
});