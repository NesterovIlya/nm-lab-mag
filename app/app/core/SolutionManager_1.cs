using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace app.core
{
    public class SolutionManager
    {
        /*
        globalMatrix - глобальная матрица [K]
        rightSide - правая часть
        boundaryConditions - массив номеров граничных узлов
        */

        public SolutionManager() { }

        public void applyBoundaryConditions(SymmetricMatrix<MatrixDimension3> globalMatrix, List<Vector3D> rightSide, int[] boundaryConditions)
        {
            int dimension = globalMatrix.Dimension;
            int boundaryCount = boundaryConditions.Length;
            MatrixDimension3 neitralMatrix = new MatrixDimension3();
            Vector3D defaultVector = new Vector3D();
            int boundaryInd;
            for (int boundary = 0; boundary < boundaryCount; boundary++)
            {
                for (int column = 0; column < dimension; column++)
                {
                    boundaryInd = boundaryConditions[boundary];
                    if (!boundaryInd.Equals(column))
                        globalMatrix.setElement(boundaryInd, column, neitralMatrix);
                    else
                    {
                        globalMatrix[boundaryInd, boundaryInd][0, 1] = 0;
                        globalMatrix[boundaryInd, boundaryInd][0, 2] = 0;
                        globalMatrix[boundaryInd, boundaryInd][1, 2] = 0;
                        globalMatrix[boundaryInd, boundaryInd][1, 0] = 0;
                        globalMatrix[boundaryInd, boundaryInd][2, 0] = 0;
                        globalMatrix[boundaryInd, boundaryInd][2, 1] = 0;
                    }
                    rightSide[boundaryInd] = defaultVector;
                }
            }
        }
    }
}
