/**
 * All the functionality for the Contact Button that lives in the navbar
 */

var ContactButtonManager = (function () {
  function ContactButtonManager(localStorageContactIdsKey, contactButtonBase) {
    this.localStorageContactIdsKey = localStorageContactIdsKey;
    this.contactButtonBase = contactButtonBase;
    this.contactIds = JSON.parse(
      localStorage.getItem(this.localStorageContactIdsKey)
    );
    // If there wasn't anything in local storage then initialise the array.
    if (this.contactIds == null) {
      this.contactIds = [];
    }
  }
  ContactButtonManager.prototype.Init = function () {
    var _this = this;
    // Initialise the button in the header that shows the number of people who can be contacted.
    this.UpdateContactCounter();
    // Set the buttons' initial styles.
    this.StyleContactButtons();
    // Set up the behaviour of the contact count button.
    $("#ContactCounter").click(function () {
      _this.ContactCounter_Click();
    });
    $(".contact-button").click(function (e) {
      _this.ContactButton_Click(e);
    });
  };
  ContactButtonManager.prototype.PrepareBiobankSlugForUrl = function (slug) {
    var output = slug.replace(" ", "+");
    return encodeURIComponent(output);
  };
  ContactButtonManager.prototype.StyleContactButtons = function () {
    var _this = this;
    $(".contact-button").each(function (index, value) {
      _this.StyleContactButton($(value));
    });
  };
  ContactButtonManager.prototype.StyleContactButton = function (button) {
    if (this.contactIds.indexOf(button.data("biobank-id")) !== -1) {
      // TODO: abstract this into separate function
      button.addClass("active");
      button.removeClass("btn-info");
      button.addClass("btn-danger");
      button.html(
        '<span class="fa fa-envelope-o labelled-icon"></span>Remove from Contact List'
      );
    } else {
      // TODO: abstract this into separate function
      button.removeClass("active");
      button.addClass("btn-info");
      button.removeClass("btn-danger");
      button.html(
        '<span class="fa fa-envelope-o labelled-icon"></span>Add to Contact List'
      );
    }
  };
  ContactButtonManager.prototype.ClearContactList = function () {
    this.contactIds = [];
    this.SaveToLocalStorage();
    this.UpdateContactCounter();
    this.StyleContactButtons();
  };
  ContactButtonManager.prototype.UpdateContactCounter = function () {
    var count = this.contactIds.length;

    // Update counter dropdown
    $("#ContactCounter").html(
      this.contactButtonBase.replace("{count}", String(count))
    );

    // Update counter
    if (count > 9) {
      $(".contact-counter").html("+");
      $(".contact-counter").addClass("active");
    } else if (count > 0) {
      $(".contact-counter").html(count);
      $(".contact-counter").addClass("active");
    } else {
      $(".contact-counter").empty();
      $(".contact-counter").removeClass("active");
    }
  };
  ContactButtonManager.prototype.ContactCounter_Click = function () {
    if (this.contactIds.length > 0) {
      // If any contacts, redirect to the contact view
      window.location.href = "/Home/Contact/";
    } else {
      // if no contacts, pop an alert explaining the functionality
      var dialog_1 = bootbox.dialog({
        message:
          "In order to contact resources, please use the search box to identify " +
          "the resources holding samples that you are interested in.<br/><br/>" +
          'Then click on the <span class="fa fa-envelope-o"></span> Contact buttons ' +
          "in the search results to select the resources you wish to contact.",
        title: "Contacting resources",
        buttons: {
          success: {
            label: "OK",
            className: "btn-primary",
          },
        },
      });
      dialog_1.on("shown.bs.modal", function () {
        dialog_1.attr("id", "contacting-resources-modal");
      });
    }
  };
  ContactButtonManager.prototype.ContactButton_Click = function (e) {
    // Stop the linking being processed.
    e.preventDefault();
    var button = $(e.target);
    //if the icon span was clicked, get the parent button, then continue
    if (button.is("span")) button = button.parent();
    // Get the biobank ID.
    var biobankId = button.data("biobank-id");
    if (this.contactIds.indexOf(biobankId) === -1) {
      // If the id isn't in the array already then add it.
      this.contactIds.push(biobankId);
      // TODO: abstract this into separate function
      button.addClass("active");
      button.removeClass("btn-info");
      button.addClass("btn-danger");
      button.html(
        '<span class="fa fa-envelope-o labelled-icon"></span>Remove from Contact List'
      );
      window.feedbackMessage(
        "Resource added to the contact list. Access the contact list from the button at the top right.",
        "success"
      );
    } else {
      // Otherwise, remove it.
      var index = this.contactIds.indexOf(biobankId);
      if (index !== -1) {
        this.contactIds.splice(index, 1);
        // TODO: abstract this into separate function
        button.removeClass("active");
        button.addClass("btn-info");
        button.removeClass("btn-danger");
        button.html(
          '<span class="fa fa-envelope-o labelled-icon"></span>Add to Contact List'
        );
      }
      window.feedbackMessage("Resource removed from contact list.", "success");
    }
    this.SaveToLocalStorage();
    this.UpdateContactCounter();
  };
  ContactButtonManager.prototype.SaveToLocalStorage = function () {
    localStorage.setItem(
      this.localStorageContactIdsKey,
      JSON.stringify(this.contactIds)
    );
  };
  return ContactButtonManager;
})();
$(function () {
  var cbm = new ContactButtonManager(
    "contactIds",
    '<span class="fa fa-envelope-o labelled-icon"></span>Contact List ({count})'
  );
  cbm.Init();
});
window.onunload = function () {
  //firefox back button fix
};
//Safari back button fix
$(window).bind("pageshow", function (event) {
  if (event.originalEvent["persisted"]) {
    window.location.reload();
  }
});
