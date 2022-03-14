//casino

let interiors = [
    {ipl: 'vw_casino_main', pos: new mp.Vector3(1100.000, 220.000, -50.000)},
    {ipl:'vw_casino_garage', pos: new mp.Vector3(1295.000, 230.000, -50.000)},	
    {ipl:'vw_casino_carpark', pos: new mp.Vector3(1380.000, 200.000, -50.000)},	
    {ipl:'vw_casino_penthouse', pos: new mp.Vector3(976.636, 70.295, 115.164)}	
];

mp.events.add("showCasino", () => {
    let phIntID = mp.game.interior.getInteriorAtCoords(976.636, 70.295, 115.164);
        let phPropList = [
            "Set_Pent_Tint_Shell",
            "Set_Pent_Pattern_01",
            "Set_Pent_Spa_Bar_Open",
            "Set_Pent_Media_Bar_Open",
            "Set_Pent_Dealer",
            "Set_Pent_Arcade_Modern",
            "Set_Pent_Bar_Clutter",
            "Set_Pent_Clutter_01",
            "set_pent_bar_light_01",
            "set_pent_bar_party_0"
        ];
        for (const propName of phPropList) 
        {
            mp.game.interior.enableInteriorProp(phIntID, propName);
            mp.game.invoke("0xC1F1920BAF281317", phIntID, propName, 1);
        }
        mp.game.interior.refreshInterior(phIntID);
});
