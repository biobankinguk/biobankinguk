const refdataType = "Disease status";
const redirectUrl = "/ADAC/DiseaseStatuses";
const apiUrl = "/api/DiseaseStatus";

var adacDiseaseStatusVM;
var dataTable;

function DiseaseStatus(ontologyTermId, description, otherTerms, displayOnDirectory) {
    this.ontologyTermId = ko.observable(ontologyTermId);
    this.description = ko.observable(description);
    this.otherTerms = ko.observableArray(otherTerms);
    this.displayOnDirectory = ko.observable(displayOnDirectory);
}

function DiseaseStatusModal(ontologyTermId, description, otherTerms, displayOnDirectory) {
    this.modalModeAdd = "Add";
    this.modalModeEdit = "Update";

    this.mode = ko.observable(this.modalModeAdd);

    this.diseaseStatus = ko.observable(
        new DiseaseStatus(ontologyTermId, description, otherTerms, displayOnDirectory)
    );
}

function AdacDiseaseStatusViewModel() {

    var _this = this;

    this.modalId = "#disease-status-modal";
    this.modal = new DiseaseStatusModal("", "", [], false);
    this.dialogErrors = ko.observableArray([]);

    this.showModal = function () {
        _this.dialogErrors.removeAll(); //clear errors on a new show
        $(_this.modalId).modal("show");
    };

    this.hideModal = function () {
        $(_this.modalId).modal("hide");
    };

    this.openModalForAdd = function () {
        _this.modal.mode(_this.modal.modalModeAdd);
        _this.modal.diseaseStatus(new DiseaseStatus("", "", [], false));
        _this.setPartialEdit(false);
        _this.showModal();
    };

    this.openModalForEdit = function (_, event) {

        _this.modal.mode(_this.modal.modalModeEdit);

        // Get Row Data From DataTable
        var rowIndex = $(event.currentTarget).data("row");
        var diseaseStatus = dataTable.row(rowIndex).data();

        let otherTerms = (diseaseStatus.OtherTerms
          ? diseaseStatus.OtherTerms.split(",").map(item => item.trim())
          : diseaseStatus.OtherTerms)

        _this.modal.diseaseStatus(
            new DiseaseStatus(
                diseaseStatus.OntologyTermId,
                diseaseStatus.Description,
                otherTerms,
                diseaseStatus.DisplayOnDirectory
            )
        );

        _this.setPartialEdit(diseaseStatus.CollectionCapabilityCount > 0);
        _this.showModal();
    };

    this.modalSubmit = function (e) {
        e.preventDefault();
        var form = $(e.target); // get form as a jquery object

        //Concatenate other terms (exclude null/empty/whitespace strings)
        $("#OtherTerms").val(_this.modal.diseaseStatus().otherTerms().filter(x => x && x.trim()).join(','));

        // Get Action Type
        var action = _this.modal.mode();
        if (action == 'Add') {
            addRefData(_this, form.data("resource-url"), form.serialize(),
                form.data("success-redirect"), form.data("refdata-type")); // cf. adac-refdata-utility.js
        } else if (action == 'Update') {
            editRefData(_this, form.data("resource-url") + '/' + $(e.target.Id).val(), form.serialize(),
                form.data("success-redirect"), form.data("refdata-type"));
        }
    };

    this.setPartialEdit = function (flag) {
        if (flag == true) {
            $("#OntologyTermId").prop('readonly', true);
            $("#Description").prop('readonly', true);
        }
        else {
            $("#OntologyTermId").prop('readonly', false);
            $("#Description").prop('readonly', false);
        }
    }

    this.addOtherTerms = function () {
        _this.modal.diseaseStatus().otherTerms.push("");
    }
    this.removeOtherTerms = function (idx) {
        _this.modal.diseaseStatus().otherTerms.splice(idx,1)
    }
}

