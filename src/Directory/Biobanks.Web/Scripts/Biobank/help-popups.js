/*
 * TODO Since there are a lot of these now, should refactor into a generic way.
 * This should include making it unobtrusive ideally. The text should definitely be in HTML.
 */

/*
 * Don't do bootboxes for material type form as this is itself in a
 * bootbox and it doesn't support multiple bootboxes open at the same time
 */

var helpIconBootboxButtons = {
    success: {
        label: "OK",
        className: "btn-primary"
    }
};

$(".diagnosis-help").click(function () {
    bootbox.dialog({
        message: "We are seeking to harmonise the terms used to describe diseases and we are adopting SNOMED-CT in this effort. We are seeking to discuss with the domain experts what terms are most relevant for certain diseases. Therefore, if you do not see a relevant term there, please <a href='mailto:" + $("#support-email").data("email") + "'>get in contact</a>. If you are collecting from healthy volunteers, please use the term 'Fit and Well'.",
        title: "Disease Status & SNOMED",
        buttons: helpIconBootboxButtons
    });
});

$(".help-associateddata").click(function () {
    var $link = $(this);
    bootbox.dialog({
        message: $link.data("message"),
        title: $link.data("title"),
        buttons: helpIconBootboxButtons
    });
});


$(".help-label-resource-name").click(function () {
    bootbox.dialog({
        message: "What is the name of the resource you are registering? It is up to you how you present your resource in the directory.<br/>",
        title: "Name",
        buttons: helpIconBootboxButtons
    });
});

$(".help-label-resource-description").click(function () {
    bootbox.dialog({
        message: "Use this for information that is not captured elsewhere.",
        title: "Desciption",
        buttons: helpIconBootboxButtons
    });
});

$(".help-label-resource-url").click(function () {
    bootbox.dialog({
        message: "If your resource has a website or page you can include it here.",
        title: "URL",
        buttons: helpIconBootboxButtons
    });
});

$(".help-label-resource-contactemail").click(function () {
    bootbox.dialog({
        message: "The email address that researchers will use to contact you.",
        title: "Contact Email",
        buttons: helpIconBootboxButtons
    });
});

$(".help-label-resource-contactnumber").click(function () {
    bootbox.dialog({
        message: "The telephone number that researchers will use to contact you.",
        title: "Contact Phone Number",
        buttons: helpIconBootboxButtons
    });
});

$(".help-label-resource-address").click(function () {
    bootbox.dialog({
        message: "Use the address where access to the samples is based rather than where they are stored.",
        title: "Address",
        buttons: helpIconBootboxButtons
    });
});

$(".help-label-resource-institution").click(function () {
    bootbox.dialog({
        message: "The organisation where your governance is managed.",
        title: "Institution",
        buttons: helpIconBootboxButtons
    });
});

$(".help-label-resource-ethicsregistration").click(function () {
    bootbox.dialog({
        message: "IRAS number.",
        title: "Ethics Registration",
        buttons: helpIconBootboxButtons
    });
});

$(".help-label-resource-services").click(function () {
    bootbox.dialog({
        message: "Does your resource offer any extra services? If so indicate that here.",
        title: "Services",
        buttons: helpIconBootboxButtons
    });
});

$(".help-label-resource-datasharingsettings").click(function () {
    bootbox.dialog({
        message: "Some data will be shared with other directories such as that of BBMRI-ERIC’s. Tick to opt-out of this sharing. Contact us to find our more.",
        title: "Data Sharing Settings",
        buttons: helpIconBootboxButtons
    });
});

$(".help-label-resource-reasonsforregistering").click(function () {
    bootbox.dialog({
        message: "Select the reasons for registering your sample resource with the directory. You may select multiple reasons.",
        title: "Reasons for Registering",
        buttons: helpIconBootboxButtons
    });
});

$(".help-label-diseasestatus").click(function () {
    bootbox.dialog({
        message: "We are seeking to harmonise the terms used to describe diseases and we are adopting SNOMED-CT in this effort. We are seeking to discuss with the domain experts what terms are most relevant for certain diseases. Therefore, if you do not see a relevant term there, please <a href=\"" + $("support-email").data("email") + "\">get in contact</a>. If you are collecting from healthy volunteers, please use the term 'Fit and Well'.",
        title: "Disease Status",
        buttons: helpIconBootboxButtons
    });
});

$(".help-label-collection-title").click(function () {
    bootbox.dialog({
        message: "This is an optional field for when your collection has a specific title.",
        title: "Title",
        buttons: helpIconBootboxButtons
    });
});

