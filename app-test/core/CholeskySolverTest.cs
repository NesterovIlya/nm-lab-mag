using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using app.core;
using System.Windows.Media.Media3D;

namespace app_test.core
{
    [TestClass]
    public class CholeskySolverTest
    {
        [TestMethod]
        public void Test_Solve2Dimension()
        {
            /*
                Созданная матрица:
                
                1  0  0   2  2  2
                0  1  1  -2 -2 -2 
                0  1  0  -8 -8 -8

                2 -2 -8   3  3  3 
                2 -2 -8   3  0  3
                2 -2 -8   3  3  6

                Правая часть:
                 1  2  1
                -8 -8 -8

                Ожидаемый результат:
                решение:
                1 1 1
                0 0 0

            */
            int dimension = 2;

            SymmetricMatrix<MatrixDimension3> sourceMatrix = new SymmetricMatrix<MatrixDimension3>(dimension, new MatrixDimension3());

            MatrixDimension3 globalPart00 = new MatrixDimension3();
            globalPart00[0, 0] = 1;
            globalPart00[0, 1] = 0;
            globalPart00[1, 0] = globalPart00[0, 1];
            globalPart00[0, 2] = 0;
            globalPart00[2, 0] = globalPart00[0, 2];
            globalPart00[1, 1] = 1;
            globalPart00[1, 2] = 1;
            globalPart00[2, 1] = globalPart00[1, 2];
            globalPart00[2, 2] = 0;

            MatrixDimension3 globalPart01 = new MatrixDimension3();
            globalPart01[0, 0] = 2;
            globalPart01[0, 1] = 2;
            globalPart01[1, 0] = -2;
            globalPart01[0, 2] = 2;
            globalPart01[2, 0] = -8;
            globalPart01[1, 1] = -2;
            globalPart01[1, 2] = -2;
            globalPart01[2, 1] = -8;
            globalPart01[2, 2] = -8;

            MatrixDimension3 globalPart11 = new MatrixDimension3();
            globalPart11[0, 0] = 3;
            globalPart11[0, 1] = 3;
            globalPart11[1, 0] = globalPart11[0, 1];
            globalPart11[0, 2] = 3;
            globalPart11[2, 0] = globalPart11[0, 2];
            globalPart11[1, 1] = 0;
            globalPart11[1, 2] = 3;
            globalPart11[2, 1] = globalPart11[1, 2];
            globalPart11[2, 2] = 0;

            sourceMatrix[0, 0] = globalPart00;
            sourceMatrix[0, 1] = globalPart01;
            sourceMatrix[1, 1] = globalPart11;

            List<Vector3D> rightPart = new List<Vector3D>();
            Vector3D part1 = new Vector3D(1, 2, 1);
            Vector3D part2 = new Vector3D(-8, -8, -8);
            rightPart.Insert(0, part1);
            rightPart.Insert(1, part2);

            CholeskySolver solver = new CholeskySolver();

            SymmetricMatrix<DoubleContainerElement> BMatrix = new SymmetricMatrix<DoubleContainerElement>(2 * 3, new DoubleContainerElement());
            SymmetricMatrix<DoubleContainerElement> CMatrix = new SymmetricMatrix<DoubleContainerElement>(2 * 3, new DoubleContainerElement());
            
            IList<Vector3D> solution = solver.solve(SymmetricMatrix<DoubleContainerElement>.extractMatrix(sourceMatrix), rightPart);

            //Ожидаемый результат
            List<Vector3D> expectVector = new List<Vector3D>();
            Vector3D part3 = new Vector3D(1, 1, 1);
            Vector3D part4 = new Vector3D(0, 0, 0);
            expectVector.Insert(0, part3);
            expectVector.Insert(1, part4);

            //Проверка
            foreach (Vector3D vector in solution)
            {
                Console.Write(vector.ToString() + " ");
            }
            Console.WriteLine();

            foreach (Vector3D vector in expectVector)
            {
                Console.Write(vector.ToString() + " ");
            }
            Console.WriteLine();

        }

