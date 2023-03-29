var refdataType = "Disease status";
var redirectUrl = "/Admin/ReferenceData/DiseaseStatuses";
var apiUrl = "/api/DiseaseStatus";

var adacDiseaseStatusVM;
var dataTable;

function DiseaseStatus(
  ontologyTermId,
  description,
  otherTerms,
  displayOnDirectory,
  associatedData
) {
  this.ontologyTermId = ko.observable(ontologyTermId);
  this.description = ko.observable(description);
  this.otherTerms = ko.observableArray(otherTerms);
  this.displayOnDirectory = ko.observable(displayOnDirectory);
  this.associatedData = ko.observableArray(associatedData);
}

function DiseaseStatusModal(
  ontologyTermId,
  description,
  otherTerms,
  displayOnDirectory,
  associatedData
) {
  this.modalModeAdd = "Add";
  this.modalModeEdit = "Update";

  this.mode = ko.observable(this.modalModeAdd);

  this.diseaseStatus = ko.observable(
    new DiseaseStatus(
      ontologyTermId,
      description,
      otherTerms,
      displayOnDirectory,
      associatedData
    )
  );
}

function AdacDiseaseStatusViewModel() {
  var _this = this;

  this.modalId = "#disease-status-modal";
  this.modal = new DiseaseStatusModal("", "", [], false, []);
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
    _this.modal.diseaseStatus(new DiseaseStatus("", "", [], false, []));
    _this.setPartialEdit(false);
    _this.showModal();
  };

  this.openModalForEdit = function (_, event) {
    _this.modal.mode(_this.modal.modalModeEdit);

    // Get Row Data From DataTable
    var rowIndex = $(event.currentTarget).data("row");
    var diseaseStatus = dataTable.row(rowIndex).data();

    var otherTerms = diseaseStatus.otherTerms
      ? diseaseStatus.otherTerms.split(",").map(function (item) {
          return item.trim();
        })
      : diseaseStatus.otherTerms;

    var associatedData = diseaseStatus.associatedDataTypes
      ? diseaseStatus.associatedDataTypes
      : [];

    _this.modal.diseaseStatus(
      new DiseaseStatus(
        diseaseStatus.ontologyTermId,
        diseaseStatus.description,
        otherTerms,
        diseaseStatus.displayOnDirectory,
        associatedData
      )
    );

    $("#OntologyTermId").prop("readonly", true);
    _this.setPartialEdit(diseaseStatus.collectionCapabilityCount > 0);
    _this.showModal();
  };

  this.modalSubmit = function (e) {
    e.preventDefault();
    var form = $(e.target); // get form as a jquery object

    //Concatenate other terms (exclude null/empty/whitespace strings)
    $("#OtherTerms").val(
      _this.modal
        .diseaseStatus()
        .otherTerms()
        .filter(function (x) {
          return x && x.trim();
        })
        .join(",")
    );

    // Get Form Data
    var data = serializeFormData(form);
    data.DisplayOnDirectory = data.DisplayOnDirectory === "true" ? true : false;

    // Get Action Type
    var action = _this.modal.mode();
    if (action == "Add") {
      addRefData(_this, apiUrl, data, redirectUrl, refdataType);
    } else if (action == "Update") {
      // Parse Edit Url
      var url = apiUrl + "/" + $(e.target.Id).val();
      editRefData(_this, url, data, redirectUrl, refdataType);
    }
  };

  this.setPartialEdit = function (flag) {
    if (flag == true) $("#Description").prop("readonly", true);
    else $("#Description").prop("readonly", false);
  };

  this.addOtherTerms = function () {
    _this.modal.diseaseStatus().otherTerms.push("");
  };
  this.removeOtherTerms = function (idx) {
    _this.modal.diseaseStatus().otherTerms.splice(idx, 1);
  };
  this.addAssociatedData = function () {
    if (
      _this.modal
        .diseaseStatus()
        .associatedData()
        .find(function (item) {
          return item.Id === JSON.parse($("#ass-data-select").val()).id;
        }) == null
    ) {
      _this.modal
        .diseaseStatus()
        .associatedData.push(JSON.parse($("#ass-data-select").val()));
    }
  };
  this.removeAssociatedData = function (idx) {
    _this.modal.diseaseStatus().associatedData.splice(idx, 1);
  };

  // Get the list of Associated Data and insert into modal select input.
  $.ajax({
    type: "GET",
    url: "/api/AssociatedDataType",
    beforeSend: function () {
      //setLoading(true); // Show loader icon
    },
    success: function (response) {
      response.forEach(function (type) {
        var val = JSON.stringify(type);
        $("#ass-data-select").append(
          "<option value='" + val + "'>" + type.name + "</option>"
        );
      });
    },
    complete: function () {
      //setLoading(false); // Hide loader icon
    },
    failure: function (jqXHR, textStatus, errorThrown) {
      console.log("an error was thrown");
    },
  });
}

