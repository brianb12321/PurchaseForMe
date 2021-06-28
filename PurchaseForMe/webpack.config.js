const path = require('path');

module.exports = {
    devtool: "source-map",
    mode: "development",
    entry: {
        all: './src/_build/initWorkspace.js'
    },
    output: {
        filename: 'bundle.min.js',
        path: path.resolve(__dirname, 'wwwroot/js')
    }
};