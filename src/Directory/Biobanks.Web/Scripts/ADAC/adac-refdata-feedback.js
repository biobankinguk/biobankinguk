
function setAddFeedback(name, redirectTo, refdata) {
    // Send Feedback message
    var url = "/ADAC/AddRefDataSuccessFeedbackAjax";
    var data = {
        name: name,
        redirectUrl: redirectTo,
        refDataType: refdata
    };
    window.location.href = url + "?" + $.param(data);
}
function setEditFeedback (name, redirectTo, refdata) {
    // Send Feedback message
    var url = "/ADAC/EditRefDataSuccessFeedbackAjax";
    var data = {
        name: name,
        redirectUrl: redirectTo,
        refDataType: refdata
    };
    window.location.href = url + "?" + $.param(data);
}
function setDeleteFeedback (name, redirectTo, refdata) {
    // Send Feedback message
    var url = "/ADAC/DeleteRefDataSuccessFeedbackAjax";
    var data = {
        name: name,
        redirectUrl: redirectTo,
        refDataType: refdata
    };
    window.location.href = url + "?" + $.param(data);
}