$(".help-label-collection-description").click(function () {
    bootbox.dialog({
        message: "Open text field where you can describe the collection.",
        title: "Description",
        buttons: helpIconBootboxButtons
    });
});

$(".help-label-collection-yearstarted").click(function () {
    bootbox.dialog({
        message: "The first year samples were collected.",
        title: "Year Started",
        buttons: helpIconBootboxButtons
    });
});

$(".help-label-collection-yearfinished").click(function () {
    bootbox.dialog({
        message: "If this collection is on-going, you can leave this blank.",
        title: "Year Finished",
        buttons: helpIconBootboxButtons
    });
});

$(".help-label-collection-accesscondition").click(function () {
    bootbox.dialog({
        message: "How is access to the samples managed?",
        title: "Access Condition",
        buttons: helpIconBootboxButtons
    });
});

$(".help-label-collection-collectiontype").click(function () {
    bootbox.dialog({
        message: "Select a category that best describes your collection. If you feel one is missing please contact us.",
        title: "Collection Type",
        buttons: helpIconBootboxButtons
    });
});

$(".help-label-collection-collectionstatus").click(function () {
    bootbox.dialog({
        message: "At which point in the process of collecting are you?",
        title: "Collection Status",
        buttons: helpIconBootboxButtons
    });
});

$(".help-label-collection-contentrestrictions").click(function () {
    bootbox.dialog({
        message: "Please highlight any areas of research you do not have consent for.",
        title: "Content Restrictions",
        buttons: helpIconBootboxButtons
    });
});

$(".help-label-associateddata").click(function () {
    bootbox.dialog({
        message: "We want to show what data you have for the sample and how long it would take you to provide it. If you can’t get any of the data listed don’t tick anything. It doesn’t matter how long it takes, it’s just to make the researcher aware.",
        title: "Associated Data",
        buttons: helpIconBootboxButtons
    });
});

$(".help-label-sampleset-sex").click(function () {
    bootbox.dialog({
        message: "Each sample set is divided by sex.",
        title: "Sex",
        buttons: helpIconBootboxButtons
    });
});

$(".help-label-sampleset-agerange").click(function () {
    bootbox.dialog({
        message: "Select the age category that best reflects the sample set.",
        title: "Age Range",
        buttons: helpIconBootboxButtons
    });
});

$(".help-label-sampleset-numberofdonors").click(function () {
    bootbox.dialog({
        message: "Select the approximate range of donor numbers in the sample set.",
        title: "Number of Donors",
        buttons: helpIconBootboxButtons
    });
});

$(".help-label-capability-protocols").click(function () {
    bootbox.dialog({
        message: "Select these if you can offer custom-made SOPs and consent procedures.",
        title: "Protocols",
        buttons: helpIconBootboxButtons
    });
});

$(".help-label-capability-annualdonorexpectation").click(function () {
    bootbox.dialog({
        message: "Please provide an estimate on the number of patients that you could collect samples and data from that have the specified condition in a year.",
        title: "Annual Donor Expectation",
        buttons: helpIconBootboxButtons
    });
});

$(".help-label-adac-storagetemperature-override").click(function () {
    bootbox.dialog({
        message: "This function overrides the site wide " + document.getElementById("StorageTemperatureRefValue").value + " with a specified user input value.",
        title: "Overriding " + document.getElementById("StorageTemperatureRefValue").value,
        buttons: helpIconBootboxButtons
    });
});

$(".help-label-adac-donorcount-override").click(function () {
    bootbox.dialog({
        message: "This function overrides the site wide " + document.getElementById("DonorCountRefValue").value + " with a specified user input value.",
        title: "Overriding " + document.getElementById("DonorCountRefValue").value,
        buttons: helpIconBootboxButtons
    });
});

$(".help-label-adac-macroscopicassessment-override").click(function () {
    bootbox.dialog({
        message: "This function overrides the site wide " + document.getElementById("MacroscopicAssessmentRefValue").value + " with a specified user input value.",
        title: "Overriding " + document.getElementById("MacroscopicAssessmentRefValue").value,
        buttons: helpIconBootboxButtons
    });
});

$(".help-label-submissions-publickey").click(function () {
    bootbox.dialog({
        message: "The Client ID is used to identify the Organisation when making an authentication request to the Submissions API.",
        title: "Client ID",
        buttons: helpIconBootboxButtons
    });
});

$(".help-label-submissions-privatekey").click(function () {
    bootbox.dialog({
        message: "As the Client Secret, it should NOT be shared. The Client Secret is used to identify the Organisation when making an authentication request to the Submissions API.",
        title: "Client Secret",
        buttons: helpIconBootboxButtons
    });
});