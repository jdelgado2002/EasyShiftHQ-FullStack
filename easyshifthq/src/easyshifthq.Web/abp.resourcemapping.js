module.exports = {
    aliases: {
        "@luxon": "@node_modules/luxon/build/global/luxon.min.js",
        "@timepicker": "@node_modules/timepicker/jquery.timepicker.min.js"
    },
    mappings: {
        "@node_modules/luxon/build/global/*.*": "@libs/luxon/",
        "@node_modules/timepicker/*.*": "@libs/timepicker/"
    }
};
