// Modals
var adacAgeRangeVM;

function AgeRange(id, description, sortOrder, lowerBound, upperBound, lowerDuration, upperDuration) {
    this.id = id;
    this.description = ko.observable(description);
    this.sortOrder = sortOrder;
    this.lowerBound = ko.observable(lowerBound);
    this.upperBound = ko.observable(upperBound);
    this.lowerDuration = ko.observable(lowerDuration);
    this.upperDuration = ko.observable(upperDuration);
}

function AgeRangeModal(id, description, sortOrder, lowerBound, upperBound, lowerDuration, upperDuration) {
    this.modalModeAdd = "Add";
    this.modalModeEdit = "Update";

    this.mode = ko.observable(this.modalModeAdd);

    this.ageRange = ko.observable(
        new AgeRange(id, description, sortOrder, lowerBound, upperBound, lowerDuration, upperDuration)
    );
}

function AdacAgeRangeViewModel() {
    var _this = this;

    this.modalId = "#age-ranges-modal";
    this.modal = new AgeRangeModal(0, "", 0, 0, 0);
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
        _this.modal.ageRange(new AgeRange(0, "", 0, 0, 0));
        _this.showModal();
    };

    this.openModalForEdit = function (_, event) {
        _this.modal.mode(_this.modal.modalModeEdit);

        var ageRange = $(event.currentTarget).data("age-range");

        // Selected value in dropdown
        var lowerDuration = ageRange.LowerBound.replace(/[^a-zA-Z]+/g, '');
        document.getElementById("lowerDuration").value = lowerDuration.charAt(0);

        var upperDuration = ageRange.UpperBound.replace(/[^a-zA-Z]+/g, '');
        document.getElementById("upperDuration").value = upperDuration.charAt(0);

        _this.modal.ageRange(
            new AgeRange(
                ageRange.Id,
                ageRange.Description,
                ageRange.SortOrder,
                ageRange.LowerBound.replace(/[a-z]/gi, '').replace(/\s/g, ""),
                ageRange.UpperBound.replace(/[a-z]/gi, '').replace(/\s/g, ""),
                ageRange.LowerDuration,
                ageRange.UpperDuration
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
    $("#modal-age-ranges-form").submit(function (e) {
        adacAgeRangeVM.modalSubmit(e);
    });

    $(".delete-confirm").click(function (e) {
        e.preventDefault();

        var $link = $(this);
        var linkData = $link.data("age-range")
        var url = $link.data("resource-url") + "/" + linkData.Id;

        bootbox.confirm("Are you sure you want to delete " + linkData.Description + "?",
            function (confirmation) {
                if (confirmation) {
                    deleteRefData(url, $link.data("success-redirect"), $link.data("refdata-type"));
                }
            }
        );
    });

    adacAgeRangeVM = new AdacAgeRangeViewModel();
    ko.applyBindings(adacAgeRangeVM);
});


function checkBound(obj, id) {
    var upperInput = document.getElementById(id);
    upperInput.disabled = obj.value == "N/A";
}

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
        $.ajax({
            url: $(triggerRow.node).data('resource-url') +
                "/" + $(triggerRow.node).data('age-range-id') + "/move",
            type: 'POST',
            dataType: 'json',
            data: {
                id: $(triggerRow.node).data('age-range-id'),
                description: $(triggerRow.node).data('age-range-desc'),
                sortOrder: (triggerRow.newPosition + 1), //1-indexable,
                lowerBound: $(triggerRow.node).data('age-range-lowerbound'),
                upperBound: $(triggerRow.node).data('age-range-upperbound')
            }
        });
    });
});
