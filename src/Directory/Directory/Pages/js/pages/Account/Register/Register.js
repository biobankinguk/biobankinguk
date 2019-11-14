import React from "react";
import Layout from "@/layouts/Clean";
import UnconfirmedAccountFound from "@/components/UnconfirmedAccountFound";
import ModelValidationSummary from "@/components/ModelValidationSummary";
import TryThisAlert from "@/components/TryThisAlert";
import RegisterForm from "./components/RegisterForm";
import ConditionalContent from "@/components/ConditionalContent";

const Register = vm => (
  <Layout heading="Register">
    <TryThisAlert
      my={2}
      text="Already have an account?"
      linkText="Log in"
      href="/auth/login"
    />

    <ConditionalContent
      condition={vm.AllowResend}
      trueRender={() => <UnconfirmedAccountFound username={vm.Email} />}
      falseRender={() => <ModelValidationSummary errors={vm.ModelState} />}
    />
    
    <RegisterForm {...vm} />
  </Layout>
);

export default Register;
