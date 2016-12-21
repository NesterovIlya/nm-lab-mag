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
        private int Hx;
        private int Hy;
        private int Hz;

        private int Nx;
        private int Ny;
        private int Nz;

        List<int> visibleIndeces = new List<int>();

        private List<Point3D> nodesPoints = new List<Point3D>();

        private void FindNodesPoints()
        {
            for (int i = 0; i < Nx; i++)
            {
                for (int j = 0; j < Ny; j++)
                {
                    for (int k = 0; k < Nz; k++)
                    {
                        nodesPoints.Add(new Point3D(Hx * i, Hy * j, Hz * k));
                    }
                }
            }
        }

        public Visualizer(int Hx, int Hy, int Hz, int Nx, int Ny, int Nz)
        {

            this.Hx = Hx;
            this.Hy = Hy;
            this.Hz = Hz;
            this.Nx = Nx;
            this.Ny = Ny;
            this.Nz = Nz;

            FindNodesPoints();
            FindVisibleIndices();
        }


        public List<SphereVisual3D> GetNodesModels(bool onlyVisible)
        {
            List<SphereVisual3D> nodesModels = new List<SphereVisual3D>();

            for (int i = 0; i < nodesPoints.Count; i++)
            {

                SphereVisual3D nodeModel = new SphereVisual3D();
                nodeModel.Center = nodesPoints[i];
                nodeModel.Radius = 0.25;
                nodeModel.Material = new DiffuseMaterial(new SolidColorBrush(Colors.LightBlue));

                if (onlyVisible)
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

        public LinesVisual3D GetZParallels(bool onlyVisible)
        {
            LinesVisual3D ZLines = new LinesVisual3D();
            for (int i = 0; i < Nx * Ny; i++)
            {
                for (int j = i * Nz; j < Nz * (i + 1) - 1; j++)
                {
                    if (onlyVisible)
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


        public LinesVisual3D GetYParallels(bool onlyVisible)
        {
            LinesVisual3D YLines = new LinesVisual3D();

            for (int i = 0; i < Nx * Ny * Nz; i += Ny * Nz)
            {
                for (int j = i; j < i + Nz; j++)
                {
                    for (int k = j; k < j + Ny * Nz - Nz; k += Nz)
                    {
                        if (onlyVisible)
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

        public LinesVisual3D GetXParallels(bool onlyVisible)
        {
            LinesVisual3D XLines = new LinesVisual3D();

            for (int i = 0; i < Nz; i++)
            {
                for (int j = i; j <= i + Ny * Nz - Nz; j += Nz)
                {
                    for (int k = j; k < j + Nx * Ny * Nz - Ny * Nz; k += Ny * Nz)
                    {
                        if (onlyVisible)
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



        private void FindBackSideVisibleIndices()
        {
            for (int i = 0; i < Ny * Nz; i++)
            {
                visibleIndeces.Add(i);
            }
        }


        private void FindFrontSideVisibleIndices()
        {
            for (int i = Nx * Ny * Nz - Ny * Nz; i < Nx * Ny * Nz; i++)
            {
                visibleIndeces.Add(i);
            }
        }

        private void FindRightSideVisibleIndices()
        {
            for (int i = 0; i < Nx * Ny * Nz - Ny * Nz; i += Ny * Nz)
            {
                for (int j = i; j < i + Nz; j++)
                {
                    visibleIndeces.Add(j);
                }
            }
        }

        private void FindLeftSideVisibleIndices()
        {
            for (int i = Ny * Nz - Nz; i < Nx * Ny * Nz - Nz; i += Ny * Nz)
            {
                for (int j = i; j < i + Nz; j++)
                {
                    visibleIndeces.Add(j);
                }
            }
        }

        private void FindUpperSideVisibleIndices()
        {
            for (int i = Nz - 1; i < Nx * Ny * Nz - Ny * Nz + Nz; i += Ny * Nz)
            {
                for (int j = i; j < Nx * Ny * Nz; j += Nz)
                {
                    visibleIndeces.Add(j);
                }

            }
        }

        private void FindLowerSideVisibleIndeces()
        {
            for (int i = 0; i < Nx * Ny * Nz - Ny * Nz; i += Ny * Nz)
            {
                for (int j = i; j < Nx * Ny * Nz - Nz; j += Nz)
                {
                    visibleIndeces.Add(j);
                }

            }
        }


        private void FindVisibleIndices()
        {
            FindBackSideVisibleIndices();
            FindFrontSideVisibleIndices();
            FindLeftSideVisibleIndices();
            FindRightSideVisibleIndices();
            FindUpperSideVisibleIndices();
            FindLowerSideVisibleIndeces();
        }


       

    }
}
