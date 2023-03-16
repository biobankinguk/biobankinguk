var fundersList = [];

function initFundersBloodhound() {
  var searchElement = $("#FunderName");

  var funders = new Bloodhound({
    datumTokenizer: Bloodhound.tokenizers.whitespace("vval"),
    queryTokenizer: Bloodhound.tokenizers.whitespace,
    remote: {
      url: searchElement.data("resource-url") + "?wildcard=%QUERY",
      filter: function (x) {
        return $.map(x, function (item) {
          return { desc: item.name };
        });
      },
      wildcard: "%QUERY",
    },
  });

  funders.initialize();

  searchElement.typeahead(
    {
      hint: false,
      highlight: true,
      minLength: 1,
      autoselect: true,
    },
    {
      name: "desc",
      displayKey: "desc",
      source: funders.ttAdapter(),
    }
  );
}
