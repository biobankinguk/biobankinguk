function AnnualStatisticGroup(id, name) {
  this.id = id;
    this.name = ko.observable(name);
}

function AnnualStatisticGroupModal(id, name) {
  this.modalModeAdd = "Add";
  this.modalModeEdit = "Update";

  this.mode = ko.observable(this.modalModeAdd);
    this.annualStatisticGroup = ko.observable(
        new AnnualStatisticGroup(id, name)
  );
}

function AdacAnnualStatisticGroupViewModel() {
  var _this = this;

  this.modalId = "#annual-statistic-group-modal";
  this.modal = new AnnualStatisticGroupModal(0, "");
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
    _this.modal.annualStatisticGroup(new AnnualStatisticGroup(0, ""));
    _this.showModal();
  };

  this.openModalForEdit = function (_, event) {
    _this.modal.mode(_this.modal.modalModeEdit);

    var annualStatisticGroup = $(event.currentTarget).data("annual-statistic-group");
    _this.modal.annualStatisticGroup(
      new AnnualStatisticGroup(
          annualStatisticGroup.AnnualStatisticGroupId,
          annualStatisticGroup.Name

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
            var feedbackfn = setAddFeedback // cf. adac-refdata-feedback.js
        } else if (action == 'Update') {
            var ajaxType = 'PUT';
            var url = resourceUrl + '/' + $(e.target.AnnualStatisticGroupId).val();
            var feedbackfn = setEditFeedback // cf. adac-refdata-feedback.js
        }

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
                    feedbackfn(data.name,
                        form.data("success-redirect"), form.data("refdata-type"))
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

var adacAnnualStatisticGroupVM;

$(function () {
  //jquery plugin to serialise checkboxes as bools
  (function ($) {
    $.fn.serialize = function () {
      return $.param(this.serializeArray());
    };

    $.fn.serializeArray = function () {
      var o = $.extend(
        {
          checkboxesAsBools: true,
        },
        {}
      );

      var rselectTextarea = /select|textarea/i;
      var rinput = /text|hidden|password|search/i;

      return this.map(function () {
        return this.elements ? $.makeArray(this.elements) : this;
      })
        .filter(function () {
          return (
            this.name &&
            !this.disabled &&
            (this.checked ||
              (o.checkboxesAsBools && this.type === "checkbox") ||
              rselectTextarea.test(this.nodeName) ||
              rinput.test(this.type))
          );
        })
        .map(function (i, elem) {
          const val = $(this).val();
          return val == null
            ? null
            : $.isArray(val)
            ? $.map(val, (innerVal) => ({ name: elem.name, value: innerVal }))
            : {
                name: elem.name,
                value:
                  o.checkboxesAsBools && this.type === "checkbox" //moar ternaries!
                    ? this.checked
                      ? "true"
                      : "false"
                    : val,
              };
        })
        .get();
    };
  })(jQuery);

  $("#annual-statistic-groups")["DataTable"]({
    columnDefs: [
      { orderable: false, targets: 2 },
      { width: "250px", targets: 2 },
    ],
    paging: false,
    info: false,
    autoWidth: false,
    language: {
      search: "Filter: ",
    },
  });

  $("#modal-annual-statistic-group-form").submit(function (e) {
    adacAnnualStatisticGroupVM.modalSubmit(e);
  });

  $(".delete-confirm").click(function (e) {
      e.preventDefault();
      var $link = $(this);
      var linkData = $link.data("annual-statistic-group")
      var url = $link.data("resource-url") + "/" + linkData.AnnualStatisticGroupId;

      bootbox.confirm("Are you sure you want to delete " + linkData.Name + "?",
          function (confirmation) {
              if (confirmation) {
                  // Make AJAX Call
                  $.ajax({
                      url: url,
                      type: 'DELETE',
                      success: function (data, textStatus, xhr) {
                          if (data.success) {
                              setDeleteFeedback(data.name,
                                  $link.data("success-redirect"), $link.data("refdata-type"))
                          }
                          else {
                              if (Array.isArray(data.errors)) {
                                  if (data.errors.length > 0) {
                                      window.feedbackMessage(data.errors[0], "warning");
                                  }
                              }
                          }
                      }
                  });
              }
          }
      );
  });

  adacAnnualStatisticGroupVM = new AdacAnnualStatisticGroupViewModel();

  ko.applyBindings(adacAnnualStatisticGroupVM);
});
