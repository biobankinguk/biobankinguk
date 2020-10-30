function RegisterEntityAdminViewModel(userId, name, email, emailConfirmed) {
  this.userId = ko.observable(userId);
  this.name = ko.observable(name);
  this.email = ko.observable(email);
  this.emailConfirmed = ko.observable(emailConfirmed);
}
