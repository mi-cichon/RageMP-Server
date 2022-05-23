var playerId = null;
var playerToId = null;
var newMessage = false;
var useEmoji = true;

mp.trigger("messenger_requestConversationsData");


var emojis = [
    {id: 0, name: ":angry:", code: `<div class="emoji" style="background-image: url(img/emojis/angry.png)"></div>`},
    {id: 1, name: ":o", code: `<div class="emoji" style="background-image: url(img/emojis/astonished.png)"></div>`},
    {id: 2, name: ":boom:", code: `<div class="emoji" style="background-image: url(img/emojis/boom.png)"></div>`},
    {id: 3, name: ":cowboy:", code: `<div class="emoji" style="background-image: url(img/emojis/cowboy.png)"></div>`},
    {id: 4, name: ":devil:", code: `<div class="emoji" style="background-image: url(..img/emojis/devil.png)"></div>`},
    {id: 5, name: ":/", code: `<div class="emoji" style="background-image: url(..img/emojis/diagonal.png)"></div>`},
    {id: 6, name: ":|", code: `<div class="emoji" style="background-image: url(..img/emojis/expressionless.png)"></div>`},
    {id: 7, name: ":D", code: `<div class="emoji" style="background-image: url(img/emojis/laugh.png)"></div>`},
    {id: 8, name: ":rofl:", code: `<div class="emoji" style="background-image: url(img/emojis/rofl.png)"></div>`},
    {id: 9, name: ":sad:", code: `<div class="emoji" style="background-image: url(img/emojis/sad.png)"></div>`},
    {id: 10, name: ":smile:", code: `<div class="emoji" style="background-image: url(img/emojis/smile.png)"></div>`},

    {id: 11, name: ":lmao:", code: `<div class="emoji" style="background-image: url(img/emojis/lmao.png)"></div>`},
    {id: 12, name: ":yawn:", code: `<div class="emoji" style="background-image: url(img/emojis/yawn.png)"></div>`},
    {id: 13, name: ":*", code: `<div class="emoji" style="background-image: url(img/emojis/kiss.png)"></div>`},
    {id: 14, name: ":liar:", code: `<div class="emoji" style="background-image: url(img/emojis/liar.png)"></div>`},
    {id: 15, name: ":x", code: `<div class="emoji" style="background-image: url(img/emojis/x.png)"></div>`},
    {id: 16, name: ":puke:", code: `<div class="emoji" style="background-image: url(img/emojis/puke.png)"></div>`},
    {id: 17, name: ":shush:", code: `<div class="emoji" style="background-image: url(img/emojis/shush.png)"></div>`},
    {id: 18, name: ":zzz:", code: `<div class="emoji" style="background-image: url(img/emojis/zzz.png)"></div>`},
    {id: 19, name: "<3", code: `<div class="emoji" style="background-image: url(img/emojis/heart.png)"></div>`}
];

function setConversationsData(data){
    data = JSON.parse(data);
    data.forEach(conversation => {
        $(".panel_messenger_contacts_scroll").append(`
        <div class="panel_messenger_contact ${conversation[1] == "true" ? "notseen" : ""}" playerId="${conversation[0]}">
            <div class="avatar" style="background-image:url(http://51.38.128.119/avatars/${conversation[0]}/avatar.png)"></div>
            <div class="nick">${conversation[2]}</div>
        </div>
        `);
    });
    $(".panel_messenger_messages_scroll").empty();
    $(".panel_messenger_contact").off("click");
    $(".panel_messenger_contact").on("click", function(){
        if(!$(this).hasClass("selected")){
            newMessage = false;
            $(".panel_messenger_contact.selected").removeClass("selected");
            $(this).addClass("selected");
            playerToId = $(this).attr("playerId");
            $(".panel_messenger_start").css("display", "none");
            $(".panel_messenger_messages_body").css("display", "block");
            $(".panel_messenger_search_body").css("display", "none");
            requestMessageData();
        }
    });
    // setTimeout(() => {
    //     if(!newMessage){
    //         requestConversationsData();
    //     }
    //     else{
    //         let interval = setInterval(() => {
    //             if(!newMessage){
    //                 requestConversationsData();
    //                 clearInterval(interval);
    //             }
    //         },3000)
    //     }
    // }, 3000);
}

function useEmojis(state){
    useEmoji = state;
}

function setMessageData(data){
    $(".panel_messenger_messages_scroll").empty();
    data = JSON.parse(data);
    data.forEach(message => {
        let text = checkForEmojis(message[1]);
        $(".panel_messenger_messages_scroll").append(`
            <div class="panel_messenger_message ${message[0] == "to" ? "outgoing" : "incoming"}">
                ${text}
                <div class="date">${message[2]}</div>
            </div>
        `);
    });

    $(".panel_messenger_messages").scrollTop(1000000);

    setTimeout(() => {
        requestMessageData();
    }, 3000);

}

