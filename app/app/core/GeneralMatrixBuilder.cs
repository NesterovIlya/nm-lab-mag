using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace app.core
{
    public class GeneralMatrixBuilder
    {

        public GeneralMatrixBuilder() {}

        public SymmetricMatrix<DoubleContainerElement> build(ElementsMap elementsMap)
        {
            InputData input = elementsMap.input;

            SymmetricMatrix<MatrixDimension3> result = new SymmetricMatrix<MatrixDimension3>((input.Nx + 1) * (input.Ny + 1) * (input.Nz + 1), new MatrixDimension3());

            Matrix resultSegerlind = new Matrix(result.Dimension * 3, result.Dimension * 3);

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

            StreamWriter sw = new StreamWriter("C:\\Data\\university\\master_1_year\\NM\\nm-lab-mag\\files\\log.txt");
            sw.Close();

            foreach (var element in elementsMap.elements)
            {
                createMatrKESegerlind(element, resultSegerlind, matrixD);
                createMatrKE(element, result, matrixD);
            }
            resultSegerlind.printToFile("C:\\Data\\university\\master_1_year\\NM\\nm-lab-mag\\files\\GeneralMatrixSegerlind.txt");
            var sum = resultSegerlind.SumByRow();

            var tempRes = SymmetricMatrix<MatrixDimension3>.ToMatrix(result);
            var sumByRowTempRes = tempRes.SumByRow();
            return SymmetricMatrix<DoubleContainerElement>.fromMatrix(resultSegerlind);
        }

        private void createMatrKE(Element element, SymmetricMatrix<MatrixDimension3> matr, Matrix matrixD)
        {

            //----TODO: remove------
            Matrix bigMatrix = new Matrix(12, 12);

            //----------------------

            double v = 1/(36 * element.volume);
            for(int r = 0; r < 4; r++)
            {
                for(int s = r; s < 4; s++)
                {
                    Node rNode = getNodeByIndex(element, r);
                    Matrix matrBr = transpose(createMatrB(rNode));

                    Node sNode = getNodeByIndex(element, s);
                    Matrix matrBs = createMatrB(sNode);

                    //TODO: rename
                    var buf = MatrixDimension3.getFromMatrix(matrBr * matrixD * matrBs) * v;

                    for (int rowNum = 0; rowNum < 2; rowNum++)
                    {
                        for (int colNum = 0; colNum < 2; colNum++)
                        {
                            bigMatrix[3*r + rowNum, 3*s + colNum] = buf[rowNum, colNum];
                            
                            if (r != s)
                            {
                                bigMatrix[3 * s + colNum, 3 * r + rowNum] = bigMatrix[3 * r + rowNum, 3 * s + colNum];
                            }
                        }
                    }

                    matr[rNode.id, sNode.id] = matr[rNode.id, sNode.id] + buf; //(rNode.id > sNode.id ? (MatrixDimension3) buf.getTransposed() : buf);                   
                }
            }
            bigMatrix.printToFile("C:\\Data\\university\\master_1_year\\NM\\nm-lab-mag\\files\\resBlock.txt");  
        }

        private void createMatrKESegerlind(Element element, Matrix matr, Matrix matrixD)
        {
            StreamWriter sw = new StreamWriter("C:\\Data\\university\\master_1_year\\NM\\nm-lab-mag\\files\\log.txt", true);
            Matrix matrB = createMatrBSegerling(element);
            Matrix result = transpose(matrB) * matrixD * matrB * element.volume;
            matrB.printToFile("C:\\Data\\university\\master_1_year\\NM\\nm-lab-mag\\files\\matrB.txt");
            result.printToFile("C:\\Data\\university\\master_1_year\\NM\\nm-lab-mag\\files\\resSegerlindElement" + element.id + ".txt");

            for (int r = 0; r < 4; r++)
            {
                for (int s = 0; s < 4; s++)
                {
                    Node rNode = getNodeByIndex(element, r);
                    Node sNode = getNodeByIndex(element, s);

                    sw.WriteLine("Block [" + r + "," + s + "] of KE " + element.id + " will be added to the block [" + rNode.id + "," + sNode.id + "] of global matrix.");
                    
                    for (int rowInd = 0; rowInd < 3; rowInd++)
                    {
                        for (int colInd = 0; colInd < 3; colInd++)
                        {
                            matr[3 * rNode.id + rowInd, 3 * sNode.id + colInd] =
                                matr[3 * rNode.id + rowInd, 3 * sNode.id + colInd] + result[3*r + rowInd, 3*s + colInd];
                        }
                    }

                }
            }
            sw.Close();
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
            //OLD
            /*matrix[4, 1] = node.coefD;
            matrix[4, 2] = node.coefC;
            matrix[5, 0] = node.coefD;
            matrix[5, 2] = node.coefB;*/
            matrix[5, 1] = node.coefD;
            matrix[5, 2] = node.coefC;
            matrix[4, 0] = node.coefD;
            matrix[4, 2] = node.coefB;


            return matrix;
        }

        private Matrix createMatrBSegerling(Element elem)
        {
            /*
             *   b_i  0   0  
             *    0  c_i  0  
             *    0   0  d_i 
             *   c_i b_i  0  
             *   d_i  0  b_i 
             *    0  d_i c_i ...
             */
            Matrix matrix = new Matrix(6, 12);
            matrix[0, 0] = elem.nodeI.coefB;
            matrix[0, 3] = elem.nodeJ.coefB;
            matrix[0, 6] = elem.nodeK.coefB;
            matrix[0, 9] = elem.nodeP.coefB;


            matrix[1, 1] = elem.nodeI.coefC;
            matrix[1, 4] = elem.nodeJ.coefC;
            matrix[1, 7] = elem.nodeK.coefC;
            matrix[1, 10] = elem.nodeP.coefC;


            matrix[2, 2] = elem.nodeI.coefD;
            matrix[2, 5] = elem.nodeJ.coefD;
            matrix[2, 8] = elem.nodeK.coefD;
            matrix[2, 11] = elem.nodeP.coefD;


            matrix[3, 0] = elem.nodeI.coefC;
            matrix[3, 1] = elem.nodeI.coefB;
            matrix[3, 3] = elem.nodeJ.coefC;
            matrix[3, 4] = elem.nodeJ.coefB;
            matrix[3, 6] = elem.nodeK.coefC;
            matrix[3, 7] = elem.nodeK.coefB;
            matrix[3, 9] = elem.nodeP.coefC;
            matrix[3, 10] = elem.nodeP.coefB;


            matrix[4, 0] = elem.nodeI.coefD;
            matrix[4, 2] = elem.nodeI.coefB;
            matrix[4, 3] = elem.nodeJ.coefD;
            matrix[4, 5] = elem.nodeJ.coefB;
            matrix[4, 6] = elem.nodeK.coefD;
            matrix[4, 8] = elem.nodeK.coefB;
            matrix[4, 9] = elem.nodeP.coefD;
            matrix[4, 11] = elem.nodeP.coefB;


            matrix[5, 1] = elem.nodeI.coefD;
            matrix[5, 2] = elem.nodeI.coefC;
            matrix[5, 4] = elem.nodeJ.coefD;
            matrix[5, 5] = elem.nodeJ.coefC;
            matrix[5, 7] = elem.nodeK.coefD;
            matrix[5, 8] = elem.nodeK.coefC;
            matrix[5, 10] = elem.nodeP.coefD;
            matrix[5, 11] = elem.nodeP.coefC;

            matrix = matrix * (1/(6 * elem.volume));


            return matrix;
        }

    }

}
