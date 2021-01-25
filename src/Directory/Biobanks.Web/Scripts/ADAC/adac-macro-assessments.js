// Modals
var adacMacroAssessmentVM;

function MacroAssessment(id, description, sortOrder) {
    this.id = id;
    this.description = ko.observable(description);
    this.sortOrder = sortOrder;
}

function MacroAssessmentModal(id, description, sortOrder) {
    this.modalModeAdd = "Add";
    this.modalModeEdit = "Update";

    this.mode = ko.observable(this.modalModeAdd);

    this.macroAssessment = ko.observable(
        new MacroAssessment(id, description, sortOrder)
    );
}

function AdacMacroAssessmentViewModel() {
    var _this = this;

    this.modalId = "#macro-assessments-modal";
    this.modal = new MacroAssessmentModal(0, "", 0);
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
        _this.modal.macroAssessment(new MacroAssessment(0, "", 0));
        _this.showModal();
    };

    this.openModalForEdit = function (_, event) {
        _this.modal.mode(_this.modal.modalModeEdit);

        var macroAssessment = $(event.currentTarget).data("macro-assessment");

        _this.modal.macroAssessment(
            new MacroAssessment(
                macroAssessment.Id,
                macroAssessment.Description,
                macroAssessment.SortOrder
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
    $("#modal-macro-assessments-form").submit(function (e) {
        adacMacroAssessmentVM.modalSubmit(e);
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

    adacMacroAssessmentVM = new AdacMacroAssessmentViewModel();
    ko.applyBindings(adacMacroAssessmentVM);
});

$(function () {
    $("#change-macroscopic-assessment-name-form").submit(function (e) {
        e.preventDefault();
        $.ajax({
            type: "POST",
            url: "UpdateReferenceTermName",
            data: { newReferenceTermKey: "site.display.macroscopicassessment.name", newReferenceTermName: document.getElementById("MacroscopicAssessmentName").value },
            dataType: "json",
            success: function () {
                location.reload();
            }
        });
    });

    $("#macroscopicAssessmentTitle").click(function () {
        document.getElementById("titleName").setAttribute("hidden", true);
        document.getElementById("change-macroscopic-assessment-name-form").removeAttribute("hidden");

    });

    $("#macroscopicAssessmentTitleCancel").click(function () {
        document.getElementById("change-macroscopic-assessment-name-form").setAttribute("hidden", true);
        document.getElementById("titleName").removeAttribute("hidden");

    });

});

// DataTables
$(function () {
    var table = $("#macro-assessments")["DataTable"]({
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
                "/" + $(triggerRow.node).data('macro-assessment-id') + "/move",
            type: 'POST',
            dataType: 'json',
            data: {
                id: $(triggerRow.node).data('macro-assessment-id'),
                description: $(triggerRow.node).data('macro-assessment-desc'),
                sortOrder: (triggerRow.newPosition + 1) //1-indexable
            }
        });
    });
});
