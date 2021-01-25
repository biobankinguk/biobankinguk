function ServiceOffering(id, name, sortOrder) {
    this.id = ko.observable(id);
    this.name = ko.observable(name);
    this.sortOrder = ko.observable(sortOrder)
}
function ServiceOfferingModal(id, name, sortOrder) {
    this.modalModeAdd = "Add";
    this.modalModeEdit = "Update";
    this.mode = ko.observable(this.modalModeAdd);
    this.serviceOffering = ko.observable(
        new ServiceOffering(id, name, sortOrder)
    );
}
function AdacServiceOfferingViewModel() {
    var _this = this;

    this.modalId = "#service-offering-modal";
    this.modal = new ServiceOfferingModal(0, "", 0);
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
        _this.modal.serviceOffering(new ServiceOffering(0, "", 0));
        _this.showModal();
    };

    this.openModalForEdit = function (_, event) {
        _this.modal.mode(_this.modal.modalModeEdit);

        var serviceOffering = $(event.currentTarget).data("service-offering");
        _this.modal.serviceOffering(
            new ServiceOffering(
                serviceOffering.Id,
                serviceOffering.Name,
                serviceOffering.SortOrder
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

var adacServiceOfferingVM;
$(function () {
    //jquery plugin to serialise checkboxes as bools
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
    })(jQuery);

    $(function () {
        var table = $("#service-offering")["DataTable"]({
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
                    "/" + $(triggerRow.node).data('offering-id') + "/move",
                type: 'POST',
                dataType: 'json',
                data: {
                    id: $(triggerRow.node).data('offering-id'),
                    name: $(triggerRow.node).data('offering-name'),
                    sortOrder: (triggerRow.newPosition + 1) //1-indexable
                }
            });
        });
    });

    $("#modal-service-offering-form").submit(function (e) {
        adacServiceOfferingVM.modalSubmit(e);
    });

    $(".delete-confirm").click(function (e) {
        e.preventDefault();

        var $link = $(this);
        var linkData = $link.data("refdata-model")
        var url = $link.data("resource-url") + "/" + linkData.Id;

        bootbox.confirm("Are you sure you want to delete " + linkData.Name + "?",
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

    adacServiceOfferingVM = new AdacServiceOfferingViewModel();

    ko.applyBindings(adacServiceOfferingVM);
});
