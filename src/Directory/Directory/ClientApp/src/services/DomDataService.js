export const getBaseUrl = () =>
  document
    .getElementsByTagName("base")[0]
    .getAttribute("href")
    .replace(/\/$/, "") || process.env.REACT_APP_BASE_URL_FALLBACK;
