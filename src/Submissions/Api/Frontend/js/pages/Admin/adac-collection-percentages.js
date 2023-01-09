// Modals
var adacCollectionPercentageVM;

function CollectionPercentage(id, description, sortOrder, lowerBound, upperBound) {
    this.id = id;
    this.description = ko.observable(description);
    this.sortOrder = sortOrder;
    this.lowerBound = ko.observable(lowerBound);
    this.upperBound = ko.observable(upperBound);
}

function CollectionPercentageModal(id, description, sortOrder, lowerBound, upperBound) {
    this.modalModeAdd = "Add";
    this.modalModeEdit = "Update";

    this.mode = ko.observable(this.modalModeAdd);

    this.collectionPercentage = ko.observable(
        new CollectionPercentage(id, description, sortOrder, lowerBound, upperBound)
    );
}

function AdacCollectionPercentageViewModel() {
    var _this = this;

    this.modalId = "#collection-percentages-modal";
    this.modal = new CollectionPercentageModal(0, "", 0, 0, 0);
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
        _this.modal.collectionPercentage(new CollectionPercentage(0, "", 0, 0, 0));
        _this.showModal();
    };

    this.openModalForEdit = function (_, event) {
        _this.modal.mode(_this.modal.modalModeEdit);

        var collectionPercentage = $(event.currentTarget).data("collection-percentage");

        _this.modal.collectionPercentage(
            new CollectionPercentage(
                collectionPercentage.Id,
                collectionPercentage.Description,
                collectionPercentage.SortOrder,
                collectionPercentage.LowerBound,
                collectionPercentage.UpperBound
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
    $("#modal-collection-percentages-form").submit(function (e) {
        adacCollectionPercentageVM.modalSubmit(e);
    });

    $(".delete-confirm").click(function (e) {
        e.preventDefault();

        var $link = $(this);
        var linkData = $link.data("refdata-model")
        var url = $link.data("resource-url") + "/" + linkData.Id;

        bootbox.confirm("Are you sure you want to delete " + linkData.Description + "?",
            function (confirmation) {
                if (confirmation) {
                    deleteRefData(url, $link.data("success-redirect"), $link.data("refdata-type"));
                }
            }
        );
    });

    adacCollectionPercentageVM = new AdacCollectionPercentageViewModel();
    ko.applyBindings(adacCollectionPercentageVM);
});

// DataTables
$(function () {
    var table = $("#collection-percentages")["DataTable"]({
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
        var triggerRow = diff.filter(function (row) { return row.node == edit.triggerRow.node();})[0];

        //AJAX Update
        $.ajax({
            url: $(triggerRow.node).data('resource-url') +
                "/" + $(triggerRow.node).data('collection-percentage-id') + "/move",
            type: 'POST',
            dataType: 'json',
            data: {
                id: $(triggerRow.node).data('collection-percentage-id'),
                description: $(triggerRow.node).data('collection-percentage-desc'),
                sortOrder: (triggerRow.newPosition + 1), //1-indexable
                lowerBound: $(triggerRow.node).data('collection-percentage-lower-bound'),
                upperBound: $(triggerRow.node).data('collection-percentage-upper-bound'),
            }
        });
    });
});
