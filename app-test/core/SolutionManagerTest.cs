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
        [TestMethod]
        public void Test_SolutionManager()
        {
            int dimension = 3;

            // Создаем глобальную матрицу
            MatrixDimension3 matrixDim3 = new MatrixDimension3();
            SymmetricMatrix<MatrixDimension3> globalMatrix = new SymmetricMatrix<MatrixDimension3>(dimension, matrixDim3);
            for (int row = 0; row < dimension; row++)
                for (int column = row; column < dimension; column++)
                {
                    matrixDim3[0, 0] = row * row + column;
                    matrixDim3[0, 1] = row * row + column + 1;
                    matrixDim3[0, 2] = row * row + column + 2;
                    matrixDim3[1, 1] = row * row + column + 3;
                    matrixDim3[1, 2] = row * row + column + 4;
                    matrixDim3[2, 2] = row * row + column + 5;

                    globalMatrix.setElement(row, column, matrixDim3);
                }

            // Задаем граничные условия
            int[] boundaryConditions = { 1, 2 };
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
            solution.applyBoundaryConditions(globalMatrix, rightSide, boundaryConditions);

            // Задаем ожидаемый результат
            Vector3D neitralVector = new Vector3D();
            List<Vector3D> expectVector = new List<Vector3D>();
            expectVector.Insert(0, rightPart);
            expectVector.Insert(1, neitralVector);
            expectVector.Insert(2, neitralVector);

            MatrixDimension3 neitralMatrix = new MatrixDimension3();
            MatrixDimension3 expmatrixDim3 = new MatrixDimension3();
            SymmetricMatrix<MatrixDimension3> expectMatrix = new SymmetricMatrix<MatrixDimension3>(dimension, neitralMatrix);

            expmatrixDim3[0, 0] = 0;
            expmatrixDim3[0, 1] = 1;
            expmatrixDim3[0, 2] = 2;
            expmatrixDim3[1, 1] = 3;
            expmatrixDim3[1, 2] = 4;
            expmatrixDim3[2, 2] = 5;
            expectMatrix.setElement(0, 0, expmatrixDim3);

            expectMatrix.setElement(0, 1, neitralMatrix);
            expectMatrix.setElement(0, 2, neitralMatrix);
            expectMatrix.setElement(1, 2, neitralMatrix);

            expmatrixDim3 = neitralMatrix;
            expmatrixDim3[0, 0] = 2;
            expmatrixDim3[1, 1] = 5;
            expmatrixDim3[2, 2] = 7;
            expectMatrix.setElement(1, 1, expmatrixDim3);

            expmatrixDim3[0, 0] = 6;
            expmatrixDim3[1, 1] = 9;
            expmatrixDim3[2, 2] = 11;
            expectMatrix.setElement(2, 2, expmatrixDim3);
            
            // Проверка
            Assert.AreEqual(expectVector.ToString(), rightSide.ToString());
            Assert.AreEqual(expectMatrix.ToString(), globalMatrix.ToString());
        }
    }
}

/*
Созданная матрица:

0 1 2   1 2 3   2 3 4 
0 3 4   0 4 5   0 5 6
0 0 5   0 0 6   0 0 7

1 2 3   2 3 4   3 4 5
0 4 5   0 5 6   0 6 7
0 0 6   0 0 7   0 0 8

2 3 4   3 4 5   6 7 8
0 5 6   0 6 7   0 9 10
0 0 7   0 0 8   0 0 11

Правая часть:
0 0 10

0 0 10

0 0 10

Ожмдаемый результат:

Матрица:
0 1 2   0 0 0   0 0 0 
0 3 4   0 0 0   0 0 0
0 0 5   0 0 0   0 0 0

0 0 0   2 0 0   0 0 0
0 0 0   0 5 0   0 0 0
0 0 0   0 0 7   0 0 0

0 0 0   0 0 0   6 0 0
0 0 0   0 0 0   0 9 0
0 0 0   0 0 0   0 0 11

Правая часть:
0 0 10

0 0 0

0 0 0

*/
