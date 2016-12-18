using app.core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace app_test.core
{
    [TestClass]
    public class ElementsMapTest
    {
        /*
         * Построение сетки 1х1х1 с одинаковым по всем осям шагом.
         * Проверяем корректность поэлементного разбиения, а также узлов сетки (номер, координаты, функции формы). 
         */
        [TestMethod]
        public void Test_Init1x1x1()
        {
            double hx = 1;
            double hy = 1;
            double hz = 1;
            int Nx = 1;
            int Ny = 1;
            int Nz = 1;
            double elasticityModulus = 70;
            double poissonRatio = 0.34;
            double density = 1.0;
            int iterationsCount = 10;
            int[] boundaryConditions = { 0, 2, 4, 6 };
            InputData inputData = new InputData(hx, hy, hz, Nx, Ny, Nz, elasticityModulus, poissonRatio, density, iterationsCount, boundaryConditions);

            ElementsMap parsedElements = new ElementsMap(inputData);

            Assert.IsNotNull(parsedElements.input, "Input Data property of elements map must be not null!");
            Assert.AreEqual(inputData, parsedElements.input, "Input Data property of elements map must be equal to inbound input data object.");
            Assert.IsTrue(parsedElements.elements.Count == 6, "Elements map must contains 6 elements! Actual: " + parsedElements.elements.Count);

            /* |------------------|  |--------------|   |--------------------------------------------------|
             * |Узлы:             |  |Элементы:     |   |Элементы (координаты):                            |
             * |N: 0 1 2 3 4 5 6 7|  |N: 0 1 2 3 4 5|   |N:    0       1       2       3       4       5   |
             * |------------------|  |--------------|   |--------------------------------------------------|
             * |x: 0 0 0 0 1 1 1 1|  |I: 5 2 2 1 4 4|   |    x y z   x y z   x y z   x y z   x y z   x y z |
             * |y: 0 0 1 1 0 0 1 1|  |J: 3 6 7 3 5 3|   |I: (1,0,1) (0,1,0) (0,1,0) (0,0,1) (1,0,0) (1,0,0)|
             * |z: 0 1 0 1 0 1 0 1|  |K: 7 7 3 5 3 2|   |J: (0,1,1) (1,1,0) (1,1,1) (0,1,1) (1,0,1) (0,1,1)|
             * |------------------|  |P: 4 4 4 0 0 0|   |K: (1,1,1) (1,1,1) (0,1,1) (1,0,1) (0,1,1) (0,1,0)|
             *                       |--------------|   |P: (1,0,0) (1,0,0) (1,0,0) (0,0,0) (0,0,0) (0,0,0)|
             *                                          |--------------------------------------------------|
             * Расчет коэффициентов функции формы: (запись x_ij означает x_i - x_j)
             * b = (b_i, b_j, b_k, b_p)                 c = (c_i, c_j, c_k, c_p)                d = (d_i, d_j, d_k, d_p)            
             * b_i = - (y_kj * z_pj - y_pj * z_kj);     c_i = x_kj * z_pj - x_pj * z_kj;    d_i = - (x_kj * y_pj - x_pj * y_kj);                   
             * b_j = - (y_pk * z_ik - y_ik * z_pk);     c_j = x_pk * z_ik - x_ik * z_pk;    d_j = - (x_pk * y_ik - x_ik * y_pk);
             * b_k = - (y_ip * z_jp - y_jp * z_ip);     c_k = x_ip * z_jp - x_jp * z_ip;    d_k = - (x_ip * y_jp - x_jp * y_ip);
             * b_p = - (y_ji * z_ki - y_ki * z_ji);     c_p = x_ji * z_ki - x_ki * z_ji;    d_p = - (x_ji * y_ki - x_ki * y_ji);                 
             */

            Element firstExpectedElement = new Element(
                0,
                0,
                new Node(5, new Point3D(1, 0, 1), 0, -1, 1),
                new Node(3, new Point3D(0, 1, 1), 1, 0, 0),
                new Node(7, new Point3D(1, 1, 1), 1, 1, 0),
                new Node(4, new Point3D(1, 0, 0), 0, 0, 1)
            );
            Element firstActualElement = parsedElements.elements[0];

            Assert.AreEqual(firstExpectedElement.id, firstActualElement.id);
            Assert.AreEqual(firstExpectedElement.nodeI, firstActualElement.nodeI);
            Assert.AreEqual(firstExpectedElement.nodeJ, firstActualElement.nodeJ);
            Assert.AreEqual(firstExpectedElement.nodeK, firstActualElement.nodeK);
            Assert.AreEqual(firstExpectedElement.nodeP, firstActualElement.nodeP);


        }
    }
}
