let renderInterval = setInterval(render, 10);
let movingItem = null;
let mouseX = 0, mouseY = 0;
let equipmentGrid;
let closestGrid;
let contextItem = null;

let hoverElement = $(".hover");
let contextMenu = $(".contextMenu");
let hoveredItem;


function render(){
    if(movingItem){
        movingItem.object.offset({top: mouseY - movingItem.object.height()/2, left: mouseX - movingItem.object.width()/2});
        // movingItem.object.css("left", mouseX + "px");
        // movingItem.object.css("top", mouseY + "px");
        closestGrid = getClosestGrid(movingItem, movingItem.panel);
    }
    if(hoveredItem && !movingItem && $(".background").css("display") != "none"){
        hoverElement.css("display", "block");
        hoverElement.css("top", mouseY - 10 + "px");
        hoverElement.css("left", mouseX + 10 + "px");
        hoverElement.html(hoveredItem.itemType.name);
    }
    else{
        hoverElement.css("display", "none");
    }
}


class Item{
    constructor(itemType, gridPanel, slot){
        this.itemType = itemType;
        this.id = -1;
        this.panel = gridPanel;
        this.size = new Object();
        this.size.x = itemType.size.x;
        this.size.y = itemType.size.y;
        this.slot = slot;
        this.object = $("<div class=\"item\"></div>");
        this.object.appendTo(gridPanel.object.children()[slot]);
        this.object.css("width", (this.object.parent().width() * this.size.x) + (this.size.x * 2) -1 + "px");
        this.object.css("height", (this.object.parent().height() * this.size.y) + (this.size.y * 2) -1 +"px");
        this.object.attr("title", itemType.name);
        this.object.css("background-image", `url("${itemType.img}")`);
        let item = this;
        this.object.click(function(){
            if(!movingItem && !contextItem){
                makeItemMovable(item, itemType);
            }
            else if(movingItem == item && closestGrid){
                placeItem(item, closestGrid);
            }
        });
        let taken = []
        let row = parseInt(slot/gridPanel.x);
        for(var i = row; i < row + itemType.size.y; i++){
            for(var j = slot - row * gridPanel.x; j < (slot - row * gridPanel.x) + itemType.size.x; j++){
                let child = gridPanel.object.children()[i * gridPanel.x + j];
                $(child).addClass("taken");
                let index = i * gridPanel.x + j;
                taken.push(index);
            }
        }
        let thisItem = this;
        this.taken = taken;
        this.object.contextmenu(function(event){
            event.preventDefault();
            // mp.events.call("useItem", thisItem.itemType.id, thisItem.id);
            if(!movingItem){
                openContext(thisItem);
            }
            
        });
        this.object.mouseenter(function() {
            hoveredItem = thisItem;
          });
        this.object.mouseleave(function() {
        hoveredItem = null;
        });
    }
    setTaken(val){
        this.taken.forEach(index => {
            $(this.panel.object.children()[index]).removeClass("taken");
        });
        val.forEach(index => {
            $(this.panel.object.children()[index]).addClass("taken");
        });
        this.taken = val;
    }
    removeItem(update){
        // this.panel.items.splice($(this.panel.items).index(this), 1);
        this.panel.items[$(this.panel.items).index(this)] = null;
        this.taken.forEach(index => {
            $(this.panel.object.children()[index]).removeClass("taken");
        });
        this.object.remove();
        if(update){
            mp.events.call("updateEquipment", this.panel.getItems());
        }
        delete this;
    }
}

class ItemType{
    constructor(id, name, size, img){
        this.id = id;
        this.name = name;
        this.size = new Object();
        this.size.x = size[0];
        this.size.y = size[1];
        this.img = img;
    }
}

class GridPanel{
    constructor(x, y, parent){
        this.x = x;
        this.y = y;
        this.parent = parent;
        this.object = $(`<div class="equipment"></div>`);
        this.items = [];
        this.object.appendTo(this.parent);
        for(var i = 0; i < this.x * this.y; i++){
            $(`<div class="slot"></div>`).appendTo(this.object);
        }
        this.object.css("width", ($(".slot").outerWidth() * this.x + 10).toString()  + "px");   
    }
    getItems(){
        let itemArray = [];
        this.items.forEach(item => {
            if(item != null){
                let slot = item.slot.toString();
                let typeId = item.itemType.id;
                let id = item.id.toString();
                itemArray.push({
                    id: id,
                    slot: slot,
                    typeID: typeId
                });
            }
            
        });
        return JSON.stringify(itemArray);
    }
    changeSize(size){
        this.y = 4 + size;
        this.object.remove();
        this.object = $(`<div class="equipment"></div>`);
        this.object.appendTo(this.parent);
        for(var i = 0; i < this.x * this.y; i++){
            $(`<div class="slot"></div>`).appendTo(this.object);
        }
        this.object.css("width", ($(".slot").outerWidth() * this.x + 10).toString()  + "px");
    }
}

