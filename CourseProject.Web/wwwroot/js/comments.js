let connection = new signalR.HubConnectionBuilder().withUrl("/ItemComments").build();

connection.start().then(function () {
    connection.invoke("Subscribe", itemId.toString());
}).catch(function (err) {
    return console.error(err.toString());
});

connection.on("ReceiveComment", function (comment) {
    let currentTheme = localStorage.getItem('lightSwitch');
    let commentStart = '<br/><div class="card">\n'
    if (currentTheme === 'dark') {
        commentStart = '<br/><div class="card" style="background-color:#2f2f2f">\n'
    }
    let tr = commentStart +
        '            <div class="card-header">\n' +
                        comment.createdDate+'\n' +
        '            </div>\n' +
        '            <div class="card-body">\n' +
        '                <h5 class="card-title">'+comment.userName+'</h5>\n' +
        '                <p class="card-text">'+comment.text+'</p>\n' +
        '            </div>\n' +
        '        </div>';
    $('#comments').prepend(tr);
});

$('#send-comment').click(function () {
    let comment ={
        ItemId: itemId,
        Text: $('#comment-message').val()
    };
    connection.invoke("SendComment", comment).catch(function (err) {
        return console.error(err.toString());
    });
        $('#comment-message').val('');
});