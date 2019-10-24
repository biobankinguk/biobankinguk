import * as Yup from "yup";

const { string, object, ref } = Yup;

export default object().shape({
  FullName: string().required(),
  Email: string()
    .email("Invalid email")
    .required(),
  EmailConfirm: string()
    .oneOf([ref("Email")], "Email Address must match.")
    .required(),
  Password: string().required("Required"),
  PasswordConfirm: string()
    .oneOf([ref("Password")], "Passwords must match.")
    .required()
});
