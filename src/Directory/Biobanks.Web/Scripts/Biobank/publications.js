﻿// HTML Link Generator
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
    $(row).toggleClass("success", data.Accepted == true);
    $(row).toggleClass("danger", data.Accepted == false);
    $(row).find(".status").toggleClass("flex-around", data.Accepted == null);
}

function formatTitle(data, type, row) {
    if (row.DOI == null)
        return data;

    // DOI External Link
    return link(data, "", null, ("http://doi.org/" + row.DOI));
}

function formatStatus(data, type, row) {
    if (row.Accepted == true) {
        return link("Accepted", "btn btn-success btn-full");
    }
    else if (row.Accepted == false) {
        return link("Rejected", "btn btn-danger btn-full");
    }
    else {
        return link("Accept", "btn btn-success", "acceptPublication(" + row.PublicationId + ")")
             + link("Reject", "btn btn-danger", "rejectPublication(" + row.PublicationId + ")");
    }
}

function formatAuthor(data, type, row) {
    if (data) {
        let authorLink = $("<a>", {
            text: "see all",
            attr: {
                href: "#",
                "data-target": "#authorModal",
                "data-toggle":"modal",
                "data-authors": data
            }
        }).get(0).outerHTML;

        let authors = data.split(",")
        return authors[0] + ((authors.length > 1) ? " et al. " + authorLink : "");
    }
    return "";
}

$('#authorModal').on('show.bs.modal', function (event) {
    var link = $(event.relatedTarget);
    var authors = link.data('authors');
    $(this).find('.modal-body span').text(authors);
});

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
            { title: "Authors", width: "160px", data: "Authors", render: formatAuthor},
            { title: "Year",    width: "35px",  data: "Year" },
            { title: "Journal", width: "140px", data: "Journal" },
            { title: "Status",  width: "120px", data: "Accepted", render: formatStatus, className: "status" }
        ],
        "rowId": 'PublicationId',
        "rowCallback": colorRow
    });

    // Table Controls
    var controls = $("<div class='row'><div class='col-md-12'>")
        .css("margin-bottom", "10px")
        .prependTo("#biobank-publications_wrapper")
        .children();

    var rightCtrls = $("<div class='row pull-right'><div class='col-md-12'>")
        .prependTo("#biobank-publications_wrapper")
        .children();

    $('#biobank-publications_wrapper').hide();
    var filterBtns = new $.fn.dataTable.Buttons(table, {
        buttons: [
            "colvis",
            {
                text: 'Accepted',
                action: function (e, dt, node, conf) {
                    toggleButton(node);
                    filter(function (row) {
                        return row.data().Accepted == true;
                    });
                }
            },
            {
                text: 'Rejected',
                action: function (e, dt, node, conf) {
                    toggleButton(node);
                    filter(function (row) {
                        return row.data().Accepted == false;
                    });
                }
            },
            {
                text: 'Undecided',
                action: function (e, dt, node, conf) {
                    toggleButton(node);
                    filter(function (row) {
                        return row.data().Accepted == null;
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

    var addBtn = new $.fn.dataTable.Buttons(table, {
        buttons: [
            {
                text: 'Add Publication',
                action: function (e, dt, node, conf) {
                    publicationsVM.openModal();
                }
            },
        ],
        dom: {
            button: {
                className: 'btn btn-success'
            }
        }
    });

    filterBtns.container().prependTo(controls);
    addBtn.container().prependTo(rightCtrls);
    //table.buttons().container().prependTo(controls);


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

    publicationsVM = new PublicationsViewModel();
    ko.applyBindings(publicationsVM);

    $("#publications-modal").submit(function (e) {
        publicationsVM.modalSubmit(e);
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


// Modals
var publicationsVM;


function PublicationsModal(id) {
    this.modalModeSearch = "Search";
    this.modalModeApprove = "Approve";

    this.mode = ko.observable(this.modalModeSearch);

    this.publicationId = ko.observable(id);
}

function PublicationsViewModel() {
    var _this = this;

    this.modalId = "#publications-modal";
    this.modal = new PublicationsModal("");
    this.dialogErrors = ko.observableArray([]);
    this.searchResult = ko.observable("");

    this.showModal = function () {
        _this.dialogErrors.removeAll(); //clear errors on a new show
        _this.searchResult(""); //clear search result
        $(_this.modalId).modal("show");
    };

    this.hideModal = function () {
        $(_this.modalId).modal("hide");
    };

    this.openModal = function () {
        $("#publicationId").attr("readonly", false);
        _this.modal.mode(_this.modal.modalModeSearch);
        _this.modal.publicationId("");
        _this.showModal();
    };


    this.modalSubmit = function (e) {
        e.preventDefault();

        // Get Action Type
        var action = _this.modal.mode();
        if (action == _this.modal.modalModeSearch) {
            $.getJSON("RetrievePublicationsAjax?publicationId=" + _this.modal.publicationId(), function (data) {
                if (data && !jQuery.isEmptyObject(data)) {
                    // If successfull search
                    _this.searchResult("<b>If this is the correct publication, then press the approve button to add this publication to your Biobanks approved publications list</b> <br/>"
                        + "<br/>" + data.Authors + " \"" + data.Title + "\", " + data.Year + ". "
                        + link("View on PubMed", "", null, ("https://pubmed.ncbi.nlm.nih.gov/" + data.PublicationId)));
                    $("#publicationId").attr("readonly", true);
                    _this.modal.mode(_this.modal.modalModeApprove);
                }
                else {
                    // no results
                    _this.searchResult("No publication found for the given PubMed Id.");
                }
            });
        } else if (action == _this.modal.modalModeApprove) {
            var publicationId = _this.modal.publicationId();
            $.post("AddPublicationAjax", { publicationId }, function (data) {
                //if successfull
                if (data && !jQuery.isEmptyObject(data)) {
                    window.location.href = $(e.target).data("success-redirect")
                        + "?publicationId=" + publicationId;
                }
                else
                    _this.dialogErrors.push("Something went wrong! Please try again later.")
            });
        }
    };
}