using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TileToLonLat.ViewModel;

namespace TileToLonLat
{
        /// <summary>
        /// Interaction logic for App.xaml
        /// </summary>
        public partial class App : Application
        {
                static App()
                {

                }

                protected override void OnStartup(StartupEventArgs e)
                {

                        // Select the text in a TextBox when it receives focus.
                        EventManager.RegisterClassHandler(typeof(TextBox), TextBox.PreviewMouseLeftButtonDownEvent,
                            new MouseButtonEventHandler(SelectivelyIgnoreMouseButton));
                        EventManager.RegisterClassHandler(typeof(TextBox), TextBox.GotKeyboardFocusEvent,
                            new RoutedEventHandler(SelectAllText));
                        EventManager.RegisterClassHandler(typeof(TextBox), TextBox.MouseDoubleClickEvent,
                            new RoutedEventHandler(SelectAllText));


                        base.OnStartup(e);

                        MainWindow win = new MainWindow();

                        var viewModel = new MainWindowViewModel();

                        viewModel.RequestClose += (x, y) =>
                        {
                                viewModel.RequestClose -= x as System.EventHandler;
                                win.Close();
                        };

                        win.DataContext = viewModel;

                        win.Show();
                }

                /// <summary>
                /// Function 1 of 2 of the code that will select all text in a textbox when it gains focus
                /// </summary>
                /// <param name="sender"></param>
                /// <param name="e"></param>
                void SelectivelyIgnoreMouseButton(object sender, MouseButtonEventArgs e)
                {
                        // Find the TextBox
                        DependencyObject parent = e.OriginalSource as UIElement;
                        while (parent != null && !(parent is TextBox))
                                parent = VisualTreeHelper.GetParent(parent);

                        if (parent != null)
                        {
                                var textBox = (TextBox)parent;
                                if (!textBox.IsKeyboardFocusWithin)
                                {
                                        // If the text box is not yet focused, give it the focus and
                                        // stop further processing of this click event.
                                        textBox.Focus();
                                        e.Handled = true;
                                }
                        }
                }

                /// <summary>
                /// Function 2 of 2 of the code that will select all text in a textbox when it gains focus
                /// </summary>
                /// <param name="sender"></param>
                /// <param name="e"></param>
                void SelectAllText(object sender, RoutedEventArgs e)
                {
                        var textBox = e.OriginalSource as TextBox;
                        if (textBox != null)
                                textBox.SelectAll();
                }
        }
}
