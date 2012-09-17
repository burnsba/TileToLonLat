using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace TileToLonLat
{
        /// <summary>
        /// Simple tile class
        /// </summary>
        public class OsmTile
        {
                private int _XColumn;
                private int _YRow;
                private int _Zoom;

                public int YRow
                {
                        get { return _YRow; }
                        set { _YRow = value; }
                }

                public int XColumn
                {
                        get { return _XColumn; }
                        set { _XColumn = value; }
                }

                public int Zoom
                {
                        get { return _Zoom; }
                        set { _Zoom = value; }
                }


                public OsmTile()
                {
                        this.Zoom = 1;
                        this.XColumn = 0;
                        this.YRow = 0;
                }

                /// <summary>
                /// Copy constructor
                /// </summary>
                /// <param name="t"></param>
                public OsmTile(OsmTile t)
                        : this()
                {
                        if (t != null)
                        {
                                this.XColumn = t.XColumn;
                                this.YRow = t.YRow;
                                this.Zoom = t.Zoom;
                        }
                }
                /// <summary>
                /// Creates a tile for the given row and column. Zoom is not set.
                /// </summary>
                /// <param name="x">Column of new tile</param>
                /// <param name="y">Row of new tile</param>
                public OsmTile(int x, int y)
                        : this()
                {
                        this.XColumn = x;
                        this.YRow = y;
                }
                /// <summary>
                /// Creates a tile
                /// </summary>
                /// <param name="x">Column of new tile</param>
                /// <param name="y">Row of new tile</param>
                /// <param name="z">Zoom of new tile</param>
                public OsmTile(int x, int y, int z)
                        : this(x, y)
                {
                        this.Zoom = z;
                }

                /// <summary>
                /// Show tile as xcol, yrow, zoom
                /// </summary>
                /// <returns></returns>
                override public String ToString()
                {
                        string s = "";
                        s += XColumn.ToString() + ", " + YRow.ToString() + ", " + Zoom.ToString();
                        return s;
                }

                /// <summary>
                /// Helper function for OSM tile maths
                /// </summary>
                /// <param name="zoom"></param>
                /// <returns></returns>
                private static double ExpandZoom(int zoom)
                {
                        return (double)(1 << zoom);
                }
                /// <summary>
                /// Helper function for OSM tile maths
                /// </summary>
                /// <param name="latitude"></param>
                /// <returns></returns>
                private static double ExpandLatToRow(double latitude)
                {
                        return (double)((1.0 - System.Math.Log(System.Math.Tan((double)latitude * System.Math.PI / 180.0) + 1.0 / System.Math.Cos((double)latitude * System.Math.PI / 180.0)) / System.Math.PI) / 2.0);
                }
                /// <summary>
                /// Helper function for OSM tile maths
                /// </summary>
                /// <param name="longitude"></param>
                /// <returns></returns>
                private static double ExpandLonToCol(double longitude)
                {
                        return (longitude + 180.0) / 360.0;
                }

                /// <summary>
                /// Returns the (possibly fractional) OSM tile row number from the given latitude and zoom
                /// </summary>
                /// <param name="latitude"></param>
                /// <param name="zoom"></param>
                /// <returns></returns>
                public static double ExactYRowFromLat(double latitude, int zoom)
                {
                        double decimalZoom = OsmTile.ExpandZoom(zoom);
                        double decimalY = OsmTile.ExpandLatToRow(latitude);
                        return (decimalY * decimalZoom);
                }
                /// <summary>
                /// Returns the (possibly fractional) OSM tile column number from the given longitude and zoom
                /// </summary>
                /// <param name="longitude"></param>
                /// <param name="zoom"></param>
                /// <returns></returns>
                public static double ExactXColFromLon(double longitude, int zoom)
                {
                        double decimalZoom = OsmTile.ExpandZoom(zoom);
                        double decimalX = OsmTile.ExpandLonToCol(longitude);
                        return (decimalX * decimalZoom);
                }
                /// <summary>
                /// Returns the OSM tile row number from the given latitude and zoom. The result is truncated to the nearest tile.
                /// </summary>
                /// <param name="latitude"></param>
                /// <param name="zoom"></param>
                /// <returns></returns>
                public static int YRowFromLat(double latitude, int zoom)
                {
                        return (int)System.Math.Floor(OsmTile.ExactYRowFromLat(latitude, zoom));
                }
                /// <summary>
                /// Returns the OSM tile column number from the given longitude and zoom. The result is truncated to the nearest tile.
                /// </summary>
                /// <param name="longitude">longitude in degrees</param>
                /// <param name="zoom"></param>
                /// <returns></returns>
                public static int XColFromLon(double longitude, int zoom)
                {
                        return (int)System.Math.Floor(OsmTile.ExactXColFromLon(longitude, zoom));
                }

                /// <summary>
                /// Returns the longitude the tile is located at for the for given zoom level.
                /// </summary>
                public static double LongitudeFromTile(OsmTile t)
                {
                        return OsmTile.LongitudeFromColumn(t.XColumn, t.Zoom);
                }
                /// <summary>
                /// Returns the longitude the tile is located at for the for given zoom level.
                /// </summary>
                /// <param name="xcol">OSM tile column</param>
                /// <param name="zoom">OSM zoom level</param>
                public static double LongitudeFromColumn(int xcol, int zoom)
                {
                        double decimalZoom = ExpandZoom(zoom);
                        double longitude = xcol / decimalZoom * 360.0 - 180.0;
                        return longitude;
                }
                /// <summary>
                /// Returns the latitude the tile is located at for the for given zoom level.
                /// </summary>
                public static double LatitudeFromTile(OsmTile t)
                {
                        return OsmTile.LatitudeFromRow(t.YRow, t.Zoom);
                }
                /// <summary>
                /// Returns the latitude the tile is located at for the for given zoom level.
                /// </summary>
                /// <param name="yrow">OSM tile column</param>
                /// <param name="zoom">OSM zoom level</param>
                public static double LatitudeFromRow(int yrow, int zoom)
                {
                        double decimalZoom = ExpandZoom(zoom);
                        double n = (System.Math.PI - 2 * System.Math.PI * yrow / decimalZoom);
                        double latitude = (180.0 / System.Math.PI * System.Math.Atan(0.5 * (System.Math.Exp(n) - System.Math.Exp(-n))));
                        return latitude;
                }

                /// <summary>
                /// Creates a tile using X for the column and Y for the row. Input is not checked for validity. Zoom is not set.
                /// </summary>
                /// <param name="xcol"></param>
                /// <param name="yrow"></param>
                /// <returns></returns>
                public static OsmTile FromColRow(int xcol, int yrow)
                {
                        // no sanity checks, hey.
                        return new OsmTile(xcol, yrow);
                }

        }



}
