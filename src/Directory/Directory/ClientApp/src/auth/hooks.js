import { useContext } from "react";
import { AuthServiceContext, AuthStateContext } from "./Context";

/**
 * Provides Authentication Service State,
 * such as the current user, their access token etc.
 */
export const useAuth = () => useContext(AuthStateContext);

/**
 * Provides access to methods and configured properties
 * of the Authentication Service
 * e.g. methods for signing in and out, or redirect URLs
 */
export const useAuthService = () => useContext(AuthServiceContext);
