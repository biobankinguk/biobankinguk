function setAddFeedback(name, redirectTo, refdata) {
  // Send Feedback message
  var url = "/Admin/ReferenceData/AddRefDataSuccessFeedbackAjax";
  var data = {
    name: name,
    redirectUrl: redirectTo,
    refDataType: refdata,
  };
  window.location.href = url + "?" + $.param(data);
}
function setEditFeedback(name, redirectTo, refdata) {
  // Send Feedback message
  var url = "/Admin/ReferenceData/EditRefDataSuccessFeedbackAjax";
  var data = {
    name: name,
    redirectUrl: redirectTo,
    refDataType: refdata,
  };
  window.location.href = url + "?" + $.param(data);
}
function setDeleteFeedback(name, redirectTo, refdata) {
  // Send Feedback message
  var url = "/Admin/ReferenceData/DeleteRefDataSuccessFeedbackAjax";
  var data = {
    name: name,
    redirectUrl: redirectTo,
    refDataType: refdata,
  };
  window.location.href = url + "?" + $.param(data);
}

function addRefData(adacRefdataVM, url, data, redirectTo, refdata) {
  // Make AJAX Call
  $.ajax({
    url: url,
    type: "POST",
    contentType: "application/json; charset=utf-8",
    dataType: "json",
    data: JSON.stringify(data),
    success: function (data, textStatus, xhr) {
      adacRefdataVM.dialogErrors.removeAll();
      adacRefdataVM.hideModal();
      setAddFeedback(data.name, redirectTo, refdata);
    },
    error: function (error) {
      if (error) {
        adacRefdataVM.dialogErrors.removeAll();
        adacRefdataVM.dialogErrors.push(error);
      }
    },
  });
}

function editRefData(adacRefdataVM, url, data, redirectTo, refdata) {
  // Make AJAX Call
  $.ajax({
    url: url,
    type: "PUT",
    contentType: "application/json; charset=utf-8",
    dataType: "json",
    data: JSON.stringify(data),
    success: function (data, textStatus, xhr) {
      adacRefdataVM.dialogErrors.removeAll();
      adacRefdataVM.hideModal();
      setEditFeedback(data.name, redirectTo, refdata);
    },
    error: function (error) {
      if (error) {
        adacRefdataVM.dialogErrors.removeAll();
        adacRefdataVM.dialogErrors.push(error);
      }
    },
  });
}

function deleteRefData(url, redirectTo, refdata) {
  // Make AJAX Call
  $.ajax({
    url: url,
    type: "DELETE",
    success: function (data, textStatus, xhr) {
      setDeleteFeedback(data.name, redirectTo, refdata);
    },
    error: function (error) {
      if (error) {
        window.feedbackMessage(error, "warning", false);
      }
    },
  });
}

/**
 * Serializes a form into a JSON object
 * @returns json object
 * @param form
 */
function serializeFormData(form) {
  var json = {};
  $.each(form.serializeArray(), function () {
    json[this.name] = this.value || "";
  });
  return json;
}
