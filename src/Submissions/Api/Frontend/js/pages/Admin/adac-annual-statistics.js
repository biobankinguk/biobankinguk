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

  this.annualStatistic = ko.observable(new AnnualStatistic(id, name, groupId));

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
        annualStatistic.id,
        annualStatistic.name,
        annualStatistic.annualStatisticGroupId,
        this.groups
      )
    );

    _this.showModal();
  };

  this.modalSubmit = function (e) {
    e.preventDefault();
    var form = $(e.target); // get form as a jquery object

    // Get Form Data
    var data = serializeFormData(form);

    // Get Action Type
    var action = _this.modal.mode();
    if (action == "Add") {
      addRefData(
        _this,
        form.data("resource-url"),
        data,
        form.data("success-redirect"),
        form.data("refdata-type")
      ); // cf. adac-refdata-utility.js
    } else if (action == "Update") {
      editRefData(
        _this,
        form.data("resource-url") + "/" + $(e.target.Id).val(),
        data,
        form.data("success-redirect"),
        form.data("refdata-type")
      );
    }
  };
}

$(function () {
  $("#modal-annual-stats-form").submit(function (e) {
    adacAnnualStatisticVM.modalSubmit(e);
  });

  $(".delete-confirm").click(function (e) {
    e.preventDefault();

    var $link = $(this);
    var linkData = $link.data("refdata-model");
    var url = $link.data("resource-url") + "/" + linkData.id;

    bootbox.confirm(
      "Are you sure you want to delete " + linkData.name + "?",
      function (confirmation) {
        if (confirmation) {
          deleteRefData(
            url,
            $link.data("success-redirect"),
            $link.data("refdata-type")
          );
        }
      }
    );
  });

  adacAnnualStatisticVM = new AdacAnnualStatisticViewModel();
  ko.applyBindings(adacAnnualStatisticVM);
});
