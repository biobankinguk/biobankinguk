// Modals
var adacAssociatedDataTypeVM;

function AssociatedDataType(id, name, message, groupId, groups, ontologyTerms) {
  this.id = id;
  this.name = ko.observable(name);
  this.message = ko.observable(message);
  this.groupId = ko.observable(groupId);
  this.groups = ko.observableArray(groups);
  this.ontologyTerms = ko.observableArray(ontologyTerms);
}

function AssociatedDataTypeModal(
  id,
  name,
  message,
  groupId,
  groups,
  ontologyTerms
) {
  this.modalModeAdd = "Add";
  this.modalModeEdit = "Update";

  this.mode = ko.observable(this.modalModeAdd);

  this.associatedDataType = ko.observable(
    new AssociatedDataType(id, name, message, groupId, ontologyTerms)
  );

  this.groups = ko.observableArray(groups);
}

function AdacAssociatedDataTypeViewModel() {
  var _this = this;
  this.modalId = "#associated-types-modal";

  this.groups = $(this.modalId).data("groups");

  this.modal = new AssociatedDataTypeModal(0, "", "", 0, this.groups, []);
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
    _this.modal.associatedDataType(
      new AssociatedDataType(0, "", "", 0, this.groups, [])
    );
    _this.showModal();
  };

  this.openModalForEdit = function (_, event) {
    _this.modal.mode(_this.modal.modalModeEdit);

    var associatedDataType = $(event.currentTarget).data("associated-types");

    var ontologyTerms = associatedDataType.ontologyTerms
      ? associatedDataType.ontologyTerms
      : [];
    console.log(ontologyTerms);

    _this.modal.associatedDataType(
      new AssociatedDataType(
        associatedDataType.id,
        associatedDataType.name,
        associatedDataType.message,
        associatedDataType.associatedDataTypeGroupId,
        this.groups,
        ontologyTerms
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
  this.addOntologyTerm = function () {
    if (
      _this.modal
        .associatedDataType()
        .ontologyTerms()
        .find(function (item) {
          return (
            item.ontologyTermId ==
            JSON.parse($(".diagnosis-search").attr("data-id")).ontologyTermId
          );
        }) == undefined
    ) {
      _this.modal
        .associatedDataType()
        .ontologyTerms.push(JSON.parse($(".diagnosis-search").attr("data-id")));
    }
    $(".diagnosis-search").val("");
    $("#diagnosis-submit").prop("disabled", true);
  };
  this.removeOntologyTerm = function (idx) {
    _this.modal.associatedDataType().ontologyTerms.splice(idx, 1);
  };

  var diseases = new Bloodhound({
    datumTokenizer: Bloodhound.tokenizers.whitespace("vval"),
    queryTokenizer: Bloodhound.tokenizers.whitespace,
    remote: {
      url: "/Search/ListOntologyTerms?wildcard=%QUERY",
      filter: function (x) {
        return $.map(x, function (item) {
          return item;
        });
      },
      wildcard: "%QUERY",
    },
  });

  diseases.initialize();

  var searchElement = $(".diagnosis-search");
  searchElement
    .typeahead(
      {
        hint: false,
        highlight: true,
        minLength: 1,
        autoselect: true,
      },
      {
        name: "description",
        displayKey: "description",
        source: diseases.ttAdapter(),
        limit: 100,
      }
    )
    .bind("typeahead:select", function (ev, item) {
      $(".diagnosis-search").attr("data-id", JSON.stringify(item));
      onchangeAssociatedData();
    });
  function onchangeAssociatedData() {
    // set button to be enabled
    $("#diagnosis-submit").removeAttr("disabled");
  }

  document
    .getElementsByClassName("diagnosis-search")[0]
    .addEventListener("input", function () {
      removeId();
    });

  function removeId() {
    $(".diagnosis-search").removeAttr("data-id");
    $("#diagnosis-submit").prop("disabled", true);
  }
}

$(function () {
  $("#modal-associated-types-form").submit(function (e) {
    adacAssociatedDataTypeVM.modalSubmit(e);
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

  adacAssociatedDataTypeVM = new AdacAssociatedDataTypeViewModel();
  ko.applyBindings(adacAssociatedDataTypeVM);
});
