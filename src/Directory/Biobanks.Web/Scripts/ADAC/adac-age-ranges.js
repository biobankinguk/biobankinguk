// Modals
var adacAgeRangeVM;

function AgeRange(id, description, sortOrder) {
    this.id = id;
    this.description = ko.observable(description);
    this.sortOrder = sortOrder;
}

function AgeRangeModal(id, description, sortOrder) {
    this.modalModeAdd = "Add";
    this.modalModeEdit = "Edit";

    this.mode = ko.observable(this.modalModeAdd);

    this.ageRange = ko.observable(
        new AgeRange(id, description, sortOrder)
    );
}

function AdacAgeRangeViewModel() {
    var _this = this;

    this.modalId = "#age-ranges-modal";
    this.modal = new AgeRangeModal(0, "", 0);
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
        _this.modal.ageRange(new AgeRange(0, "", 0));
        _this.showModal();
    };

    this.openModalForEdit = function (_, event) {
        _this.modal.mode(_this.modal.modalModeEdit);

        var ageRange = $(event.currentTarget).data("age-range");

        _this.modal.ageRange(
            new AgeRange(
                ageRange.Id,
                ageRange.Description,
                ageRange.SortOrder
            )
        );

        _this.showModal();
    };

    this.modalSubmit = function (e) {
        e.preventDefault();

        // Get Action Type
        var action = _this.modal.mode().toLowerCase();
        var url = `${action}AgeRangeAjax`;

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
    $("#modal-age-ranges-form").submit(function (e) {
        adacAgeRangeVM.modalSubmit(e);
    });

    $(".delete-confirm").click(function (e) {
        e.preventDefault();

        var $link = $(this);

        bootbox.confirm("Are you sure you want to delete " + $link .data("age-range") + "?",
            function (confirmation) {
                confirmation && window.location.assign($link.attr("href"));
            }
        );
    });

    adacAgeRangeVM = new AdacAgeRangeViewModel();
    ko.applyBindings(adacAgeRangeVM);
});

// DataTables
$(function () {
    var table = $("#age-ranges")["DataTable"]({
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
        $.post("EditAgeRangeAjax?sortOnly=true",
            {
                id: $(triggerRow.node).data('age-range-id'),
                description: $(triggerRow.node).data('age-range-desc'),
                sortOrder: (triggerRow.newPosition + 1) //1-indexable
            });
    });
});
