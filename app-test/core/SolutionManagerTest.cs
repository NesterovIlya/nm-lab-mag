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
    public class SolutionManagerTest
    {
        //For debugging purpose only
        //[TestMethod]
        public void Test_ApplyBoundaryConditions_When_DimensionIsGreaterThan2TimesBandwidth()
        {

/*
Созданная матрица:

1 2 3   2 3 4   0 0 0   0 0 0   0 0 0
0 4 5   0 5 6   0 0 0   0 0 0   0 0 0
0 0 6   0 0 7   0 0 0   0 0 0   0 0 0
                                
= = =   3 4 5   4 5 6   0 0 0   0 0 0
= = =   0 6 7   0 7 8   0 0 0   0 0 0
= = =   0 0 8   0 0 9   0 0 0   0 0 0
                                
= = =   = = =   5 6 7   6 7 8   0 0 0
= = =   = = =   0 8 9   0 9 10  0 0 0 
= = =   = = =   0 0 10  0 0 11  0 0 0

= = =   = = =   = = =   7 8  9   8 9  10 
= = =   = = =   = = =   0 10 11  0 11 12
= = =   = = =   = = =   0 0  12  0 0  13

= = =   = = =   = = =   = = =    9 10 11
= = =   = = =   = = =   = = =    0 12 13
= = =   = = =   = = =   = = =    0 0  14

Правая часть:
0 0 10

0 0 10

0 0 10

0 0 10

0 0 10

Ожидаемый результат:

Матрица:
1 2 3   0 0 0   0 0 0   0 0 0   0 0 0
0 4 5   0 0 0   0 0 0   0 0 0   0 0 0
0 0 6   0 0 0   0 0 0   0 0 0   0 0 0
                                
= = =   3 0 0   0 0 0   0 0 0   0 0 0
= = =   0 6 0   0 0 0   0 0 0   0 0 0
= = =   0 0 8   0 0 0   0 0 0   0 0 0
                                
= = =   = = =   5 6 7   0 0 0   0 0 0
= = =   = = =   0 8 9   0 0 0   0 0 0 
= = =   = = =   0 0 10  0 0 0   0 0 0

= = =   = = =   = = =   7 0  0   0 0 0 
= = =   = = =   = = =   0 10 0   0 0 0
= = =   = = =   = = =   0 0  12  0 0 0

= = =   = = =   = = =   = = =    9 10 11
= = =   = = =   = = =   = = =    0 12 13
= = =   = = =   = = =   = = =    0 0  14

Правая часть:
0 0 10

0 0 0

0 0 10

0 0 0

0 0 10

*/

            int dimension = 5;
            int bandWidth = 2;

            // Создаем глобальную матрицу
            SymmetricMatrix<MatrixDimension3> globalMatrix = new SymmetricMatrix<MatrixDimension3>(dimension, new MatrixDimension3());
            for (int row = 0; row < dimension; row++)
            {
                int rightBound = row + bandWidth < dimension ? row + bandWidth : dimension;
                for (int column = row; column < rightBound; column++)
                {
                    MatrixDimension3 matrixDim3 = new MatrixDimension3();
                    matrixDim3[0, 0] = row + column + 1;
                    matrixDim3[0, 1] = row + column + 2;
                    matrixDim3[0, 2] = row + column + 3;
                    matrixDim3[1, 1] = row + column + 4;
                    matrixDim3[1, 2] = row + column + 5;
                    matrixDim3[2, 2] = row + column + 6;

                    globalMatrix.setElement(row, column, matrixDim3);
                }
            }
            // Задаем граничные условия
            int[] boundaryConditions = { 1, 3 };
            int boundaryCount = boundaryConditions.Count();

            // Задаем правую часть
            Vector3D rightPart = new Vector3D(0,0,10);
            List<Vector3D> rightSide = new List<Vector3D>();
            for (int count = 0; count < dimension; count++)
                rightSide.Insert(count, rightPart);

            // Учитываем граничные условия
            SolutionManager solution = new SolutionManager();
            //solution.applyBoundaryConditions(globalMatrix, rightSide, boundaryConditions);

            // Задаем ожидаемый результат
            Vector3D neitralVector = new Vector3D();
            List<Vector3D> expectVector = new List<Vector3D>();
            expectVector.Insert(0, rightPart);
            expectVector.Insert(1, neitralVector);
            expectVector.Insert(2, rightPart);
            expectVector.Insert(3, neitralVector);
            expectVector.Insert(4, rightPart);

            MatrixDimension3 neitralMatrix = new MatrixDimension3();
            MatrixDimension3 expmatrixDim3 = new MatrixDimension3();
            SymmetricMatrix<MatrixDimension3> expectMatrix = new SymmetricMatrix<MatrixDimension3>(dimension, new MatrixDimension3());

            expmatrixDim3[0, 0] = 1;
            expmatrixDim3[0, 1] = 2;
            expmatrixDim3[0, 2] = 3;
            expmatrixDim3[1, 1] = 4;
            expmatrixDim3[1, 2] = 5;
            expmatrixDim3[2, 2] = 6;
            expectMatrix.setElement(0, 0, expmatrixDim3);

            //expectMatrix.setElement(0, 1, neitralMatrix);
            //expectMatrix.setElement(0, 2, neitralMatrix);
            //expectMatrix.setElement(1, 2, neitralMatrix);

            MatrixDimension3 expmatrixDim3_1 = new MatrixDimension3();
            expmatrixDim3_1[0, 0] = 3;
            expmatrixDim3_1[1, 1] = 6;
            expmatrixDim3_1[2, 2] = 8;
            expectMatrix.setElement(1, 1, expmatrixDim3_1);

            MatrixDimension3 expmatrixDim3_2 = new MatrixDimension3();
            expmatrixDim3_2[0, 0] = 5;
            expmatrixDim3_2[0, 1] = 6;
            expmatrixDim3_2[0, 2] = 7;
            expmatrixDim3_2[1, 1] = 8;
            expmatrixDim3_2[1, 2] = 9;
            expmatrixDim3_2[2, 2] = 10;
            expectMatrix.setElement(2, 2, expmatrixDim3_2);

            MatrixDimension3 expmatrixDim3_3 = new MatrixDimension3();
            expmatrixDim3_3[0, 0] = 7;
            expmatrixDim3_3[1, 1] = 10;
            expmatrixDim3_3[2, 2] = 12;
            expectMatrix.setElement(3, 3, expmatrixDim3_3);

            MatrixDimension3 expmatrixDim3_4 = new MatrixDimension3();
            expmatrixDim3_4[0, 0] = 9;
            expmatrixDim3_4[0, 1] = 10;
            expmatrixDim3_4[0, 2] = 11;
            expmatrixDim3_4[1, 1] = 12;
            expmatrixDim3_4[1, 2] = 13;
            expmatrixDim3_4[2, 2] = 14;
            expectMatrix.setElement(4, 4, expmatrixDim3_4);

            // Проверка
            foreach (Vector3D vector in rightSide)
            {
                Console.Write(vector.ToString() + " ");
            }
            Console.WriteLine();
            foreach (Vector3D vector in expectVector)
            {
                Console.Write(vector.ToString() + " ");
            }
            Console.WriteLine();

            Assert.IsTrue(expectMatrix.Equals(globalMatrix));
        }

        //For debugging purpose only
        //[TestMethod]
        public void Test_ApplyBoundaryConditions_When_DimensionIsNotGreaterThan2TimesBandwidth()
        {

            /*
            Созданная матрица:

            1 2 3   2 3 4   3 4 5   0 0 0   0 0 0
            0 4 5   0 5 6   0 6 7   0 0 0   0 0 0
            0 0 6   0 0 7   0 0 8   0 0 0   0 0 0

            = = =   3 4 5   4 5 6   5 6 7   0 0 0
            = = =   0 6 7   0 7 8   0 8 9   0 0 0
            = = =   0 0 8   0 0 9   0 0 10  0 0 0

            = = =   = = =   5 6 7   6 7 8   7 8  9 
            = = =   = = =   0 8 9   0 9 10  0 10 11 
            = = =   = = =   0 0 10  0 0 11  0 0  12

            = = =   = = =   = = =   7 8  9   8 9  10 
            = = =   = = =   = = =   0 10 11  0 11 12
            = = =   = = =   = = =   0 0  12  0 0  13

            = = =   = = =   = = =   = = =    9 10 11
            = = =   = = =   = = =   = = =    0 12 13
            = = =   = = =   = = =   = = =    0 0  14

            Правая часть:
            0 0 10

            0 0 10

            0 0 10

            0 0 10

            0 0 10

            Ожидаемый результат:

            Матрица:
            1 2 3   0 0 0   0 0 0   0 0 0   0 0 0
            0 4 5   0 0 0   0 0 0   0 0 0   0 0 0
            0 0 6   0 0 0   0 0 0   0 0 0   0 0 0

            = = =   3 0 0   0 0 0   0 0 0   0 0 0
            = = =   0 6 0   0 0 0   0 0 0   0 0 0
            = = =   0 0 8   0 0 0   0 0 0   0 0 0

            = = =   = = =   5 0 0   0 0 0   0 0 0
            = = =   = = =   0 8 0   0 0 0   0 0 0 
            = = =   = = =   0 0 10  0 0 0   0 0 0

            = = =   = = =   = = =   7 0  0   0 0 0 
            = = =   = = =   = = =   0 10 0   0 0 0
            = = =   = = =   = = =   0 0  12  0 0 0

            = = =   = = =   = = =   = = =    9 10 11
            = = =   = = =   = = =   = = =    0 12 13
            = = =   = = =   = = =   = = =    0 0  14

            Правая часть:
            0 0 10

            0 0 0

            0 0 0

            0 0 0

            0 0 10

            */

            int dimension = 5;
            int bandWidth = 3;

            // Создаем глобальную матрицу
            SymmetricMatrix<MatrixDimension3> globalMatrix = new SymmetricMatrix<MatrixDimension3>(dimension, new MatrixDimension3());
            for (int row = 0; row < dimension; row++)
            {
                int rightBound = row + bandWidth < dimension ? row + bandWidth : dimension;
                for (int column = row; column < rightBound; column++)
                {
                    MatrixDimension3 matrixDim3 = new MatrixDimension3();
                    matrixDim3[0, 0] = row + column + 1;
                    matrixDim3[0, 1] = row + column + 2;
                    matrixDim3[0, 2] = row + column + 3;
                    matrixDim3[1, 1] = row + column + 4;
                    matrixDim3[1, 2] = row + column + 5;
                    matrixDim3[2, 2] = row + column + 6;

                    globalMatrix.setElement(row, column, matrixDim3);
                }
            }
            // Задаем граничные условия
            int[] boundaryConditions = { 1, 2, 3 };
            int boundaryCount = boundaryConditions.Count();

            // Задаем правую часть
            Vector3D rightPart = new Vector3D();
            rightPart.X = 0;
            rightPart.Y = 0;
            rightPart.Z = 10;
            List<Vector3D> rightSide = new List<Vector3D>();
            for (int count = 0; count < dimension; count++)
                rightSide.Insert(count, rightPart);

            // Учитываем граничные условия
            SolutionManager solution = new SolutionManager();
            //solution.applyBoundaryConditions(globalMatrix, rightSide, boundaryConditions);


            // Задаем ожидаемый результат
            Vector3D neitralVector = new Vector3D();
            List<Vector3D> expectVector = new List<Vector3D>();
            expectVector.Insert(0, rightPart);
            expectVector.Insert(1, neitralVector);
            expectVector.Insert(2, neitralVector);
            expectVector.Insert(3, neitralVector);
            expectVector.Insert(4, rightPart);

            MatrixDimension3 neitralMatrix = new MatrixDimension3();
            MatrixDimension3 expmatrixDim3 = new MatrixDimension3();
            SymmetricMatrix<MatrixDimension3> expectMatrix = new SymmetricMatrix<MatrixDimension3>(dimension, new MatrixDimension3());

            expmatrixDim3[0, 0] = 1;
            expmatrixDim3[0, 1] = 2;
            expmatrixDim3[0, 2] = 3;
            expmatrixDim3[1, 1] = 4;
            expmatrixDim3[1, 2] = 5;
            expmatrixDim3[2, 2] = 6;
            expectMatrix.setElement(0, 0, expmatrixDim3);

            //expectMatrix.setElement(0, 1, neitralMatrix);
            //expectMatrix.setElement(0, 2, neitralMatrix);
            //expectMatrix.setElement(1, 2, neitralMatrix);

            MatrixDimension3 expmatrixDim3_1 = new MatrixDimension3();
            expmatrixDim3_1[0, 0] = 3;
            expmatrixDim3_1[1, 1] = 6;
            expmatrixDim3_1[2, 2] = 8;
            expectMatrix.setElement(1, 1, expmatrixDim3_1);

            MatrixDimension3 expmatrixDim3_2 = new MatrixDimension3();
            expmatrixDim3_2[0, 0] = 5;
            expmatrixDim3_2[1, 1] = 8;
            expmatrixDim3_2[2, 2] = 10;
            expectMatrix.setElement(2, 2, expmatrixDim3_2);

            MatrixDimension3 expmatrixDim3_3 = new MatrixDimension3();
            expmatrixDim3_3[0, 0] = 7;
            expmatrixDim3_3[1, 1] = 10;
            expmatrixDim3_3[2, 2] = 12;
            expectMatrix.setElement(3, 3, expmatrixDim3_3);

            MatrixDimension3 expmatrixDim3_4 = new MatrixDimension3();
            expmatrixDim3_4[0, 0] = 9;
            expmatrixDim3_4[0, 1] = 10;
            expmatrixDim3_4[0, 2] = 11;
            expmatrixDim3_4[1, 1] = 12;
            expmatrixDim3_4[1, 2] = 13;
            expmatrixDim3_4[2, 2] = 14;
            expectMatrix.setElement(4, 4, expmatrixDim3_4);

            // Проверка
            foreach (Vector3D vector in rightSide)
            {
                Console.Write(vector.ToString() + " ");
            }
            Console.WriteLine();
            foreach (Vector3D vector in expectVector)
            {
                Console.Write(vector.ToString() + " ");
            }
            Console.WriteLine();

            Assert.IsTrue(expectMatrix.Equals(globalMatrix), "All is bad");
        }

        [TestMethod]
        public void Test_BuildSolution()
        {
            SolutionManager solutionManager = new SolutionManager();
            int[] boundaryConditions = new int[2] { 0, 2 };
            InputData inputData = new InputData(
                2, 2, 2,
                1, 1, 1,
                1, 0.25, 1,
                1,
                boundaryConditions
            );

            IList<IList<Vector3D>> solutions = solutionManager.buildSolution(inputData);
            Assert.AreEqual(1, solutions.Count);
            foreach (IList<Vector3D> solution in solutions)
            {
                Assert.AreEqual(8, solution.Count);
            }
        }
    }
}
