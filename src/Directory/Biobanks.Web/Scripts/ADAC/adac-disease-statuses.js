function DiseaseStatus(id, snomedIdentifier, description, otherTerms) {
  this.id = id;
  this.snomedIdentifier = ko.observable(snomedIdentifier);
    this.description = ko.observable(description);
    this.otherTerms = ko.observable(otherTerms);
}

function DiseaseStatusModal(id, snomedIdentifier, description, otherTerms) {
  this.modalModeAdd = "Add";
  this.modalModeEdit = "Update";

  this.mode = ko.observable(this.modalModeAdd);
    this.diseaseStatus = ko.observable(
        new DiseaseStatus(id, snomedIdentifier, description, otherTerms)
  );
}

function AdacDiseaseStatusViewModel() {
  var _this = this;

  this.modalId = "#disease-status-modal";
  this.modal = new DiseaseStatusModal(0, "", "", "");
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
    _this.modal.diseaseStatus(new DiseaseStatus(0, "", "", ""));
    _this.showModal();
  };

  this.openModalForEdit = function (_, event) {
    _this.modal.mode(_this.modal.modalModeEdit);

    var diseaseStatus = $(event.currentTarget).data("disease-status");
    _this.modal.diseaseStatus(
      new DiseaseStatus(
        diseaseStatus.Id,
        diseaseStatus.SnomedIdentifier,
          diseaseStatus.Description,
          diseaseStatus.OtherTerms

      )
    );

    _this.setPartialEdit($(event.currentTarget).data("partial-edit"));
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

    //Turns on/off partial editing of some input fields
    this.setPartialEdit = function (flag) {
        if (flag == true) {
            $("#SnomedIdentifier").prop('readonly', true);
            $("#Description").prop('readonly', true);
        }
        else {
            $("#SnomedIdentifier").prop('readonly', false);
            $("#Description").prop('readonly', false);
        }
    }
}

var adacDiseaseStatusVM;

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

  $("#disease-statuses")["DataTable"]({
    columnDefs: [
      { orderable: false, targets: 4 },
      { width: "180px", targets: 4 },
    ],
    paging: false,
    info: false,
    autoWidth: false,
    language: {
      search: "Filter: ",
    },
  });

  $("#modal-disease-status-form").submit(function (e) {
    adacDiseaseStatusVM.modalSubmit(e);
  });

  $(".delete-confirm").click(function (e) {
    e.preventDefault();
    var $link = $(this);
    bootbox.confirm(
      "Are you sure you want to delete " + $link.data("disease-status") + "?",
      function (confirmation) {
        confirmation && window.location.assign($link.attr("href"));
      }
    );
  });

  adacDiseaseStatusVM = new AdacDiseaseStatusViewModel();

  ko.applyBindings(adacDiseaseStatusVM);
});
