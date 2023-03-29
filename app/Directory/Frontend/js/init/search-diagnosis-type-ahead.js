/**
 * Enables typeahead wildcard searching of Disease Status ontology terms
 * against a text input with the class `diagnosis-search`
 */

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
      url: "/Search/SearchOntologyTerms",
      replace: function (url, query) {
        var type = $("input[name='searchRadio']:checked").val();
        return (url += "?searchDocumentType=" + type + "&wildcard=" + query);
      },
      filter: function (x) {
        return $.map(x, function (item) {
          return {
            desc: item.description,
            other: item.otherTerms,
            mother: item.matchingOtherTerms.join(", "),
            nother: item.nonMatchingOtherTerms.join(", "),
          };
        });
      },
    },
  });

  diseases.initialize();

  var searchElement = $(".diagnosis-search");

  searchElement.typeahead(
    {
      hint: false,
      highlight: true,
      minLength: 3,
      autoselect: true,
    },
    {
      name: "desc",
      displayKey: "desc",
      source: diseases.ttAdapter(),
      limit: 100,
      templates: {
        suggestion: function (e) {
          return (
            '<div class="search-list"><b>' +
            e.desc +
            '</b><div style="font-size:small">' +
            (!e.mother ? "" : "<span>" + e.mother + "</span><br/>") +
            (!e.nother ? "" : "<span>(..." + e.nother + ")</span>") +
            "</div></div>"
          );
        },
      },
    }
  );
});
