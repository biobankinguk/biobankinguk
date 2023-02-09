function BiobankFundersViewModel() {
  var _this = this;
  this.elements = {
    id: "#BiobankId",
    modal: "#modalAddBiobankFunder",
    form: "#modalAddBiobankFunderForm",
  };

  this.dialogErrors = ko.observableArray([]);
  this.biobankId = $(this.elements.id).data("biobank-id");

  this.openAddFunderDialog = function () {
    $.ajax({
      url: "/Biobank/Profile/AddFunderAjax/" + this.biobankId,
      data: { biobankId: this.biobankId },
      contentType: "application/html",
      success: function (content) {
        //clear form errors (as these are in the page's ko model)
        _this.dialogErrors.removeAll();

        _this.cleanNodeJquerySafe(_this.elements.modal);

        //populate the modal with the form
        $(_this.elements.modal).html(content);

        //apply ko bindings to the ajax'd elements
        ko.applyBindings(biobankFundersVm, $(_this.elements.modal)[0]);

        //init bloodhound for the typeahead
        window.initFundersBloodhound();

        //wire up the form submission
        $(_this.elements.form).submit(function (e) {
          _this.submitAddDialog(e);
        });
      },
    });
  };

  this.submitAddDialog = function (e) {
    e.preventDefault();
    var form = $(_this.elements.form);
    var successURl =
      form.data("success-redirect") + "/" + this.biobankId + "?Name=";

    $.ajax({
      type: "POST",
      url: form.data("action"),
      data: form.serialize(),
      success: function (data) {
        //clear form errors (as these are in the page's ko model)
        _this.dialogErrors.removeAll();
        $(_this.elements.modal).modal("hide");
        //now we can redirect (force a page reload, following the successful AJAX submit
        //(why not just do a regular POST submit? for nice AJAX modal form valdation)
        window.location.href = successURl + data.name;
      },
      error: function (error) {
        var message = JSON.parse(error.responseText);
        _this.dialogErrors.push(Object.values(message)[0]);
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

var biobankFundersVm;

$(function () {
  biobankFundersVm = new BiobankFundersViewModel();
  ko.applyBindings(biobankFundersVm);

  $("#biobank-funders")["DataTable"]({
    columnDefs: [{ orderable: false, targets: 1 }],
    paging: false,
    info: false,
    autoWidth: false,
    language: {
      search: "Filter: ",
    },
    drawCallback: function () {
      //select the fixed row(s)
      var rows = $("#biobank-funders tr.datatables-fixed-row");

      //stick them at the top of the tbody
      $("#biobank-funders tbody").prepend(rows);
    },
  });

  //wire up bootbox confirmation
  $(".confirm-delete").click(function (e) {
    e.preventDefault();
    var $link = $(this);
    bootbox.confirm(
      "Are you sure you wish to remove " +
        $link.data("funder-name") +
        " from your list of funders?",
      function (confirmation) {
        confirmation && window.location.assign($link.attr("href"));
      }
    );
  });
});
