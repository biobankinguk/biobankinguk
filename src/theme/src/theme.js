import { theme } from "@chakra-ui/core";
import { literals, site, brand } from "./data/colors";
import buildPalettes from "./build/color-palette";

export default {
  ...theme,

  colors: {
    ...theme.colors,
    literals,
    brand,
    defaultBackground: theme.colors.gray["200"],
    ...buildPalettes(site)
  },

  // Chakra's icons don't survive our current build process
  // but we can use other libraries such as `react-icons`
  // in the actual app, and the app build will tree shake
  icons: undefined
};
