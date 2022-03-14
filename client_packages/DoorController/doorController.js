let player = mp.players.local;


const doors = [
    {id: 1002, hash: -2023754432,  position: new mp.Vector3(469.9679, -1014.452, 26.53623)},
    {id: 1003, hash: -2023754432,  position: new mp.Vector3(467.3716, -1014.452, 26.53623)},
    {id: 1009, hash: -1320876379,  position: new mp.Vector3(446.5728, -980.0106, 30.8393)}, 
    {id: 1013, hash: 1557126584,   position: new mp.Vector3(450.1041, -985.7384, 30.8393)}, 
    {id: 1017, hash: 185711165,    position: new mp.Vector3(443.4078, -989.4454, 30.8393)},
    {id: 1018, hash: 185711165,    position: new mp.Vector3(446.0079, -989.4454, 30.8393)},
    {id: 1025, hash: 1817008884,   position: new mp.Vector3(422.7392, -998.1159, 30.97704)}
]


mp.keys.bind(0x45, true, () =>
{
    if(player.hasVariable("lspd_power") && player.getVariable("lspd_power") > 0){
        doors.forEach((door) =>
        {
            if(mp.game.gameplay.getDistanceBetweenCoords(door.position.x, door.position.y, door.position.z, mp.players.local.position.x, mp.players.local.position.y, mp.players.local.position.z, true) < 2)
            {
                if(door.id != 1003 && door.id != 1018){
                    if(door.id == 1002){
                        mp.events.callRemote("door_switch", door.id, true);
                        mp.events.callRemote("door_switch", 1003, false);
                    }
                    else if(door.id == 1017){
                        mp.events.callRemote("door_switch", door.id, true);
                        mp.events.callRemote("door_switch", 1018, false);
                    }
                    else{
                        mp.events.callRemote("door_switch", door.id, true);
                    }
                }
            }
        });
    }
});

mp.events.add("door_state", (hash, x, y, z, state) => {
    mp.game.object.doorControl(hash, x, y, z, state, 0.0, 0.0, 0.0);
});

const textShapes = [
    {shape: mp.colshapes.newTube(423.86166, -998.0099, 30.710981, 2, 2.0), label: null, position: new mp.Vector3(423.86166, -998.0099, 31.710981)},
    {shape: mp.colshapes.newTube(444.75317, -989.4086, 30.689604, 2, 2.0), label: null, position: new mp.Vector3(444.75317, -989.4086, 31.689604)},
    {shape: mp.colshapes.newTube(447.3121, -979.9772, 30.689602, 2, 2.0), label: null, position: new mp.Vector3(447.3121, -979.9772, 31.689602)},
    {shape: mp.colshapes.newTube(450.04004, -986.4068, 30.6896, 2, 2.0), label: null, position: new mp.Vector3(450.04004, -986.4068, 31.6896)},
    {shape: mp.colshapes.newTube(468.64685, -1014.6321, 26.386383, 2, 2.0), label: null, position: new mp.Vector3(468.64685, -1014.6321, 27.386383)}
]



mp.events.add('playerEnterColshape', (_shape) => {
    if(player.hasVariable("lspd_power") && player.getVariable("lspd_power") > 0){
        textShapes.forEach(shape => {
            if(shape.shape == _shape){
                if(shape.label == null){
                    shape.label = mp.labels.new("Naciśnij E aby otworzyć/zamknąć drzwi", shape.position, {
                        color: [255,255,255,255],
                        drawDistance: 3.5,
                        font: 4,
                        los: false
                    });
                }
            }
        })
    }
});

mp.events.add('playerExitColshape', (_shape) => {
    if(player.hasVariable("lspd_power") && player.getVariable("lspd_power") > 0){
        textShapes.forEach(shape => {
            if(shape.shape == _shape){
                if(shape.label != null && mp.labels.exists(shape.label)){
                    shape.label.destroy();
                    shape.label = null;
                }
            }
        })
    }
});