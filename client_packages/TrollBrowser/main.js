let trollBrowser = null;
mp.events.add("openTrollBrowser", (type = "") => {
    if(trollBrowser && mp.browsers.exists(trollBrowser)){
        trollBrowser.destroy();
        trollBrowser = null;
    }
    else{
        trollBrowser = mp.browsers.new('package://TrollBrowser/index.html');
        trollBrowser.execute(`start('${type}')`);
    }
});

mp.events.add("closeTrollBrowser", () => {
    if(trollBrowser){
        trollBrowser.destroy();
        trollBrowser = null;
    }
});