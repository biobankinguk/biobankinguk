import { WebStorageStateStore } from "oidc-client";
import { getBaseUrl } from "../services/DomDataService";

export const ApplicationName = "UKCRC Tissue Directory";

export const ClientConfig = {
  authority: process.env.REACT_APP_JWT_AUTHORITY,
  client_id: "directory-webapp",
  redirect_uri: `${getBaseUrl()}/callback`,
  response_type: "code",
  scope: "openid profile refdata",
  post_logout_redirect_uri: getBaseUrl(),
  automaticSilentRenew: true,
  includeIdTokenInSilentRenew: true,
  userStore: new WebStorageStateStore({ prefix: ApplicationName })
};

export const AuthenticationResultStatus = {
    Redirect: 'redirect',
    Success: 'success',
    Fail: 'fail'
};