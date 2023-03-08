function NetworkAdminsViewModel() {
  var _this = this;
  var base = "NetworkAdmin";
  this.elements = {
    id: "#NetworkId",
    modal: "#modalInvite" + base,
    form: "#modalInvite" + base + "Form",
  };

  this.dialogErrors = ko.observableArray([]);
  this.networkId = $(this.elements.id).data("network-id");

  this.openInviteDialog = function () {
    $.ajax({
      url: "/Network/Settings/" + _this.networkId + "/InviteAdminAjax/",
      data: { networkId: _this.networkId },
      contentType: "application/html",
      success: function (content) {
        //clear form errors (as these are in the page's ko model)
        _this.dialogErrors.removeAll();

        _this.cleanNodeJquerySafe(_this.elements.modal);

        //populate the modal with the form
        $(_this.elements.modal).html(content);

        //apply ko bindings to the ajax'd elements
        ko.applyBindings(networkAdminsVm, $(_this.elements.modal)[0]);

        //wire up the form submission
        $(_this.elements.form).submit(function (e) {
          return _this.submitInviteDialog(e);
        });

        //intialise jQuery Validation for the new elements
        //$(_this.elements.form).validate({
        //    rules: {
        //        Name: "required",
        //        Email: {
        //            required: true,
        //            email: true
        //        }
        //    }
        //});
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
            data.errors.forEach(function (error, i) {
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

var networkAdminsVm;

$(function () {
  networkAdminsVm = new NetworkAdminsViewModel();
  ko.applyBindings(networkAdminsVm);

  $("#network-admins")["DataTable"]({
    columnDefs: [{ orderable: false, targets: 2 }],
    paging: false,
    info: false,
    autoWidth: false,
    language: {
      search: "Filter: ",
    },
    drawCallback: function () {
      //select the fixed row(s)
      var rows = $("#network-admins tr.datatables-fixed-row");

      //stick them at the top of the tbody
      $("#network-admins tbody").prepend(rows);
    },
  });

  //wire up bootbox confirmation
  $(".confirm-delete").click(function (e) {
    e.preventDefault();
    var $link = $(this);
    bootbox.confirm(
      "Are you sure you wish to remove " +
        $link.data("admin-name") +
        " from your network admins?",
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
