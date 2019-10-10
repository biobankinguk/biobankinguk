import { QueryParams } from "../constants/oidc";

/**
 * Get the base URL of the App.
 *
 * Tries to get an absolute URL from the `<base />` element.
 * If that doesn't work, or the result is simply `/`,
 * falls back to `env.REACT_APP_BASE_URL_FALLBACK`.
 */
export const getBaseUrl = () =>
  document
    .getElementsByTagName("base")[0]
    .getAttribute("href")
    .replace(/\/$/, "") || process.env.REACT_APP_BASE_URL_FALLBACK;

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
