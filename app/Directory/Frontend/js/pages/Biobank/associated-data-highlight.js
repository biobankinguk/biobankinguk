// Associated Data Provision Time
$(function() {
    $(".ass-dat-chk").change(function() { onCheckboxChange(this) });
    $(".ass-dat-rad").click(function() { onRadioClick(this) });

    $.each($(".ass-dat-chk"), function(i, obj) {
        onCheckboxChange(obj);
    });
});

var onRadioClick = function (rad) {
    var chk = $(rad).closest("tr").find(".ass-dat-chk");
    chk.prop('checked', true);
    onCheckboxChange(chk[0]);
}

var onCheckboxChange = function (chk) {
    //get table row
    var row = $(chk).closest("tr");

    if (!chk.checked) { //if we've unchecked
        //clear radios
        var radios = row.find(".ass-dat-rad");

        radios.prop('checked', false);

        //remove row highlight
        row.removeClass("success");
    }
    else //if we've checked
        row.addClass("success"); //add row highlight
}