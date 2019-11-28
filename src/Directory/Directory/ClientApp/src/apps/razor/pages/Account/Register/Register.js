import React from "react";
import Layout from "layouts/Clean";
import UnconfirmedAccountFound from "components/UnconfirmedAccountFound";
import ModelValidationSummary from "components/ModelValidationSummary";
import TryThisAlert from "components/TryThisAlert";
import RegisterForm from "./components/RegisterForm";
import Conditional from "components/Conditional";

const Register = vm => (
  <Layout heading="Register">
    <TryThisAlert
      my={2}
      text="Already have an account?"
      linkText="Log in"
      href="/auth/login"
    />

    <Conditional expression={vm.AllowResend}>
      <UnconfirmedAccountFound username={vm.Email} />
      <ModelValidationSummary default errors={vm.ModelState} />
    </Conditional>

    <RegisterForm {...vm} />
  </Layout>
);

export default Register;
