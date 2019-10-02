// create a global namespace, to be consistent
// for anything that needs it
// NOTE: globals should be set here, so it's clear and explicit
// not inside modules
window.__UKCRC_TDCC__ = {};

import "jquery-validation-unobtrusive";

import Redirect from "./meta-redirect";
window.__UKCRC_TDCC__.Redirect = Redirect;