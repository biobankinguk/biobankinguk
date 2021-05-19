$(function () {
    var fromapi = $("#FromApi").val().toLowerCase();
    
    if (fromapi == "true") {
        // set text inputs except description to read only
        $('input[type=text], input[type=number], textarea:not(#Description)').attr("readonly", true);
        //disable unselected radio buttons
        $(':radio:not(:checked)').attr('disabled', true);
        //disable checkboses
        $(':checkbox').attr('disabled', true);
    }
});