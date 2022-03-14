let doorOpen = false;

function insertData(name, id, amount, price){
    let prices = [];
    $(".interface-body").append(`
    <h3 class="h3-${id}">${name}<span>&#8595;</span></h3>
    <div class="dropdown hidden">
        <div class="dropdown-content">
            <table class="manage-table ${id}-table">
                <colgroup>
                    <col width="30%"><col width="20%"><col width="25%"><col width="25%">
                </colgroup>
            </table>
        </div>
    </div>
    `);
    for(let i = -1; i < amount; i++){
        if(i==-1){
            $(`.${id}-table`).append(`<tr class="tr-${id}-${i}">
            <td>${name} stock</td>
            <td></td>
            <td><input type="button" id="show-${id}-${i}" class="button_select" value="Podgląd"></td>
            <td>
            </tr>`);
            $(`#show-${id}-${i}`).on("click", function(){
                mp.trigger("showVisuTune", id, i);
                $(`.${id}-table`).children().removeClass(`select`);
                $(`.tr-${id}-${i}`).addClass(`select`);
            });
        }
        else{
            prices[i] = parseInt(price + (price * (0.05 * i)));
            $(`.${id}-table`).append(`<tr class="tr-${id}-${i}">
            <td>${name} ${i+1}</td>
            <td>$${prices[i]}</td>
            <td><input type="button" id="show-${id}-${i}" class="button_select" value="Podgląd"></td>
            <td><input type="button" id="apply-${id}-${i}" class="button_select" value="Zamontuj"></td>
            </tr>`);
            $(`#show-${id}-${i}`).on("click", function(){
                mp.trigger("showVisuTune", id, i);
                $(`.${id}-table`).children().removeClass(`select`);
                $(`.tr-${id}-${i}`).addClass(`select`);
            });
            $(`#apply-${id}-${i}`).on("click", function(){
                mp.trigger("applyVisuTune", id, i, prices[i]);
                $(`.${id}-table`).children().removeClass(`select`);
            });
        }
    }
    $(`.h3-${id}`).on("click", function(e){
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
}

function insertCurrent(data){
    let current = JSON.parse(data);
    if(current && current.length > 0){
        $(".interface-body").append(`
            <h3 class="h3-current">Demontaż Części<span>&#8595;</span></h3>
            <div class="dropdown hidden">
                <div class="dropdown-content">
                    <table class="manage-table table-current">
                        <colgroup>
                            <col width="40%"><col width="35%"><col width="25%">
                        </colgroup>
                    </table>
                </div>
            </div>
        `);

        current.forEach(part => {
            let name = part[0];
            let type = part[1];
            let id = part[2];
            let price = parseInt(part[3]);
            price = parseInt(price + (price * (0.05 * parseInt(id))));
            price = parseInt(price * 0.7);

            $(".table-current").append(`
            <tr class="tr-current-${type}-${id}">
                <td>${name}</td>
                <td>+ $${price}</td>
                <td><input type="button" id="current-${type}-${id}" class="button_cancel" value="Demontuj"></td>
            </tr>
            `);
            $(`#current-${type}-${id}`).on("click", function(){
                mp.trigger("removeVisu", type, id, price);
                $(`.tr-current-${type}-${id}`).remove();
            });
        });

        $(`.h3-current`).on("click", function(e){
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
    }
    
}

function removeData(){
    $("h3").remove();
    $(".dropdown").remove();
}

function removeType(type){
    $(`.${type}-table`).remove();
    $(`.h3-${type}`).remove();
}

$(".closeBrowser").on("click", function(){
    mp.trigger("closeVisuTuneBrowser");
});

$(".show").on("click", () => {
    $("body").css("display", "none");
    mp.trigger("showVehicleVisuTune");
});

$(".openDoor").on("click", () => {
    doorOpen = !doorOpen;
    mp.trigger("switchVehiclesDoor", doorOpen);
    $(".openDoor").val(doorOpen ? "Zamknij drzwi":"Otwórz drzwi");
});

function showBody(){
    $("body").css("display", "block");
}
