
mp.trigger("mainPanel_requestHourBonusInfo");




var rewardId = 0;
function rollABonus(){
    $(".panel_bonus_menu").css("display", "none");
    var rewards = [
        {name: "Kasa", prob: 0.14, img:"kasa3.png", rarity:1, id:0},
        {name: "Kasa", prob: 0.1, img:"kasa3.png", rarity:2, id:1},
        {name: "Kasa", prob: 0.07, img:"kasa3.png", rarity:3, id:2},
    
        {name: "EXP", prob: 0.15, img:"xp.png", rarity:1, id:3},
        {name: "EXP", prob: 0.1, img:"xp.png", rarity:2, id:4},
        {name: "EXP", prob: 0.07, img:"xp.png", rarity:3, id:5},
        
        {name: "Ryba", prob: 0.028725, img:"ryba.png", rarity:4, id:6},
        {name: "Przedmiot", prob: 0.028725, img:"sakwa.png", rarity:4, id:7},
        
        {name: "Jabłko", prob: 0.07, img:"jablko.png", rarity:2, id:8},
        {name: "Chleb", prob: 0.07, img:"chleb.png", rarity:2, id:9},
        {name: "Woda", prob: 0.07, img:"woda.png", rarity:2, id:10},
        {name: "Nitro", prob: 0.07, img:"nitro.png", rarity:2, id:11},
        
        {name: "Umiejętność", prob: 0.14, img:"umiejetnosc.png", rarity:4, id:12},
    
        {name: "Pojazd", prob: 0.00005, img:"pojazd.png", rarity:5, id:13}
    ]

    var rarities = [
        "#787878",
        "#4a80ff",
        "#145aff",
        "#b514ff",
        "#DAA520"
    ]
    
    
    //probability array
    var probabilities = [];

    $(".panel_bonus_roller_body").css("display", "block");
    var body = $(".panel_bonus_roller");
    var itemsCount = 100;
    rewards.forEach((reward,index) => {
        for(let i = 0; i < reward.prob * 1000000; i++){
            probabilities.push(index);
        }
    })

    for(let i=0; i<itemsCount; i++){
        let reward = rewards[probabilities[getRandomInt(0, probabilities.length)]];
        body.append(`
        <div class="panel_bonus_item" style="background-color: ${rarities[reward.rarity-1]}">
        <div class="panel_bonus_img" style="background-image: url(img/bonus/${reward.img})"></div>
        </div>
        `);
        if(i==79){
            rewardId = reward.id;
        }
    }
    setTimeout(()=>{
        scroll();
    }, 1000);

}

function getRandomInt(min, max){
    min = Math.ceil(min);
    max = Math.floor(max);
    return Math.floor(Math.random() * (max - min)) + min;
}

function scroll(){
    let current = 0;
    let body = $(".panel_bonus_scroll");
    let scrollWidth = body.width();
    let bodyWidth = $(".panel_bonus_roller").width();
    let itemWidth = $(".panel_bonus_item").width();
    let toScroll = bodyWidth * 0.8 - (2*scrollWidth/3) + getRandomInt(0, 2*itemWidth/3);
    scrollInt(current, toScroll, 15);
}
var scrollInterval = null;

function scrollInt(current, toScroll, step){
    if(scrollInterval!=null){
        clearInterval(scrollInterval);
    }
    scrollInterval = setInterval(()=>{
        let body = $(".panel_bonus_scroll");
        current += step;
        body.scrollLeft(current);
        if(current >= toScroll){
            setTimeout(()=>{
                console.log(rewardId)
                $(".panel_bonus_roller_body").css("display", "none");
                $(".panel_bonus_menu").css("display", "flex");
                mp.trigger("mainPanel_bonusReward", rewardId);
            }, 2000);
            clearInterval(scrollInterval);
        }
        if(toScroll - current < toScroll/100){
            if(step!=1)
                scrollInt(current, toScroll, 1);
        }
        else if(toScroll - current < toScroll/50){
            if(step!=3)
                scrollInt(current, toScroll, 3);
        }
        else if(toScroll - current < toScroll/20){
            if(step!=5)
                scrollInt(current, toScroll, 5);
        }
        else if(toScroll - current < toScroll/10){
            if(step!=7)
                scrollInt(current, toScroll, 7);
        }
        else if(toScroll - current < toScroll/4){
            if(step!=9)
                scrollInt(current, toScroll, 9);
        }
        else if(toScroll - current < toScroll/3){
            if(step!=11)
                scrollInt(current, toScroll, 11);
        }
        else if(toScroll - current < toScroll/2){
            if(step!=13)
                scrollInt(current, toScroll, 13);
        }
    }, 7);
}

function setHourBonus(minutes){
    let button = $(".panel_bonus_menu_hourbutton");
    if(minutes < 60){
        $(button).text(`Pozostało ${60-minutes} minut!`);
    }
    else{
        $(button).removeClass("panel_bonus_menu_button_notactive");
        $(button).text(`Odbierz nagrodę!`);
        $(button).on("click", function(){
            mp.trigger("mainPanel_setBonus", true);
            rollABonus();
            $(button).text(`Pozostało 60 minut!`);
            $(button).addClass("panel_bonus_menu_button_notactive");
        });
    }
}

