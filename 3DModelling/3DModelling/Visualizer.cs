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

        List<int> visibleIndeces = new List<int>();
        List<int> fixedIndeces = new List<int>();
        private List<Point3D> nodesPoints = new List<Point3D>();

        public Visualizer(int Nx, int Ny, int Nz, List<Point3D> nodesPoints,List<int> visibleIndeces, List<int> fixedIndeces, bool showHidden)
        {

            
            this.Nx = Nx;
            this.Ny = Ny;
            this.Nz = Nz;
            this.nodesPoints = nodesPoints;
            this.visibleIndeces = visibleIndeces;
            this.fixedIndeces = fixedIndeces;
            this.ShowHidden = showHidden;
        }


        public List<SphereVisual3D> GetNodesModels()
        {
            List<SphereVisual3D> nodesModels = new List<SphereVisual3D>();

            for (int i = 0; i < nodesPoints.Count; i++)
            {

                SphereVisual3D nodeModel = new SphereVisual3D();
                nodeModel.Center = nodesPoints[i];
                nodeModel.Radius = 0.25;
                if (fixedIndeces.Contains(i))
                {
                  nodeModel.Material = new DiffuseMaterial(new SolidColorBrush(Colors.Red));
                }
                else
                {
                  nodeModel.Material = new DiffuseMaterial(new SolidColorBrush(Colors.LightBlue));
                }
                

                if (!ShowHidden)
                {
                    if (visibleIndeces.Contains(i))
                        nodesModels.Add(nodeModel);
                }
                else
                {
                    nodesModels.Add(nodeModel);
                }                
            }
            return nodesModels;
        }

        public List<Point3D> GetNodesPoints()
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
                    if (!ShowHidden)
                    {
                        if (visibleIndeces.Contains(j) && visibleIndeces.Contains(j + 1))
                        {
                            ZLines.Points.Add(nodesPoints[j]);
                            ZLines.Points.Add(nodesPoints[j + 1]);
                        }
                    }
                    else
                    {
                        ZLines.Points.Add(nodesPoints[j]);
                        ZLines.Points.Add(nodesPoints[j + 1]);
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
                        if (!ShowHidden)
                        {
                            if (visibleIndeces.Contains(k) && visibleIndeces.Contains(k + Nz))
                            {
                                YLines.Points.Add(nodesPoints[k]);
                                YLines.Points.Add(nodesPoints[k + Nz]);
                            }
                        }
                        else
                        {
                            YLines.Points.Add(nodesPoints[k]);
                            YLines.Points.Add(nodesPoints[k + Nz]);
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
                        if (!ShowHidden)
                        {
                            if (visibleIndeces.Contains(k) && visibleIndeces.Contains(k + Ny * Nz))
                            {
                                XLines.Points.Add(nodesPoints[k]);
                                XLines.Points.Add(nodesPoints[k + Ny * Nz]);
                            }
                        }
                        else
                        {
                            XLines.Points.Add(nodesPoints[k]);
                            XLines.Points.Add(nodesPoints[k + Ny * Nz]);
                        }
                    }
                }
            }
            return XLines;
        }
        
    }
}
