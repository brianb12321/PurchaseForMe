var gulp = require('gulp');

gulp.task('copyBlockly',
    function (done) {
        gulp.src(['node_modules/blockly/blockly_compressed.js']).pipe(gulp.dest('wwwroot/lib/blockly'));
        gulp.src(['node_modules/blockly/javascript_compressed.js']).pipe(gulp.dest('wwwroot/lib/blockly'));
        gulp.src(['node_modules/blockly/blocks_compressed.js']).pipe(gulp.dest('wwwroot/lib/blockly'));
        gulp.src(['node_modules/blockly/msg/en.js']).pipe(gulp.dest('wwwroot/lib/blockly/msg'));
        return done();
    });
gulp.task('copyBlocklyMaps',
    function(done) {
        gulp.src(['node_modules/blockly/blockly_compressed.js.map']).pipe(gulp.dest('wwwroot/lib/blockly'));
        gulp.src(['node_modules/blockly/javascript_compressed.js.map']).pipe(gulp.dest('wwwroot/lib/blockly'));
        gulp.src(['node_modules/blockly/blocks_compressed.js.map']).pipe(gulp.dest('wwwroot/lib/blockly'));
        return done();
    });
gulp.task('default', gulp.series(['copyBlockly', 'copyBlocklyMaps']));