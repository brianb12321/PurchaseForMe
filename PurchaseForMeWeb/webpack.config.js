const path = require('path');

module.exports = {
    devtool: "source-map",
    mode: "development",
    entry: {
        blockly: './src/_build/initWorkspace.js',
        taskConsole: './src/_build/initTaskConsole.js'
    },
    output: {
        filename: '[name].bundle.min.js',
        path: path.resolve(__dirname, 'wwwroot/js')
    }
};