function checkForEmojis(txt){
    let text = txt;
    if(useEmoji){
        emojis.forEach(emoji => {
            text = replaceAll(text, emoji.name, emoji.code);
        });
    }
    return text;
}

function requestMessageData(){
    mp.trigger("messenger_requestMessageData", playerToId);
}

function requestConversationsData(){
    $(".panel_messenger_contacts_scroll").empty();
    $(".panel_messenger_messages_scroll").empty();
    $(".panel_messenger_start").css("display", "flex");
    $(".panel_messenger_messages_body").css("display", "none");
    $(".panel_messenger_search_body").css("display", "none");
    mp.trigger("messenger_requestConversationsData");
}


document.getElementById("messenger_input").addEventListener("keyup", function(event){
    if(event.key === "Enter")
    {
        sendMessage();
    }
}, true);

$(".panel_messenger_send").on("click", function(){
    sendMessage();
});

function sendMessage(){
    if(playerToId!=null){
        let text = $("#messenger_input").val();
        if(text!="" && replaceAll(text, " ", "") != ""){
            $("#messenger_input").val("");
            text = stripHtml(text);
            mp.trigger("messenger_sendMessage", playerToId, text);
            if(newMessage){
                newMessage = false;
            }
        }
    }
}

function replaceAll(text, from, to){
    while(text.includes(from)){
        text = text.replace(from, to);
    }
    return text;
}

function stripHtml(html)
{
   let tmp = document.createElement("DIV");
   tmp.innerHTML = html;
   return tmp.textContent || tmp.innerText || "";
}

$(".panel_messenger_add").on("click", function(){
    $(".panel_messenger_messages_scroll").empty();
    $(".panel_messenger_messages_body").css("display", "none");
    $(".panel_messenger_start").css("display", "none");
    $(".panel_messenger_search_body").css("display", "flex");
    $(".panel_messenger_contact.selected").removeClass("selected");
});

document.getElementById("panel_messenger_search_input").addEventListener("keyup", function(event){
    if(event.key === "Enter")
    {
        if($("#panel_messenger_search_input").val().length > 2){
            searchForPlayers($("#panel_messenger_search_input").val());
        }
        
    }
}, true);

$(".panel_messenger_search > div").on("click", function(){
    if($("#panel_messenger_search_input").val().length > 2){
        searchForPlayers($("#panel_messenger_search_input").val());
    }
});

function searchForPlayers(keyword){
    $(".panel_messenger_search_players").empty();
    mp.trigger("messenger_searchForPlayers", keyword);
}

function insertPlayers(data){
    data = JSON.parse(data);
    data.forEach(player => {
        $(".panel_messenger_search_players").append(`
        <div class="panel_messenger_search_player" id="player_${player[0]}">
            <div class="img" style="background-image:url('http://51.38.128.119/avatars/${player[0]}/avatar.png')"></div>
            <div class="nick">${player[1]}</div>
        </div>

        `);

        $(`#player_${player[0]}`).on("click", function(){

            newMessage = true;

            $(".panel_messenger_search_body").css("display", "none");
            $(".panel_messenger_messages_body").css("display", "block");
            playerToId = player[0];

            let found = false;
            $(".panel_messenger_contact").removeClass("selected");
            $(".panel_messenger_contacts_scroll").children().each(function(){
                if($(this).attr("playerId") == player[0]){
                    found = true;
                    $(this).addClass("selected");
                    requestMessageData();
                }
            });
            if(!found){
                $(".panel_messenger_contacts_scroll").append(`
                <div class="panel_messenger_contact selected" playerId="${player[0]}">
                    <div class="avatar" style="background-image:url(http://51.38.128.119/avatars/${player[0]}/avatar.png)"></div>
                    <div class="nick">${player[1]}</div>
                </div>

            `);
            $(".panel_messenger_contact").off("click");
            $(".panel_messenger_contact").on("click", function(){
                if(!$(this).hasClass("selected")){
                    newMessage = false;
                    $(".panel_messenger_contact.selected").removeClass("selected");
                    $(this).addClass("selected");
                    playerToId = $(this).attr("playerId");
                    $(".panel_messenger_start").css("display", "none");
                    $(".panel_messenger_messages_body").css("display", "block");
                    requestMessageData();
                }
            });
            }
        });
    });
}
$(".emojisList_button").on("click", () => {
    $(".emojisList_list").css("display", $(".emojisList_list").css("display") == "none" ? "grid" : "none");
    let input = $("#messenger_input");
    input.trigger("focus");
})

  $(window).on("load", () => {
    emojis.forEach(emoji => {
        let element = $(emoji.code);
        element.attr("id", emoji.id);
        $(".emojisList_list").append(element);
    });
    $(".emojisList_list > .emoji").on("click", function(){
        let id = $(this).attr("id");
        $("#messenger_input").val($("#messenger_input").val() + emojis.find(emoji => emoji.id == parseInt(id)).name);
        let input = $("#messenger_input");
        input.trigger("focus");
    });
  })