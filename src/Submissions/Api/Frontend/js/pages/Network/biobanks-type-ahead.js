var biobanksList = [];
var networkId = $("#NetworkId").data("network-id");

$(function () {
  var searchUrl = "/Network/Profile/SearchBiobanks/" + networkId;
  var biobanks = new Bloodhound({
    datumTokenizer: Bloodhound.tokenizers.whitespace("vval"),
    queryTokenizer: Bloodhound.tokenizers.whitespace,
    remote: {
      url: searchUrl + "?wildcard=%QUERY",
      filter: function (x) {
        return $.map(x, function (item) {
          return { desc: item.name };
        });
      },
      wildcard: "%QUERY",
    },
  });

  biobanks.initialize();

  var searchElement = $("#BiobankName");

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
      source: biobanks.ttAdapter(),
      limit: 20,
    }
  );
});
