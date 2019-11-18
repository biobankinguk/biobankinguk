import brand from "./brand-colors.json";
export { brand };

/**
 * A color dictionary for the site theme.
 *
 * These will be turned into shaded palettes for components to use.
 */
export const site = {
  primary: "#0066bf", // Darker than brand blue, based on the old directory's primary
  accent: brand.brightOrange,
  secondary: brand.secondary.brightBlue,
  success: brand.secondary.green,
  danger: brand.pale.brightOrange,
  warning: brand.pale.goldenYellow,
  info: brand.pale.secondary.brightBlue,
  light: brand.pale.charcoalGrey,
  dark: brand.charcoalGrey
};

/**
 * Some extra color literals to make available in the theme
 */
export const literals = {
  black: "#111",
  orangeFull: "#dd8500",
  orangeMid: "#ecae51",
  orangePale: "#fbd8a2"
};
