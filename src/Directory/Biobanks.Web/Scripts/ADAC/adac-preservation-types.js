// Modals
var adacPreservationTypeVM;

function PreservationType(id, description, sortOrder) {
    this.id = id;
    this.description = ko.observable(description);
    this.sortOrder = sortOrder;
}

function PreservationTypeModal(id, description, sortOrder) {
    this.modalModeAdd = "Add";
    this.modalModeEdit = "Update";

    this.mode = ko.observable(this.modalModeAdd);

    this.preservationType = ko.observable(
        new PreservationType(id, description, sortOrder)
    );
}

function AdacPreservationTypeViewModel() {
    var _this = this;

    this.modalId = "#preservation-types-modal";
    this.modal = new PreservationTypeModal(0, "", 0);
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
        _this.modal.preservationType(new PreservationType(0, "", 0));
        _this.showModal();
    };

    this.openModalForEdit = function (_, event) {
        _this.modal.mode(_this.modal.modalModeEdit);

        var preservationType = $(event.currentTarget).data("preservation-type");

        _this.modal.preservationType(
            new PreservationType(
                preservationType.Id,
                preservationType.Description,
                preservationType.SortOrder
            )
        );

        _this.showModal();
    };

    this.modalSubmit = function (e) {
        e.preventDefault();
        var form = $(e.target); // get form as a jquery object

        // Get Action Type
        var resourceUrl = form.data("resource-url")
        var action = _this.modal.mode();
        if (action == 'Add') {
            var ajaxType = 'POST'
            var url = resourceUrl;
            var feedbackfn = setAddFeedback; // cf. adac-refdata-feedback.js
        } else if (action == 'Update') {
            var ajaxType = 'PUT';
            var url = resourceUrl + '/' + $(e.target.Id).val();
            var feedbackfn = setEditFeedback; // cf. adac-refdata-feedback.js
        }

        // Make AJAX Call
        $.ajax({
            url: url,
            type: ajaxType,
            dataType: 'json',
            data: form.serialize(),
            success: function (data, textStatus, xhr) {
                _this.dialogErrors.removeAll();
                if (data.success) {
                    _this.hideModal();
                    feedbackfn(data.name,
                        form.data("success-redirect"), form.data("refdata-type"));
                }
                else {
                    if (Array.isArray(data.errors)) {
                        for (var error of data.errors) {
                            _this.dialogErrors.push(error);
                        }
                    }
                }
            }
        });
    };
}

$(function () {
    $("#modal-preservation-types-form").submit(function (e) {
        adacPreservationTypeVM.modalSubmit(e);
    });

    $(".delete-confirm").click(function (e) {
        e.preventDefault();

        var $link = $(this);
        var linkData = $link.data("refdata-model")
        var url = $link.data("resource-url") + "/" + linkData.Id;

        bootbox.confirm("Are you sure you want to delete " + linkData.Description + "?",
            function (confirmation) {
                if (confirmation) {
                    // Make AJAX Call
                    $.ajax({
                        url: url,
                        type: 'DELETE',
                        success: function (data, textStatus, xhr) {
                            if (data.success) {
                                setDeleteFeedback(data.name,
                                    $link.data("success-redirect"), $link.data("refdata-type"))
                            }
                            else {
                                if (Array.isArray(data.errors)) {
                                    if (data.errors.length > 0) {
                                        window.feedbackMessage(data.errors[0], "warning");
                                    }
                                }
                            }
                        }
                    });
                }
            }
        );
    });

    adacPreservationTypeVM = new AdacPreservationTypeViewModel();
    ko.applyBindings(adacPreservationTypeVM);
});

$(function () {
    $("#change-preservation-type-name-form").submit(function (e) {
        e.preventDefault();
        $.ajax({
            type: "POST",
            url: "UpdateReferenceTermName",
            data: { newReferenceTermKey: "site.display.preservation.name", newReferenceTermName: document.getElementById("PreservationTypeName").value},
            dataType: "json",
            success: function () {
                location.reload();
            }
        });
    });

    $("#preservationTypeTitle").click(function () {
        document.getElementById("titleName").setAttribute("hidden", true);
        document.getElementById("change-preservation-type-name-form").removeAttribute("hidden");

    });

    $("#preservationTypeTitleCancel").click(function () {
        document.getElementById("change-preservation-type-name-form").setAttribute("hidden", true);
        document.getElementById("titleName").removeAttribute("hidden");

    });

});


// DataTables
$(function () {
    var table = $("#preservation-types")["DataTable"]({
        paging: false,
        info: false,
        autoWidth: false,
        rowReorder: true,
        columnDefs: [
            { orderable: true, "visible": false, className: 'reorder', targets: 0 }, // Column must be orderable for rowReorder
            { orderable: false, targets: '_all' }
        ],
        language: {
            search: "Filter: ",
        },
    });

    // Re-Order Event
    table.on('row-reorder', function (e, diff, edit) {

        // Find the row that was moved
        var triggerRow = diff.filter(row => row.node == edit.triggerRow.node())[0];

        //AJAX Update
        $.ajax({
            url: $(triggerRow.node).data('resource-url') +
                "/" + $(triggerRow.node).data('preservation-id') + "/move",
            type: 'POST',
            dataType: 'json',
            data: {
                id: $(triggerRow.node).data('preservation-id'),
                description: $(triggerRow.node).data('preservation-desc'),
                sortOrder: (triggerRow.newPosition + 1) //1-indexable
            }
        });
    });
});
