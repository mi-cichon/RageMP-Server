function resetData(){
    $(".members-table tr td").each(function(index){
        $(this).parent().remove();
    });

    $(".vehicles-table tr td").each(function(index){
        $(this).parent().remove();
    });

    $(".member-requests-table tr td").each(function(index){
        $(this).parent().remove();
    });

    $(".vehicle-requests-table tr td").each(function(index){
        $(this).parent().remove();
    });
    
    $(".vehicle-share-table tr td").each(function(index){
        $(this).parent().remove();
    });
}

function insertMember(SID, name, self){
    if(self){
        $(".members-table").append(`
        <tr><td>${SID}</td><td>${name}</td>
        </tr>`);
    }
    else{
        $(".members-table").append(`
        <tr><td>${SID}</td><td>${name}</td><td><input type="button" class="button_cancel" id="${SID}" value="Usuń"></td>
        </tr>`);
        $(".members-table #" + SID.toString() + ".button_cancel").on("click", function(){
            mp.trigger("kickMemberFromOrg", SID);
            $(this).parent().parent().remove();
        });
    }
}

function insertVehicle(ID, name, owner){
    $(".vehicles-table").append(`
    <tr><td>${ID}</td><td>${name}</td><td>${owner}</td>
    <td><input type="button" class="button_cancel" id="${ID}" value="Usuń"></td>
    </tr>`);
    $(".vehicles-table #" + ID.toString() + ".button_cancel").on("click", function(){
        mp.trigger("removeSharedVehicle", ID);
        $(this).parent().parent().remove();
    });
}

function insertMemberRequest(SID, name, id){
    $(".member-requests-table").append(`
    <tr><td>${name}</td><td>${SID}</td>
    <td><input type="button" class="button_select" id="${id}" value="Potwierdź"></td>
    <td><input type="button" class="button_cancel" id="${id}" value="Usuń"></td>
    </tr>`);

    $(".member-requests-table #" + id.toString() + ".button_select").on("click", function(){
        mp.trigger("answerMemberRequest", id, true);
        $(this).parent().parent().remove();
    });
    $(".member-requests-table #" + id.toString() + ".button_cancel").on("click", function(){
        mp.trigger("answerMemberRequest", id, false);
        $(this).parent().parent().remove();
    });
}

function insertVehicleRequest(ID, name, owner){
    $(".vehicle-requests-table").append(`
    <tr><td>${ID}</td><td>${name}</td><td>${owner}</td>
    <td><input type="button" class="button_select" id="${ID}" value="Potwierdź"></td>
    <td><input type="button" class="button_cancel" id="${ID}" value="Usuń"></td>
    </tr>`);

    $(".vehicle-requests-table #" + ID.toString() + ".button_select").on("click", function(){
        mp.trigger("answerVehicleRequest", ID, true);
        $(this).parent().parent().remove();
    });
    $(".vehicle-requests-table #" + ID.toString() + ".button_cancel").on("click", function(){
        mp.trigger("answerVehicleRequest", ID, false);
        $(this).parent().parent().remove();
    });
}


function insertVehiclesToShare(ID, name){
    $(".vehicle-share-table").append(`
    <tr><td>${ID}</td><td>${name}</td>
    <td><input type="button" class="button_select" id="${ID}" value="Udostępnij"></td>
    </tr>`);

    $(".vehicle-share-table #" + ID.toString() + ".button_select").on("click", function(){
        mp.trigger("shareManageVehicle", ID);
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
    mp.trigger("removeOrg"); 
})

$(".buttons .button_cancel").on("click", function(){
    mp.trigger("closeManageOrgBrowser");
});