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

        // Порядок узлов при разбиении блока на тетраэдры
        // i:0, l:1, j:2, k:3, p:4, s:5, q:6, r:7
        private static int[,] ELEMENT_DECOMPOSITION = { 
            { 2, 1, 3, 6 }, 
            { 5, 7, 3, 6 }, 
            { 5, 3, 1, 6 }, 
            { 0, 1, 2, 4 },
            { 6, 2, 1, 4 }, 
            { 6, 1, 5, 4 }
        };

        public InputData input { get; private set; }
        public IList<Element> elements { get; private set; }

        // Коэффициент для расчета правой части. Для каждого узла расчитывается \rho * V * g / 4;
        public double[] nodeProportions { get; private set; }

        public ElementsMap(InputData input)
        {
            this.elements = new List<Element>();
            this.input = input;
            int blockNum;
            int startNodeNum;

            this.nodeProportions = new double[(input.Nx + 1) * (input.Ny + 1) * (input.Nz + 1)];

            for (int ix = 0; ix < input.Nx; ix++)
            {
                for (int iy = 0; iy < input.Ny; iy++)
                {
                    for (int iz = 0; iz < input.Nz; iz++)
                    {

                        //номер кирпича
                        blockNum = ix * input.Nz * input.Ny + iy * input.Nz + iz;

                        // номер ближнего левого нижнего узла кирпича (узел p)
                        startNodeNum = ix * (input.Nz + 1) * (input.Ny + 1) + iy * (input.Nz + 1) + iz;
                        /* вычисление номеров узлов кирпича
                         * 
                         * узел: i(0) l(1) j(2) k(3) p(4) s(5) q(6) r(7)
                         *   dx:  0    0    1    1    0    0    1    1
                         *   dy:  0    1    0    1    0    1    0    1
                         *   dz:  1    1    1    1    0    0    0    0
                         *   
                         *   nodeNum(dx,dy,dz) = 
                         *      = (ix + dx) * nz' * ny' + (iy + dy) * nz' + (iz + dz) = 
                         *      = startNodeNum + dx * nz' * ny' + dy * nz' + dz;
                         */
                        int[] pointNumbers = new int[8];
                        pointNumbers[0] = startNodeNum + 1;
                        pointNumbers[1] = startNodeNum + (input.Nz + 1) + 1;
                        pointNumbers[2] = startNodeNum + (input.Nz + 1) * (input.Ny + 1) + 1;
                        pointNumbers[3] = startNodeNum + (input.Nz + 1) * (input.Ny + 1) + (input.Nz + 1) + 1;
                        pointNumbers[4] = startNodeNum;
                        pointNumbers[5] = startNodeNum + (input.Nz + 1);
                        pointNumbers[6] = startNodeNum + (input.Nz + 1) * (input.Ny + 1);
                        pointNumbers[7] = startNodeNum + (input.Nz + 1) * (input.Ny + 1) + (input.Nz + 1);

                        //вычисление координат всех узлов
                        Point3D[] points = new Point3D[8];
                        points[0] = new Point3D(ix * input.hx, iy * input.hy, (iz + 1) * input.hz);            //i
                        points[1] = new Point3D(ix * input.hx, (iy + 1) * input.hy, (iz + 1) * input.hz);      //l
                        points[2] = new Point3D((ix + 1) * input.hx, iy * input.hy, (iz + 1) * input.hz);      //j
                        points[3] = new Point3D((ix + 1) * input.hx, (iy + 1) * input.hy, (iz + 1) * input.hz);//k

                        points[4] = new Point3D(ix * input.hx, iy * input.hy, iz * input.hz);                  //p
                        points[5] = new Point3D(ix * input.hx, (iy + 1) * input.hy, iz * input.hz);            //s
                        points[6] = new Point3D((ix + 1) * input.hx, iy * input.hy, iz * input.hz);            //q
                        points[7] = new Point3D((ix + 1) * input.hx, (iy + 1) * input.hy, iz * input.hz);      //r

                        for (int elementTypeIndex = 0; elementTypeIndex < 6; elementTypeIndex++)
                        {

                            Element elem = new Element(6 * blockNum + elementTypeIndex);

                            //узел I тетраэдра
                            elem.nodeI = createNode(
                                ELEMENT_DECOMPOSITION[elementTypeIndex, 0],
                                points,
                                pointNumbers
                            );

                            //узел J тетраэдра
                            elem.nodeJ = createNode(
                                ELEMENT_DECOMPOSITION[elementTypeIndex, 1],
                                points,
                                pointNumbers
                            );

                            //узел K тетраэдра
                            elem.nodeK = createNode(
                                ELEMENT_DECOMPOSITION[elementTypeIndex, 2],
                                points,
                                pointNumbers
                            );

                            //узел P тетраэдра
                            elem.nodeP = createNode(
                                ELEMENT_DECOMPOSITION[elementTypeIndex, 3],
                                points,
                                pointNumbers
                            );

                            Vector3D ipVec = elem.nodeP.point - elem.nodeI.point;
                            Vector3D ijVec = elem.nodeJ.point - elem.nodeI.point;
                            Vector3D ikVec = elem.nodeK.point - elem.nodeI.point;
                            elem.volume = Vector3D.DotProduct(ipVec, Vector3D.CrossProduct(ijVec, ikVec)) / 6;

                            //заполняем коэффициенты
                            fillNodeProportionsCoef(elem.nodeI.id, elem.volume, input.density);
                            fillNodeProportionsCoef(elem.nodeJ.id, elem.volume, input.density);
                            fillNodeProportionsCoef(elem.nodeK.id, elem.volume, input.density);
                            fillNodeProportionsCoef(elem.nodeP.id, elem.volume, input.density);


                            /* 
                            * Расчет коэффициентов функции формы: (запись x_ij означает x_i - x_j)
                            * b = (b_i, b_j, b_k, b_p)                 c = (c_i, c_j, c_k, c_p)            d = (d_i, d_j, d_k, d_p)            
                            * b_i = - (y_kj * z_pj - y_pj * z_kj);     c_i = x_kj * z_pj - x_pj * z_kj;    d_i = - (x_kj * y_pj - x_pj * y_kj);                   
                            * b_j = - (y_pk * z_ik - y_ik * z_pk);     c_j = x_pk * z_ik - x_ik * z_pk;    d_j = - (x_pk * y_ik - x_ik * y_pk);
                            * b_k = - (y_ip * z_jp - y_jp * z_ip);     c_k = x_ip * z_jp - x_jp * z_ip;    d_k = - (x_ip * y_jp - x_jp * y_ip);
                            * b_p = - (y_ji * z_ki - y_ki * z_ji);     c_p = x_ji * z_ki - x_ki * z_ji;    d_p = - (x_ji * y_ki - x_ki * y_ji);                 
                            */
                            elem.nodeI.coefB = - СalculateFormFunctionCoef(elem.nodeJ.point.Y, elem.nodeK.point.Y, elem.nodeP.point.Y, elem.nodeJ.point.Z, elem.nodeK.point.Z, elem.nodeP.point.Z);
                            elem.nodeJ.coefB = СalculateFormFunctionCoef(elem.nodeK.point.Y, elem.nodeP.point.Y, elem.nodeI.point.Y, elem.nodeK.point.Z, elem.nodeP.point.Z, elem.nodeI.point.Z);
                            elem.nodeK.coefB = - СalculateFormFunctionCoef(elem.nodeP.point.Y, elem.nodeI.point.Y, elem.nodeJ.point.Y, elem.nodeP.point.Z, elem.nodeI.point.Z, elem.nodeJ.point.Z);
                            elem.nodeP.coefB = СalculateFormFunctionCoef(elem.nodeI.point.Y, elem.nodeJ.point.Y, elem.nodeK.point.Y, elem.nodeI.point.Z, elem.nodeJ.point.Z, elem.nodeK.point.Z);

                            elem.nodeI.coefC = СalculateFormFunctionCoef(elem.nodeJ.point.X, elem.nodeK.point.X, elem.nodeP.point.X, elem.nodeJ.point.Z, elem.nodeK.point.Z, elem.nodeP.point.Z);
                            elem.nodeJ.coefC = - СalculateFormFunctionCoef(elem.nodeK.point.X, elem.nodeP.point.X, elem.nodeI.point.X, elem.nodeK.point.Z, elem.nodeP.point.Z, elem.nodeI.point.Z);
                            elem.nodeK.coefC = СalculateFormFunctionCoef(elem.nodeP.point.X, elem.nodeI.point.X, elem.nodeJ.point.X, elem.nodeP.point.Z, elem.nodeI.point.Z, elem.nodeJ.point.Z);
                            elem.nodeP.coefC = - СalculateFormFunctionCoef(elem.nodeI.point.X, elem.nodeJ.point.X, elem.nodeK.point.X, elem.nodeI.point.Z, elem.nodeJ.point.Z, elem.nodeK.point.Z);

                            elem.nodeI.coefD = - СalculateFormFunctionCoef(elem.nodeJ.point.X, elem.nodeK.point.X, elem.nodeP.point.X, elem.nodeJ.point.Y, elem.nodeK.point.Y, elem.nodeP.point.Y);
                            elem.nodeJ.coefD = СalculateFormFunctionCoef(elem.nodeK.point.X, elem.nodeP.point.X, elem.nodeI.point.X, elem.nodeK.point.Y, elem.nodeP.point.Y, elem.nodeI.point.Y);
                            elem.nodeK.coefD = - СalculateFormFunctionCoef(elem.nodeP.point.X, elem.nodeI.point.X, elem.nodeJ.point.X, elem.nodeP.point.Y, elem.nodeI.point.Y, elem.nodeJ.point.Y);
                            elem.nodeP.coefD = СalculateFormFunctionCoef(elem.nodeI.point.X, elem.nodeJ.point.X, elem.nodeK.point.X, elem.nodeI.point.Y, elem.nodeJ.point.Y, elem.nodeK.point.Y);

                            elements.Add(elem);

                        }
                    }
                }

            }
        }

        private double СalculateFormFunctionCoef(double y1, double y2, double y3, double z1, double z2, double z3)
        {
            return ((y2 - y1) * (z3 - z1) - (z2 - z1) * (y3 - y1));
        }

        private void fillNodeProportionsCoef(int nodeNum, double volume, double density)
        {
            double currentCoef = volume * density * 9.81 / 4;
            if (this.nodeProportions[nodeNum].Equals(0))
            {
                this.nodeProportions[nodeNum] = currentCoef;
            }
            /*else if (!this.nodeProportions[nodeNum].Equals(currentCoef))
            {
                throw new ArithmeticException("Error occurred while filling proportion coefficient for node " + nodeNum + 
                    ". Existed coef " + this.nodeProportions[nodeNum] + " is not equal to computed " +  currentCoef + " for current element.");
            }*/ else
            {
                this.nodeProportions[nodeNum] += currentCoef;
            }
        }

        private Node createNode(int pointIndex, Point3D[] points, int[] pointNumbers) 
        {
            return new Node(
                pointNumbers[pointIndex], 
                new Point3D(points[pointIndex].X, points[pointIndex].Y, points[pointIndex].Z)
            );
        }
    }
}
