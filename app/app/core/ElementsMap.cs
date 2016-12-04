using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace app.core
{
    public class ElementsMap
    {

        private static int[,] ELEMENT_DECOMPOSITION = { { 2, 1, 3, 6 }, { 5, 7, 3, 6 }, { 5, 3, 1, 6 }, { 0, 1, 2, 4 }, { 6, 2, 1, 4 }, { 6, 5, 5, 4 } };


        public InputData input { get; private set; }
        public IList<Element> elements { get; private set; }
      

        public ElementsMap(InputData input)
        {
            this.elements = new List<Element>();
            this.input = input;
            int blockNum;

            int sh = 0;
            for (int ix = 0; ix < input.Nx; ix++)
            {
                for (int iy = 0; iy < input.Ny; iy++)
                {
                    for (int iz = 0; iz < input.Nz; iz++)
                    {
                        Node NodeOne = new Node();
                        NodeOne.point = new Point3D(ix * input.hx, iy * input.hy, iz * input.hz);
                        NodeOne.id = sh;

                        //вычисление всех узлов
                        Point3D[] MassPoint = new Point3D[8];
                        MassPoint[0] = new Point3D(ix * input.hx, iy * input.hy, (iz + 1) * input.hz);
                        MassPoint[1] = new Point3D(ix * input.hx, (iy + 1) * input.hy, (iz + 1) * input.hz);
                        MassPoint[2] = new Point3D((ix + 1) * input.hx, iy * input.hy, (iz + 1) * input.hz);
                        MassPoint[3] = new Point3D((ix + 1) * input.hx, (iy + 1) * input.hy, (iz + 1) * input.hz);

                        MassPoint[4] = new Point3D(ix * input.hx, iy * input.hy, iz * input.hz);
                        MassPoint[5] = new Point3D(ix * input.hx, (iy + 1) * input.hy, iz * input.hz);
                        MassPoint[6] = new Point3D((ix + 1) * input.hx, iy * input.hy, iz * input.hz);
                        MassPoint[7] = new Point3D((ix + 1) * input.hx, (iy + 1) * input.hy, iz * input.hz);

                        //номер кирпича
                        blockNum = iy * input.Nz + ix * input.Nz * input.Ny + iz;

                        int tetrNum;

                        for (int iMap = 0; iMap < 6; iMap++)
                        {

                            Element elem1 = new Element();

                            //узел I тетраэдра
                            int tempElem = ELEMENT_DECOMPOSITION[iMap, 0];
                            Point3D pointTemp = new Point3D(MassPoint[tempElem].X, MassPoint[tempElem].Y, MassPoint[tempElem].Z);
                            elem1.nodeI = new Node();
                            elem1.nodeI.point = pointTemp;

                            //узел J тетраэдра
                            tempElem = ELEMENT_DECOMPOSITION[iMap, 1];
                            pointTemp = new Point3D(MassPoint[tempElem].X, MassPoint[tempElem].Y, MassPoint[tempElem].Z);
                            elem1.nodeJ = new Node();
                            elem1.nodeJ.point = pointTemp;

                            //узел K тетраэдра
                            tempElem = ELEMENT_DECOMPOSITION[iMap, 2];
                            pointTemp = new Point3D(MassPoint[tempElem].X, MassPoint[tempElem].Y, MassPoint[tempElem].Z);
                            elem1.nodeK = new Node();
                            elem1.nodeK.point = pointTemp;

                            //узел P тетраэдра
                            tempElem = ELEMENT_DECOMPOSITION[iMap, 3];
                            pointTemp = new Point3D(MassPoint[tempElem].X, MassPoint[tempElem].Y, MassPoint[tempElem].Z);
                            elem1.nodeP = new Node();
                            elem1.nodeP.point = pointTemp;

                            //коэффициенты функции формы
                            elem1.nodeI.coefB = - СalculateFormFunctionCoef(elem1.nodeJ.point.Y, elem1.nodeK.point.Y, elem1.nodeP.point.Y, elem1.nodeJ.point.Z, elem1.nodeK.point.Z, elem1.nodeP.point.Z);
                            elem1.nodeJ.coefB = - СalculateFormFunctionCoef(elem1.nodeK.point.Y, elem1.nodeP.point.Y, elem1.nodeI.point.Y, elem1.nodeK.point.Z, elem1.nodeP.point.Z, elem1.nodeI.point.Z);
                            elem1.nodeK.coefB = - СalculateFormFunctionCoef(elem1.nodeP.point.Y, elem1.nodeI.point.Y, elem1.nodeJ.point.Y, elem1.nodeP.point.Z, elem1.nodeI.point.Z, elem1.nodeJ.point.Z);
                            elem1.nodeP.coefB = - СalculateFormFunctionCoef(elem1.nodeI.point.Y, elem1.nodeJ.point.Y, elem1.nodeK.point.Y, elem1.nodeI.point.Z, elem1.nodeJ.point.Z, elem1.nodeK.point.Z);

                            elem1.nodeI.coefC = СalculateFormFunctionCoef(elem1.nodeJ.point.X, elem1.nodeK.point.X, elem1.nodeP.point.X, elem1.nodeJ.point.Z, elem1.nodeK.point.Z, elem1.nodeP.point.Z);
                            elem1.nodeJ.coefC = СalculateFormFunctionCoef(elem1.nodeK.point.X, elem1.nodeP.point.X, elem1.nodeI.point.X, elem1.nodeK.point.Z, elem1.nodeP.point.Z, elem1.nodeI.point.Z);
                            elem1.nodeK.coefC = СalculateFormFunctionCoef(elem1.nodeP.point.X, elem1.nodeI.point.X, elem1.nodeJ.point.X, elem1.nodeP.point.Z, elem1.nodeI.point.Z, elem1.nodeJ.point.Z);
                            elem1.nodeP.coefC = СalculateFormFunctionCoef(elem1.nodeI.point.X, elem1.nodeJ.point.X, elem1.nodeK.point.X, elem1.nodeI.point.Z, elem1.nodeJ.point.Z, elem1.nodeK.point.Z);

                            elem1.nodeI.coefD = - СalculateFormFunctionCoef(elem1.nodeJ.point.X, elem1.nodeK.point.X, elem1.nodeP.point.X, elem1.nodeJ.point.Y, elem1.nodeK.point.Y, elem1.nodeP.point.Y);
                            elem1.nodeJ.coefD = - СalculateFormFunctionCoef(elem1.nodeK.point.X, elem1.nodeP.point.X, elem1.nodeI.point.X, elem1.nodeK.point.Y, elem1.nodeP.point.Y, elem1.nodeI.point.Y);
                            elem1.nodeK.coefD = - СalculateFormFunctionCoef(elem1.nodeP.point.Y, elem1.nodeI.point.Y, elem1.nodeJ.point.Y, elem1.nodeP.point.Y, elem1.nodeI.point.Y, elem1.nodeJ.point.Y);
                            elem1.nodeP.coefD = - СalculateFormFunctionCoef(elem1.nodeI.point.Y, elem1.nodeJ.point.Y, elem1.nodeK.point.Y, elem1.nodeI.point.Y, elem1.nodeJ.point.Y, elem1.nodeK.point.Y);

                            //номер тетраэдра
                            tetrNum = 6 * blockNum + iMap;
                            elem1.id = tetrNum;

                            elements.Add(elem1);

                        }

                        sh++;
                    }
                }

            }
        }

        private double СalculateFormFunctionCoef(double y1, double y2, double y3, double z1, double z2, double z3)
        {
            return ((y2 - y1) * (z3 - z1) - (z2 - z1) * (y3 - y1));
        }
    }
}
