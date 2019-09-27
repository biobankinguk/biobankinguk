import path from "path";
import MiniCssExtractPlugin from "mini-css-extract-plugin";
import { CleanWebpackPlugin } from "clean-webpack-plugin";

export default {
    mode: "production",
    entry: "./Pages/Shared/Bundle/bundle.js",
    output: {
        path: path.resolve(__dirname, "../../../wwwroot/dist"),
        filename: "bundle.js"
    },
    devtool: "source-map",
    module: {
        rules: [
            {
                test: /\.js$/,
                exclude: /node_modules/,
                use: ["babel-loader"]
            },
            //{
            //    test: /\.(png|jpg)$/,
            //    loader: "url-loader",
            //    options: {
            //        limit: 100
            //    }
            //},
            {
                test: /\.scss/,
                use: [
                    {
                        loader: MiniCssExtractPlugin.loader,
                        options: { sourceMap: true }
                    },
                    {
                        loader: "css-loader",
                        options: { sourceMap: true }
                    },
                    {
                        loader: "sass-loader",
                        options: { sourceMap: true }
                    }
                ]
            }
        ]
    },
    plugins: [
        new MiniCssExtractPlugin({
            filename: "bundle.css"
        }),
        new CleanWebpackPlugin()
    ]
}