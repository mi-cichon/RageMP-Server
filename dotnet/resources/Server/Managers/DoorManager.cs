using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace ServerSide
{
    public class DoorManager
    {
        public List<Door> Doors = new List<Door>();
        public DoorManager()
        {
            MissionRowLSPD();
        }

        private void MissionRowLSPD()
        {
            Doors.Add(new Door(1002, -2023754432, true, new Vector3(469.9679, -1014.452, 26.53623)));
            Doors.Add(new Door(1003, -2023754432, true, new Vector3(467.3716, -1014.452, 26.53623)));
            Doors.Add(new Door(1009, -1320876379, true, new Vector3(446.5728, -980.0106, 30.8393)));
            Doors.Add(new Door(1013, 1557126584, true, new Vector3(450.1041, -985.7384, 30.8393)));
            Doors.Add(new Door(1017, 185711165, true, new Vector3(443.4078, -989.4454, 30.8393)));
            Doors.Add(new Door(1018, 185711165, true, new Vector3(446.0079, -989.4454, 30.8393)));
            Doors.Add(new Door(1025, 1817008884, true, new Vector3(422.7392, -998.1159, 30.97704)));
        }

        public void SetDoorsForPlayer(Player player)
        {
            foreach(Door door in Doors)
            {
                player.TriggerEvent("door_state", door.Hash, door.Position.X, door.Position.Y, door.Position.Z, door.Locked);
            }
        }
    }

    public class Door
    {
        public int Id { get; set; }
        public int Hash { get; set; }
        public bool Locked { get; set; }
        public Vector3 Position { get; set; }

        public Door(int id, int hash, bool locked, Vector3 position)
        {
            Id = id;
            Hash = hash;
            Locked = locked;
            Position = position;
        }
    }
}
