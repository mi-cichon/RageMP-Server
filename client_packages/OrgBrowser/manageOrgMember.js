function resetData(){
    $(".members-table tr td").each(function(index){
        $(this).parent().remove();
    });

    $(".vehicles-table tr td").each(function(index){
        $(this).parent().remove();
    });

    $(".vehicle-share-table tr td").each(function(index){
        $(this).parent().remove();
    });

    $(".shared-vehicles-table tr td").each(function(index){
        $(this).parent().remove();
    });
}


function insertMember(name, SID){
    $(".members-table").append(`
    <tr><td>${name}</td><td>${SID}</td></tr>`);
}

function insertVehicle(ID, name, owner){
    $(".vehicles-table").append(`
    <tr><td>${ID}</td><td>${name}</td><td>${owner}</td></tr>`);
}

function insertVehiclesToShare(ID, name){
    $(".vehicle-share-table").append(`
    <tr><td>${ID}</td><td>${name}</td>
    <td><input type="button" class="button_select" id="${ID}" value="Udostępnij"></td>
    </tr>`);

    $(".vehicle-share-table #" + ID.toString() + ".button_select").on("click", function(){
        mp.trigger("shareVehicle", ID);
        $(this).parent().parent().remove();
    });
}

function insertSharedVehicles(ID, name){
    $(".shared-vehicles-table").append(`
    <tr><td>${ID}</td><td>${name}</td>
    <td><input type="button" class="button_cancel" id="${ID}" value="Cofnij"></td>
    </tr>`);

    $(".shared-vehicles-table #" + ID.toString() + ".button_cancel").on("click", function(){
        mp.trigger("removeSharedVehicleMember", ID);
        $(this).parent().parent().remove();
    });
}

$(".members").on("click", function(){
    $(".vehicles-list").css("display", "none");
    $(".manage-tab").css("display", "none");
    $(".members-list").css("display", "block");
});

$(".vehicles").on("click", function(){
    $(".vehicles-list").css("display", "block");
    $(".manage-tab").css("display", "none");
    $(".members-list").css("display", "none");
});

$(".manage").on("click", function(){
    $(".vehicles-list").css("display", "none");
    $(".manage-tab").css("display", "block");
    $(".members-list").css("display", "none");
});


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
    if(dropContent.find('table tr td').length > 0){
        dropContent.find('table').css("display", "table");
    }
    else{
        dropContent.find('table').css("display", "none");
    }
});

$(".removeButton").on("click", function(){
    mp.trigger("leaveOrg"); 
})

$(".buttons .button_cancel").on("click", function(){
    mp.trigger("closeMemberOrgBrowser");
});