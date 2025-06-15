const path = require('path');
const HtmlWebpackPlugin = require('html-webpack-plugin');
const ESLintPlugin = require('eslint-webpack-plugin');
const DotenvWebpackPlugin = require('dotenv-webpack');

module.exports = (_env, argv) => {
    const isProduction = argv.mode === 'production';
    const dotenvFilename = isProduction ? '.env.production' : '.env.development';

    return {
        mode: 'development',
        entry: {
            app: './src/index.tsx'
        },
        output: {
            filename: '[name].[contenthash].js',
            path: path.resolve(__dirname, 'dist'),
            clean: true,
            publicPath: '/'
        },
        devtool: 'inline-source-map',
        devServer: {
            static: path.resolve(__dirname, 'dist'),
            hot: true,
            historyApiFallback: true
        },
        module: {
            rules: [
                {
                    test: /\.css$/i,
                    use: ['style-loader', 'css-loader', 'postcss-loader'],
                },
                {
                    test: /\.(png|jpg|jpeg|gif|svg)$/i,
                    type: 'asset/resource',
                },
                {
                    test: /\.tsx?$/,
                    use: 'ts-loader',
                    exclude: /node_modules/,
                },
                {
                    test: /\.(js|jsx)$/,
                    exclude: /node_modules/,
                    use: {
                        loader: 'babel-loader',
                        options: {
                            presets: ['@babel/preset-env', '@babel/preset-react'],
                        },
                    },
                },
            ],
        },
        resolve: {
            extensions: ['.tsx', '.ts', '.js', '.jsx'],
        },
        plugins: [
            new HtmlWebpackPlugin({
                template: './src/index.html',
            }),
            new ESLintPlugin({
                extensions: ['ts', 'tsx', 'js', 'jsx'],
            }),
            new DotenvWebpackPlugin({
                path: dotenvFilename,
            })
        ]
    }
};