// DataTables
$(function () {
    dataTable = $("#disease-statuses").DataTable({
        ajax: "./PagingatedDiseaseStatuses",
        processing: true,
        serverSide: true,
        paging: true,
        ordering: false,
        info: false,
        autoWidth: false,
        language: {
            search: "Filter: ",
        },
        columns: [
            { data: "Description" },
            { data: "OntologyTermId" },
            { data: "OtherTerms" },
            { data: "CollectionCapabilityCount" },
            { data: "DisplayOnDirectory" },
            {
                // Generate Action Links
                data: function (row, type, val, meta) {

                    // Edit Link - Binds To Knockout Modal
                    var editLink = $('<a/>', {
                        "data-row": meta.row,
                        "data-bind": "click: openModalForEdit",
                        "class": "action-icon",
                        "href": "#",
                        "html":
                            $('<span/>', {
                                class: "fa fa-edit labelled-icon",
                            })
                            .add($('<span/>', {
                                text: "Edit"
                            }))
                    });

                    if (row.CollectionCapabilityCount == 0) {

                        // Delete Link - Triggered By jQuery
                        var deleteLink = $('<a/>', {
                            "data-row": meta.row,
                            "class": "action-icon click-delete", 
                            "href": "#",
                            "html":
                                $('<span/>', {
                                    class: "fa fa-trash labelled-icon",
                                })
                                .add($('<span/>', {
                                    text: "Delete"
                                }))

                        });

                        // Convert To HTML String
                        return $('<div/>')
                            .append(editLink)
                            .append(deleteLink)
                            .html();
                    }
                    else {

                        // In-Use Edit Only
                        return $('<div/>')
                            .append(editLink)
                            .html();
                    }
                }
            }
        ],
        createdRow: function (row, data, dataIndex) {
            // Highlight In-Use Disease Statuses
            if (data.CollectionCapabilityCount > 0) {
                $(row).addClass("info");
            }

            // Bind Knockout View Model To Row
            ko.applyBindingsToDescendants(adacDiseaseStatusVM, row);
        }
    });
});

// Start-Up
$(function () {

    // Modal Submission Event Listener
    $("#modal-disease-status-form").submit(function (e) {
        adacDiseaseStatusVM.modalSubmit(e);
    });

    // Delete Row Link Listener
    $(document.body).on("click", ".click-delete", function (e) {
        e.preventDefault();

        var rowIndex = $(this).data("row")
        var data = dataTable.row(rowIndex).data();

        bootbox.confirm("Are you sure you want to delete " + data.Description + "?",
            function (confirmation) {
                if (confirmation) {
                    deleteRefData(apiUrl, redirectUrl, refdataType);
                }
            }
        );
    });

    // Knockout View Model Binding
    adacDiseaseStatusVM = new AdacDiseaseStatusViewModel();
    ko.applyBindings(adacDiseaseStatusVM);

    // jquery plugin to serialise checkboxes as bools
    (function ($) {
        $.fn.serialize = function () {
            return $.param(this.serializeArray());
        };

        $.fn.serializeArray = function () {
            var o = $.extend(
                {
                    checkboxesAsBools: true,
                },
                {}
            );

            var rselectTextarea = /select|textarea/i;
            var rinput = /text|hidden|password|search/i;

            return this.map(function () {
                return this.elements ? $.makeArray(this.elements) : this;
            })
                .filter(function () {
                    return (
                        this.name &&
                        !this.disabled &&
                        (this.checked ||
                            (o.checkboxesAsBools && this.type === "checkbox") ||
                            rselectTextarea.test(this.nodeName) ||
                            rinput.test(this.type))
                    );
                })
                .map(function (i, elem) {
                    const val = $(this).val();
                    return val == null
                        ? null
                        : $.isArray(val)
                            ? $.map(val, (innerVal) => ({ name: elem.name, value: innerVal }))
                            : {
                                name: elem.name,
                                value:
                                    o.checkboxesAsBools && this.type === "checkbox" //moar ternaries!
                                        ? this.checked
                                            ? "true"
                                            : "false"
                                        : val,
                            };
                })
                .get();
        };
    }
    )(jQuery);
});
