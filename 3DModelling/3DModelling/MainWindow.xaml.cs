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
using System.ComponentModel;

namespace _3DModelling
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        private readonly Dictionary<string, PropertyChangedEventArgs> eventArgsCache;

        private SolutionManager solutionManager = new SolutionManager();


        private IList<int> visibleIndeces = new List<int>();
        private IList<int> fixedIndeces = new List<int>();
        private IList<Point3D> initNodesState = new List<Point3D>();

        private IList<Point3D> currentPointsState = new List<Point3D>();

        private IList<IList<Point3D>> nodesPointsStates = new List<IList<Point3D>>();
        private int currentStateIndex = 0;

        private IList<IList<Vector3D>> shifts = new List<IList<Vector3D>>();


        private int _hx;
        public int Hx
        {
            get
            {
                return this._hx;
            }
            set
            {
                this._hx = value;
                OnPropertyChanged("Hx");
                IsGridInitialized = false;
            }
        }

        private int _hy;
        public int Hy
        {
            get
            {
                return this._hy;
            }
            set
            {
                this._hy = value;
                OnPropertyChanged("Hy");
                IsGridInitialized = false;
            }
        }

        private int _hz;
        public int Hz
        {
            get
            {
                return this._hz;
            }
            set
            {
                this._hz = value;
                OnPropertyChanged("Hz");
                IsGridInitialized = false;
            }
        }

        private int _nx;
        public int Nx
        {
            get
            {
                return this._nx;
            }
            set
            {
                this._nx = value;
                OnPropertyChanged("Nx");
                IsGridInitialized = false;
            }
        }

        private int _ny;
        public int Ny
        {
            get
            {
                return this._ny;
            }
            set
            {
                this._ny = value;
                OnPropertyChanged("Ny");
                IsGridInitialized = false;
            }
        }

        private int _nz;
        public int Nz
        {
            get
            {
                return this._nz;
            }
            set
            {
                this._nz = value;
                OnPropertyChanged("Nz");
                IsGridInitialized = false;
            }
        }

        public double ElasticityModulus { get; set; }
        public double PoissonRatio { get; set; }
        public double Density { get; set; }

        //для слайдера
        private int _iterationsCount;
        public int IterationsCount
        {
            get
            {
                return this._iterationsCount;
            }
            set
            {
                this._iterationsCount = value;
                OnPropertyChanged("IterationsCount");
            }
        }
        // для текстового поля
        public int Iterations { get; set; }

        private bool _showNodes = true;
        public bool ShowNodes
        {
            get
            {
                return this._showNodes;
            }
            set
            {
                this._showNodes = value;
                ChangeNodesState();
            }
        }
        private bool _showRibs = true;
        public bool ShowRibs
        {
            get
            {
                return this._showRibs;
            }
            set
            {
                this._showRibs = value;
                ChangeRibsState();
            }
        }

        private bool _showHidden = false;
        public bool ShowHidden
        {
            get
            {
                return this._showHidden;
            }
            set
            {
                this._showHidden = value;
                DrawNewNet(currentPointsState);
            }
        }

        private bool _isGridInitialized = false;
        public bool IsGridInitialized
        {
            get
            {
                return this._isGridInitialized;
            }
            set
            {
                this._isGridInitialized = value;
                OnPropertyChanged("IsGridInitialized");
            }
        }

        private bool _isSolutionBuilt = false;
        public bool IsSolutionBuilt
        {
            get
            {
                return this._isSolutionBuilt;
            }
            set
            {
                this._isSolutionBuilt = value;
                OnPropertyChanged("IsSolutionBuilt");
            }
        }

        Visualizer visualizer;

        LinesVisual3D outerLines;
        LinesVisual3D innerLines;

        IList<SphereVisual3D> nodesModels;

        GridBuilder gridBuilder;



        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            eventArgsCache = new Dictionary<string, PropertyChangedEventArgs>();

            Hx = 2;
            Hy = 2;
            Hz = 2;

            Nx = 1;
            Ny = 5;
            Nz = 1;


            rebuildGrid();

            ElasticityModulus = 1000000;
            PoissonRatio = 0.25;
            Density = 1000;

            Iterations = 1;
            IterationsCount = 1;

            IsSolutionBuilt = false;
            IsGridInitialized = true;

        }

        private void DrawNewNet(IList<Point3D> currentPointsState)
        {
            points.Children.Clear();
            grid.Children.Clear();

            outerLines = new LinesVisual3D();
            innerLines = new LinesVisual3D();


            visualizer = new Visualizer(Nx + 1, Ny + 1, Nz + 1, currentPointsState, visibleIndeces, fixedIndeces, ShowHidden);

            nodesModels = visualizer.GetNodesModels();

            visualizer.GetXParallels();
            visualizer.GetYParallels();
            visualizer.GetZParallels();

            outerLines = visualizer.getOuterParallels();
            innerLines = visualizer.getInnerParallels();
            /*
           ZLines = visualizer.GetZParallels();           
           ZLines.Color = Colors.Blue;

           YLines = visualizer.GetYParallels();            
           YLines.Color = Colors.Green;

           XLines = visualizer.GetXParallels();            
           XLines.Color = Colors.Coral;
           */


            ChangeRibsState();

            ChangeNodesState();

            foreach (var nodeModel in nodesModels)
            {
                points.Children.Add(nodeModel);
            }

            grid.Children.Add(outerLines);
            grid.Children.Add(innerLines);
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
                        ((SphereVisual3D)(viewport.FindNearestVisual(currentPoint))).Material = new DiffuseMaterial(new SolidColorBrush(Colors.Magenta));
                    }
                    else
                    {
                        fixedIndeces.RemoveAt(fixedIndeces.IndexOf(fixedNode));
                        ((SphereVisual3D)(viewport.FindNearestVisual(currentPoint))).Material = new DiffuseMaterial(new SolidColorBrush(Colors.Blue));
                    }
                }
            }
        }

        private void Execute_Click(object sender, RoutedEventArgs e)
        {
            InputData input = new InputData(
                Hx, Hy, Hz,
                Nx, Ny, Nz,
                ElasticityModulus, PoissonRatio, Density,
                Iterations, fixedIndeces.ToArray()
           );

            SolutionManager solutionManager = new SolutionManager();
            shifts = solutionManager.buildSolution(input);
            IList<Point3D> firstState = nodesPointsStates[0];
            nodesPointsStates.Clear();
            nodesPointsStates.Add(firstState);

            for (int iterationId = 1; iterationId <= shifts.Count; iterationId++)
            {
                IList<Point3D> newNodesPoints = new List<Point3D>();
                for (int pointId = 0; pointId < initNodesState.Count; pointId++)
                {
                    if (!fixedIndeces.Contains(pointId))
                    {
                        newNodesPoints.Add(Vector3D.Add(shifts[iterationId - 1][pointId], nodesPointsStates[iterationId - 1][pointId]));
                    }
                    else
                    {
                        newNodesPoints.Add(nodesPointsStates[0][pointId]);
                    }
                }
                nodesPointsStates.Add(newNodesPoints);
            }

            currentPointsState = nodesPointsStates[0];
            StateSlider.Value = 0;

            DrawNewNet(currentPointsState);
            IterationsCount = Iterations;
            IsSolutionBuilt = true;
        }

        private void RebuildGrid_Click(object sender, RoutedEventArgs e)
        {
            rebuildGrid();
        }

        private void rebuildGrid()
        {
            gridBuilder = new GridBuilder(Hx, Hy, Hz, Nx + 1, Ny + 1, Nz + 1);

            initNodesState = gridBuilder.GetNodesPoints();
            /*Random rand = new Random();
            for (int iterationId = 0; iterationId < Iterations; iterationId++)
            {
                List<Vector3D> iterationShifts = new List<Vector3D>();
                for (int pointId = 0; pointId < initNodesState.Count; pointId++)
                {


                    iterationShifts.Add(new Vector3D(rand.Next(-10, 10), rand.Next(-10, 10), rand.Next(-10, 10)));
                }
                shifts.Add(iterationShifts);
            }*/

            visibleIndeces = gridBuilder.GetVisibleIndeces();
            StateSlider.Value = 0;
            currentPointsState = initNodesState;
            nodesPointsStates.Clear();
            nodesPointsStates.Add(initNodesState);

            fixedIndeces.Clear();

            DrawNewNet(currentPointsState);
            IsGridInitialized = true;
        }

        private void ChangeNodesState()
        {
            if (points != null)
            {
                foreach (var point in points.Children)
                {
                    ((SphereVisual3D)point).Visible = this.ShowNodes;
                }
            }
        }

        private void ChangeRibsState()
        {
            if (grid != null)
            {
                foreach (var ribs in grid.Children)
                {

                    ((LinesVisual3D)ribs).Thickness = Convert.ToInt32(this.ShowRibs);
                }
            }
        }

        private void StateSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!IsGridInitialized || nodesPointsStates.Count == 0)
            {
                return;
            }
            int stateIndex = Convert.ToInt32(((Slider)sender).Value);
            if (currentStateIndex != stateIndex)
            {
                currentStateIndex = stateIndex;
                DrawNewNet(nodesPointsStates[stateIndex]);
            }
                        
        }

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventArgs args;
            if (!eventArgsCache.TryGetValue(propertyName, out args))
            {
                args = new PropertyChangedEventArgs(propertyName);
                eventArgsCache.Add(propertyName, args);
            }

            OnPropertyChanged(args);
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, args);
        }

        #endregion
    }
}
