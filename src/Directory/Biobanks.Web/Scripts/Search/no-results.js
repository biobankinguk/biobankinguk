$(function () {
    var suggestionsdiv = $("#typeahead-suggestions");
    $.ajax({
        url: "/Search/SearchDiagnoses",
    });
});
