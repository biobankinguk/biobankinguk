function AssociatedDataTypeGroup(id, name) {
    this.id = id;
    this.name = ko.observable(name);
}

function AssociatedDataTypeGroupModal(id, name) {
    this.modalModeAdd = "Add";
    this.modalModeEdit = "Update";

    this.mode = ko.observable(this.modalModeAdd);
    this.associatedDataTypeGroup = ko.observable(
        new AssociatedDataTypeGroup(id, name)
    );
}

function AdacAssociatedDataTypeGroupViewModel() {
    var _this = this;

    this.modalId = "#associated-groups-modal";
    this.modal = new AssociatedDataTypeGroupModal(0, "");
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
        _this.modal.associatedDataTypeGroup(new AssociatedDataTypeGroup(0, ""));
        _this.showModal();
    };

    this.openModalForEdit = function (_, event) {
        _this.modal.mode(_this.modal.modalModeEdit);

        var associatedDataTypeGroup = $(event.currentTarget).data("associated-groups");
        _this.modal.associatedDataTypeGroup(
            new AssociatedDataTypeGroup(
                associatedDataTypeGroup.AssociatedDataTypeGroupId,
                associatedDataTypeGroup.Name

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
            var url = resourceUrl + '/' + $(e.target.AssociatedDataTypeGroupId).val();
        }
        var successRedirect = action.toLowerCase() + "-success-redirect";

        // Make AJAX Call
        $.ajax({
            url: url,
            type: ajaxType,
            dataType: 'json',
            data: form.serialize(),
            success: function (data, textStatus, xhr) {
                _this.dialogErrors.removeAll();
                _this.hideModal();
                window.location.href =
                    form.data(successRedirect) + "?Name=" + data.name;
            },
            error: function (data, xhr, textStatus, errorThrown) {
                _this.dialogErrors.removeAll();
                if (Array.isArray(data.errors)) {
                    for (var error of data.errors) {
                        _this.dialogErrors.push(error);
                    }
                }
            }
        });
    };
}

var adacAssociatedDataTypeGroupVM;

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

    $("#associated-groups")["DataTable"]({
        columnDefs: [
            { orderable: false, targets: 2 },
            { width: "250px", targets: 2 },
        ],
        paging: false,
        info: false,
        autoWidth: false,
        language: {
            search: "Filter: ",
        },
    });

    $("#modal-associated-groups-form").submit(function (e) {
        adacAssociatedDataTypeGroupVM.modalSubmit(e);
    });

    $(".delete-confirm").click(function (e) {
        e.preventDefault();
        var $link = $(this);
        bootbox.confirm(
            "Are you sure you want to delete " + $link.data("associated-groups") + "?",
            function (confirmation) {
                confirmation && window.location.assign($link.attr("href"));
            }
        );
    });

    adacAssociatedDataTypeGroupVM = new AdacAssociatedDataTypeGroupViewModel();

    ko.applyBindings(adacAssociatedDataTypeGroupVM);
});
