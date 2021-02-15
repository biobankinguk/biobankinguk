// HTML Link Generator
function link(name, classes, onclick, href) {
    return $("<a>", {
        text: name,
        class: classes,
        attr: {
            onclick: onclick,
            href: href,
            target: (href == null ? null : "_blank"),
            disabled: (onclick == null && href == null),
        }
    })
    .get(0).outerHTML;
}

// Ajax Calls
function claimPublication(publicationId, accept) {
    $.post("ClaimPublicationAjax", { publicationId, accept },
        function (response) {
            table.row("#" + publicationId).data(response).draw(false); // Update Row With New Data
        }
    );
}

function acceptPublication(publicationId) { claimPublication(publicationId, true); }
function rejectPublication(publicationId) { claimPublication(publicationId, false); }

// DataTable
var table;

function colorRow(row, data) {
    $(row).toggleClass("success", data.Approved == true);
    $(row).toggleClass("danger", data.Approved == false);
    $(row).find(".status").toggleClass("flex-around", data.Approved == null);
}

function formatTitle(data, type, row) {
    if (row.DOI == null)
        return data;

    // DOI External Link
    return link(data, "", null, ("http://doi.org/" + row.DOI));
}

function formatStatus(data, type, row) {
    if (row.Approved == true) {
        return link("Accepted", "btn btn-success btn-full");
    }
    else if (row.Approved == false) {
        return link("Rejected", "btn btn-danger btn-full");
    }
    else {
        return link("Accept", "btn btn-success", "acceptPublication(" + row.PublicationId + ")")
             + link("Reject", "btn btn-danger", "rejectPublication(" + row.PublicationId + ")");
    }
}

function toggleButton(button) {
    $(".dt-buttons .btn").removeClass("active");
    $(button).addClass("active");
} 

function filter(filterFn) {
    $.fn.dataTable.ext.search.pop();
    $.fn.dataTable.ext.search.push(
        function (settings, data, dataIndex) {
            return filterFn(table.row(dataIndex));
        }
    );
    table.draw();
}

$(function () {
    table = $("#biobank-publications").DataTable({
        "paging": true,
        "pageLength": 25,
        "info": false,
        "language": {
            "search": "Filter: "
        },
        "ajax": {
            "url": "GetPublicationsAjax",
            "dataSrc": ""
        },
        "columns": [
            { title: "Title",   width: "",      data: "Title",  render: formatTitle },
            { title: "Authors", width: "160px", data: "Authors"},
            { title: "Year",    width: "35px",  data: "Year" },
            { title: "Journal", width: "140px", data: "Journal" },
            { title: "Status",  width: "120px", data: "Approved", render: formatStatus, className: "status" }
        ],
        "rowId": 'PublicationId',
        "rowCallback": colorRow
    });

    // Table Controls
    var controls = $("<div class='row'><div class='col-md-12'>")
        .css("margin-bottom", "10px")
        .prependTo("#biobank-publications_wrapper")
        .children();
    $('#biobank-publications_wrapper').hide();
    new $.fn.dataTable.Buttons(table, {
        buttons: [
            "colvis",
            {
                text: 'Accepted',
                action: function (e, dt, node, conf) {
                    toggleButton(node);
                    filter(function (row) {
                        return row.data().Approved == true;
                    });
                }
            },
            {
                text: 'Rejected',
                action: function (e, dt, node, conf) {
                    toggleButton(node);
                    filter(function (row) {
                        return row.data().Approved == false;
                    });
                }
            },
            {
                text: 'Undecided',
                action: function (e, dt, node, conf) {
                    toggleButton(node);
                    filter(function (row) {
                        return row.data().Approved == null;
                    });
                }
            },
            {
                text: 'All',
                action: function (e, dt, node, conf) {
                    toggleButton(node);
                    filter(function (row) {
                        return true;
                    });
                }
            },
        ]
    });

    table.buttons().container().prependTo(controls);
    $.getJSON('/api/Biobank/IncludePublications/' + biobankId, function (data) {
        if (data) {
            $('#IncludePublications').prop('checked', true);
            $('#biobank-publications_wrapper').show();
        }
        else {
            $('#IncludePublications').prop('checked', false);
            $('#biobank-publications_wrapper').hide();
        }
    });
});


$('#IncludePublications').change(function () {
    var val = this.checked;
    if (val)
        $('#biobank-publications_wrapper').show();
    else
        $('#biobank-publications_wrapper').hide();

    $.ajax({
        url: '/api/Biobank/IncludePublications/' + biobankId + '/' + val,
        type: 'PUT'
    });
});

//Biobank Id
var biobankId = $('#BiobankId').data("biobank-id");