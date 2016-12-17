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
        private GeneralMatrixBuilder _generalMatrixBuilder = new GeneralMatrixBuilder();

        public SolutionManager() { }

        public IList<IList<Vector3D>> buildSolution(InputData inputData)
        {
            IList<IList<Vector3D>> solutions = new List<IList<Vector3D>>();

            ElementsMap elementsRegistry = new ElementsMap(inputData, null);
            SymmetricMatrix<MatrixDimension3> generalMatrix = _generalMatrixBuilder.build(elementsRegistry);

            double weightCoef = 0;
            double delta = 1 / (double)inputData.iterationsCount;
            while (weightCoef < 1) {
                weightCoef += delta;
                solutions.Add(solveSystem(generalMatrix, elementsRegistry, weightCoef));
            }

            return solutions;
        }

        private IList<Vector3D> solveSystem(SymmetricMatrix<MatrixDimension3> generalMatrix, ElementsMap elementsRegistry, double weightCoef)
        {
            IList<Vector3D> rightSide = new List<Vector3D>();
            foreach (double proportionCoef in elementsRegistry.nodeProportions)
            {
                rightSide.Add(new Vector3D(0, 0, proportionCoef * weightCoef));
            }

            return null;
        }

        /*
         * Применение граничных условий.
         * globalMatrix - глобальная матрица [K]
         * rightSide - правая часть
         * boundaryConditions - массив номеров граничных узлов (нумерация начинается с 0!!!!!)
         */
        public void applyBoundaryConditions(SymmetricMatrix<MatrixDimension3> globalMatrix, IList<Vector3D> rightSide, int[] boundaryConditions)
        {
            int dimension = globalMatrix.Dimension;
            int bandWidth = globalMatrix.getBandWidth();
            int boundaryCount = boundaryConditions.Length;
            MatrixDimension3 neitralMatrix = new MatrixDimension3();
            Vector3D defaultVector = new Vector3D();
            
            for (int boundary = 0; boundary < boundaryCount; boundary++)
            {
                int boundaryInd = boundaryConditions[boundary];
                int leftBound = boundaryInd < globalMatrix.getBandWidth() ? 0 : boundaryInd - globalMatrix.getBandWidth() + 1;
                int rightBound = boundaryInd + globalMatrix.getBandWidth() < globalMatrix.Dimension ? boundaryInd + globalMatrix.getBandWidth() : globalMatrix.Dimension;

                    for (int columnInd = leftBound; columnInd < rightBound; columnInd++)
                    {
                        if (!boundaryInd.Equals(columnInd))
                            globalMatrix.setElement(boundaryInd, columnInd, neitralMatrix);

                        globalMatrix[boundaryInd, boundaryInd][0, 1] = 0;
                        globalMatrix[boundaryInd, boundaryInd][0, 2] = 0;
                        globalMatrix[boundaryInd, boundaryInd][1, 2] = 0;
                        globalMatrix[boundaryInd, boundaryInd][1, 0] = 0;
                        globalMatrix[boundaryInd, boundaryInd][2, 0] = 0;
                        globalMatrix[boundaryInd, boundaryInd][2, 1] = 0;

                        rightSide[boundaryInd] = defaultVector;
                    }
            }
            
        }
    }
}
