function insertData(id, name, pp, rank, ping, org, color){
    color = JSON.parse(color);
    $(".interface-table").append(`<tr class="playerRow"><td class="id">${id}</td><td class="name">${name}</td><td class="org">${org}</td><td class="rank" style="color: rgb(${color[0]},${color[1]},${color[2]})">${rank}</td><td>${ping}</td><td>${pp}</td></tr>`);
}


$(".interface-search").keyup(function(){
    let searchPhrase = $(this).val();
    $(".playerRow").each((index) => {
        let display = false;
        $($(".playerRow").get(index)).children().each(i => {
            let row = $($($(".playerRow").get(index)).children().get(i));
            if(row.hasClass("id") || row.hasClass("rank") || row.hasClass("name") || row.hasClass("org"))
                if(row.text().toLowerCase().includes(searchPhrase.toLowerCase())){
                    display = true;
                }
        });
        if(!display){
            $($(".playerRow").get(index)).css("display", "none");
        }
        else{
            $($(".playerRow").get(index)).css("display", "table-row");
        }
    });
});

function research(el) {
    let playerRows = document.getElementsByClassName('playerRow');
    if(el.value != '' || el.value != ' ') {
        for(var i = 0; i < playerRows.length; i++) {
            var content = playerRows[i].innerHTML.toLowerCase();
            if(content.indexOf(`"id">${el.value.trim().toLowerCase()}`) != -1 || content.indexOf(`"name">${el.value.trim().toLowerCase()}`) != -1 || content.indexOf(`"rank">${el.value.trim().toLowerCase()}`) != -1) {
                playerRows[i].className = 'playerRow';
            } else {
                playerRows[i].className = 'playerRow hidden';
            }
        }
    } else {
        for(var i = 0; i < playerRows.length; i++) {
            playerRows[i].className = 'playerRow';
        }
    }
}

function setOnline(online){
    $(".online").text("Graczy online: " + online);
}