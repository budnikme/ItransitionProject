$("#checkAll").click(function(){
    $('.table input:checkbox').not(this).prop('checked', this.checked);
});

async function changeUserStatus(isActive){
    console.log(getCheckedIds())
    let userIds = getCheckedIds();
    await sendAjax('/Admin/ChangeUserStatus', {userIds: userIds, isActive: isActive});
}

async function setRoles(role){
    let userIds = getCheckedIds();
    console.log(userIds);
    await sendAjax('/Admin/SetRole', {userIds: userIds, role: role});
}

$('#delete-button').click(async function(){
    let userIds = getCheckedIds();
    await sendAjax('/Admin/DeleteUsers', {userIds: userIds});
});

async function sendAjax(url, data, success, error){
    console.log(data);
    await $.ajax({
        url: url,
        type: 'POST',
        data: data,
        timeout: 10000,
        success: function () {
            location.reload()
        },
    });
}

function getCheckedIds(){
    let userIds = [];
    $(".table-body input[type=checkbox]:checked").each(function () {
        userIds.push(this.value);
    });
    return userIds;
}

