var biobanksList = [];

$(function() {
    var biobanks = new Bloodhound({
        datumTokenizer: Bloodhound.tokenizers.whitespace("vval"),
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        remote: {
            url: "/Network/SearchBiobanks?wildcard=%QUERY",
            filter: function(x) {
                return $.map(x, function (item) {
                    return { desc: item.Name };
                });
            },
            wildcard: "%QUERY"
        }
    });

    biobanks.initialize();

    var searchElement = $("#BiobankName");

    searchElement.typeahead({
        hint: false,
        highlight: true,
        minLength: 1,
        autoselect: true
    },
        {
            name: "desc",
            displayKey: "desc",
            source: biobanks.ttAdapter()
        }
    );
});