// Modals
var adacAssociatedDataTypeVM;

function AssociatedDataType(id, name, message, groupId) {
    this.id = id;
    this.name = ko.observable(name);
    this.message = ko.observable(message);
    this.groupId = ko.observable(groupId);
}

function AssociatedDataTypeModal(id, name, message, groupId, groups) {
    this.modalModeAdd = "Add";
    this.modalModeEdit = "Update";

    this.mode = ko.observable(this.modalModeAdd);

    this.associatedDataType = ko.observable(
        new AssociatedDataType(id, name, message, groupId)
    );

    this.groups = ko.observableArray(groups);
}

function AdacAssociatedDataTypeViewModel() {
    var _this = this;
    this.modalId = "#associated-types-modal";

    this.groups = $(this.modalId).data("groups");

    this.modal = new AssociatedDataTypeModal(0, "","", 0, this.groups);
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
        _this.modal.associatedDataType(new AssociatedDataType(0, "", "", 0, this.groups));
        _this.showModal();
    };

    this.openModalForEdit = function (_, event) {
        _this.modal.mode(_this.modal.modalModeEdit);

        var associatedDataType = $(event.currentTarget).data("associated-types");

        _this.modal.associatedDataType(
            new AssociatedDataType(
                associatedDataType.Id,
                associatedDataType.Name,
                associatedDataType.Message,
                associatedDataType.AssociatedDataTypeGroupId,
                this.groups
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

$(function () {
    $("#modal-associated-types-form").submit(function (e) {
        adacAssociatedDataTypeVM.modalSubmit(e);
    });

    $(".delete-confirm").click(function (e) {
        e.preventDefault();

        var $link = $(this);

        bootbox.confirm("Are you sure you want to delete " + $link.data("associated-types") + "?",
            function (confirmation) {
                confirmation && window.location.assign($link.attr("href"));
            }
        );
    });

    adacAssociatedDataTypeVM = new AdacAssociatedDataTypeViewModel();
    ko.applyBindings(adacAssociatedDataTypeVM);
});
