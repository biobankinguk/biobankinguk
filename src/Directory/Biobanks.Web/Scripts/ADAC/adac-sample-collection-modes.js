// Modals
var adacCollectionModeVM;

function CollectionMode(id, description, sortOrder) {
    this.id = id;
    this.description = ko.observable(description);
    this.sortOrder = sortOrder;
}

function CollectionModeModal(id, description, sortOrder) {
    this.modalModeAdd = "Add";
    this.modalModeEdit = "Edit";

    this.mode = ko.observable(this.modalModeAdd);

    this.collectionMode = ko.observable(
        new CollectionMode(id, description, sortOrder)
    );
}

function AdacCollectionModeViewModel() {
    var _this = this;

    this.modalId = "#collection-modes-modal";
    this.modal = new CollectionModeModal(0, "", 0);
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
        _this.modal.collectionMode(new CollectionMode(0, "", 0));
        _this.showModal();
    };

    this.openModalForEdit = function (_, event) {
        _this.modal.mode(_this.modal.modalModeEdit);

        var collectionMode = $(event.currentTarget).data("collection-mode");

        _this.modal.collectionMode(
            new CollectionMode(
                collectionMode.Id,
                collectionMode.Description,
                collectionMode.SortOrder
            )
        );

        _this.showModal();
    };

    this.modalSubmit = function (e) {
        e.preventDefault();

        // Get Action Type
        var action = _this.modal.mode().toLowerCase();
        var url = `/api/SampleCollectionModes/${action}SampleCollectionModeAjax`;

        console.log($(e.target).serialize());

        // Make AJAX Call
        $.post(url, $(e.target).serialize(), function (data) {

            // Clear any previous errors
            _this.dialogErrors.removeAll();

            if (data.success) {
                _this.hideModal();
                window.location.replace(data.redirect);
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

$(function () {
    $("#modal-collection-modes-form").submit(function (e) {
        adacCollectionModeVM.modalSubmit(e);
    });

    $(".delete-confirm").click(function (e) {
        e.preventDefault();

        var $link = $(this);

        bootbox.confirm("Are you sure you want to delete " + $link .data("collection-mode") + "?",
            function (confirmation) {
                confirmation && window.location.assign($link.attr("href"));
            }
        );
    });

    adacCollectionModeVM = new AdacCollectionModeViewModel();
    ko.applyBindings(adacCollectionModeVM);
});

// DataTables
$(function () {
    var table = $("#collection-modes")["DataTable"]({
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
        $.post("/api/SampleCollectionModes/EditSampleCollectionModeAjax?sortOnly=true",
            {
                id: $(triggerRow.node).data('collection-mode-id'),
                description: $(triggerRow.node).data('collection-mode-desc'),
                sortOrder: (triggerRow.newPosition + 1) //1-indexable
            });
    });
});
