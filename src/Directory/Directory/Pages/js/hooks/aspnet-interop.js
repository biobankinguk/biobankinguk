import { constants } from "js-forms";

/**
 * Retrieves and returns useful data from an ASP.NET Core generated `<form>` element.
 *
 * @param {string} [formId] ID of the `<form>`
 */
export const useAspForm = (formId = "asp-form") => {
  const aspForm = document.getElementById(formId);
  const csrfToken = aspForm.elements[constants.aspNetCoreCsrf].value;
  return {
    /** The form action */
    action: aspForm.action,
    /** The ID and value of the field containing a server generated CSRF Token */
    csrf: { [constants.aspNetCoreCsrf]: csrfToken }
  };
};