        //For debugging purpose only
        //[TestMethod]
        public void Test_ExtractVector()
        {
            List<Vector3D> sourceVector = new List<Vector3D>();
            for (int index = 0; index < 5; index++)
            {
                Vector3D vector = new Vector3D();
                vector.X = index;
                vector.Y = index + 1;
                vector.Z = index + 2;

                sourceVector.Insert(index, vector);
            }

            //List<Double> result = new CholeskySolver().extractVector(sourceVector);
            //List<Vector3D> resVector = new CholeskySolver().extractFromVector(result);
            
            //Assert.IsTrue(sourceVector.Equals(resVector));
        }

        //For debugging purpose only
        //[TestMethod]
        public void Test_CreateBCMatrix()
        {
            /*
                Созданная матрица:
                
                1  0  0  2  2  2
                0  1  1 -2 -2 -2 
                0  1  0 -8 -8 -8
                2 -2 -8  3  3  3 
                2 -2 -8  3  0  3
                2 -2 -8  3  3  6

            */
            int dimension = 6;

            SymmetricMatrix<DoubleContainerElement> sourceMatrix = new SymmetricMatrix<DoubleContainerElement>(dimension, new DoubleContainerElement());

            sourceMatrix[0, 0] = new DoubleContainerElement(1);
            sourceMatrix[0, 1] = new DoubleContainerElement(0);
            sourceMatrix[0, 2] = new DoubleContainerElement(0);
            sourceMatrix[0, 3] = new DoubleContainerElement(2);
            sourceMatrix[0, 4] = new DoubleContainerElement(2);
            sourceMatrix[0, 5] = new DoubleContainerElement(2);

            sourceMatrix[1, 1] = new DoubleContainerElement(1);
            sourceMatrix[1, 2] = new DoubleContainerElement(1);
            sourceMatrix[1, 3] = new DoubleContainerElement(-2);
            sourceMatrix[1, 4] = new DoubleContainerElement(-2);
            sourceMatrix[1, 5] = new DoubleContainerElement(-2);

            sourceMatrix[2, 2] = new DoubleContainerElement(0);
            sourceMatrix[2, 3] = new DoubleContainerElement(-8);
            sourceMatrix[2, 4] = new DoubleContainerElement(-8);
            sourceMatrix[2, 5] = new DoubleContainerElement(-8);

            sourceMatrix[3, 3] = new DoubleContainerElement(3);
            sourceMatrix[3, 4] = new DoubleContainerElement(3);
            sourceMatrix[3, 5] = new DoubleContainerElement(3);

            sourceMatrix[4, 4] = new DoubleContainerElement(0);
            sourceMatrix[4, 5] = new DoubleContainerElement(3);

            sourceMatrix[5, 5] = new DoubleContainerElement(0);

            SymmetricMatrix<DoubleContainerElement> BMatrix = new SymmetricMatrix<DoubleContainerElement>(dimension, new DoubleContainerElement());
            SymmetricMatrix<DoubleContainerElement> CMatrix = new SymmetricMatrix<DoubleContainerElement>(dimension, new DoubleContainerElement());

            CholeskySolver solver = new CholeskySolver();

            //solver.createBCMatrix(BMatrix, CMatrix, sourceMatrix);

            // Ожидаемый результат:
            SymmetricMatrix<DoubleContainerElement> expBMatrix = new SymmetricMatrix<DoubleContainerElement>(dimension, new DoubleContainerElement());
            SymmetricMatrix<DoubleContainerElement> expCMatrix = new SymmetricMatrix<DoubleContainerElement>(dimension, new DoubleContainerElement());

            //Матрица B

            expBMatrix[0, 0] = new DoubleContainerElement(1);
            expBMatrix[1, 0] = new DoubleContainerElement(0);
            expBMatrix[2, 0] = new DoubleContainerElement(0);
            expBMatrix[3, 0] = new DoubleContainerElement(2);
            expBMatrix[4, 0] = new DoubleContainerElement(2);
            expBMatrix[5, 0] = new DoubleContainerElement(2);

            expBMatrix[1, 1] = new DoubleContainerElement(1);
            expBMatrix[2, 1] = new DoubleContainerElement(1);
            expBMatrix[3, 1] = new DoubleContainerElement(-2);
            expBMatrix[4, 1] = new DoubleContainerElement(-2);
            expBMatrix[5, 1] = new DoubleContainerElement(-2);

            expBMatrix[2, 2] = new DoubleContainerElement(-1);
            expBMatrix[3, 2] = new DoubleContainerElement(-6);
            expBMatrix[4, 2] = new DoubleContainerElement(-6);
            expBMatrix[5, 2] = new DoubleContainerElement(-6);

            expBMatrix[3, 3] = new DoubleContainerElement(31);
            expBMatrix[4, 3] = new DoubleContainerElement(31);
            expBMatrix[5, 3] = new DoubleContainerElement(31);

            expBMatrix[4, 4] = new DoubleContainerElement(-3);
            expBMatrix[5, 4] = new DoubleContainerElement(0);

            expBMatrix[5, 5] = new DoubleContainerElement(-3);

            //Матрица C

            expCMatrix[0, 0] = new DoubleContainerElement(1);
            expCMatrix[0, 1] = new DoubleContainerElement(0);
            expCMatrix[0, 2] = new DoubleContainerElement(0);
            expCMatrix[0, 3] = new DoubleContainerElement(2);
            expCMatrix[0, 4] = new DoubleContainerElement(2);
            expCMatrix[0, 5] = new DoubleContainerElement(2);

            expCMatrix[1, 1] = new DoubleContainerElement(1);
            expCMatrix[1, 2] = new DoubleContainerElement(1);
            expCMatrix[1, 3] = new DoubleContainerElement(-2);
            expCMatrix[1, 4] = new DoubleContainerElement(-2);
            expCMatrix[1, 5] = new DoubleContainerElement(-2);

            expCMatrix[2, 2] = new DoubleContainerElement(1);
            expCMatrix[2, 3] = new DoubleContainerElement(6);
            expCMatrix[2, 4] = new DoubleContainerElement(6);
            expCMatrix[2, 5] = new DoubleContainerElement(6);

            expCMatrix[3, 3] = new DoubleContainerElement(1);
            expCMatrix[3, 4] = new DoubleContainerElement(1);
            expCMatrix[3, 5] = new DoubleContainerElement(1);

            expCMatrix[4, 4] = new DoubleContainerElement(1);
            expCMatrix[4, 5] = new DoubleContainerElement(0);

            expCMatrix[5, 5] = new DoubleContainerElement(1);

            // Проверка
            //Assert.IsTrue(expCMatrix.Equals(CMatrix), "All is bad");
            Assert.IsTrue(expBMatrix.Equals(BMatrix), "All is bad");
            Assert.IsTrue(expCMatrix.Equals(CMatrix), "All is bad");

        }

