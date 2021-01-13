// Because imports are hoisted,
// any imports that depend on globals
// will fail if in the same file as the global assignment

// This file assigns globals, so imports in `index.js` (and elsewhere!) can use them

// This pretty much only affects jQuery dependents

import $ from  "jquery";

window.$ = $;
window.jQuery = $;