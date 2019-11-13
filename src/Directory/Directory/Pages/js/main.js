import React from "react";
import ReactDOM from "react-dom";
import merge from "lodash-es/merge";
import { theme } from "@chakra-ui/core";
import ukcrcTheme from "../../../../theme/dist/theme";
import routes from "./routes";
import App from "./App";

/*
 * Below is where the actual React bootstrapping happens.
 * It shouldn't need touching often
 */
const root = document.getElementById("react-app");

if (root && root.dataset.route) {
  const { route } = root.dataset;
  if (routes.hasOwnProperty(route)) {
    merge(theme, ukcrcTheme);
    ReactDOM.render(
      <App theme={theme} page={routes[route]} />,
      document.getElementById("react-app")
    );
  }
}
