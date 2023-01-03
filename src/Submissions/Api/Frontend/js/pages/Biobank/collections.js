$(function () {
    $(".clickable-row").click(function () {
        window.location.href = $(this).data("href");
    });

    $table = $("#biobank-collections").DataTable({
        paging: false,
        info: false,
        autoWidth: false,
        order: [[1, "asc"]],
        columnDefs: [
            { targets: 0, orderable: false, width: "1.2em" },
        ],
        language: {
            "search": "Filter: "
        }
    });

    // Hide First Column If Empty
    var warnData = $table.column(0)
        .data()
        .filter(function (el) {
            return (el && el.length > 0); // Filter out null/empty strings
        });

    $table.column(0).visible(warnData.length > 0);
});