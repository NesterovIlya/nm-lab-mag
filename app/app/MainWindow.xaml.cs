using app.Utils;
using app.ViewModel;
using HelixToolkit.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace app
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly BaseViewModel _mainViewModel = new MainViewModel();

        List<int> visibleIndeces = new List<int>();
        int Hx = 2;
        int Hy = 2;
        int Hz = 2;

        int Nx = 5;
        int Ny = 5;
        int Nz = 5;

        Visualizer visualizer;

        LinesVisual3D ZLines;
        LinesVisual3D YLines;
        LinesVisual3D XLines;

        List<SphereVisual3D> nodesModels;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = _mainViewModel;
            this.DrawNewNet();
        }

        private void DrawNewNet()
        {
            grid.Children.Clear();
            points.Children.Clear();
            bool onlyVisible = false;
            ZLines = new LinesVisual3D();
            YLines = new LinesVisual3D();
            YLines = new LinesVisual3D();

            visualizer = new Visualizer(Hx, Hy, Hz, Nx, Ny, Nz);


            nodesModels = visualizer.GetNodesModels(onlyVisible);

            ZLines = visualizer.GetZParallels(onlyVisible);
            ZLines.Thickness = 1;
            ZLines.Color = Colors.Blue;

            YLines = visualizer.GetYParallels(onlyVisible);
            YLines.Thickness = 1;
            YLines.Color = Colors.Green;

            XLines = visualizer.GetXParallels(onlyVisible);
            XLines.Thickness = 1;
            XLines.Color = Colors.Coral;

            foreach (var nodeModel in nodesModels)
            {
                points.Children.Add(nodeModel);
            }

            grid.Children.Add(ZLines);
            grid.Children.Add(YLines);
            grid.Children.Add(XLines);
        }


        private void viewport_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point currentPoint = e.GetPosition(viewport);
            if (viewport.FindNearestVisual(currentPoint) != null)
            {
                if (viewport.FindNearestVisual(currentPoint).GetType() == typeof(SphereVisual3D))
                {
                    ((SphereVisual3D)(viewport.FindNearestVisual(currentPoint))).Material = new DiffuseMaterial(new SolidColorBrush(Colors.Red));
                }
            }

        }
    }
}
