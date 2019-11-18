import { RAZOR } from "apps/root-ids";
import React from "react";
import ReactDOM from "react-dom";
import merge from "lodash-es/merge";
import { theme } from "@chakra-ui/core";
import ukcrcTheme from "theme";
import routes from "./routes";
import App from "./App";

/*
 * Below is where the actual React bootstrapping happens.
 * It shouldn't need touching often
 */

const root = document.getElementById(RAZOR);

if (root && root.dataset.route) {
  const { route } = root.dataset;
  routes(route).then(({ default: page }) => {
    if (page) {
      merge(theme, ukcrcTheme);
      ReactDOM.render(<App theme={theme} page={page} />, root);
    } else {
      console.error(`No configured React route was found matching: ${route}`);
    }
  });
}
