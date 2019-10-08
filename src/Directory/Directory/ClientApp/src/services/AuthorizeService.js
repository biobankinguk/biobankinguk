import { UserManager } from "oidc-client";
import { ClientConfig, AuthenticationResultStatus } from "../constants/oidc";

const args = returnUrl => ({
  useReplaceToNavigate: true,
  data: { returnUrl }
});

/**
 * A singleton service that abstracts OIDC client use
 * to authenticate and provide user helpers.
 *
 * Can be subscribed to to notify on user changes.
 */
export class AuthorizeService {
  _callbacks = [];
  _nextSubscriptionId = 0;
  _user = null;
  _isAuthenticated = false;

  constructor() {
    this.userManager = new UserManager(ClientConfig);
    this.userManager.removeUser().then(() => this.updateState(undefined));
  }

  /**
   * Get the initialised singleton instance of the service.
   */
  static get instance() {
    return authorizeService;
  }

  /**
   * Update the service state and notify subscribers.
   * @param {*} user
   */
  updateState = user => {
    this._user = user;
    this._isAuthenticated = !!this.user;
    this.notifySubscribers();
  };

  /**
   * Is the current user authenticated?
   */
  isAuthenticated = async () => !!(await this.getUser());

  /**
   * Get the current user profile.
   */
  getUser = async () => {
    if (this._user && this._user.profile) return this._user.profile;

    const user = await this.userManager.getUser();
    return user && user.profile;
  };

  /**
   * Get an API access token via the current user.
   */
  getAccessToken = async () => {
    const user = await this.userManager.getUser();
    return user && user.access_token;
  };

  /**
   * Sign in via OIDC
   */
  signIn = async returnUrl => {
    try {
      // We try to see if we can authenticate the user silently.
      // This happens when the user is already logged in on the IdP
      // and is done using a hidden iframe on the client.
      const silentUser = await this.userManager.signinSilent(args());
      this.updateState(silentUser);
      return {
        status: AuthenticationResultStatus.Success,
        state: { returnUrl }
      };
    } catch (silentError) {
      // It's possible to do a popup auth window here, but that feels more
      // appropriate for third-party apps, not first party (but external) apps.

      // Silent sign in failed; redirect to the IdP for traditional sign in flow
      try {
        await this.userManager.signinRedirect(args(returnUrl));
        return { status: AuthenticationResultStatus.Redirect };
      } catch (signInError) {
        console.log("Sign In Error: ", signInError);
        return { status: AuthenticationResultStatus.Fail, signInError };
      }
    }
  };

  /**
   * Complete an OIDC SignIn
   */
  completeSignIn = async url => {
    try {
      const user = await this.userManager.signinCallback(url);
      this.updateState(user);
      return {
        status: AuthenticationResultStatus.Success,
        state: user && user.state
      };
    } catch (error) {
      const generalError = "There was an error signing in";
      console.log(generalError, ": ", error);
      return {
        status: AuthenticationResultStatus.Fail,
        message: `${generalError}.`
      };
    }
  };

  /**
   * SignOut via OIDC
   */
  signOut = async returnUrl => {
    // PopUp SignOut is an option here, but we don't do it.
    try {
      await this.userManager.signoutRedirect(args(returnUrl));
      return { status: AuthenticationResultStatus.Redirect };
    } catch (signOutError) {
      console.log("Sign Out Error: ", signOutError);
      return { status: AuthenticationResultStatus.Fail, signOutError };
    }
  };

  /**
   * Complete an OIDC SignOut
   */
  completeSignOut = async url => {
    try {
      const response = await this.userManager.signoutCallback(url);
      this.updateState(null);
      return {
        status: AuthenticationResultStatus.Success,
        state: response && response.data
      };
    } catch (error) {
      console.log("There was an error signing out:", error);
      return { status: AuthenticationResultStatus.Fail, error };
    }
  };

  /*************************
   * Subscription management
   *************************/
  subscribe = callback => {
    this._callbacks.push({
      callback,
      id: this._nextSubscriptionId
    });
    return this._nextSubscriptionId++;
  };

  unsubscribe = id => {
    this._callbacks = this._callbacks.reduce(
      (callbacks, item) => (item.id !== id ? [...callbacks, item] : callbacks),
      []
    );
  };

  notifySubscribers = () => this._callbacks.forEach(cb => cb());
}

const authorizeService = new AuthorizeService(); // init the service (and by extension the OIDC UserManager)
export default authorizeService;
