import React from "react";
import Layout from "Components/Layout";
import UnconfirmedAccountFound from "Components/UnconfirmedAccountFound";
import ModelValidationSummary from "Components/ModelValidationSummary";
import TryThisAlert from "Components/TryThisAlert";
import RegisterForm from "./components/RegisterForm";

const Register = vm => {
  let failureAlert;
  if (vm.AllowResend)
    failureAlert = <UnconfirmedAccountFound username={vm.Email} />;
  else failureAlert = <ModelValidationSummary errors={vm.ModelState} />;

  return (
    <Layout heading="Register">
      <TryThisAlert
        my={2}
        text="Already have an account?"
        linkText="Log in"
        href="/auth/login"
      />

      {failureAlert}
      <RegisterForm {...vm} />
    </Layout>
  );
};

export default Register;
