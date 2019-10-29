import React from "react";
import Layout from "../../Shared/Layout";
import RegisterForm from "../components/RegisterForm";
import TryThisAlert from "../components/TryThisAlert";
import ModelValidationSummary from "../components/ModelValidationSummary";

const Register = vm => {
  return (
    <Layout heading="Register">
      <TryThisAlert
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
