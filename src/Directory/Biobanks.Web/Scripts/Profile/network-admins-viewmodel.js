var NetworkAdminsViewModel = (function () {
    function NetworkAdminsViewModel() {
        var _this = this;
        this.getAdmins = function (networkId) {
            $.ajax({
                url: "/Profile/GetNetworkAdminsAjax/",
                data: { networkId: networkId, excludeCurrentUser: true, timeStamp: Date.now() },
                contentType: "application/json",
                success: function (data) {
                    _this.admins.removeAll(); //empty the array since we'll be re-popping it from the ajax data
                    for (var _i = 0; _i < data.length; _i++) {
                        var admin = data[_i];
                        _this.admins.push(new RegisterEntityAdminViewModel(admin.UserId, admin.UserFullName, admin.UserEmail, admin.EmailConfirmed));
                    }
                }
            });
        };
        this.openInviteDialog = function () {
            $.ajax({
                url: "/Profile/InviteNetworkAdminAjax/",
                data: { networkId: _this.networkId },
                contentType: "application/html",
                success: function (content) {
                    //clear form errors (as these are in the page's ko model)
                    _this.dialogErrors.removeAll();
                    profileVM.cleanNodeJquerySafe(_this.elements.modal);
                    //populate the modal with the form
                    $(_this.elements.modal).html(content);
                    //apply ko bindings to the ajax'd elements
                    ko.applyBindings(profileVM, $(_this.elements.modal)[0]);
                    //wire up the form submission
                    $(_this.elements.form).submit(function (e) { return _this.submitInviteDialog(e); });
                    //intialise jQuery Validation for the new elements
                    $(_this.elements.form).validate({
                        rules: {
                            Name: "required",
                            Email: {
                                required: true,
                                email: true
                            }
                        }
                    });
                }
            });
        };
        this.submitInviteDialog = function (e) {
            e.preventDefault();
            var form = $(_this.elements.form);
            $.ajax({
                type: "POST",
                url: form.data("action"),
                data: form.serialize(),
                success: function (data) {
                    //clear form errors (as these are in the page's ko model)
                    _this.dialogErrors.removeAll();
                    if (data.success) {
                        $(_this.elements.modal).modal("hide");
                        _this.admins.push(new RegisterEntityAdminViewModel(data.userId, data.name, data.email, data.emailConfirmed));
                    }
                    else {
                        if (Array.isArray(data.errors)) {
                            for (var _i = 0, _a = data.errors; _i < _a.length; _i++) {
                                var error = _a[_i];
                                _this.dialogErrors.push(error);
                            }
                        }
                    }
                }
            });
        };
        this.deleteAdmin = function (admin) {
            bootbox.confirm("Are you sure want to remove " + admin.name() + " as a Network Admin?", function (result) {
                if (result) {
                    $.ajax({
                        type: "POST",
                        url: "/Profile/DeleteNetworkAdminAjax/",
                        data: { NetworkUserId: admin.userId, NetworkId: _this.networkId },
                        success: function (data) {
                            if (data.success) {
                                _this.admins.remove(admin);
                            }
                        }
                    });
                }
            });
        };
        this.resendConfirm = function (admin) {
            bootbox.confirm(admin.name() + " (" + admin.email() + ") has not yet confirmed their account.<br/>Do you want to resend their invitation link?", function (result) {
                if (result) {
                    window.location.href = "/Auth/ResendConfirmLink?userEmail=" + admin.email() + "&onBehalf=True&returnUrl=%2FProfile";
                }
            });
        };
        var base = "NetworkAdmin";
        this.elements = {
            id: "#NetworkModel_NetworkId",
            modal: "#modalInvite" + base,
            form: "#modalInvite" + base + "Form"
        };
        this.admins = ko.observableArray([]);
        this.dialogErrors = ko.observableArray([]);
        this.networkId = $(this.elements.id).val();
        this.getAdmins(this.networkId);
    }
    return NetworkAdminsViewModel;
})();
//# sourceMappingURL=network-admins-viewmodel.js.map