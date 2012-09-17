using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TileToLonLat
{
        public class LonLatStringEventArgs : EventArgs
        {
                public LonLatStringEventArgs(double lon, double lat)
                {
                        this.Longitude = lon;
                        this.Latitude = lat;
                }

                public double Longitude { get; private set; }
                public double Latitude { get; private set; }
        }
}
