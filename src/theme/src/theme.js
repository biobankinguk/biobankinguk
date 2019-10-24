import { literals, site, brand } from "./data/colors";
import buildPalettes from "./build/color-palette";

export default {
  colors: {
    literals,
    brand,
    defaultBackground: "#e9ecef",
    ...buildPalettes(site)
  }
};
