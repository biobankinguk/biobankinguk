import * as Yup from "yup";

const { string, object, ref } = Yup;

export default object().shape({
  FullName: string().required("Please enter your full name."),
  Email: string()
    .email("Please enter a valid email address.")
    .required("Please enter your email address."),
  EmailConfirm: string()
    .oneOf([ref("Email")], "Both Email Address entries must match.")
    .required("Please confirm your email address."),
  Password: string().required("Please enter a password."),
  PasswordConfirm: string()
    .oneOf([ref("Password")], "Both Password entries must match.")
    .required("Please confirm your password.")
});
