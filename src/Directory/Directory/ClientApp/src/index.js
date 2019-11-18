import * as roots from "apps/root-ids";
import * as serviceWorker from "./serviceWorker";

// Work out what which App we bootstrapping
const razor = document.getElementById(roots.RAZOR);
const spa = document.getElementById(roots.SPA);

if (!!razor) import("apps/razor");
if (!!spa) import("apps/spa");

// If you want your app to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: https://bit.ly/CRA-PWA
serviceWorker.unregister();
