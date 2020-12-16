// Modals
var adacAnnualStatisticVM;

function AnnualStatistic(id, name, groupId) {
    this.id = id;
    this.name = ko.observable(name);
    this.groupId = ko.observable(groupId);
}

function AnnualStatisticModal(id, name, groupId, groups) {
    this.modalModeAdd = "Add";
    this.modalModeEdit = "Update";

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
