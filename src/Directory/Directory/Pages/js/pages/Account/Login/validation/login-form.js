import object from "yup/lib/object";
import string from "yup/lib/string";

export default object().shape({
  Username: string()
    .test(
      "valid-username",
      "Please enter a valid email address.",
      v =>
        string()
          .email()
          .isValidSync(v) ||
        string()
          .matches(/@localhost$/)
          .isValidSync(v)
    )
    .required("Please enter your account email address."),
  Password: string().required("Please enter your account password.")
});
