
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

function addRefData(adacRefdataVM, url, data, redirectTo, refdata) {
    // Make AJAX Call
    $.ajax({
        url: url,
        type: 'POST',
        dataType: 'json',
        data: data,
        success: function (data, textStatus, xhr) {
            adacRefdataVM.dialogErrors.removeAll();
            if (data.success) {
                adacRefdataVM.hideModal();
                setAddFeedback(data.name,
                    redirectTo, refdata);
            }
            else {
                if (Array.isArray(data.errors)) {
                    data.errors.forEach(function (error, index) {
                      adacRefdataVM.dialogErrors.push(error);
                    })
                }
            }
        }
    });
}

function editRefData(adacRefdataVM, url, data, redirectTo, refdata) {
    // Make AJAX Call
    $.ajax({
        url: url,
        type: 'PUT',
        dataType: 'json',
        data: data,
        success: function (data, textStatus, xhr) {
            adacRefdataVM.dialogErrors.removeAll();
            if (data.success) {
                adacRefdataVM.hideModal();
                setEditFeedback(data.name,
                    redirectTo, refdata);
            }
            else {
                if (Array.isArray(data.errors)) {
                    data.errors.forEach(function (error, index) { 
                      adacRefdataVM.dialogErrors.push(error);
                    })
                }
            }
        }
    });
}

function deleteRefData(url, redirectTo, refdata) {
    // Make AJAX Call
    $.ajax({
        url: url,
        type: 'DELETE',
        success: function (data, textStatus, xhr) {
            if (data.success) {
                setDeleteFeedback(data.name,
                    redirectTo, refdata)
            }
            else {
                if (Array.isArray(data.errors)) {
                    if (data.errors.length > 0) {
                        window.feedbackMessage(data.errors[0], "warning");
                    }
                }
            }
        }
    });
}
