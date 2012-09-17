using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace TileToLonLat.ViewModel
{

        class LonLatItemsViewModel : ViewModelBase, IDataErrorInfo 
        {
                // data objects to use for display: lat and lon
                private double _latitude;
                private double _longitude;

                // throw an event when the lat/lon changes to update any listening tiles
                public event EventHandler<LonLatStringEventArgs> LonLatChanged;

                public LonLatItemsViewModel()
                {
                        _latitude = 0.0;
                        _longitude = 0.0;
                }

                // this is uesd for validation by Idataerrorinfo
                public string this[string item]
                {
                        get
                        {
                                string result = null;
                                if (item == "Latitude")
                                {
                                        if (Latitude > 90.0 || Latitude < -90.0)
                                                result = "Latitude must be within the range -90.0 to 90.0.";
                                }
                                if (item == "Longitude")
                                {
                                        if (Longitude > 180.0 || Longitude < -180.0)
                                                result = "Longitude must be within the range -180.0 to 180.0.";
                                }
                                return result;
                        }

                }
                // must be implemented for Idataerrorinfo
                public string Error
                {
                        get { throw new NotImplementedException(); }
                }  

                public double Longitude
                {
                        get
                        {
                                return _longitude;
                        }
                        set
                        {
                                // don't update if it's not necessary
                                if (_longitude == value)
                                        return;

                                _longitude = value;

                                // fire event
                                if (this.LonLatChanged != null)
                                        this.LonLatChanged(this, new LonLatStringEventArgs(_longitude, _latitude));

                                // update combined string
                                OnPropertyChanged("LonLatString");
                                // and update validation
                                base.OnPropertyChanged("Longitude");
                        }
                }
                public double Latitude
                {
                        get
                        {
                                return _latitude;
                        }
                        set
                        {
                                // don't update if it's not necessary
                                if (_latitude == value)
                                        return;

                                _latitude = value;

                                // fire event
                                if (this.LonLatChanged != null)
                                        this.LonLatChanged(this, new LonLatStringEventArgs(_longitude, _latitude));

                                // update combined string
                                OnPropertyChanged("LonLatString");
                                // and update validation
                                base.OnPropertyChanged("Latitude");
                        }
                }

                /// <summary>
                /// Combines longitude and latitude into a more user friendly single string
                /// </summary>
                public string LonLatString
                {
                        get
                        {
                                if (double.IsNaN(this._longitude) == true ||
                                        double.IsInfinity(this._longitude) == true ||
                                        double.IsNaN(this._latitude) == true ||
                                        double.IsInfinity(this._latitude) == true ||
                                        this._latitude < -90.0 || 
                                        this._latitude > 90.0 ||
                                        this._longitude < -180.0 ||
                                        this._longitude > 180.0)
                                {
                                        return "NaN";
                                }

                                string output = "";

                                output += _longitude.ToString("N6");
                                        
                                if (this._longitude < 0)
                                {
                                        output += "W";
                                }
                                else
                                {
                                        output += "E";
                                }

                                output += ", ";

                                output += _latitude.ToString("N6");

                                if (this._latitude < 0)
                                {
                                        output += "S";
                                }
                                else
                                {
                                        output += "N";
                                }

                                return output;
                        }
                }

                // event handler for tile changes
                public void OnReceieveOsmTile(object sender, LonLatStringEventArgs e)
                {
                        this.Latitude = e.Latitude;
                        this.Longitude = e.Longitude;
                }
        }
}
