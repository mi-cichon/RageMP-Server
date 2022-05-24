using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
namespace ServerSide
{
    public class PetrolStation
    {
        public ColShape vehicleColShape;
        public ColShape distributorColShape;
        CustomMarkers customMarkers = new CustomMarkers();
        public Vector3 vehiclePosition;
        public Vehicle currentVehicle = null;
        public PetrolStation(Vector3 vehiclePosition, Vector3 distributorPosition)
        {
            vehiclePosition = vehiclePosition - new Vector3(0, 0, 0.8);
            this.vehiclePosition = vehiclePosition;
            vehicleColShape = NAPI.ColShape.CreateCylinderColShape(vehiclePosition, 3f, 3.0f);
            vehicleColShape.SetSharedData("type", "stationveh");
            NAPI.Marker.CreateMarker(25, vehiclePosition, new Vector3(), new Vector3(), 2.0f, new Color(255, 255, 255));
            NAPI.TextLabel.CreateTextLabel("Stanowisko napełniania", vehiclePosition + new Vector3(0, 0, 1.5), 10.0f, 2.0f, 4, new Color(255, 255, 255));
            NAPI.Blip.CreateBlip(361, vehiclePosition, 0.5f, 4, name: "Stacja paliw", shortRange: true);

            distributorColShape = NAPI.ColShape.CreateCylinderColShape(distributorPosition - new Vector3(0,0,0.8), 1.0f, 2.0f);
            distributorColShape.SetSharedData("type", "distributor");
            distributorColShape.SetSharedData("vehpos", vehiclePosition);
            customMarkers.CreateSimpleMarker(distributorPosition, "Dystrybutor", true);
        }
    }
}
