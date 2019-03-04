var gulp = require('gulp');
var gutil = require('gulp-util');
//var bower = require('bower');
var concat = require('gulp-concat');
//var sass = require('gulp-sass');
var annotate = require('gulp-ng-annotate');
//var minifyCss = require('gulp-minify-css');
//var rename = require('gulp-rename');
//var sh = require('shelljs');

var paths = {
  sass: ['./scss/**/*.scss'],
  controller:['./www/js/controllers/**/*.js'],
  service:['./www/js/services/**/*.js'],
  jsBase:'./www/js'
};

gulp.task('default', ['concatController','concatServices']);

//合并controllers文件
gulp.task('concatController', function () {
  gulp.src(paths.controller)
    .pipe(annotate())
    .pipe(concat('controllers.js'))
    .pipe(gulp.dest(paths.jsBase))
});
//合并services文件
gulp.task('concatServices', function () {
  gulp.src(paths.service)
    .pipe(annotate())
    .pipe(concat('services.js'))
    .pipe(gulp.dest(paths.jsBase))
});

