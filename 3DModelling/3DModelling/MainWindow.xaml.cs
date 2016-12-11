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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;

namespace _3DModelling
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<int> visibleIndeces = new List<int>();
        int Hx = 3;
        int Hy = 4;
        int Hz = 5;

        int Nx = 4;
        int Ny = 8;
        int Nz = 5;

        Visualizer visualizer;

        LinesVisual3D ZLines;
        LinesVisual3D YLines;
        LinesVisual3D XLines;

        List<SphereVisual3D> nodesModels;

        public MainWindow()
        {
            InitializeComponent();
            DrawNewNet();
            
        }


        private void DrawNewNet()
        {
            cube.Children.Clear();
            bool onlyVisible = true;
            ZLines = new LinesVisual3D();
            YLines = new LinesVisual3D();
            YLines = new LinesVisual3D();

            visualizer = new Visualizer(Hx, Hy, Hz, Nx, Ny, Nz);


            if (ShowInvisible.IsChecked==true)
            {
                onlyVisible = false;
            }

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
                cube.Children.Add(nodeModel);
            }

            cube.Children.Add(ZLines);
            cube.Children.Add(YLines);
            cube.Children.Add(XLines);
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

        private void ShowNodes_Checked(object sender, RoutedEventArgs e)
        {
            
        }




    }
}
