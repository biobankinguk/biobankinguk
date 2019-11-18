import * as Yup from "yup";

const { string, object, ref } = Yup;

export default object().shape({
  Password: string().required("Please enter a password."),
  PasswordConfirm: string()
    .oneOf([ref("Password")], "Both Password entries must match.")
    .required("Please confirm your password.")
});
