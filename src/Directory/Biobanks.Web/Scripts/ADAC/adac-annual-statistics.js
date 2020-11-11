// Modals
var adacAnnualStatisticVM;

function AnnualStatistic(id, name, groupId) {
    this.id = id;
    this.name = ko.observable(name);
    this.groupId = ko.observable(groupId);
}

function AnnualStatisticModal(id, name, groupId, groups) {
    this.modalModeAdd = "Add";
    this.modalModeEdit = "Edit";

    this.mode = ko.observable(this.modalModeAdd);

    this.annualStatistic = ko.observable(
        new AnnualStatistic(id, name, groupId)
    );

    this.groups = ko.observableArray(groups);
}

function AdacAnnualStatisticViewModel() {
    var _this = this;
    this.modalId = "#annual-stats-modal";

    this.groups = $(this.modalId).data("groups");

    this.modal = new AnnualStatisticModal(0, "", 0, this.groups);
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
        _this.modal.annualStatistic(new AnnualStatistic(0, "", 0, this.groups));
        _this.showModal();
    };

    this.openModalForEdit = function (_, event) {
        _this.modal.mode(_this.modal.modalModeEdit);

        var annualStatistic = $(event.currentTarget).data("annual-stats");

        _this.modal.annualStatistic(
            new AnnualStatistic(
                annualStatistic.Id,
                annualStatistic.Name,
                annualStatistic.AnnualStatisticGroupId,
                this.groups
            )
        );

        _this.showModal();
    };

    this.modalSubmit = function (e) {
        e.preventDefault();

        // Get Action Type
        var action = _this.modal.mode().toLowerCase();
        var url = `/api/AnnualStatistics/${action}AnnualStatisticAjax`;

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
    $("#modal-annual-stats-form").submit(function (e) {
        adacAnnualStatisticVM.modalSubmit(e);
    });

    $(".delete-confirm").click(function (e) {
        e.preventDefault();

        var $link = $(this);

        bootbox.confirm("Are you sure you want to delete " + $link.data("annual-stats") + "?",
            function (confirmation) {
                confirmation && window.location.assign($link.attr("href"));
            }
        );
    });

    adacAnnualStatisticVM = new AdacAnnualStatisticViewModel();
    ko.applyBindings(adacAnnualStatisticVM);
});
