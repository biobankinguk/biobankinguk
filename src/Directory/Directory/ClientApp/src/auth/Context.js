import { createContext } from "react";

const AuthContext = createContext({});

export const AuthProvider = AuthContext.Provider;
export default AuthContext;
