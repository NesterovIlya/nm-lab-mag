using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app.core
{
    class GeneralMatrixBuild
    {
        
        const double  nu = 1;
        private static Matrix matrixD = new Matrix(6, 6);
        static GeneralMatrixBuild()
        {
            matrixD.setElement(0, 0, 1);
            matrixD.setElement(0, 1, nu / (1 - nu));
            matrixD.setElement(0, 2, nu /( 1 - nu));
            matrixD.setElement(1, 1, 1);
            matrixD.setElement(1, 2, nu / (1 - nu));
            matrixD.setElement(2, 2, 1);
            matrixD.setElement(3, 3, (1 - 2 * nu) / 2*(1 - nu));
            matrixD.setElement(4, 4, (1 - 2 * nu) / 2 * (1 - nu));
            matrixD.setElement(4, 5, (1 - 2 * nu) / 2 * (1 - nu));

        }

        public static SymmetricMatrix<MatrixDimension3> build(ElementsMap elementsMap)
        {
            InputData input = elementsMap.input;
            MatrixDimension3 temp = new MatrixDimension3();
            SymmetricMatrix<MatrixDimension3> result = new SymmetricMatrix<MatrixDimension3>(input.Nx*input.Ny*input.Nz,temp.getNeutralElememt() as MatrixDimension3);
            foreach (var element in elementsMap.elements)
            {
                createMatrKE(element, result);
            }
            return null;
        }

        private static void createMatrKE(Element element, SymmetricMatrix<MatrixDimension3> matr)
        {
            double v = 1/(36*1);
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

        public static Matrix transpose (Matrix matrix)
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

        public static void multiply (MatrixDimension3 matrix, double v)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    matrix[i, j] = matrix[i, j] * v;
                }
            }
        }
        public static void createMatrB(Matrix matrix, Node node)
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
