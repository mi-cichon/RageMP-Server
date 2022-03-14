function insertData(id, informer, reported, desc, time){
    $(".interface-table").append(`<tr class="playerRow ${id}"><td class="id">${id}</td><td class="name">${informer}</td><td class="name">${reported}</td><td class="desc">${desc}</td><td>${time}</td></tr>`);
    $(`.${id}`).click(function(){
        mp.trigger(`selectReport`, id);
    });
    $('.playerRow').hover(function() {
        $(this).addClass('hover');
    }, function() {
        $(this).removeClass('hover');
    });

    $(`.${id}`).contextmenu(function(){
        mp.trigger(`removeReport`, id);
    });
}
