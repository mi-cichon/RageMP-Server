function insertData(name, tag, players, orgId){
    $(".list-table").append(`
    <tr><td>${tag}</td><td>${name}</td><td>${players}</td>
    <td><input type="button" id="${orgId}"" class="button_select" value="Aplikuj"></td>
    </tr>
    `);
    $("#" + orgId.toString()).on("click", function(){
        mp.trigger("sendOrgRequest", orgId);
    });
}

$(".listTab").on("click", function(){
    $(".list").css("display", "block");
    $(".create").css("display", "none");
});

$(".createTab").on("click", function(){
    $(".create").css("display", "block");
    $(".list").css("display", "none");
});


$(".createButton").on("click", function(){
    let name = $(".name").val();
    let tag = $(".tag").val();
    if(name.length < 4 || name.length > 20){
        $(".error").text("Nazwa musi mieć długość 4-20 znaków!")
    }
    else if(!onlyLettersAndSpace(name)){
        $(".error").text("Nazwa zawiera znaki specjalne!")
    }
    else if(tag.length < 2 || tag.length > 4){
        $(".error").text("Tag musi mieć długość 2-4 znaków!")
    }
    else if(!onlyLetters(tag)){
        $(".error").text("Tag zawiera znaki specjalne!")
    }
    else{
        mp.trigger("createOrg", name, tag);
    }
});
$(".button_cancel").on("click", function(){
   mp.trigger("closeOrgBrowser");
});
function onlyLetters(str) {
    return str.match("^[A-Za-z0-9]+$");
}
function onlyLettersAndSpace(str) {
    return str.match("^[A-Za-z0-9 _]*[A-Za-z0-9][A-Za-z0-9 _]*$");
}

function showError(text){
    $(".error").text(text);
}

function refreshData(name, tag, players, orgId){
    
}