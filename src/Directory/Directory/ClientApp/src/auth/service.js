import { UserManager } from "oidc-client";
import { Results } from "constants/oidc";

const defaultArgs = { useReplaceToNavigate: true };

/**
 * A service that abstracts OIDC client use
 * to authenticate and provide user helpers.
 *
 * Can be subscribed to to notify on user changes,
 * though in React the provided Context and Hooks should be used instead.
 */
export class AuthService {
  _callbacks = [];
  _nextSubscriptionId = 0;
  _user = null;

  constructor(config) {
    this.userManager = new UserManager(config.oidc);
    this.userManager.events.addUserSignedOut(async () => {
      await this.userManager.removeUser();
      this.updateState(undefined);
    });

    this.unauthorised_uri = config.unauthorized_uri;
  }

  /**
   * Update the service state and notify subscribers.
   * @param {*} user
   */
  updateState = user => {
    this._user = user;
    this.notifySubscribers();
  };

  /**
   * Is the current user authenticated?
   */
  isAuthenticated = async () => !!(await this.getUserProfile());

  /**
   * Get the stored user state
   */
  getUser = async () => this._user || (await this.userManager.getUser());

  /**
   * Get the current user profile.
   */
  getUserProfile = async () => {
    const user = await this.getUser();
    return user && user.profile;
  };

  /**
   * Get an API access token via the current user.
   */
  getAccessToken = async () => {
    const user = await this.getUser();
    return user && user.access_token;
  };

  /**
   * Sign in via OIDC
   */
  signIn = async ({ returnUrl }) => {
    try {
      // We try to see if we can authenticate the user silently.
      // This happens when the user is already logged in on the IdP
      // and is done using a hidden iframe on the client.
      const silentUser = await this.userManager.signinSilent(defaultArgs);
      this.updateState(silentUser);
      return {
        status: Results.Success,
        state: { returnUrl }
      };
    } catch (silentError) {
      // It's possible to do a popup auth window here, but that feels more
      // appropriate for third-party apps, not first party (but external) apps.

      // Silent sign in failed; redirect to the IdP for traditional sign in flow
      try {
        await this.userManager.signinRedirect({
          ...defaultArgs,
          data: returnUrl
        });
        return { status: Results.Redirect };
      } catch (signInError) {
        console.log("Sign In Error: ", signInError);
        return { status: Results.Fail, signInError };
      }
    }
  };

  /**
   * Complete an OIDC SignIn
   */
  completeSignIn = async ({ url }) => {
    try {
      const user = await this.userManager.signinCallback(url);
      this.updateState(user);
      return {
        status: Results.Success,
        state: user && user.state
      };
    } catch (error) {
      const generalError = "There was an error signing in";
      console.log(generalError, ": ", error);
      return {
        status: Results.Fail,
        message: `${generalError}.`
      };
    }
  };

  /**
   * SignOut via OIDC
   */
  signOut = async ({ returnUrl }) => {
    // PopUp SignOut is an option here, but we don't do it.
    try {
      await this.userManager.signoutRedirect({
        ...defaultArgs,
        data: returnUrl
      });
      return { status: Results.Redirect };
    } catch (signOutError) {
      console.log("Sign Out Error: ", signOutError);
      return { status: Results.Fail, signOutError };
    }
  };

  /**
   * Complete an OIDC SignOut
   */
  completeSignOut = async ({ url }) => {
    try {
      const response = await this.userManager.signoutCallback(url);
      this.updateState(null);
      return {
        status: Results.Success,
        state: response && response.data
      };
    } catch (error) {
      console.log("There was an error signing out:", error);
      return { status: Results.Fail, error };
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

export default AuthService;
