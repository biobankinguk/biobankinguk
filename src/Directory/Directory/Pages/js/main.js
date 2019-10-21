import { metaRedirect, redirect } from "./redirect";
import "./routes";

// create a global namespace (if it's not already present),
// to be consistent for anything that needs it.
// NOTE: globals should be set here, so it's clear and explicit
// not inside modules
window.__UKCRC_TDCC__ = {} || window.__UKCRC_TDCC__;

window.__UKCRC_TDCC__.MetaRedirect = metaRedirect;
window.__UKCRC_TDCC__.Redirect = redirect;
