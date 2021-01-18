// Modals
var adacStorageTemperatureVM;

function StorageTemperature(id, value, sortOrder) {
    this.id = id;
    this.value = ko.observable(value);
    this.sortOrder = sortOrder;
}

function StorageTemperatureModal(id, value, sortOrder) {
    this.modalModeAdd = "Add";
    this.modalModeEdit = "Update";

    this.mode = ko.observable(this.modalModeAdd);

    this.storageTemperature = ko.observable(
        new StorageTemperature(id, value, sortOrder)
    );
}

function AdacStorageTemperatureViewModel() {
    var _this = this;

    this.modalId = "#storage-temperatures-modal";
    this.modal = new StorageTemperatureModal(0, "", 0);
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
        _this.modal.storageTemperature(new StorageTemperature(0, "", 0));
        _this.showModal();
    };

    this.openModalForEdit = function (_, event) {
        _this.modal.mode(_this.modal.modalModeEdit);

        var storageTemperature = $(event.currentTarget).data("storage-temperature");

        _this.modal.storageTemperature(
            new StorageTemperature(
                storageTemperature.Id,
                storageTemperature.Value,
                storageTemperature.SortOrder
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
                if (data.success) {
                    _this.hideModal();
                    window.location.href =
                        form.data(successRedirect) + "?Name=" + data.name;
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
    $("#modal-storage-temperatures-form").submit(function (e) {
        adacStorageTemperatureVM.modalSubmit(e);
    });

    $(".delete-confirm").click(function (e) {
        e.preventDefault();

        var $link = $(this);

        bootbox.confirm("Are you sure you want to delete " + $link .data("storage-temperature") + "?",
            function (confirmation) {
                confirmation && window.location.assign($link.attr("href"));
            }
        );
    });

    adacStorageTemperatureVM = new AdacStorageTemperatureViewModel();
    ko.applyBindings(adacStorageTemperatureVM);
});

$(function () {
    $("#change-storage-temperature-name-form").submit(function (e) {
        e.preventDefault();
        $.ajax({
            type: "POST",
            url: "UpdateReferenceTermName",
            data: { newReferenceTermKey: "site.display.preservation.name", newReferenceTermName: document.getElementById("StorageTemperatureName").value},
            dataType: "json",
            success: function () {
                location.reload();
            }
        });
    });

    $("#storageTemperatureTitle").click(function () {
        document.getElementById("titleName").setAttribute("hidden", true);
        document.getElementById("change-storage-temperature-name-form").removeAttribute("hidden");

    });

    $("#storageTemperatureTitleCancel").click(function () {
        document.getElementById("change-storage-temperature-name-form").setAttribute("hidden", true);
        document.getElementById("titleName").removeAttribute("hidden");

    });

});


// DataTables
$(function () {
    var table = $("#storage-temperatures")["DataTable"]({
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
        var $row = $(triggerRow.node);

        //AJAX Update
        $.ajax({
            url: $row.data('resource-url') + "/" + $row.data('id') + "/move",
            type: 'POST',
            dataType: 'json',
            data: {
                id: $row.data('id'),
                value: $row.data('value'),
                sortOrder: (triggerRow.newPosition + 1) //1-indexable
            }
        });
    });
});
