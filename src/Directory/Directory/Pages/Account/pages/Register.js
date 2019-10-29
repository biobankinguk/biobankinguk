import React from "react";
import Layout from "../../Shared/Layout";
import RegisterForm from "../components/RegisterForm";
import WrongFormAlert from "../components/WrongFormAlert";
import ModelValidationSummary from "../components/ModelValidationSummary";

const Register = vm => {
  return (
    <Layout heading="Register">
      <WrongFormAlert
        text="Already have an account?"
        linkText="Log in"
        href="/auth/login"
      />

      <ModelValidationSummary errors={vm.ModelState} />
      <RegisterForm {...vm} />
    </Layout>
  );
};

export default Register;
