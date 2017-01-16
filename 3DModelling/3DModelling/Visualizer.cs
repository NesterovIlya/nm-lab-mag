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
    class Visualizer
    {
        private int Nx;
        private int Ny;
        private int Nz;
        private bool ShowHidden;

        IList<int> visibleIndeces = new List<int>();
        IList<int> fixedIndeces = new List<int>();
        private IList<Point3D> nodesPoints = new List<Point3D>();

        LinesVisual3D outerXParallels = new LinesVisual3D();
        LinesVisual3D outerYParallels = new LinesVisual3D();
        LinesVisual3D outerZParallels = new LinesVisual3D();
        LinesVisual3D outerParallels = new LinesVisual3D();
        LinesVisual3D innerXParallels = new LinesVisual3D();
        LinesVisual3D innerYParallels = new LinesVisual3D();
        LinesVisual3D innerZParallels = new LinesVisual3D();
        LinesVisual3D innerParallels = new LinesVisual3D();

        public Visualizer(int Nx, int Ny, int Nz, IList<Point3D> nodesPoints,IList<int> visibleIndeces, IList<int> fixedIndeces, bool showHidden)
        {

            
            this.Nx = Nx;
            this.Ny = Ny;
            this.Nz = Nz;
            this.nodesPoints = nodesPoints;
            this.visibleIndeces = visibleIndeces;
            this.fixedIndeces = fixedIndeces;
            this.ShowHidden = showHidden;
        }


        public IList<SphereVisual3D> GetNodesModels()
        {
            IList<SphereVisual3D> nodesModels = new List<SphereVisual3D>();

            for (int i = 0; i < nodesPoints.Count; i++)
            {

                SphereVisual3D nodeModel = new SphereVisual3D();
                nodeModel.Center = nodesPoints[i];
                nodeModel.Radius = 0.25;
                if (fixedIndeces.Contains(i))
                {
                  nodeModel.Material = new DiffuseMaterial(new SolidColorBrush(Colors.Magenta));
                }
                else
                {
                  nodeModel.Material = new DiffuseMaterial(new SolidColorBrush(Colors.Blue));
                }
                

                if (!ShowHidden)
                {

                    if (!visibleIndeces.Contains(i))
                    {
                        nodeModel.Visible = false;
                    }
                    
                }
                else
                {
                    if (!visibleIndeces.Contains(i))
                    {
                        nodeModel.Material = new DiffuseMaterial(new SolidColorBrush(Colors.SkyBlue));
                    }
                    
                }

                nodesModels.Add(nodeModel);
                             
            }
            return nodesModels;
        }

        public IList<Point3D> GetNodesPoints()
        {           
            return nodesPoints;
        }

        public LinesVisual3D GetZParallels()
        {
            LinesVisual3D ZLines = new LinesVisual3D();
            for (int i = 0; i < Nx * Ny; i++)
            {
                for (int j = i * Nz; j < Nz * (i + 1) - 1; j++)
                {
                    if (visibleIndeces.Contains(j) && visibleIndeces.Contains(j + 1))
                    {
                        outerZParallels.Points.Add(nodesPoints[j]);
                        outerZParallels.Points.Add(nodesPoints[j + 1]);
                    }
                    else
                    {
                        innerZParallels.Points.Add(nodesPoints[j]);
                        innerZParallels.Points.Add(nodesPoints[j + 1]);
                    }
                   
                }
            }
            return ZLines;
        }


        public LinesVisual3D GetYParallels()
        {
            LinesVisual3D YLines = new LinesVisual3D();

            for (int i = 0; i < Nx * Ny * Nz; i += Ny * Nz)
            {
                for (int j = i; j < i + Nz; j++)
                {
                    for (int k = j; k < j + Ny * Nz - Nz; k += Nz)
                    {
                        if (visibleIndeces.Contains(k) && visibleIndeces.Contains(k + Nz))
                        {
                            outerYParallels.Points.Add(nodesPoints[k]);
                            outerYParallels.Points.Add(nodesPoints[k + Nz]);
                        }


                        else
                        {
                            innerYParallels.Points.Add(nodesPoints[k]);
                            innerYParallels.Points.Add(nodesPoints[k + Nz]);
                        }
                    }
                }
            }

            return YLines;
        }

        public LinesVisual3D GetXParallels()
        {
            LinesVisual3D XLines = new LinesVisual3D();

            for (int i = 0; i < Nz; i++)
            {
                for (int j = i; j <= i + Ny * Nz - Nz; j += Nz)
                {
                    for (int k = j; k < j + Nx * Ny * Nz - Ny * Nz; k += Ny * Nz)
                    {

                        if (visibleIndeces.Contains(k) && visibleIndeces.Contains(k + Ny * Nz))
                        {
                            outerXParallels.Points.Add(nodesPoints[k]);
                            outerXParallels.Points.Add(nodesPoints[k + Ny * Nz]);
                        }

                        else
                        {
                            innerXParallels.Points.Add(nodesPoints[k]);
                            innerXParallels.Points.Add(nodesPoints[k + Ny * Nz]);
                        }
                    }
                }
            }
            return XLines;
        }

        public LinesVisual3D getOuterParallels()
        {
            foreach (var point in outerXParallels.Points)
            {
                outerParallels.Points.Add(point);
            }
            foreach (var point in outerYParallels.Points)
            {
                outerParallels.Points.Add(point);
            }
            foreach (var point in outerZParallels.Points)
            {
                outerParallels.Points.Add(point);
            }

            outerParallels.Thickness = 1;
            outerParallels.Color = Colors.Blue;

            return outerParallels;
        }

        public LinesVisual3D getInnerParallels()
        {
            foreach (var point in innerXParallels.Points)
            {
                innerParallels.Points.Add(point);
            }
            foreach (var point in innerYParallels.Points)
            {
                innerParallels.Points.Add(point);
            }
            foreach (var point in innerZParallels.Points)
            {
                innerParallels.Points.Add(point);
            }

            if (!ShowHidden)
            {
                innerParallels.Thickness = 0;
            }
            else
            {
                innerParallels.Thickness = 0.7;
                innerParallels.Color = Colors.SkyBlue;
            }

            return innerParallels;
        }
    }
}
