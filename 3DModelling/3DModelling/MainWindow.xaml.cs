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
using app.core;


namespace _3DModelling
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SolutionManager solutionManager = new SolutionManager();

        

        List<int> visibleIndeces = new List<int>();
        List<int> fixedIndeces  = new List<int>();
        List<Point3D> initNodesState = new List<Point3D>();

        List<Point3D> currentPointsState = new List<Point3D>();

        List<List<Point3D>> nodesPointsStates = new List<List<Point3D>>();

        List<List<Vector3D>> shifts = new List<List<Vector3D>>();

        int Hx = 3;
        int Hy = 4;
        int Hz = 5;

        int Nx = 5;
        int Ny = 4;
        int Nz = 3;

        int iterations = 5;


        bool showNodes = true;
        bool showRibs = true;
        bool showHidden = false;

        Visualizer visualizer;

        LinesVisual3D ZLines;
        LinesVisual3D YLines;
        LinesVisual3D XLines;

        List<SphereVisual3D> nodesModels;

        GridBuilder gridBuilder;

        public MainWindow()
        {
            InitializeComponent();
            gridBuilder = new GridBuilder(Hx, Hy, Hz, Nx, Ny, Nz);

            
            initNodesState = gridBuilder.GetNodesPoints();
            Random rand = new Random();
            for (int iterationId = 0; iterationId < iterations; iterationId++)
            {
                List<Vector3D> iterationShifts = new List<Vector3D>();
                for (int pointId = 0; pointId < initNodesState.Count; pointId++)
                {
                   

                    iterationShifts.Add(new Vector3D(rand.Next(-10, 10), rand.Next(-10, 10), rand.Next(-10, 10)));
                }
                shifts.Add(iterationShifts);
            }

            visibleIndeces = gridBuilder.GetVisibleIndeces();
            StateSlider.Minimum = 0;
            StateSlider.Maximum = iterations - 1;
            StateSlider.Value = 0;
            StateSlider.TickFrequency = 1;
            StateSlider.TickPlacement = System.Windows.Controls.Primitives.TickPlacement.BottomRight;
            currentPointsState = initNodesState;
            nodesPointsStates.Add(initNodesState);
            DrawNewNet(currentPointsState);
        }
                     


        private void DrawNewNet(List<Point3D> currentPointsState)
        {
            points.Children.Clear();
            grid.Children.Clear();

            ZLines = new LinesVisual3D();
            YLines = new LinesVisual3D();
            XLines = new LinesVisual3D();
         
            visualizer = new Visualizer(Nx, Ny, Nz, currentPointsState, visibleIndeces, fixedIndeces, showHidden);

            nodesModels = visualizer.GetNodesModels();
            
            ZLines = visualizer.GetZParallels();           
            ZLines.Color = Colors.Blue;

            YLines = visualizer.GetYParallels();            
            YLines.Color = Colors.Green;

            XLines = visualizer.GetXParallels();            
            XLines.Color = Colors.Coral;

            ChangeRibsState(showRibs);

            ChangeNodesState(showNodes);

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
                    SphereVisual3D nearestSphere = (SphereVisual3D)(viewport.FindNearestVisual(currentPoint));
                    int fixedNode = nodesModels.IndexOf(nearestSphere);
                    if (!fixedIndeces.Contains(fixedNode))
                    {
                        fixedIndeces.Add(fixedNode);
                        ((SphereVisual3D)(viewport.FindNearestVisual(currentPoint))).Material = new DiffuseMaterial(new SolidColorBrush(Colors.Red));
                    }
                    else
                    {
                        fixedIndeces.RemoveAt(fixedIndeces.IndexOf(fixedNode));
                        ((SphereVisual3D)(viewport.FindNearestVisual(currentPoint))).Material = new DiffuseMaterial(new SolidColorBrush(Colors.LightBlue));
                    }
                }
            }
        }

        private void ShowNodes_Checked(object sender, RoutedEventArgs e)
        {
            showNodes = true;
            ChangeNodesState(showNodes);
        }

        private void ShowNodes_Unchecked(object sender, RoutedEventArgs e)
        {
            showNodes = false;
            ChangeNodesState(showNodes);
        }
        

        private void ShowRibs_Checked(object sender, RoutedEventArgs e)
        {
            showRibs = true;
            ChangeRibsState(showRibs);
        }

        private void ShowRibs_Unchecked(object sender, RoutedEventArgs e)
        {
            showRibs = false;
            ChangeRibsState(showRibs);
        }

        private void ShowInvisible_Checked(object sender, RoutedEventArgs e)
        {
            showHidden = true;
            DrawNewNet(currentPointsState);
        }

        private void ShowInvisible_Unchecked(object sender, RoutedEventArgs e)
        {
            showHidden = false;
            DrawNewNet(currentPointsState);
        }

        private void Execute_Click(object sender, RoutedEventArgs e)
        {
            InputData input = new InputData(Hx,Hy,Hz, Nx-1, Ny-1, Nz-1, 1,1,1,iterations, fixedIndeces.ToArray());

            SolutionManager solutionManager = new SolutionManager();
            //shifts = solutionManager.buildSolution(input);


            
            for (int iterationId = 1; iterationId<shifts.Count+1; iterationId++)
            {
                List<Point3D> newNodesPoints = new List<Point3D>();
                for (int pointId=0; pointId < initNodesState.Count; pointId++)
                {
                    if (!fixedIndeces.Contains(pointId))
                    {
                        newNodesPoints.Add(Vector3D.Add(shifts[iterationId-1][pointId], nodesPointsStates[iterationId - 1][pointId]));
                    }
                    else
                    {
                        newNodesPoints.Add(nodesPointsStates[0][pointId]);
                    }                     
                }
                nodesPointsStates.Add(newNodesPoints);
                //currentPointsState = nodesPointsStates[iterationId];
                //StateSlider.Value = shiftId;

                //DrawNewNet(currentPointsState);
            }
        }

        private void ChangeNodesState(bool visible)
        {
            if (points != null)
            {
                foreach (var point in points.Children)
                {
                    ((SphereVisual3D)point).Visible = visible;
                }
            }
        }

        private void ChangeRibsState(bool visible)
        {            
            if (grid != null)
            {
                foreach (var ribs in grid.Children)
                {

                    ((LinesVisual3D)ribs).Thickness = Convert.ToInt32(visible);
                }
            }
        }

        private void StateSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int stateIndex;
            stateIndex = Convert.ToInt32(((Slider)sender).Value);
            DrawNewNet(nodesPointsStates[stateIndex]);            
        }
    }
}
