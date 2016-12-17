using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace app.core
{
    public class GeneralMatrixBuilder
    {

        public GeneralMatrixBuilder() {}

        public SymmetricMatrix<MatrixDimension3> build(ElementsMap elementsMap)
        {
            InputData input = elementsMap.input;

            SymmetricMatrix<MatrixDimension3> result = new SymmetricMatrix<MatrixDimension3>((input.Nx + 1) * (input.Ny + 1) * (input.Nz + 1), new MatrixDimension3());
            
            Matrix matrixD = new Matrix(6, 6);
            matrixD.setElement(0, 0, 1);
            matrixD.setElement(0, 1, input.poissonRatio / (1 - input.poissonRatio));
            matrixD.setElement(0, 2, input.poissonRatio / (1 - input.poissonRatio));
            matrixD.setElement(1, 1, 1);
            matrixD.setElement(1, 2, input.poissonRatio / (1 - input.poissonRatio));
            matrixD.setElement(2, 2, 1);
            matrixD.setElement(3, 3, (1 - 2 * input.poissonRatio) / 2 * (1 - input.poissonRatio));
            matrixD.setElement(4, 4, (1 - 2 * input.poissonRatio) / 2 * (1 - input.poissonRatio));
            matrixD.setElement(4, 5, (1 - 2 * input.poissonRatio) / 2 * (1 - input.poissonRatio));


            foreach (var element in elementsMap.elements)
            {
                createMatrKE(element, result, matrixD);
            }

            return result;
        }

        private void createMatrKE(Element element, SymmetricMatrix<MatrixDimension3> matr, Matrix matrixD)
        {

            double v = 1/(36 * element.volume * element.volume);
            Node node = null;
            for(int i = 0; i < 4; i++)
            {
                for(int j = i; j < 4; j++)
                {
                    switch(i){
                        case 0:
                            node = element.nodeI;
                            break;
                        case 1:
                            node = element.nodeJ;
                            break;
                        case 2:
                            node = element.nodeK;
                            break;
                        case 3:
                            node = element.nodeP;

                            break;
                    }

                    Matrix matrBi = new Matrix(6, 3);
                    createMatrB(matrBi, node);
                    matrBi = transpose(matrBi);

                    switch (j)
                    {
                        case 0:
                            node = element.nodeI;
                            break;
                        case 1:
                            node = element.nodeJ;
                            break;
                        case 2:
                            node = element.nodeK;
                            break;
                        case 3:
                            node = element.nodeP;

                            break;
                    }

                    Matrix matrBj = new Matrix(6, 3);
                    createMatrB(matrBj, node);
                   
                    MatrixDimension3 matrix = MatrixDimension3.getFromMatrix(matrBi * matrixD * matrBj);
                    multiply(matrix, v);
                    int globali = ElementsMap.ElementDecomposition[element.id, i];
                    int globalj = ElementsMap.ElementDecomposition[element.id, j];
                    matr[globali, globalj] = matr[globali, globalj] + matrix;
                }
            }
            
        }

        private Matrix transpose(Matrix matrix)
        {
            Matrix result = new Matrix(matrix.ColumnsCount, matrix.RowsCount);
         
            for (int i = 0; i < matrix.RowsCount; i++)
            {
                for (int j = 0; j < matrix.ColumnsCount; j++)
                {
                    result[j,i] = matrix[i,j];
                }
            }
            return result;
        }

        private void multiply (MatrixDimension3 matrix, double v)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    matrix[i, j] = matrix[i, j] * v;
                }
            }
        }
        private void createMatrB(Matrix matrix, Node node)
        {
            for (int i = 0; i < matrix.RowsCount; i++)
            {
                for (int j = 0; j < matrix.ColumnsCount; j++)
                {
                    matrix[0, 0] = node.coefB;
                    matrix[1, 1] = node.coefC;
                    matrix[2, 2] = node.coefD;
                    matrix[3, 0] = node.coefC;
                    matrix[3, 1] = node.coefB;
                    matrix[4, 1] = node.coefD;
                    matrix[4, 2] = node.coefC;
                    matrix[5, 0] = node.coefD;
                    matrix[5, 2] = node.coefB;
                }
            }
        }

    }

}
