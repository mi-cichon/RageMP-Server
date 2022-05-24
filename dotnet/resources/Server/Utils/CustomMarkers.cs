using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace ServerSide
{
    public class CustomMarkers
    {
        public CustomMarkers()
        {

        }




        public void CreateSimpleMarker(Vector3 position, string text, bool hold = false)
        {
            string push = "Naciśnij E";
            if (hold)
            {
                push = "Przytrzymaj E";
            }
            NAPI.TextLabel.CreateTextLabel(text, position + new Vector3(0, 0, 0.4), 6.0f, 2.0f, 4, new Color(255, 255, 255), dimension: 0);
            NAPI.TextLabel.CreateTextLabel(push, position + new Vector3(0, 0, 0.2), 6.0f, 2.0f, 4, new Color(255, 255, 255), dimension: 0);
            NAPI.Marker.CreateMarker(23, position - new Vector3(0, 0, 0.8), new Vector3(), new Vector3(), 1.0f, new Color(0, 204, 153), dimension: 0);
            NAPI.Marker.CreateMarker(20, position - new Vector3(0, 0, 0.6), new Vector3(), new Vector3(180,0,0), 0.7f, new Color(255, 255, 255), dimension: 0);
        }

        public void CreateLSPDMarker(Vector3 position, string text, bool hold = false)
        {
            string push = "Naciśnij E";
            if (hold)
            {
                push = "Przytrzymaj E";
            }
            NAPI.TextLabel.CreateTextLabel(text, position + new Vector3(0, 0, 0.4), 6.0f, 2.0f, 4, new Color(255, 255, 255), dimension: 0);
            NAPI.TextLabel.CreateTextLabel(push, position + new Vector3(0, 0, 0.2), 6.0f, 2.0f, 4, new Color(255, 255, 255), dimension: 0);
            NAPI.Marker.CreateMarker(23, position - new Vector3(0, 0, 0.8), new Vector3(), new Vector3(), 1.0f, new Color(41, 115, 184), dimension: 0);
            NAPI.Marker.CreateMarker(20, position - new Vector3(0, 0, 0.6), new Vector3(), new Vector3(180, 0, 0), 0.7f, new Color(255, 255, 255), dimension: 0);
        }

        public Marker CreateHouseMarker(Vector3 position, string text, uint id = 1000, bool owned = false)
        {
            NAPI.Marker.CreateMarker(23, position - new Vector3(0, 0, 0.8), new Vector3(), new Vector3(), 1.0f, new Color(255, 255, 255), dimension: id == 1000 ? 0 : id + 500);
            Color markerColor;
            if(owned)
            {
                markerColor = new Color(255,60,60);
            }
            else
            {
                markerColor = new Color(60,255,60);
            }
            Marker marker = NAPI.Marker.CreateMarker(20, position - new Vector3(0, 0, 0.6), new Vector3(), new Vector3(180,0,0), 0.7f, markerColor);
            if(id != 1000)
            {
                NAPI.TextLabel.CreateTextLabel(text, position + new Vector3(0, 0, 0.4), 6.0f, 2.0f, 4, new Color(255, 255, 255), dimension: id + 500);
                NAPI.TextLabel.CreateTextLabel("Naciśnij E", position + new Vector3(0, 0, 0.2), 6.0f, 2.0f, 4, new Color(255, 255, 255), dimension: id + 500);
                marker = NAPI.Marker.CreateMarker(20, position - new Vector3(0, 0, 0.6), new Vector3(), new Vector3(180,0,0), 0.6f, markerColor, dimension: id + 500);
            }
            
            return marker;
        }

        public void CreateHouseStorageMarker(Vector3 position, uint id = 1000)
        {
            NAPI.Marker.CreateMarker(23, position - new Vector3(0, 0, 0.8), new Vector3(), new Vector3(), 1.0f, new Color(255, 255, 255), dimension: id == 1000 ? 0 : id + 500);
            NAPI.Marker.CreateMarker(20, position - new Vector3(0, 0, 0.6), new Vector3(), new Vector3(180, 0, 0), 0.6f, new Color(0, 204, 153));
            NAPI.TextLabel.CreateTextLabel("Schowek na przedmioty", position + new Vector3(0, 0, 0.4), 6.0f, 2.0f, 4, new Color(255, 255, 255), dimension: id + 500);
            NAPI.TextLabel.CreateTextLabel("Naciśnij E", position + new Vector3(0, 0, 0.2), 6.0f, 2.0f, 4, new Color(255, 255, 255), dimension: id + 500);
        }

        public void CreateJobMarker(Vector3 position, string text)
        {
            NAPI.TextLabel.CreateTextLabel(text, position + new Vector3(0, 0, 0.4), 6.0f, 2.0f, 4, new Color(255, 255, 255));
            NAPI.TextLabel.CreateTextLabel("Naciśnij E aby rozpocząć pracę", position + new Vector3(0, 0, 0.2), 6.0f, 2.0f, 4, new Color(255, 255, 255));
            NAPI.Marker.CreateMarker(23, position - new Vector3(0, 0, 0.8), new Vector3(), new Vector3(), 1.0f, new Color(255, 255, 255));
            NAPI.Marker.CreateMarker(29, position - new Vector3(0, 0, 0.6), new Vector3(), new Vector3(0,0,0), 0.7f, new Color(34,139,34));
        }

        public CustomMarker CreateBusinessMarker(Vector3 position, string text, string owner)
        {
            List<TextLabel> labels = new List<TextLabel>();
            List<Marker> markers = new List<Marker>();
            if (owner != "")
            {
                labels.Add(NAPI.TextLabel.CreateTextLabel(owner, position + new Vector3(0, 0, 0.6), 6.0f, 2.0f, 4, new Color(255, 253, 141)));
            }
            else
            {
                labels.Add(NAPI.TextLabel.CreateTextLabel("Brak właściciela", position + new Vector3(0, 0, 0.6), 6.0f, 2.0f, 4, new Color(255, 255, 255)));
            }

            labels.Add(NAPI.TextLabel.CreateTextLabel(text, position + new Vector3(0, 0, 0.4), 6.0f, 2.0f, 4, new Color(255, 255, 255)));
            labels.Add(NAPI.TextLabel.CreateTextLabel("Naciśnij E", position + new Vector3(0, 0, 0.2), 6.0f, 2.0f, 4, new Color(255, 255, 255)));
            markers.Add(NAPI.Marker.CreateMarker(23, position - new Vector3(0, 0, 0.8), new Vector3(), new Vector3(), 1.0f, new Color(255, 255, 255)));
            markers.Add(NAPI.Marker.CreateMarker(29, position - new Vector3(0, 0, 0.6), new Vector3(), new Vector3(0, 0, 0), 0.7f, new Color(191, 216, 99)));

            return new CustomMarker(labels, markers);
        }

        public void CreateVehicleSpawnMarker(Vector3 position)
        {
            NAPI.Marker.CreateMarker(27, new Vector3(position.X, position.Y, position.Z - 0.8), new Vector3(), new Vector3(), 2.0f, new Color(0, 204, 153));
            NAPI.TextLabel.CreateTextLabel("Pojazd pracy", new Vector3(position.X, position.Y, position.Z - 0.5), 5.0f, 1.0f, 4, new Color(255, 255, 255, 255), entitySeethrough: false);
        }
    }

    public class CustomMarker
    {
        public List<TextLabel> Labels = new List<TextLabel>();
        public List<Marker> Markers = new List<Marker>();
        public CustomMarker(List<TextLabel> labels, List<Marker> markers)
        {
            this.Labels = labels;
            this.Markers = markers;
        }
        public void Delete()
        {
            foreach(Marker marker in Markers)
            {
                if(marker != null && marker.Exists)
                {
                    marker.Delete();
                }
            }
            foreach (TextLabel label in Labels)
            {
                if (label != null && label.Exists)
                {
                    label.Delete();
                }
            }
        }
    }
}
