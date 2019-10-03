// create a global namespace, to be consistent
// for anything that needs it
// NOTE: globals should be set here, so it's clear and explicit
// not inside modules
window.__UKCRC_TDCC__ = {};

import "jquery-validation-unobtrusive";

import { metaRedirect, redirect } from "./redirect";
window.__UKCRC_TDCC__.MetaRedirect = metaRedirect;
window.__UKCRC_TDCC__.Redirect = redirect;