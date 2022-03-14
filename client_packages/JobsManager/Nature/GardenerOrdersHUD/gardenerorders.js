function insertData(data){
    $(".gardenOrder").remove();
    data = JSON.parse(data);
    data.forEach(element => {
        id = element[0];
        $(".interface-table").append(`
        <tr class="gardenOrder" id="${id}">
            <td>${element[0]}</td>
            <td>${element[1]}</td>
            <td>${element[2]}</td>
            <td>${element[3]}</td>
            <td>${element[4]}</td>
            <td>${element[5]}</td>
        </tr>`);
        $(`#${id}`).click(function(){
            if($(this).hasClass("selected")){
                mp.trigger("gardener_pickOrder", $(this).attr("id"));
            }
            else{
                $("*").removeClass("selected");
                $(this).addClass("selected");
            }
        });
    });
}

$(".cancel").on("click", function(){
    mp.trigger("gardener_cancelOrder");
});

$(".refresh").on("click", function(){
    mp.trigger("gardener_refreshOrders");
});