$(function() {
    $.fn.dataTable.moment("DD/MM/YYYY HH:mm:ss");

    $("#adac-biobanks").DataTable({
        // for 'dom' property values (used to arrange table controls), consult https://datatables.net/reference/option/dom
        "dom": "flB<\"clear\">rt<\"bottom\"ip<\"clear\">>",

        "columns": [
            { "width": "17%" },
            { "width": "15%" },
            { "width": "13%" },
            { "width": "14%" },
            { "width": "13%" },
            { "width": "13%" },
            { "width": "15%" }
        ],
        "buttons": [
            "csv"
        ],
        "paging": true,
        "info": false,
        "autoWidth": false,
        "language": {
            "search": "Filter: "
        }
    });
});