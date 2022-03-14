let nickName = null;

function insertData(nick, admin, type, timefrom, timeto, reason){
    if(nickName == null){
        nickName = nick;
        $(".nameCol").text("Kary " + nick);
    }
    switch(type){
        case "warn":
            $(".warns-table").append(`<tr><td>${admin}</td><td>${timefrom}</td><td>${reason}</td></tr>`)
        break;
        case "kick":
            $(".kicks-table").append(`<tr><td>${admin}</td><td>${timefrom}</td><td>${reason}</td></tr>`)
        break;
        case "mute":
            $(".mutes-table").append(`<tr><td>${admin}</td><td>${timefrom}</td><td>${timeto}</td><td>${reason}</td></tr>`)
        break;
        case "licence":
        case "coflic":
            $(".licences-table").append(`<tr><td>${admin}</td><td>${timefrom}</td><td>${timeto}</td><td>${reason}</td></tr>`)
        break;
        case "ban":
            $(".bans-table").append(`<tr><td>${admin}</td><td>${timefrom}</td><td>${timeto}</td><td>${reason}</td></tr>`)
        break;
    }
}

$("h3").on("click", function(e){
    let dropContent = $(this).nextAll('.dropdown').first();
    if(dropContent.hasClass("hidden")){
        dropContent.removeClass("hidden");
        $(this).children("span").css(`transform`, `rotate(-180deg)`);
    }
    else{
        dropContent.addClass("hidden");
        $(this).children("span").css(`transform`, `rotate(0deg)`);
    }
    if(dropContent.find('table tbody tr td').length > 0){
        dropContent.find('table').css("display", "table");
    }
    else{
        dropContent.find('table').css("display", "none");
    }
});

$(".button_cancel").on("click", function(){
    mp.trigger("closePenaltiesBrowser");
});