function BiobankAdminsViewModel() {
  var _this = this;
  var base = "BiobankAdmin";
  this.elements = {
    id: "#BiobankId",
    modal: "#modalInvite" + base,
    form: "#modalInvite" + base + "Form",
  };

  this.dialogErrors = ko.observableArray([]);
  this.biobankId = $(this.elements.id).data("biobank-id");

  this.openInviteDialog = function () {
    $.ajax({
      url: $(_this.elements.modal).data("resource-url"),
      data: { biobankId: _this.biobankId },
      contentType: "application/html",
      success: function (content) {
        //clear form errors (as these are in the page's ko model)
        _this.dialogErrors.removeAll();

        _this.cleanNodeJquerySafe(_this.elements.modal);

        //populate the modal with the form
        $(_this.elements.modal).html(content);

        //apply ko bindings to the ajax'd elements
        ko.applyBindings(biobankAdminsVm, $(_this.elements.modal)[0]);

        //wire up the form submission
        $(_this.elements.form).submit(function (e) {
          _this.submitInviteDialog(e);
        });
      },
    });
  };

  this.submitInviteDialog = function (e) {
    e.preventDefault();
    var form = $(_this.elements.form);

    $.ajax({
      type: "POST",
      url: form.data("action"),
      data: form.serialize(),
      success: function (data) {
        //clear form errors (as these are in the page's ko model)
        _this.dialogErrors.removeAll();

        if (data.success) {
          $(_this.elements.modal).modal("hide");
          //now we can redirect (force a page reload, following the successful AJAX submit
          //(why not just do a regular POST submit? for nice AJAX modal form valdation)
          window.location.href =
            form.data("success-redirect") + "?Name=" + data.name;
        } else {
          if (Array.isArray(data.errors)) {
            data.errors.forEach(function (error, index) {
              _this.dialogErrors.push(error);
            });
          }
        }
      },
    });
  };

  this.cleanNodeJquerySafe = function (nodeSelector) {
    //clear knockout bindings,
    //but leave jQuery/bootstrap bindings intact!
    var original = ko.utils.domNodeDisposal["cleanExternalData"];
    ko.utils.domNodeDisposal["cleanExternalData"] = function () {};
    ko.cleanNode($(nodeSelector)[0]); //designed to work with ID selectors, so only does the first match
    ko.utils.domNodeDisposal["cleanExternalData"] = original;
  };
}

var biobankAdminsVm;

$(function () {
  biobankAdminsVm = new BiobankAdminsViewModel();
  ko.applyBindings(biobankAdminsVm);

  $("#biobank-admins")["DataTable"]({
    columnDefs: [{ orderable: false, targets: 2 }],
    paging: false,
    info: false,
    autoWidth: false,
    language: {
      search: "Filter: ",
    },
    drawCallback: function () {
      //select the fixed row(s)
      var rows = $("#biobank-admins tr.datatables-fixed-row");

      //stick them at the top of the tbody
      $("#biobank-admins tbody").prepend(rows);
    },
  });

  //wire up bootbox confirmation
  $(".confirm-delete").click(function (e) {
    e.preventDefault();
    var $link = $(this);
    bootbox.confirm(
      "Are you sure you wish to remove " +
        $link.data("admin-name") +
        " from your admins?",
      function (confirmation) {
        confirmation && window.location.assign($link.attr("href"));
      }
    );
  });

  $(".confirm-resend").click(function (e) {
    e.preventDefault();
    var $link = $(this);
    bootbox.confirm(
      $link.data("admin-name") +
        " has not yet confirmed their account.<br/>Do you want to resend their invitation link?",
      function (confirmation) {
        confirmation && window.location.assign($link.attr("href"));
      }
    );
  });
});