        //For debugging purpose only
        //[TestMethod]
        public void Test_ExtractMatrix()
        {
            /*
                Созданная матрица:
                
                0 1 2   0 0 0   0 0 0 
                1 3 4   0 0 0   0 0 0
                2 4 5   0 0 0   0 0 0
                
                0 0 0   1 2 3   0 0 0
                0 0 0   2 4 5   0 0 0
                0 0 0   3 5 6   0 0 0
                
                0 0 0   0 0 0   4 5 6
                0 0 0   0 0 0   5 7 8
                0 0 0   0 0 0   6 8 9
            */
            SymmetricMatrix<MatrixDimension3> sourceMatrix = new SymmetricMatrix<MatrixDimension3>(3, new MatrixDimension3());
            for (int row = 0; row < sourceMatrix.Dimension; row++)
            {
                MatrixDimension3 matrixDim3 = new MatrixDimension3();
                matrixDim3[0, 0] = row * row;
                matrixDim3[0, 1] = row * row + 1;
                matrixDim3[1, 0] = matrixDim3[0, 1];
                matrixDim3[0, 2] = row * row + 2;
                matrixDim3[2, 0] = matrixDim3[0, 2];
                matrixDim3[1, 1] = row * row + 3;
                matrixDim3[1, 2] = row * row + 4;
                matrixDim3[2, 1] = matrixDim3[1, 2];
                matrixDim3[2, 2] = row * row + 5;

                sourceMatrix.setElement(row, row, matrixDim3);
            }

            //SymmetricMatrix<DoubleContainerElement> actualMatrix = new CholeskySolver().extractMatrix(sourceMatrix);
            Console.WriteLine("WAT");
        }
    }
}
