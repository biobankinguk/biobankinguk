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
            desc: item.description,
            id: item.ontologyTermId,
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
    var form = $(".diagnosis-search");
    var id = form.data("id") ? form.data("id") : null;

    $(".linked-data").remove();
    // if the data-id is undefined then don't do the query
    if (!id) return;
    // next make an ajax call using the ontology term id
    $.ajax({
      type: "GET",
      url: form.data("resource-url") + "?id=" + id,
      beforeSend: function () {
        setLoading(true); // Show loader icon
      },
      success: function (response) {
        var table = document.createElement("div");
        table.innerHTML = response.trim();
        var itemList = table.getElementsByTagName("tr");
        // insert each row from result into the table under the right header
        Array.from(itemList).forEach(function (tableRow) {
          $(tableRow).insertAfter("." + tableRow.dataset.groupid);
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
