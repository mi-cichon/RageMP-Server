mp.events.call("mainPanel_requestPlayerData");
var maxLevels = [4,15,10,20,1];
var costs = [5,2,1,1,18];
var currentLevels = [];
var skillPoints = 0;
var wairForResponse = true;

function insertPlayerData(playersData, skillsData, jobData){
    playersData = JSON.parse(playersData);
    skillsData = JSON.parse(skillsData);
    jobData = JSON.parse(jobData);
    let time = parseInt(playersData[4]);
    let hours = Math.floor(time/60);
    let minutes = time%60;

    $('.panel_user_general').append(`
        <p><b>Nick: </b>${playersData[0]}</p>
        <p><b>SocialClubID: </b>${playersData[1]}</p>
        <p><b>ServerID: </b>${playersData[2]}</p>
        <p><b>Data rejestracji: </b>${playersData[3]}</p>
        <p><b>Łączny czas gry: </b>${hours}h ${minutes}m</p>
        <p><b>Postęp poziomu: </b>${playersData[5]}</p>
        <p><b>Ilość pojazdów: </b>${playersData[6]}</p>
        <p class="panel_skillPoints"><b>Dostępne punkty umiejętności: </b>${playersData[7]}</p>
        <p><b>Postęp znajdziek: </b>${playersData[8]}</p>
        <p><b>Do bonusu dziennego: </b> ${playersData[9]}m</p>
    `);

    jobData.forEach(data => {
        let currentExp = parseInt(data[2]);
        let nextLevelExp = parseInt(data[3]);
        let currentLevelExp = parseInt(data[4]);
        let left = currentExp - currentLevelExp;
        let right = nextLevelExp - currentLevelExp;

        $('.panel_user_jobs').append(`
            <div class="panel_stats_opis">
                <p>${data[0]}: ${data[1]}</p>
                <p>${data[2]}/${data[3]}</p>
            </div>
            <div class="panel_stats_job_bar">
                <div style="width: ${data[3] == "0" ? 100 : parseInt(left/right * 100)}%"></div>
            </div>
        `);
    });

    skillPoints = parseInt(playersData[7]);
    skillsData.forEach((level, index) => {
        currentLevels[index] = level;
        let className = getClassNameByIndex(index);
        $("." + className + "Lvl").text(level.toString() + "/" + maxLevels[index].toString());
    });
    waitForResponse = false;
    checkPoints();
}

function checkPoints(){
    currentLevels.forEach((level, index) => {
        let className = getClassNameByIndex(index);
        if(skillPoints >= costs[index] && level < maxLevels[index]){
            $("." + className + "Add .addPoint").removeClass("hide");
            console.log("XD");
        }
        else{
            $("." + className + "Add .addPoint").addClass("hide");
        }
    });
}

function getClassNameByIndex(index){
    let className = "";
    switch(index){
        case 0:
            className = "eq";
            break;
        case 1:
            className = "zarobki";
            break;
        case 2:
            className = "ed";
            break;
        case 3:
            className = "sloty";
            break;
        case 4:
            className = "sz";
            break;
    }
    return className;
}

function getIndexByClassName(className){
    let index = 0;
    switch(className){
        case "eqAdd":
            index = 0;
            break;
        case "zarobkiAdd":
            index = 1;
            break;
        case "edAdd":
            index = 2;
            break;
        case "slotyAdd":
            index = 3;
            break;
        case "szAdd":
            index = 4;
            break;
    }
    return index;
}

$(".addPoint").on("click", function(event){
    if(!waitForResponse){
        let c = $(this).parent().attr("class");
        let index = getIndexByClassName(c);
        mp.events.call("mainPanel_addPoint", index);
    }
    
});

function setSkillPoints(sPoints, levels){
    $(`.panel_skillPoints`).html(`<b>Dostępne punkty umiejętności: </b>${sPoints}`);
    skillPoints = sPoints;
    levels = JSON.parse(levels);
    levels.forEach((level, index) => {
        currentLevels[index] = level;
        let className = getClassNameByIndex(index);
        $("." + className + "Lvl").text(level.toString() + "/" + maxLevels[index].toString());
    });
    waitForResponse = false;
    checkPoints();
}

