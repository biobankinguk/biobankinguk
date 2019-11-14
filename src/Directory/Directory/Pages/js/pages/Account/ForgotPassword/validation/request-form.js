import * as Yup from "yup";
const { string, object } = Yup;

export default object().shape({
  Email: string()
    .email("Please enter a valid email address.")
    .required("Please enter your email address.")
});