// DataTables
$(function () {
  dataTable = $("#disease-statuses").DataTable({
    ajax: "./PagingatedDiseaseStatuses",
    processing: true,
    serverSide: true,
    paging: true,
    pageLength: 25,
    ordering: false,
    info: false,
    autoWidth: false,
    language: {
      search: "Filter: ",
    },
    columns: [
      { data: "description" },
      { data: "ontologyTermId" },
      { data: "otherTerms", defaultContent: "" },
      { data: "collectionCapabilityCount" },
      {
        data: "displayOnDirectory",
        render: function (data, type, row) {
          var bool = data.toString();
          return bool.charAt(0).toUpperCase() + bool.slice(1);
        },
      },
      {
        data: "associatedDataTypes",
        render: function (data, type, row, meta) {
          var returnString = "N/A";
          if (data) {
            if (data.length === 1) {
              returnString = "<li>" + data[0].name + "</li>";
            } else if (data.length < 4 && data.length > 1) {
              // return their values as a list
              var displayData = data
                .map(function (item) {
                  return "<li>" + item.name + "</li>";
                })
                .join("");
              returnString = "" + displayData + "";
            } else if (data.length > 3) {
              var displayData = data
                .slice(2)
                .map(function (item) {
                  return "<li>" + item.name + "</li>";
                })
                .join("");

              var ViewMore = $("<a/>", {
                "data-row": meta.row,
                class: "action-icon click-view-more-assdata",
                href: "#",
                html: $("<span/>", {
                  text: "...View All",
                }),
              });
              returnString = "" + displayData + "";
            }
          }
          return $("<div/>").append(returnString).append(ViewMore).html();
        },
      },
      {
        // Action Links
        data: function (row, type, val, meta) {
          // Edit Link - Binds To Knockout Modal
          var editLink = $("<a/>", {
            "data-row": meta.row,
            "data-bind": "click: openModalForEdit",
            class: "action-icon",
            href: "#",
            html: $("<span/>", {
              class: "fa fa-edit labelled-icon",
            }).add(
              $("<span/>", {
                text: "Edit",
              })
            ),
          });

          if (row.CollectionCapabilityCount == 0) {
            // Delete Link - Triggered By jQuery
            var deleteLink = $("<a/>", {
              "data-row": meta.row,
              class: "action-icon click-delete",
              href: "#",
              html: $("<span/>", {
                class: "fa fa-trash labelled-icon",
              }).add(
                $("<span/>", {
                  text: "Delete",
                })
              ),
            });

            // Convert To HTML String
            return $("<div/>").append(editLink).append(deleteLink).html();
          } else {
            // In Use -> Edit Only
            return $("<div/>").append(editLink).html();
          }
        },
      },
    ],
    createdRow: function (row, data, dataIndex) {
      // Highlight In Use Disease Statuses
      if (data.collectionCapabilityCount > 0) {
        $(row).addClass("info");
      }

      // Bind Knockout View Model To Row
      ko.applyBindingsToDescendants(adacDiseaseStatusVM, row);
    },
  });
});

// Start-Up
$(function () {
  // Modal Submission Event Listener
  $("#modal-disease-status-form").submit(function (e) {
    adacDiseaseStatusVM.modalSubmit(e);
  });

  // Delete Row Link Listener
  $(document.body).on("click", ".click-delete", function (e) {
    e.preventDefault();

    var rowIndex = $(this).data("row");
    var data = dataTable.row(rowIndex).data();
    var url = apiUrl + "/" + data.ontologyTermId;

    bootbox.confirm(
      "Are you sure you want to delete " + data.description + "?",
      function (confirmation) {
        if (confirmation) {
          deleteRefData(url, redirectUrl, refdataType);
        }
      }
    );
  });

  //View More Associated Data
  $(document.body).on("click", ".click-view-more-assdata", function (e) {
    e.preventDefault();

    var rowIndex = $(this).data("row");
    var data = dataTable.row(rowIndex).data();
    var displayData = data.associatedDataTypes
      .map(function (item) {
        return "<li>" + item.name + "</li>";
      })
      .join("");

    bootbox.alert(displayData);
  });

  // Knockout View Model Binding
  adacDiseaseStatusVM = new AdacDiseaseStatusViewModel();
  ko.applyBindings(adacDiseaseStatusVM);

  // jquery plugin to serialise checkboxes as bools
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
          var val = $(this).val();
          return val == null
            ? null
            : $.isArray(val)
            ? $.map(val, function (innerVal) {
                return {
                  name: elem.name,
                  value: innerVal,
                };
              })
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
});
