using app.core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app_test.core
{
    [TestClass]
    public class ElementsMapTest
    {
        [TestMethod]
        public void TestInit()
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
            int[] boundaryConditions = { 1, 2, 3, 4 };
            InputData inputD = new InputData(hx, hy, hz, Nx, Ny, Nz, elasticityModulus, poissonRatio, density, iterationsCount, boundaryConditions);

            ElementsMap elem1 = new ElementsMap(inputD);
            Assert.IsNotNull(elem1);
        }
    }
}
