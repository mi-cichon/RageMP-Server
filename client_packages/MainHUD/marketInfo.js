function setMarketValues(id, price, name, owner, desc, tune){
    $(".marketTable tr").remove();
    $(".marketInfo").css("display", "block");
    $(".marketTable").append(`
        <tr>
            <td>${name}</td>
        </tr>
        <tr>
            <td>$${betterNumbers(price)}</td>
        </tr>
        <tr>
            <td>ID: ${id}</td>
        </tr>
        <tr>
            <td>Należy do: ${owner}</td>
        </tr>
        <tr>
            <td><div class="spacer"></td>
        </tr>
        <tr>
            <td>${desc}</td>
        </tr>
    `);
    tune = JSON.parse(tune);
    if(tune.length > 0){
        $(".marketTable").append(`
            <tr>
                <td><div class="spacer"></td>
            </tr>
        `);
        tune.forEach(t => {
            $(".marketTable").append(`
                <tr>
                    <td>${t}</td>
                </tr>
            `);
        });
    }
}

function setMarketPosition(x, y){
    $('.marketInfo').css("left", ($(window).width() * x).toString() + "px");
    $('.marketInfo').css("top", ($(window).height() * y).toString() + "px");
}

function hideMarketInfo(){
    $(".marketInfo").css("display", "none");
}

function betterNumbers(val) {
    result = val.toString().trim();
    for(var i = val.toString().trim().length-1; i >= 0; i--) {
        if((val.toString().trim().length - i) % 3 == 0) {
            if(val.toString().slice(0,i) != '')
            result = result.slice(0,i) + spacer + result.slice(i);
        }
    }
    return result;
}