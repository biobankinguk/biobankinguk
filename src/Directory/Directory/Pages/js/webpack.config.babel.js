import path from "path";
import { CleanWebpackPlugin } from "clean-webpack-plugin";

// TODO: move IE config to separate file
// since IE only works in prod builds anyway...
// and we can use different ie-specific babel config then,
// saving us from unnecessary transpiling for real browsers

export default {
  entry: {
    ie: "./Pages/js/main-ie.js",
    bundle: "./Pages/js/main.js"
  },
  output: {
    path: path.resolve(__dirname, "../../wwwroot/dist"),
    filename: "[name].js",
    publicPath: "/dist/"
  },
  devtool: "source-map",
  module: {
    rules: [
      {
        test: /\.js$/,
        exclude: /node_modules/,
        use: ["babel-loader", "eslint-loader"]
      }
    ]
  },
  plugins: [new CleanWebpackPlugin()],
  resolve: {
    alias: {
      react: "preact/compat",
      "react-dom/test-utils": "preact/test-utils",
      "react-dom": "preact/compat",
      "@": path.resolve(__dirname, "./"),
      Theme: path.resolve(__dirname, "../../../../theme/dist/theme")
    }
  }
};
