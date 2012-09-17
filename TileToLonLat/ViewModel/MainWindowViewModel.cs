using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Data;
using System.Text;

namespace TileToLonLat.ViewModel
{
        class MainWindowViewModel : WorkspaceViewModel
        {
                /// <summary>
                /// Tiles to convert
                /// </summary>
                ReadOnlyCollection<CommandViewModel> _commands;
                /// <summary>
                /// Tile converting workspace
                /// </summary>
                ObservableCollection<WorkspaceViewModel> _workspaces;
                /// <summary>
                /// This should be a single view, for the lat/lon texts
                /// </summary>
                ReadOnlyCollection<LonLatItemsViewModel> _lonLatItems;


                public MainWindowViewModel()
                {
                        base.DisplayName = "Tile to Lon/Lat";
                }

                /// <summary>
                /// Returns the collection of available workspaces to display.
                /// A 'workspace' is a ViewModel that can request to be closed.
                /// </summary>
                public ObservableCollection<WorkspaceViewModel> Workspaces
                {
                        get
                        {
                                if (_workspaces == null)
                                {
                                        _workspaces = new ObservableCollection<WorkspaceViewModel>();
                                        _workspaces.CollectionChanged += this.OnWorkspacesChanged;
                                }
                                return _workspaces;
                        }
                }

                void OnWorkspacesChanged(object sender, NotifyCollectionChangedEventArgs e)
                {
                        if (e.NewItems != null && e.NewItems.Count != 0)
                                foreach (WorkspaceViewModel workspace in e.NewItems)
                                        workspace.RequestClose += this.OnWorkspaceRequestClose;

                        if (e.OldItems != null && e.OldItems.Count != 0)
                                foreach (WorkspaceViewModel workspace in e.OldItems)
                                        workspace.RequestClose -= this.OnWorkspaceRequestClose;
                }

                void OnWorkspaceRequestClose(object sender, EventArgs e)
                {
                        WorkspaceViewModel workspace = sender as WorkspaceViewModel;
                        workspace.Dispose();
                        this.Workspaces.Remove(workspace);
                }

                /// <summary>
                /// changes to another tab
                /// </summary>
                /// <param name="workspace"></param>
                void SetActiveWorkspace(WorkspaceViewModel workspace)
                {
                        Debug.Assert(this.Workspaces.Contains(workspace));

                        ICollectionView collectionView = CollectionViewSource.GetDefaultView(this.Workspaces);
                        if (collectionView != null)
                                collectionView.MoveCurrentTo(workspace);
                }

                /// <summary>
                /// Returns a read-only list of commands 
                /// that the UI can display and execute.
                /// </summary>
                public ReadOnlyCollection<CommandViewModel> Commands
                {
                        get
                        {
                                if (_commands == null)
                                {
                                        List<CommandViewModel> cmds = this.CreateCommands();
                                        _commands = new ReadOnlyCollection<CommandViewModel>(cmds);
                                }
                                return _commands;
                        }
                }

                /// <summary>
                /// right now, only supports conversion from open street map tile to lat/lon
                /// </summary>
                /// <returns></returns>
                List<CommandViewModel> CreateCommands()
                {
                        return new List<CommandViewModel>
                        {
                        new CommandViewModel(
                                "OsmTile",
                                // the command is set in the commandviewmodel, but this will call a function here
                                // in the parent
                                new RelayCommand(param => this.ShowOsmTile()))
                        };
                }

                void ShowOsmTile()
                {
                        OsmTileViewModel workspace =
                                this.Workspaces.FirstOrDefault(vm => vm is OsmTileViewModel)
                                as OsmTileViewModel;

                        if (workspace == null)
                        {
                                workspace = new OsmTileViewModel();

                                LonLatItemsViewModel llv =
                                        this._lonLatItems.FirstOrDefault(vm => vm is LonLatItemsViewModel)
                                        as LonLatItemsViewModel;
                                if (llv != null)
                                {
                                        // this is where events are hooked.
                                        // In this case, this connects the osmtile and the lat/lon viewmodels
                                        // (one for each direction)
                                        workspace.TileChanged += llv.OnReceieveOsmTile;
                                        llv.LonLatChanged += workspace.OnReceiveLonLat;
                                }

                                this.Workspaces.Add(workspace);
                        }

                        this.SetActiveWorkspace(workspace);
                }

                // just the lat/lon view at the bottom
                public ReadOnlyCollection<LonLatItemsViewModel> LonLatItems
                {
                        get
                        {
                                if (_lonLatItems == null)
                                {
                                        List<LonLatItemsViewModel> cmds = new List<LonLatItemsViewModel>
                                        {
                                                new LonLatItemsViewModel()
                                        };
                                        _lonLatItems = new ReadOnlyCollection<LonLatItemsViewModel>(cmds);
                                }
                                return _lonLatItems;
                        }
                }
        }
}
