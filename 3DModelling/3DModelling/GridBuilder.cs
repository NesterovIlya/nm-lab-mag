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
    class GridBuilder
    {
        private int Hx;
        private int Hy;
        private int Hz;

        private int Nx;
        private int Ny;
        private int Nz;

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

        List<int> visibleIndeces = new List<int>();

        public GridBuilder(int Hx, int Hy, int Hz, int Nx, int Ny, int Nz)
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


        public List<Point3D> GetNodesPoints()
        {
            return nodesPoints;
        }

        public List<int> GetVisibleIndeces()
        {
            return visibleIndeces;
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
