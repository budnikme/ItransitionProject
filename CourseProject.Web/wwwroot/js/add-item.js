$( "#date-field" ).datepicker();
$("#tags").selectize({
    plugins: ["remove_button"],
    delimiter: ",",
    persist: false,
    create: function (input) {
        return {
            value: input,
            text: input,
        };
    },
    load: function(query, callback) {
        if (!query.length) return callback();
        $.ajax({
            url: '/Search/' + encodeURIComponent(query),
            type: 'GET',
            error: function() {
                callback();
            },
            success: function(res) {
                callback(res);

            }
        });
    }
});


    
