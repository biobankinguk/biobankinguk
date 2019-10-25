import React from "react";
import Layout from "../../Shared/Layout";
import RegisterForm from "../components/RegisterForm";
import WrongFormAlert from "../components/WrongFormAlert";
import AspValidationSummary from "../components/AspValidationSummary";

const Register = () => {
  return (
    <Layout heading="Register">
      <WrongFormAlert
        text="Already have an account?"
        linkText="Log in"
        href="/auth/login"
      />

      <AspValidationSummary />
      <RegisterForm />
    </Layout>
  );
};

export default Register;
