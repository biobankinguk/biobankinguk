function copyText(fieldId) {
    /* Get the text field */
    var source = document.getElementById(fieldId);

    /* Select the text field */
    source.select();
    source.setSelectionRange(0, 99999); /* For mobile devices */

    /* Copy the text inside the text field */
    document.execCommand("copy");
}

//wire up bootbox confirmation
$(".copy-text").click(function (e) {
    e.preventDefault();
    var $btn = $(this);
    var txtbox = $btn.data("target")
    copyText(txtbox);
    /* Alert the copied text */
    alert("Copied the text: " + source.value);
});