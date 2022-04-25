$('.interface_job').on("click", function(){
    mp.trigger("refinery_selectJobType", parseInt($(this).attr("id")));
    $(".interface_type_jobs").css("display", "none");
});

function insertData(data, tank){
    $('.interface_type_jobs').css("display", "none");
    $('.interface_type_stations').css("display", "block");
    $('.interface-left').html("");
    $('.interface-right').html("");

    data = JSON.parse(data);
    data.forEach(station => {
        let value = station[1];
        let index = station[0];
        $('.interface-left').append(`<p>Stacja ${numberToLetter(parseInt(index))}:</p>`);
        $('.interface-right').append(`<p>${Math.floor(value)} L</p>`);
    })
    $('.interface-left').append(`<p>W pojeździe:</p>`);
    $('.interface-right').append(`<p>${tank} L</p>`);
}

function selectJob(){
    $('.interface_type_jobs').css("display", "block");
    $('.interface_type_stations').css("display", "none");
}


function numberToLetter(number){
    switch(number){
        case 0:
            return "A";
            break;
        case 1:
            return "B";
            break;
        case 2:
            return "C";
            break;
        case 3:
            return "D";
            break;
        case 4:
            return "E";
            break;
        case 5:
            return "F";
            break;
        case 6:
            return "G";
            break;
    }
}