// Modals
var adacAccessConditionVM;

function AccessCondition(id, description, sortOrder) {
    this.id = id;
    this.description = ko.observable(description);
    this.sortOrder = sortOrder;
}

function AccessConditionModal(id, description, sortOrder) {
    this.modalModeAdd = "Add";
    this.modalModeEdit = "Update";

    this.mode = ko.observable(this.modalModeAdd);

    this.accessCondition = ko.observable(
        new AccessCondition(id, description, sortOrder)
    );
}

function AdacAccessConditionViewModel() {
    var _this = this;

    this.modalId = "#access-conditions-modal";
    this.modal = new AccessConditionModal(0, "", 0);
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
        _this.modal.accessCondition(new AccessCondition(0, "", 0));
        _this.showModal();
    };

    this.openModalForEdit = function (_, event) {
        _this.modal.mode(_this.modal.modalModeEdit);

        var accessCondition = $(event.currentTarget).data("access-condition");

        _this.modal.accessCondition(
            new AccessCondition(
                accessCondition.Id,
                accessCondition.Description,
                accessCondition.SortOrder
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
        } else if (action == 'Update') {
            var ajaxType = 'PUT';
            var url = resourceUrl + '/' + $(e.target.Id).val();
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
                    _this.setFeedback(data.name,
                        form.data("success-redirect"), form.data("refdata-type"), action.toLowerCase())
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

    this.setFeedback = function (name, redirectTo, refdata, action) {
        // Send Feedback message
        var url = "/ADAC/SetRefDataSuccessFeedbackAjax";
        var data = {
            name: name,
            redirectUrl: redirectTo,
            refDataType: refdata,
            CRUDAction: action,
        };
        window.location.href = url + "?" + $.param(data);
    }
}

$(function () {
    $("#modal-access-conditions-form").submit(function (e) {
        adacAccessConditionVM.modalSubmit(e);
    });

    $(".delete-confirm").click(function (e) {
        e.preventDefault();

        var $link = $(this);
        var linkData = $link.data("access-condition")
        var url = $link.data("resource-url") + "/" + linkData.Id;

        bootbox.confirm("Are you sure you want to delete " + linkData.Description + "?",
            function (confirmation) {
                //confirmation && window.location.assign($link.attr("href"));
                if (confirmation) {
                    // Make AJAX Call
                    $.ajax({
                        url: url,
                        type: 'DELETE',
                        success: function (data, textStatus, xhr) {
                            if (data.success) {
                                adacAccessConditionVM.setFeedback(data.name,
                                    $link.data("success-redirect"), $link.data("refdata-type"), "delete")
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

    adacAccessConditionVM = new AdacAccessConditionViewModel();
    ko.applyBindings(adacAccessConditionVM);
});

// DataTables
$(function () {
    var table = $("#access-conditions")["DataTable"]({
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
                "/" + $(triggerRow.node).data('access-condition-id') + "/move",
            type: 'POST',
            dataType: 'json',
            data: {
                id: $(triggerRow.node).data('access-condition-id'),
                description: $(triggerRow.node).data('access-condition-desc'),
                sortOrder: (triggerRow.newPosition + 1) //1-indexable
            }
        });
    });
});
