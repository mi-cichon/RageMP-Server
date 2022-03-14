const Use3d = true;
const UseAutoVolume = true;
let lplayer = mp.players.local;
const MaxRange = 50.0;


setTimeout(function(){
	if(mp.voiceChat.muted)
		mp.voiceChat.cleanupAndReload(true, true, true);
}, 5000);

mp.events.add("render", () => {
	if(lplayer.hasVariable("settings_VoiceKey") && lplayer.hasVariable("settings_VoiceChat"))
	if(mp.keys.isUp(lplayer.getVariable("settings_VoiceKey")) && !mp.voiceChat.muted  && lplayer.getVariable("settings_VoiceChat")){
		mp.voiceChat.muted = true;
		mp.events.callRemote("setVoiceChat", false);
	}
	if(mp.keys.isDown(lplayer.getVariable("settings_VoiceKey")) && mp.voiceChat.muted && !lplayer.getVariable("controlsblocked") && !mp.players.local.getVariable("muted")  && lplayer.getVariable("settings_VoiceChat")){
		mp.voiceChat.muted = false;
		mp.events.callRemote("setVoiceChat", true);
	}
});

let g_voiceMgr =
{
	listeners: [],
	
	add: function(player)
	{
		this.listeners.push(player);
		
		player.isListening = true;		
		mp.events.callRemote("add_voice_listener", player);
		
		if(UseAutoVolume)
		{
			player.voiceAutoVolume = true;
		}
		else
		{
			player.voiceVolume = 1.0;
		}
		
		if(Use3d)
		{
			player.voice3d = true;
		}
	},
	
	remove: function(player, notify)
	{
		let idx = this.listeners.indexOf(player);
			
		if(idx !== -1)
			this.listeners.splice(idx, 1);
			
		player.isListening = false;		
		
		if(notify)
		{
			mp.events.callRemote("remove_voice_listener", player);
		}
	}
};

mp.events.add("playerQuit", (player) =>
{
	if(player.isListening)
	{
		g_voiceMgr.remove(player, false);
	}
});

setInterval(() =>
{
	let localPlayer = mp.players.local;
	let localPos = localPlayer.position;
	let sound = true;
	if(lplayer.hasVariable("settings_VoiceChat") && !lplayer.getVariable("settings_VoiceChat")){
		sound = false;
	}
	mp.players.forEachInStreamRange(player =>
	{
		if(player != localPlayer)
		{
			if(!player.isListening)
			{
				const playerPos = player.position;		
				let dist = mp.game.system.vdist(playerPos.x, playerPos.y, playerPos.z, localPos.x, localPos.y, localPos.z);
				
				if(dist <= MaxRange)
				{
					g_voiceMgr.add(player);
					
				}
				if(!sound){
					player.voiceVolume = 0;
				}
			}
		}
	});
	
	g_voiceMgr.listeners.forEach((player) =>
	{
		if(player.handle !== 0)
		{
			const playerPos = player.position;		
			let dist = mp.game.system.vdist(playerPos.x, playerPos.y, playerPos.z, localPos.x, localPos.y, localPos.z);
			
			if(dist > MaxRange)
			{
				g_voiceMgr.remove(player, true);
			}
			else if(!UseAutoVolume)
			{
				player.voiceVolume = 1 - (dist / MaxRange);
			}
			if(!sound){
				player.voiceVolume = 0;
			}
		}
		else
		{
			g_voiceMgr.remove(player, true);
		}
	});
	
}, 500);