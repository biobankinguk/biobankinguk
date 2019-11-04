import path from "path";
import { CleanWebpackPlugin } from "clean-webpack-plugin";

export default {
  entry: "./Pages/js/main.js",
  output: {
    path: path.resolve(__dirname, "../../wwwroot/dist"),
    filename: "bundle.js"
  },
  devtool: "source-map",
  module: {
    rules: [
      {
        test: /\.js$/,
        exclude: /node_modules/,
        use: ["babel-loader"]
      }
    ]
  },
  plugins: [new CleanWebpackPlugin()],
  resolve: {
    alias: {
      react: "preact/compat",
      "react-dom/test-utils": "preact/test-utils",
      "react-dom": "preact/compat"
    }
  }
};
