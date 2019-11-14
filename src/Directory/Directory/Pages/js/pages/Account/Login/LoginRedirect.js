import React, { useEffect } from "react";
import Layout from "@/layouts/Clean";
import BasicAlert from "@/components/BasicAlert";

const LoginRedirect = ({ RedirectUrl }) => {
  useEffect(() => {
    window.location.href = RedirectUrl;
  });

  return (
    <Layout heading="Login">
      <BasicAlert title="You are now being returned to the application.">
        Once complete, you may close this tab
      </BasicAlert>
    </Layout>
  );
};

export default LoginRedirect;
