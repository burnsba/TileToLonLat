using System;
using System.Windows.Input;
using System.ComponentModel;


namespace TileToLonLat.ViewModel
{
        class OsmTileViewModel : WorkspaceViewModel, IDataErrorInfo 
        {
                // background data to work with
                OsmTile _tile;

                // throw an event when the row/column/zoom changes to update the lat/lon
                public event EventHandler<LonLatStringEventArgs> TileChanged;

                public OsmTileViewModel()
                {
                        if (_tile == null)
                        {
                                _tile = new OsmTile();
                        }

                        base.DisplayName = "OsmTile";
                }

                // this is uesd for validation by Idataerrorinfo
                public string this[string item]
                {
                        get
                        {
                                string result = null;
                                if (item == "XColumn")
                                {
                                        if (XColumn < 0)
                                                result = "Column must be positive";

                                        // make sure column is in range
                                        int max_col = OsmTile.XColFromLon(179.99, Zoom);
                                        if (XColumn > max_col)
                                                result = "Column too large for zoom level. (Max " + max_col.ToString() + ")";
                                        
                                }
                                if (item == "YRow")
                                {
                                        if (YRow < 0)
                                                result = "Row must be positive";

                                        // make sure row is in range
                                        int max_row = OsmTile.YRowFromLat(-84.99, Zoom);
                                        if (YRow > max_row)
                                                result = "Row too large for zoom level. (Max " + max_row.ToString() + ")";
                                }
                                if (item == "Zoom")
                                {
                                        // make sure zoom is valid
                                        if (Zoom < 0 || Zoom > 31)
                                                result = "Zoom must be between 0 and 31";
                                }
                                return result;
                        }

                }
                // must be implemented for Idataerrorinfo
                public string Error
                {
                        get { throw new NotImplementedException(); }
                }  

                public int XColumn
                {
                        get
                        {
                                return _tile.XColumn;
                        }
                        set
                        {
                                // don't update if it's not necessary
                                if (_tile.XColumn == value)
                                        return;

                                _tile.XColumn = value;

                                // fire event
                                if (this.TileChanged != null)
                                        this.TileChanged(this, new LonLatStringEventArgs(
                                                OsmTile.LongitudeFromColumn(_tile.XColumn, _tile.Zoom),
                                                OsmTile.LatitudeFromRow(_tile.YRow, _tile.Zoom)
                                                ));
                                // update validation
                                base.OnPropertyChanged("XColumn");
                        }
                }
                public int YRow
                {
                        get
                        {
                                return _tile.YRow;
                        }
                        set
                        {
                                // don't update if it's not necessary
                                if (_tile.YRow == value)
                                        return;

                                _tile.YRow = value;

                                // fire event
                                if (this.TileChanged != null)
                                        this.TileChanged(this, new LonLatStringEventArgs(
                                                 OsmTile.LongitudeFromColumn(_tile.XColumn, _tile.Zoom),
                                                 OsmTile.LatitudeFromRow(_tile.YRow, _tile.Zoom)
                                                 ));
                                // update validation
                                base.OnPropertyChanged("YRow");
                        }
                }
                public int Zoom
                {
                        get
                        {
                                return _tile.Zoom;
                        }
                        set
                        {
                                // don't update if it's not necessary
                                if (_tile.Zoom == value)
                                        return;

                                _tile.Zoom = value;

                                // fire event
                                if (this.TileChanged != null)
                                        this.TileChanged(this, new LonLatStringEventArgs(
                                                OsmTile.LongitudeFromColumn(_tile.XColumn, _tile.Zoom),
                                                OsmTile.LatitudeFromRow(_tile.YRow, _tile.Zoom)
                                                ));
                                // update validation
                                base.OnPropertyChanged("Zoom");
                        }
                }

                // event handler for lat/lon changes
                public void OnReceiveLonLat(object sender, LonLatStringEventArgs e)
                {
                        this.XColumn = OsmTile.XColFromLon(e.Longitude, _tile.Zoom);
                        this.YRow = OsmTile.YRowFromLat(e.Latitude, _tile.Zoom);
                }
        }
}
