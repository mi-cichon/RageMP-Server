mp.events.call("mainPanel_requestStatsJobData");

function insertStatsJobData(jobData){
    jobData = JSON.parse(jobData);
    
    jobData.forEach(data => {
        let currentExp = parseInt(data[2]);
        let nextLevelExp = parseInt(data[3]);
        let currentLevelExp = parseInt(data[4]);
        let left = currentExp - currentLevelExp;
        let right = nextLevelExp - currentLevelExp;

        $('.panel_stats_job > .job_list').append(`
            <div class="job_desc">
                <p>${data[0]}: ${data[1]} lvl</p>
                <p>${data[2]}/${data[3]} exp</p>
            </div>
            <div class="job_bar">
                <div style="width: ${data[3] == "0" ? 100 : parseInt(left/right * 100)}%"></div>
            </div>
        `);
    });
}