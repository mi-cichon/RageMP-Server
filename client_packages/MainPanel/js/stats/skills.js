mp.events.call("mainPanel_requestStatsSkillData");

var maxLevels = [4,15,10,20,1];
var costs = [5,2,1,1,18];
var currentLevels = [];
var skillPoints = 0;
var wairForResponse = true;

function insertStatsSkillsData(skillsData, sPoints){
    $(".skills_points").text("Pozostałe punkty umiejętności: " + sPoints);
    skillPoints = sPoints;
    skillsData = JSON.parse(skillsData);
    skillsData.forEach((level, index) => {
        currentLevels[index] = level;
        let className = getClassNameByIndex(index);
        $("." + className + "Lvl").text(level.toString() + "/" + maxLevels[index].toString());
    });
    waitForResponse = false;
    checkPoints();
}

function checkPoints(){
    $(".skills_points").text("Pozostałe punkty umiejętności: " + skillPoints);
    currentLevels.forEach((level, index) => {
        let className = getClassNameByIndex(index);
        if(skillPoints >= costs[index] && level < maxLevels[index]){
            $("." + className + "Add .addPoint").removeClass("hide");
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

function addSkillPoint(_class){
    if(!waitForResponse){
        let index = getIndexByClassName(_class);
        mp.events.call("mainPanel_addPoint", index);
        wairForResponse = true;
    }
}

function setSkillPoints(sPoints, levels){
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
