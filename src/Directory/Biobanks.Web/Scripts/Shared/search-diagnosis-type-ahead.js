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
            url: '/Search/SearchSnomedTerms',
            replace: function(url, query) {
                var type = $("input[name='searchRadio']:checked").val();
                return url += '?searchDocumentType=' + type + '&wildcard=' + query;
            },
            filter: function (x) {
                return $.map(x, function (item) {
                    return {
                        desc: item.Description
                    };
                });
            }
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