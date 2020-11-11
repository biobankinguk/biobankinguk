function AssociatedDataProcurementTimeFrame(id, description, displayName, sortOrder) {
    this.id = ko.observable(id);
    this.description = ko.observable(description);
    this.displayName = ko.observable(displayName);
    this.sortOrder = ko.observable(sortOrder)
}
function AssociatedDataProcurementTimeFrameModal(id, description, displayName, sortOrder) {
    this.modalModeAdd = "Add";
    this.modalModeEdit = "Edit";
    this.mode = ko.observable(this.modalModeAdd);
    this.associatedDataProcurementTimeFrame = ko.observable(
        new AssociatedDataProcurementTimeFrame(id, description, displayName, sortOrder)
    );
}
function AdacAssociatedDataProcurementTimeFrameViewModel() {
    var _this = this;

    this.modalId = "#procurement-time-modal";

    this.modal = new AssociatedDataProcurementTimeFrameModal(0, "", "", 0);
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
        _this.modal.associatedDataProcurementTimeFrame(new AssociatedDataProcurementTimeFrame(0, "", "", 0));
        _this.showModal();
    };

    this.openModalForEdit = function (_, event) {
        _this.modal.mode(_this.modal.modalModeEdit);

        var associatedDataProcurementTimeFrame = $(event.currentTarget).data("procurement-time");

        _this.modal.associatedDataProcurementTimeFrame(
            new AssociatedDataProcurementTimeFrame(
                associatedDataProcurementTimeFrame.Id,
                associatedDataProcurementTimeFrame.Description,
                associatedDataProcurementTimeFrame.DisplayName,
                associatedDataProcurementTimeFrame.SortOrder
            )
        );

        _this.showModal();
    };

    this.modalSubmit = function (e) {
        e.preventDefault();

        // Get Action Type
        var action = _this.modal.mode().toLowerCase();
        var url = `/api/AssociatedDataProcurementTimeFrame/${action}AssociatedDataProcurementTimeFrameAjax`;

        console.log($(e.target).serialize());

        // Make AJAX Call
        $.post(url, $(e.target).serialize(), function (data) {

            // Clear any previous errors
            _this.dialogErrors.removeAll();

            if (data.success) {
                _this.hideModal();
                if (data.msg != null) {
                    window.feedbackMessage(data.msg, data.type);
                }
                else {
                    window.location.replace(data.redirect);
                }
                
            }
            else {
                if (Array.isArray(data.errors)) {
                    for (var error of data.errors) {
                        _this.dialogErrors.push(error);
                    }
                }
            }
        });
    };
}


var adacAssociatedDataProcurementTimeFrameVM;
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
        var table = $("#procurement-time")["DataTable"]({
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
            $.post("/api/AssociatedDataProcurementTimeFrame/EditAssociatedDataProcurementTimeFrameAjax?sortOnly=true",
                {
                    id: $(triggerRow.node).data('procurement-id'),
                    description: $(triggerRow.node).data('procurement-desc'),
                    sortOrder: (triggerRow.newPosition + 1) //1-indexable
                });
        });
    });

    $("#modal-procurement-time-form").submit(function (e) {
        adacAssociatedDataProcurementTimeFrameVM.modalSubmit(e);
    });

    $(".delete-confirm").click(function (e) {
        e.preventDefault();
        var $link = $(this);
        bootbox.confirm(
            "Are you sure you want to delete " + $link.data("procurement-time") + "?",
            function (confirmation) {
                confirmation && window.location.assign($link.attr("href"));
            }
        );
    });

    adacAssociatedDataProcurementTimeFrameVM = new AdacAssociatedDataProcurementTimeFrameViewModel();

    ko.applyBindings(adacAssociatedDataProcurementTimeFrameVM);
});