var items = [];
for (const [key, value] of Object.entries(itemTypes)) {
    items[key] = new ItemType(key, value["name"], [parseInt(value["width"]), parseInt(value["height"])], value["url"]);
}


equipmentGrid = new GridPanel(10, 4, $(".background"));
// let item = new Item(items[0], gridPanel, 2);
// new Item(items[1], gridPanel, 6);

// if(getFirstFreeGrid(items[0].size, gridPanel)){
//     let grid = getFirstFreeGrid(items[0].size, gridPanel) + 1;
//     new Item(items[0], gridPanel, grid);
// }





document.addEventListener('mousemove', (event) => {
    mouseX = event.clientX;
    mouseY = event.clientY;
});

function makeItemMovable(movItem){
    movItem.object.css("width", movItem.object.width() + "px");
    movItem.object.css("height", movItem.object.height() + "px");
    movItem.object.addClass("moving");
    movItem.object.offset({top: mouseY - movItem.object.height()/2, left: mouseX - movItem.object.width()/2});
    movingItem = movItem;
}

function placeItem(movItem, grid){
    setTakenGrids(movItem, grid);
    movItem.slot = movItem.panel.object.children().index(grid);
    $("*").removeClass("highlight");
    movItem.object.removeClass("moving");
    movItem.object.css("width", (movItem.object.parent().width() * movItem.size.x) + (movItem.size.x * 2) -1 + "px");
    movItem.object.css("height", (movItem.object.parent().height() * movItem.size.y) + (movItem.size.y * 2) -1 +"px");
    movItem.object.appendTo(grid);
    movItem.object.css("left", "0%");
    movItem.object.css("top", "0%");
    movingItem = null;
    updateEquipment();
}


function getClosestGrid(item, panel){
    var x = item.object.offset().left;
    var y = item.object.offset().top;
    var closest = null;
    panel.object.children().each(function(){
        if(!closest && (getEucDistance(x, y, $(this).offset().left, $(this).offset().top) < item.object.width() * 1.5) && doesSizeMatch(item, panel, $(this))){
            closest = $(this);
        }
        else{
            if(closest && (getEucDistance(x, y, $(this).offset().left, $(this).offset().top) < getEucDistance(x, y, closest.offset().left, closest.offset().top)) && getEucDistance(x, y, $(this).offset().left, $(this).offset().top) < item.object.width() * 1.5  && doesSizeMatch(item, panel, $(this))){
                closest = $(this);
            }
        }
    });
    if(closest){
        return closest;
    }
    return null;
}

function doesSizeMatch(item, panel, grid){
    $("*").removeClass("highlight");
    let gridIndex = panel.object.children().index(grid);
    let grids = [];
    for(var j = 0; j < item.itemType.size.y; j++){
        for(var i = (gridIndex + panel.x * j); i < (gridIndex + panel.x * j) + item.itemType.size.x; i++){
            if(panel.object.children()[i]){
                let child = panel.object.children()[i];
                if(!isInSameRow(gridIndex + panel.x * j, i, panel.x) || isTakenByAnotherItem($(child), item)){
                    return false;
                }
                else{
                    grids.push(child);
                }
            }
            else{
                return false;
            }
        }
    }
    grids.forEach(g => {
        $(g).addClass("highlight");
    });
    return true;
}
function isInSameRow(gridA, gridB, gridPanelX){
    let rowA = parseInt(gridA/gridPanelX);
    let rowB = parseInt(gridB/gridPanelX);
    return rowA == rowB;
}

function getEucDistance(ax, ay, bx, by){
    return Math.sqrt(Math.pow(ax - bx, 2) + Math.pow(ay - by, 2));
}

function setTakenGrids(item, grid){
    grid = item.panel.object.children().index(grid);
    let taken = []
    let row = parseInt(grid/item.panel.x);
    for(var i = row; i < row + item.size.y; i++){
        for(var j = grid - row * item.panel.x; j < (grid - row * item.panel.x) + item.size.x; j++){
            let child = item.panel.object.children()[i * item.panel.x + j];
            $(child).addClass("taken");
            let index = i * item.panel.x + j;
            taken.push(index);
        }
    }
    item.setTaken(taken);
}

function isTakenByAnotherItem(grid, item){
    if(item){
        if(grid.hasClass("taken") && !item.taken.includes(item.panel.object.children().index(grid))){
            return true;
        }
        else{
            return false;
        }
    }
    else{
        if(grid.hasClass("taken")){
            return true;
        }
        else{
            return false;
        }
    }
    
}

function getFirstFreeGrid(size, gridPanel){
    let grid = null;
    $(gridPanel.object).children().each(function(){
        if(willItemFit(size, gridPanel, $(this))){
            grid = gridPanel.object.children().index($(this));
            return false;
        }
    });
    return grid;
}

