import { Paths, ApplicationName } from "./constants";
import { WebStorageStateStore } from "oidc-client";

export default {
  oidc: {
    authority: Paths.Origin,
    client_id: "directory-webapp",
    redirect_uri: `${Paths.Origin}${Paths.LoginCallback}`,
    response_type: "code",
    response_mode: "query",
    scope: "openid profile refdata",
    post_logout_redirect_uri: `${Paths.Origin}${Paths.LogoutCallback}`,
    automaticSilentRenew: true,
    includeIdTokenInSilentRenew: true,
    userStore: new WebStorageStateStore({
      prefix: `${ApplicationName}.`
    })
  },
  unauthorized_uri: Paths.Login
};
