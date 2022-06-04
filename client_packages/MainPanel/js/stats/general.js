mp.events.call("mainPanel_requestStatsGeneralData");

function insertStatsGeneralData(generalData){
    generalData = JSON.parse(generalData);
    
    let time = parseInt(generalData[4]);
    let hours = Math.floor(time/60);
    let minutes = time%60;

    $('.panel_stats_general > .general_list').append(`
        <div><b>Nick</b><i class="fa-solid fa-arrow-right-long"></i><p>${generalData[0]}</p></div>
        <div><b>SocialClubID</b><i class="fa-solid fa-arrow-right-long"></i><p>${generalData[1]}</p></div>
        <div><b>ServerID</b><i class="fa-solid fa-arrow-right-long"></i><p>${generalData[2]}</p></div>
        <div><b>Data rejestracji</b><i class="fa-solid fa-arrow-right-long"></i><p>${generalData[3]}</p></div>
        <div><b>Łączny czas gry</b><i class="fa-solid fa-arrow-right-long"></i><p>${hours}h ${minutes}m</p></div>
        <div><b>Postęp poziomu</b><i class="fa-solid fa-arrow-right-long"></i><p>${generalData[5]}</p></div>
        <div><b>Ilość pojazdów</b><i class="fa-solid fa-arrow-right-long"></i><p>${generalData[6]}</p></div>
        <div><b>Punkty umiejętności</b><i class="fa-solid fa-arrow-right-long"></i><p>${generalData[7]}</p></div>
        <div><b>Postęp znajdziek</b><i class="fa-solid fa-arrow-right-long"></i><p>${generalData[8]}</p></div>
        <div><b>Do bonusu dziennego</b><i class="fa-solid fa-arrow-right-long"></i><p>${generalData[9]} minut</p></div>
    `);
}