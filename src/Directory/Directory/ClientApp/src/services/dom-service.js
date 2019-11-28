import { QueryParams } from "constants/oidc";

/**
 * Get a return URL from the query string, or the passed `state`.
 *
 * Also validates it as a local URL.
 * @param {*} [state] An object with an optional `returnUrl` property
 */
export const getReturnUrl = state => {
  const params = new URLSearchParams(window.location.search);
  const fromQuery = params.get(QueryParams.ReturnUrl);
  if (fromQuery && !fromQuery.startsWith(`${window.location.origin}/`)) {
    // This is an extra check to prevent open redirects.
    throw new Error(
      "Invalid return url. The return url needs to have the same origin as the current page."
    );
  }
  return (
    (state && state.returnUrl) || fromQuery || `${window.location.origin}/`
  );
};

const siteName = "UKCRC Tissue Directory";
const buildTitle = (title, excludeSiteName = false) => {
  if (!title) return siteName;

  if (!excludeSiteName) return `${title} - ${siteName}`;

  return title;
}; // hello

/**
 * Set `document.title`
 * @param {string} title The "page" title text
 * @param {boolean} excludeSiteName Whether to leave out the name of the site after the page
 *
 * ⚠ If no page title is given, the site name will always be used alone.
 */
export const setTitle = (title, excludeSiteName = false) => {
  document.title = buildTitle(title, excludeSiteName);
};
