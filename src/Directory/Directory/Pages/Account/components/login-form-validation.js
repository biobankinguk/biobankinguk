import object from "yup/lib/object";
import string from "yup/lib/string";

export default object().shape({
  Username: string()
    .test(
      "valid-username",
      "Invalid email",
      v =>
        string()
          .email()
          .isValidSync(v) ||
        string()
          .matches(/@localhost$/)
          .isValidSync(v)
    )
    .required("Required"),
  Password: string().required("Required")
});
