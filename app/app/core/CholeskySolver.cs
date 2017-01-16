using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace app.core
{
    public class CholeskySolver
    {
        public CholeskySolver() { }

        public IList<Vector3D> solve(SymmetricMatrix<DoubleContainerElement> globalMatrix, IList<Vector3D> rightSide)
        {
            SymmetricMatrix<DoubleContainerElement> extarctGlobalMatrix = globalMatrix;//extractMatrix(globalMatrix);

            //Создание и заполнение матриц B и C
            SymmetricMatrix<DoubleContainerElement> BMatrix = new SymmetricMatrix<DoubleContainerElement>(extarctGlobalMatrix.Dimension, new DoubleContainerElement());
            SymmetricMatrix<DoubleContainerElement> CMatrix = new SymmetricMatrix<DoubleContainerElement>(extarctGlobalMatrix.Dimension, new DoubleContainerElement());

            createBCMatrix(BMatrix, CMatrix, extarctGlobalMatrix);

            //Решаем систему BMatrix solutionBMatrixList = rightVector
            IList<Double> rightVector = extractVector(rightSide);
            IList<Double> solutionBMatrixList = solveWithBMatrix(BMatrix, rightVector);

            //Решаем систему CMatrix solutionList = solutionBMatrixList
            IList<Double> solutionList = solveWithCMatrix(CMatrix, solutionBMatrixList);

            //Конвертируем список double в список векторов
            IList<Vector3D> solution = extractFromVector(solutionList);

            return solution;
        }

        private void createBCMatrix(SymmetricMatrix<DoubleContainerElement> BMatrix, SymmetricMatrix<DoubleContainerElement> CMatrix, SymmetricMatrix<DoubleContainerElement> globalMatrix)
        {
            int rightBound = globalMatrix.getBandWidth() < globalMatrix.Dimension ? globalMatrix.getBandWidth() : globalMatrix.Dimension;
            for (int rowInd = 0; rowInd < rightBound; rowInd++)
            {
                BMatrix[rowInd, 0] = globalMatrix[rowInd, 0];
                CMatrix[0, rowInd] = BMatrix[rowInd, 0] / BMatrix[0, 0];
            }

            for (int colInd = 1; colInd < globalMatrix.Dimension; colInd++)
            {
                rightBound = colInd + globalMatrix.getBandWidth() < globalMatrix.Dimension ? colInd + globalMatrix.getBandWidth() : globalMatrix.Dimension;
                for (int rowInd = colInd; rowInd < rightBound; rowInd++)
                {
                    DoubleContainerElement sum = new DoubleContainerElement(0);
                    for (int countInd = 0; countInd < colInd; countInd++)
                    {
                        sum = sum + (BMatrix[rowInd, countInd] * CMatrix[countInd, colInd]);
                    }
                    BMatrix[rowInd, colInd] = globalMatrix[rowInd, colInd] - sum;
                    if (BMatrix[colInd, colInd] != new DoubleContainerElement(0))
                        CMatrix[colInd, rowInd] = BMatrix[rowInd, colInd] / BMatrix[colInd, colInd];
                    else
                    {
                        Console.Write("Division to 0 error!");
                        break;
                    }

                }
            }
			
        }

        private IList<Double> solveWithCMatrix(SymmetricMatrix<DoubleContainerElement> CMatrix, IList<Double> rightSide)
        {
            int dimension = CMatrix.Dimension;
            IList<Double> solution = new List<Double>();

            for (int index = 0; index < dimension; index++)
                solution.Insert(index, 0);

            if (CMatrix[dimension - 1, dimension - 1].element != 0.0)
                solution[dimension - 1] = rightSide[dimension - 1] / CMatrix[dimension - 1, dimension - 1].element;
            else return null;

            for (int rowInd = dimension - 2; rowInd >= 0; rowInd--)
            {
                int rightBound = rowInd + CMatrix.getBandWidth() < CMatrix.Dimension ? rowInd + CMatrix.getBandWidth() : CMatrix.Dimension;
                double sum = rightSide[rowInd];

                for (int colInd = rightBound - 1; colInd > rowInd; colInd--)
                {
                    sum = sum - solution[colInd] * CMatrix[rowInd, colInd].element;
                }

                if (CMatrix[rowInd, rowInd].element != 0)
                    solution[rowInd] = sum / CMatrix[rowInd, rowInd].element;
                else return null;
            }

            return solution;
        }


        private IList<Double> solveWithBMatrix(SymmetricMatrix<DoubleContainerElement> BMatrix, IList<Double> rightSide)
		{
			IList<Double> solution = new List<Double>();

            if (BMatrix[0, 0].element != 0.0)
                solution.Insert(0, rightSide[0] / BMatrix[0, 0].element);
            else return null;

            for (int rowInd = 1; rowInd < BMatrix.Dimension; rowInd++)
            {
                int leftBound = rowInd < BMatrix.getBandWidth() ? 0 : rowInd - BMatrix.getBandWidth() + 1;
                double sum = rightSide[rowInd];

                for (int colInd = leftBound; colInd < rowInd; colInd++)
                {
                    sum = sum - solution[colInd] * BMatrix[rowInd, colInd].element;
                }

                if (BMatrix[rowInd, rowInd].element != 0)
                    solution.Insert(rowInd, sum / BMatrix[rowInd, rowInd].element);
                else return null;
            }

            return solution;
		}

        private SymmetricMatrix<DoubleContainerElement> extractMatrix(SymmetricMatrix<MatrixDimension3> sourceMatrix)
        {
            int dimension = 3 * sourceMatrix.Dimension;
            SymmetricMatrix<DoubleContainerElement> result = new SymmetricMatrix<DoubleContainerElement>(dimension, new DoubleContainerElement());

            for (int sourceRowInd = 0; sourceRowInd < sourceMatrix.Dimension; sourceRowInd++)
            {
                int rightBound = sourceRowInd + sourceMatrix.getBandWidth() < sourceMatrix.Dimension ? sourceRowInd + sourceMatrix.getBandWidth() : sourceMatrix.Dimension;
                for (int sourceColInd = sourceRowInd; sourceColInd < rightBound; sourceColInd++)
                {
                    // Обрабатываем соответствующий блок
                    MatrixDimension3 block = sourceMatrix[sourceRowInd, sourceColInd];
                    
                    for (int blockRowInd = 0; blockRowInd < 3; blockRowInd++)
                    {
                        for (int blockColInd = sourceRowInd == sourceColInd ? blockRowInd : 0; blockColInd < 3; blockColInd++)
                        {
                            result[sourceRowInd * 3 + blockRowInd, sourceColInd * 3 + blockColInd] = new DoubleContainerElement(block[blockRowInd, blockColInd]);
                        }
                    }
                }
            }

            return result;
        }

        private IList<Double> extractVector(IList<Vector3D> sourceList)
        {
            IList<Double> result = new List<Double>();
            for (int count = 0; count < sourceList.Count; count++)
            {
                result.Insert(3 * count, sourceList[count].X);
                result.Insert(3 * count + 1, sourceList[count].Y);
                result.Insert(3 * count + 2, sourceList[count].Z);
            }

            return result;
        }

        private IList<Vector3D> extractFromVector(IList<Double> sourceList)
        {
            IList<Vector3D> result = new List<Vector3D>();

            for (int count = 0; count < sourceList.Count / 3; count++) 
            {
                Vector3D vector = new Vector3D();
                vector.X = sourceList[3 * count];
                vector.Y = sourceList[3 * count + 1];
                vector.Z = sourceList[3 * count + 2];

                result.Insert(count, vector);
            }

            return result;
        }

    }
}
