$(function () {
    var fromapi = $("#FromApi").val().toLowerCase();
    //  this is NOT a boolean value, it's a string because it comes from a form field.
    if (fromapi === "true") {
        // set text inputs except description to read only
        $('input[type=text], input[type=number], textarea:not(#Description)').attr("readonly", true);
        //disable unselected radio buttons
        $(':radio:not(:checked)').attr('disabled', true);
        //disable checkboses
        $(':checkbox').attr('disabled', true);
    }
});