export const metaRedirect = () =>
    window.location.href = document
        .querySelector("meta[http-equiv=refresh]")
        .getAttribute("data-url");

export const redirect = url => { if (url) window.location = url; };