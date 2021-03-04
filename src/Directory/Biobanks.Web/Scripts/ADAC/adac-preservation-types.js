// Modals
var adacPreservationTypeVM;

function PreservationType(id, name, storageTemperatureId) {
    this.id = id;
    this.name = ko.observable(name);
    this.storageTemperatureId = ko.observable(storageTemperatureId);
}

function PreservationTypeModal(id, name, storageTemperatureId, storageTemperatures) {
    this.modalModeAdd = "Add";
    this.modalModeEdit = "Update";

    this.mode = ko.observable(this.modalModeAdd);

    this.preservationType = ko.observable(
        new PreservationType(id, name, storageTemperatureId)
    );

    this.storageTemperatures = ko.observableArray(storageTemperatures);
}

function AdacPreservationTypeViewModel() {
    var _this = this;
    this.modalId = "#preservation-types-modal";

    this.storageTemperatures = $(this.modalId).data("storageTemperatures");

    this.modal = new PreservationTypeModal(0, "", 0, this.storageTemperatures);
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
                preservationType.Value,
                preservationType.StorageTemperatureId
            )
        );

        _this.showModal();
    };

    this.modalSubmit = function (e) {
        e.preventDefault();
        var form = $(e.target); // get form as a jquery object

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

        bootbox.confirm("Are you sure you want to delete " + linkData.Name + "?",
            function (confirmation) {
                if (confirmation) {
                    deleteRefData(url, $link.data("success-redirect"), $link.data("refdata-type"));
                }
            }
        );
    });

    adacPreservationTypeVM = new AdacPreservationTypeViewModel();
    ko.applyBindings(adacPreservationTypeVM);
});

// DataTables
$(function () {
    var table = $("#preservation-types")["DataTable"]({
        paging: false,
        info: false,
        autoWidth: false,
        language: {
            search: "Filter: ",
        },
        columnDefs: [
            { orderable: true, targets: 0 },
            { orderable: true, targets: 1 },
            { orderable: false, targets: '_all' }
        ]
    });
});
