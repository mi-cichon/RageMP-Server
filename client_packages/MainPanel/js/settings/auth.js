mp.events.call("mainPanel_requestAuthSettingsData");

var authcode = "";

function insertAuthSettings(ac){
    authcode = ac;
}

function copyToClipboard(str){
    const el = document.createElement('textarea');
    el.value = str;
    document.body.appendChild(el);
    el.select();
    document.execCommand('copy');
    document.body.removeChild(el);
  };