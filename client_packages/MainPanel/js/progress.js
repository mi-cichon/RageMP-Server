mp.trigger("mainPanel_requestProgressData");

var screenBody = $(".panel_progress_screen");
var body = $(".panel_progress_body");

var jobs = [
    // {id: 0, name: "warehouse", fullname: "Magazynier", lvl: 16},
    // {id: 1, name: "lawnmowing", fullname: "Koszenie trawników",lvl: 0},
    // {id: 2, name: "debriscleaner", fullname: "Zbieranie odpadów",lvl: 0},
    // {id: 3, name: "hunter", fullname: "Myśliwy",lvl: 0},
    // {id: 4, name: "forklifts", fullname: "Wózki widłowe",lvl: 12},
    // {id: 5, name: "refinery", fullname: "Rafineria",lvl: 0},
    // {id: 6, name: "diver", fullname: "Nurek",lvl: 0},
    // {id: 7, name: "fisherman", fullname: "Wędkarstwo",lvl: 0},
    // {id: 8, name: "gardener", fullname: "Ogrodnik",lvl: 0},
    // {id: 9, name: "towtrucks", fullname: "Lawety",lvl: 0}
];

var mouseDown = false;
var startHoldPos = null;
var startHoldScrollPos = null;

$(screenBody)[0].addEventListener('mousedown',function(e) {
    mouseDown = true;
});

$(document)[0].addEventListener('mouseup',function(e) {
    mouseDown = false;
    startHoldPos = null;
    startHoldScrollPos = null;
});

document.onmousemove = function(event) {
    var rect = $(body)[0].getBoundingClientRect();
    let x = event.clientX-rect.left;
    let y = event.clientY-rect.top;
    if(mouseDown){
        if(startHoldPos == null){
            startHoldPos = {x: x, y: y};
            startHoldScrollPos = {x: $(body).scrollLeft(), y: $(body).scrollTop()};
        }
        else{
            offsetX = (x - startHoldPos.x) * -1;
            offsetY = (y - startHoldPos.y) * -1;
            $(body).scrollLeft(startHoldScrollPos.x + offsetX);
            $(body).scrollTop(startHoldScrollPos.y + offsetY);
        }
    }   
}



$(() => {
    $(screenBody).height($(body).height()*2);
    $(screenBody).width($(body).width()*2);

    $(body).scrollTop(0.5 * $(body).height());
    $(body).scrollLeft(0.5 * $(body).width());
});

var nodes = [];

function nodeByIndex(index){
    let theNode = null;
    nodes.forEach(node => {
        if(node.id == index){
            theNode = node;
        }
    });
    return theNode;
}

var Node = class {
    constructor(id, type, name, connectTo, offset, requirements, desc){
        this.id = id;
        this.type = type;
        this.name = name;
        this.connectTo = connectTo;
        this.offset = offset;

        this.position = {x: 0, y: 0};

        this.line = null;

        this.requirements = requirements;

        this.desc = desc;

        this.unlocked = doesPlayerMeetTheRequirements(requirements);

        this.draw();
        

    }

    draw(){
        let offsetPos = {x: $(screenBody).width()/2, y: $(screenBody).height()/2}
        if(this.connectTo != null){
            offsetPos = this.connectTo.position;
        }
        
        let nodeDiameter = this.type == "main" ? Math.floor($(screenBody).width()/20) : Math.floor($(screenBody).width()/30);
        
        let offsetLength = Math.floor($(screenBody).width()/25);
        if(this.connectTo != null){
            if(this.connectTo.type == "main" || this.type == "main"){
                offsetLength = Math.floor($(screenBody).width()/20);
            }
        }
        
        this.position = {x: offsetPos.x + (this.offset.x * offsetLength), y: offsetPos.y + (this.offset.y * -1 * offsetLength)};
        $(screenBody).append(`
            <div class="panel_progress_node ${this.type == "main" ? "panel_progress_node_main" : "panel_progress_node_side"} ${this.unlocked ? "panel_progress_node_unlocked" : ""}" id="panel_progress_node_${this.id}" style="width: ${nodeDiameter}px;height: ${nodeDiameter}px; left: ${this.position.x}px; top: ${this.position.y}px;">
                <p>${this.name}</p>
                <div style="font-weight: bold; text-align: center; margin-bottom: 1vh">${this.desc}</div>
                <div>Wymagania:</div>
            </div>
        `);

        if(this.requirements == null){
            $(`#panel_progress_node_${this.id}`).append(`<div>Brak</div>`);
        }
        else{
            this.requirements.forEach(req => {
                $(`#panel_progress_node_${this.id}`).append(`
                    <div class="panel_progress_job">
                        <p>${jobs[req.jobId].fullname} poziom ${req.lvl}</p>
                        <div class="panel_progress_job_bar">
                            <div style="width: ${Math.min(jobs[req.jobId].lvl, req.lvl)/req.lvl * 100}%"></div>
                        </div>
                    </div>
                `);
            });
        }

        if(this.connectTo != null){
            linedraw(this.position.x, this.position.y, this.connectTo.position.x, this.connectTo.position.y, this.id, this.connectTo.id, this.unlocked);
            this.line = $(`#panel_progress_line_${this.id}-${this.connectTo.id}`);
        }
    }
}

function linedraw(x1, y1, x2, y2, parent, child, unlocked) {
    if (x2 < x1) {
        var tmp;
        tmp = x2 ; x2 = x1 ; x1 = tmp;
        tmp = y2 ; y2 = y1 ; y1 = tmp;
    }

    var lineLength = Math.sqrt(Math.pow(x2 - x1, 2) + Math.pow(y2 - y1, 2));
    var m = (y2 - y1) / (x2 - x1);

    var degree = Math.atan(m) * 180 / Math.PI;

    $(screenBody).append(`
    <div id='panel_progress_line_${parent}-${child}' class='panel_progress_line ${unlocked ? "panel_progress_line_unlocked" : ""}' style='transform: rotate(${degree}deg); width: ${lineLength}px; top: ${y1}px; left: ${x1}px;'></div>
    `);
}

function doesPlayerMeetTheRequirements(requirements){
    if(requirements == null){
        return true;
    }
    else{
        let state = true;
        requirements.forEach(req => {
            if(jobs[req.jobId].lvl < req.lvl){
                state = false;
            }
        });
        return state;
    }
}

function setProgressData(jobsData, nodesData){
    jobsData = JSON.parse(jobsData);
    jobsData.forEach((job, index) => {
        jobs.push({id: index, fullname: job[0], lvl: parseInt(job[1])});
    });

    nodesData = JSON.parse(nodesData);
    nodesData.forEach(node => {

        let requirements = [];
        if(node.Requirements == null){
            requirements = null;
        }
        else{
            node.Requirements.forEach(req => {
                requirements.push({jobId: req[0], lvl: req[1]});
            });
        }
        nodes.push(new Node(node.Id, node.Type, node.Name, node.ParentID == -1 ? null : nodeByIndex(node.ParentID), {x: node.Offset[0], y: node.Offset[1]}, requirements, node.Description));
    
    });
    linedraw(nodeByIndex(0).position.x, nodeByIndex(0).position.y, nodeByIndex(50).position.x, nodeByIndex(50).position.y, -1, -1, true);
    linedraw(nodeByIndex(0).position.x, nodeByIndex(0).position.y, nodeByIndex(100).position.x, nodeByIndex(100).position.y, -1, -1, true);
    linedraw(nodeByIndex(50).position.x, nodeByIndex(50).position.y, nodeByIndex(100).position.x, nodeByIndex(100).position.y, -1, -1, true);
}