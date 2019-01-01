const path = require("path");
const webpack = require("webpack");
const MinifyPlugin = require("terser-webpack-plugin");

var CONFIG = {
    fsharpEntry: 
        ["whatwg-fetch",
            "@babel/polyfill",
            "./src/Client/Client.fsproj"
        ],
    outputDir: './public',
    devServerPort: undefined,
    devServerProxy: {
        '/api/*': {
            target: 'http://localhost:' + (process.env.SUAVE_FABLE_PORT || "8085"),
            changeOrigin: true
        }
    },
    contentBase: path.join(__dirname, './public'),
    babel: {
        presets: [
            ["@babel/preset-env", {
                "targets": {
                    "browsers": ["last 2 versions"]
                },
                "modules": false,
                "useBuiltIns": "usage",
            }]
        ],
        plugins: ["@babel/plugin-transform-runtime"]
    }
}


var isProduction = process.argv.indexOf("-p") >= 0;
console.log("Bundling for " + (isProduction ? "production" : "development") + "...");

var commonPlugins = [
];

module.exports = {
    mode: "development",
    entry: CONFIG.fsharpEntry,
    output: {
        path: path.join(__dirname, CONFIG.outputDir),
        filename: '[name].js'
    }, 
    devtool: isProduction ? undefined : "source-map",
    optimization: {
        splitChunks: {
            cacheGroups: {
                commons: {
                    test: /node_modules/,
                    name: "vendors",
                    chunks: "all"
                }
            }
        },
        minimizer: isProduction ? [new MinifyPlugin()] : []
    },
    plugins: isProduction ?
        commonPlugins
        : commonPlugins.concat([
            new webpack.HotModuleReplacementPlugin(),
        ]),
    devServer: {
        proxy: CONFIG.devServerProxy,
        port: 8080,
        hot: true,
        inline: true,
        historyApiFallback: true,
        contentBase: CONFIG.contentBase
    },
    module: {
        rules: [
            {
                test: /\.fs(x|proj)?$/,
                use: "fable-loader"
            },
            {
                test: /\.js$/,
                exclude: /node_modules/,
                use: {
                    loader: 'babel-loader',
                    options: CONFIG.babel
                },
            }
        ]
    }
};