function willItemFit(size, panel, grid){
    let gridIndex = panel.object.children().index(grid);
    for(var j = 0; j < size.y; j++){
        for(var i = (gridIndex + panel.x * j); i < (gridIndex + panel.x * j) + size.x; i++){
            if(panel.object.children()[i]){
                let child = panel.object.children()[i];
                if(!isInSameRow(gridIndex + panel.x * j, i, panel.x) || isTakenByAnotherItem($(child), null)){
                    
                    return false;
                }
            }
            else{
                return false;
            }
        }
    }
    return true;
}

function addItem(itemId){
    var free = getFirstFreeGrid(items[itemId].size, equipmentGrid);
    if(free != null && free >=0){
        let grid = getFirstFreeGrid(items[itemId].size, equipmentGrid);
        createItem(itemId, equipmentGrid, grid);
        updateEquipment();
    }
    else{
        mp.events.call("showNotification", "Nie ma wolnego miejsca w ekwipunku!");
    }
}

function fitItem(itemId, gridPanel){
    var free = getFirstFreeGrid(items[itemId].size, equipmentGrid);
    if(free != null && free >=0){
        let grid = getFirstFreeGrid(items[itemId].size, gridPanel);
        createItem(itemId, gridPanel, grid);
        updateEquipment();
        mp.events.call("itemFit", true, itemId);
    }
    else{
        mp.events.call("itemFit", false, itemId);
    }
}
function doesItemFit(itemId, type){
    var free = getFirstFreeGrid(items[itemId].size, equipmentGrid);
    if(free != null && free >=0){
        updateEquipment();
        mp.events.call("itemWillFit", true, itemId, type);
    }
    else{
        mp.events.call("itemWillFit", false, itemId, type);
    }
}

function addItemInPlace(itemId, gridPanel, slot){
    createItem(itemId, gridPanel, slot);
}

function instantiateItems(eqString){
    eqString = JSON.parse(eqString);
    eqString.forEach(str => {
        createItem(parseInt(str["typeID"]), equipmentGrid, parseInt(str["slot"]), parseInt(str["id"]));
    });
}

function createItem(itemId, grid, slot, id = -1){
    let i = new Item(items[itemId], grid, slot);
    if(id != -1){
        grid.items.push(i);
        i.id = id;
    }else{
        grid.items.push(i);
        i.id = assignIdToItem();
    }   
}

function getItems(grid){
    let itemArray = [];
    grid.items.forEach(item => {
        if(item != null){
            let slot = item.slot.toString();
            let typeId = item.itemType.id;
            let id = item.id.toString();
            itemArray.push({
                id: id,
                slot: slot,
                typeID: typeId
            });
        }
    });
    return JSON.stringify(itemArray);
}

function updateEquipment(){
    mp.events.call("updateEquipment", getItems(equipmentGrid));
}

function switchEquipment(gui){
    if(gui){
        $(".background").css("display", "block");
        mp.events.call("toggleMouse", true);
    }
    else{
        $(".background").css("display", "none");
        hoverElement.css("display", "none");
        contextMenu.css("display", "none");
        mp.events.call("toggleMouse", false);
    }
}

function refreshEquipment(eqString){
    equipmentGrid.items.forEach(item => {
        if(item != null)
        item.removeItem(false);
    });
    equipmentGrid.items = [];
    instantiateItems(eqString);
    updateEquipment();
}

function removeItem(index, grid){
    grid.items.forEach(item => {
        if(item != null && item.id == index){
            item.removeItem(false);
            updateEquipment();
        }
    });
}

function assignIdToItem(){
    let highestId = 0;
    if(equipmentGrid.items && equipmentGrid.items.length > 0){
        equipmentGrid.items.forEach(item => {
            if(item && item.id > highestId){
                highestId = item.id;
            }
        });
    }
    return highestId + 1;
}

function openContext(item){
    if(movingItem == null){
        if(contextMenu.css("display") == "none"){
            contextMenu.css("display", "flex");
            contextMenu.css("top", mouseY - 10 + "px");
            contextMenu.css("left", mouseX + 10 + "px");
            hoverElement.css("display", "none");
            contextItem = item;
        }else{
            contextMenu.css("display", "none");
            contextItem = null;
        }
    }
}

$(".contextDrop").click(function(event){
    if(equipmentGrid.items.includes(contextItem)){
        mp.events.call("dropItem", contextItem.itemType.id, contextItem.itemType.name);
        removeItem(contextItem.id, equipmentGrid);
    }
    openContext(contextItem);
});

$(".contextUse").click(function(event){
    if(equipmentGrid.items.includes(contextItem)){
        mp.events.call("useItem", contextItem.itemType.id, contextItem.id);
    }
    openContext(contextItem);
});