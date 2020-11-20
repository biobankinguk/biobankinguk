var ProfileViewModel = (function () {
    function ProfileViewModel() {
        var _this = this;
        this.toggleSchemeString = function (current) {
            return (current === "http://") ? "https://" : "http://";
        };
        this.toggleBiobankUrlScheme = function () {
            _this.biobankUrlScheme(_this.toggleSchemeString(_this.biobankUrlScheme()));
        };
        this.toggleNetworkUrlScheme = function () {
            _this.networkUrlScheme(_this.toggleSchemeString(_this.networkUrlScheme()));
        };
        this.cleanNodeJquerySafe = function (nodeSelector) {
            //clear knockout bindings,
            //but leave jQuery/bootstrap bindings intact!
            var original = ko.utils.domNodeDisposal["cleanExternalData"];
            ko.utils.domNodeDisposal["cleanExternalData"] = function () { };
            ko.cleanNode($(nodeSelector)[0]); //designed to work with ID selectors, so only does the first match
            ko.utils.domNodeDisposal["cleanExternalData"] = original;
        };
        this.biobankAdminsModel = new BiobankAdminsViewModel();
        this.networkAdminsModel = new NetworkAdminsViewModel();
        this.biobankUrlScheme = ko.observable($("#BiobankModel_UrlScheme").val());
        this.networkUrlScheme = ko.observable($("#NetworkModel_UrlScheme").val());
    }
    return ProfileViewModel;
})();
$(function () {
    profileVM = new ProfileViewModel();
    ko.applyBindings(profileVM);
});
