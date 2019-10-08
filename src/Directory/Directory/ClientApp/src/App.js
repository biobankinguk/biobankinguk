import React from "react";
import { Route } from "react-router-dom";

const App = () => (
  <div>
    <Route exact path="/">
      Hello World
    </Route>
    <Route path="/2">
      Hello 2
    </Route>
  </div>
);

export default App;
