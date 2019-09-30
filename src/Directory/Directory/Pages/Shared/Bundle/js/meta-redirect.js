const redirect = () =>
    window.location.href = document
        .querySelector("meta[http-equiv=refresh]")
        .getAttribute("data-url");

export default redirect;