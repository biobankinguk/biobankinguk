import React from "react";
import Layout from "../../Shared/Layout";
import RegisterForm from "../components/RegisterForm";
import TryThisAlert from "../components/TryThisAlert";
import ModelValidationSummary from "../components/ModelValidationSummary";
import UnconfirmedAccountFound from "../components/UnconfirmedAccountFound";

const Register = vm => {
  let failureAlert;
  if (vm.AllowResend)
    failureAlert = (
      <UnconfirmedAccountFound
        message="This account already exists, but seems to be unconfirmed."
        username={vm.Email}
      />
    );
  else failureAlert = <ModelValidationSummary errors={vm.ModelState} />;

  return (
    <Layout heading="Register">
      <TryThisAlert
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
