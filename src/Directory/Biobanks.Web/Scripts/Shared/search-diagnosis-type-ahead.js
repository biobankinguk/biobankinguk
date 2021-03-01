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
            url: '/Search/SearchOntologyTerms',
            replace: function(url, query) {
                var type = $("input[name='searchRadio']:checked").val();
                return url += '?searchDocumentType=' + type + '&wildcard=' + query;
            },
            filter: function (x) {
                return $.map(x, function (item) {
                    return {
                        desc: item.Description,
                        other: item.OtherTerms,
                        mother: item.MatchingOtherTerms.join(', '),
                        nother: item.NonMatchingOtherTerms.join(', ')
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
        limit: 100,
        templates: {
            suggestion: function (e) {
                return '<div class="search-list"><b>' + e.desc +
                    '</b><div style="font-size:small">' +
                    (!e.mother ? "" : ('<span>' + e.mother + '</span><br/>')) +
                    (!e.nother ? "" : ('<span>(...' + e.nother + ')</span>')) + '</div></div>';
            }
        }
    }
    );
});