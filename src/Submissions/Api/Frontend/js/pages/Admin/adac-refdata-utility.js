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
      // Returned data should have one of: description, name, or value.
      setAddFeedback(
        data.description || data.name || data.value,
        redirectTo,
        refdata
      );
    },
    error: function (error) {
      adacRefdataVM.dialogErrors.removeAll();
      var message = JSON.parse(error.responseText);
      adacRefdataVM.dialogErrors.push(Object.values(message)[0]);
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
      // Returned data should have one of: description, name, or value.
      setEditFeedback(
        data.description || data.name || data.value,
        redirectTo,
        refdata
      );
    },
    error: function (error) {
      if (error) {
        adacRefdataVM.dialogErrors.removeAll();
        var message = JSON.parse(error.responseText);
        adacRefdataVM.dialogErrors.push(Object.values(message)[0]);
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
      // Returned data should have one of: description, name, or value.
      setDeleteFeedback(
        data.description || data.name || data.value,
        redirectTo,
        refdata
      );
    },
    error: function (error) {
      var message = JSON.parse(error.responseText);
      window.feedbackMessage(Object.values(message)[0], "warning", true);
    },
  });
}

/**
 * Serializes a jQuery form into an object
 * This is more useful than serializeArray
 * @param form: jQuery form
 * @returns object
 */
function serializeFormData(form) {
  var json = {};
  $.each(form.serializeArray(), function () {
    json[this.name] = this.value || "";
  });
  return json;
}
