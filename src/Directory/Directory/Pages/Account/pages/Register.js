import React from "react";
import Layout from "../../Shared/Layout";
import RegisterForm from "../components/RegisterForm";
import WrongFormAlert from "../components/WrongFormAlert";

const Register = () => {
  return (
    <Layout heading="Register">
      <WrongFormAlert
        text="Already have an account?"
        linkText="Log in"
        href="Login"
      />

      <RegisterForm />
    </Layout>
  );
};

export default Register;
