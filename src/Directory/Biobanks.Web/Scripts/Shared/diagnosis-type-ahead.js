var diseaselist = [];

$(function () {
  var diseases = new Bloodhound({
    datumTokenizer: Bloodhound.tokenizers.whitespace("vval"),
    queryTokenizer: Bloodhound.tokenizers.whitespace,
    //prefetch: {
    //    url: "countries.json",
    //    filter: function (diseaseList) {
    //        return $.map(diseaseList, function (disease) {
    //            return { vval: disease.Description };
    //        });
    //    }
    //},
    remote: {
      url: "/Search/ListOntologyTerms?wildcard=%QUERY",
      filter: function (x) {
        return $.map(x, function (item) {
          return {
            desc: item.Description,
            id: item.Id,
          };
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
        name: "desc",
        displayKey: "desc",
        source: diseases.ttAdapter(),
        limit: 100,
      }
    )
    .bind("typeahead:select", function (ev, item) {
      $(".diagnosis-search").attr("data-id", item.id);
      onchangeAssociatedData();
    });
  function onchangeAssociatedData() {
    // first get the id of the inputted ontologyTerm
    const id = $(".diagnosis-search").attr("data-id")
      ? $(".diagnosis-search").attr("data-id")
      : null;
    $(".linked-data").remove();
    // if the data-id is undefined then don't do the query
    if (!id) return;
    // next make an ajax call using the ontology term id
    $.ajax({
      type: "GET",
      url: "/api/" + "DiseaseStatus" + "/" + id + "/AssociatedDataTypes",
      beforeSend: function () {
        setLoading(true); // Show loader icon
      },
      success: function (response) {
        // insert each value from result into the table under the right header
        $.each(response, function (index, assData) {
          $(
            `<tr class='linked-data'>
                            <td>
                                <div class="checkbox">
                                    <span data-message="" data-title="Cold ischemic time" class="fa fa-info-circle help-icon-button help-associateddata"></span>
                                    <label>
                                        <input class="ass-dat-chk" data-val="true" data-val-required="The Active field is required." id="Groups_0__Types_0__Active" name="Groups[0].Types[0].Active" type="checkbox" value="true"><input name="Groups[0].Types[0].Active" type="hidden" value="false">
                                        ${assData.Value}
                                    </label>
                                    <input data-val="true" data-val-number="The field DataTypeId must be a number." data-val-required="The DataTypeId field is required." id="Groups_0__Types_0__DataTypeId" name="Groups[0].Types[0].DataTypeId" type="hidden" value="3">
                                </div>
                            </td>
                            <td>
                                <div class="radio">
                                        <label class="timeFrames">
                                            <input class="ass-dat-rad" data-val="true" data-val-number="The field ProvisionTimeId must be a number." id="Groups_0__Types_0__ProvisionTimeId" name="Groups[0].Types[0].ProvisionTimeId" type="radio" value="1">
                                            Immediate
                                        </label>
                                        <label class="timeFrames">
                                            <input class="ass-dat-rad" id="Groups_0__Types_0__ProvisionTimeId" name="Groups[0].Types[0].ProvisionTimeId" type="radio" value="2">
                                            0-3
                                        </label>
                                        <label class="timeFrames">
                                            <input class="ass-dat-rad" id="Groups_0__Types_0__ProvisionTimeId" name="Groups[0].Types[0].ProvisionTimeId" type="radio" value="3">
                                            3-6
                                        </label>
                                        <label class="timeFrames">
                                            <input class="ass-dat-rad" id="Groups_0__Types_0__ProvisionTimeId" name="Groups[0].Types[0].ProvisionTimeId" type="radio" value="4">
                                            &gt;6
                                        </label>
                                </div>
                            </td>

                        </tr>`
          ).insertAfter(`.${assData.GroupId}`);
        });
      },
      complete: function () {
        setLoading(false); // Hide loader icon
      },
      failure: function (jqXHR, textStatus, errorThrown) {
        console.log("an error was thrown");
      },
    });
  }

  function setLoading(option) {
    if (option) {
      // set the loading indicator to be true
      $("#busy-holder").show();
    } else {
      // set the loading indicator to be false
      $("#busy-holder").hide();
    }
  }

  document
    .getElementsByClassName("diagnosis-search")[0]
    .addEventListener("input", function () {
      removeId();
    });

  function removeId() {
    $(".diagnosis-search").removeAttr("data-id");
    $(".linked-data").remove();
  }
});
