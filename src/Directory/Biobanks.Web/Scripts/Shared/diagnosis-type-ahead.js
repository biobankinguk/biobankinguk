var diseaselist = [];

$(function() {
    var diseases = new Bloodhound({
        datumTokenizer: Bloodhound.tokenizers.whitespace('vval'),
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
            url: '/Search/ListOntologyTerms?wildcard=%QUERY',
            filter: function (x) {
                return $.map(x, function (item) {
                    return {
                        desc: item.Description
                    };
                });
            },
            wildcard: '%QUERY'
        }
    });

    diseases.initialize();

    var searchElement = $('.diagnosis-search');

    searchElement.typeahead({
        hint: false,
        highlight: true,
        minLength: 1,
        autoselect: true
    },
    {
        name: 'desc',
        displayKey: 'desc',
        source: diseases.ttAdapter(),
        limit: 100
    }
    );
});