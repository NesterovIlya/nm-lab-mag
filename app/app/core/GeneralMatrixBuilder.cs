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
            matrixD[0, 0] = 1;
            matrixD[0, 1] = input.poissonRatio / (1 - input.poissonRatio);
            matrixD[1, 0] = matrixD[0, 1];
            matrixD[0, 2] = input.poissonRatio / (1 - input.poissonRatio);
            matrixD[2, 0] = matrixD[0, 2];
            matrixD[1, 1] = 1;
            matrixD[1, 2] = input.poissonRatio / (1 - input.poissonRatio);
            matrixD[2, 1] = matrixD[1, 2];
            matrixD[2, 2] = 1;
            matrixD[3, 3] = (1 - 2 * input.poissonRatio) / (2 * (1 - input.poissonRatio));
            matrixD[4, 4] = (1 - 2 * input.poissonRatio) / (2 * (1 - input.poissonRatio));
            matrixD[5, 5] = (1 - 2 * input.poissonRatio) / (2 * (1 - input.poissonRatio));

            matrixD = matrixD * (input.elasticityModulus * (1 - input.poissonRatio) / ((1 + input.poissonRatio) * (1 - 2 * input.poissonRatio)));


            foreach (var element in elementsMap.elements)
            {
                createMatrKE(element, result, matrixD);
            }

            var tempRes = SymmetricMatrix<MatrixDimension3>.ToMatrix(result);
            var sumByRowTempRes = tempRes.SumByRow();
            return result;
        }

        private void createMatrKE(Element element, SymmetricMatrix<MatrixDimension3> matr, Matrix matrixD)
        {
            double v = 1/(36 * element.volume);
            for(int r = 0; r < 4; r++)
            {
                for(int s = r; s < 4; s++)
                {
                    Node rNode = getNodeByIndex(element, r);
                    Matrix matrBr = transpose(createMatrB(rNode));

                    Node sNode = getNodeByIndex(element, s);
                    Matrix matrBs = createMatrB(sNode);


                    var buf = MatrixDimension3.getFromMatrix(matrBr * matrixD * matrBs) * v;
                    matr[rNode.id, sNode.id] = matr[rNode.id, sNode.id] + buf; //(rNode.id > sNode.id ? (MatrixDimension3) buf.getTransposed() : buf);                   
                }
            }    
        }

        private Node getNodeByIndex(Element element, int index)
        {
            switch (index)
            {
                case 0:
                    return element.nodeI;
                case 1:
                    return element.nodeJ;
                case 2:
                    return element.nodeK;
                case 3:
                    return element.nodeP;
                default: throw new IndexOutOfRangeException("Index must be 0,1,2 or 3! Actual: " + index);
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

        private Matrix createMatrB(Node node)
        {
            /*
             *   b_i  0   0
             *    0  c_i  0
             *    0   0  d_i
             *   c_i b_i  0
             *    0  d_i c_i
             *   d_i  0  b_i
             */
            Matrix matrix = new Matrix(6, 3);
            matrix[0, 0] = node.coefB;
            matrix[1, 1] = node.coefC;
            matrix[2, 2] = node.coefD;
            matrix[3, 0] = node.coefC;
            matrix[3, 1] = node.coefB;
            matrix[4, 1] = node.coefD;
            matrix[4, 2] = node.coefC;
            matrix[5, 0] = node.coefD;
            matrix[5, 2] = node.coefB;
           

            return matrix;
        }

    }

}
