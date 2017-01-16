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

        private CholeskySolver _choleskySolver = new CholeskySolver();

        public SolutionManager() { }

        public IList<IList<Vector3D>> buildSolution(InputData inputData)
        {
            // Список перемещений на каждой итерации. Внутренний список - перемещения каждого узла внутри итерации.
            IList<IList<Vector3D>> solutions = new List<IList<Vector3D>>();

            // Весовой коэффициент (для итеративного процесса)
            double weightCoef = 0; 

            // Последнее перемещения узлов
            IList<Vector3D> lastTransition = new List<Vector3D>();

            // Формируем реестр элементов
            ElementsMap elementsRegistry = new ElementsMap(inputData);

            for (int iterIndex = 0; iterIndex < inputData.iterationsCount; iterIndex++)
            {
                weightCoef += 1 / (double)inputData.iterationsCount;
                
                // Генерируем правую часть
                IList<Vector3D> rightSide = new List<Vector3D>();

                foreach (double proportionCoef in elementsRegistry.nodeProportions)
                {
                    rightSide.Add(new Vector3D(0, 0, -proportionCoef * weightCoef));
                }

                // Формируем обобщенную матрицу
                //SymmetricMatrix<MatrixDimension3> generalMatrix = _generalMatrixBuilder.build(elementsRegistry);
                SymmetricMatrix<DoubleContainerElement> generalMatrix = _generalMatrixBuilder.build(elementsRegistry);

                // Применяем граничные условия
                applyBoundaryConditions(generalMatrix, rightSide, inputData.boundaryConditions);

                // Решаем систему
                IList<Vector3D> solution = _choleskySolver.solve(generalMatrix, rightSide);

                // Считаем перемещение на текущей итерации и добавляем в результат
                if (lastTransition.Count > 0)
                {
                    if (lastTransition.Count != solution.Count)
                    {
                        throw new ArgumentException("Size of lastTransition and solution list must be the same!");
                    }
                    List<Vector3D> buffer = new List<Vector3D>();
                    for (int nodeIndex = 0; nodeIndex < lastTransition.Count; nodeIndex++)
                    {
                        buffer.Add(solution[nodeIndex] - lastTransition[nodeIndex]);
                    }
                    solutions.Add(buffer);
                } else
                {
                    solutions.Add(solution);
                }

                lastTransition = solution;

            }


            return solutions;
        }

        /*
         * Применение граничных условий.
         * globalMatrix - глобальная матрица [K]
         * rightSide - правая часть
         * boundaryConditions - массив номеров граничных узлов (нумерация начинается с 0!!!!!)
         */
        private void applyBoundaryConditions(SymmetricMatrix<DoubleContainerElement> globalMatrix, IList<Vector3D> rightSide, int[] boundaryConditions)
        {
            int dimension = globalMatrix.Dimension;
            int bandWidth = globalMatrix.getBandWidth();
            int boundaryCount = boundaryConditions.Length;
            //MatrixDimension3 neitralMatrix = new MatrixDimension3();
            DoubleContainerElement neitralMatrix = new DoubleContainerElement();
            Vector3D defaultVector = new Vector3D();
            
            for (int boundary = 0; boundary < boundaryCount; boundary++)
            {

                for (int delta = 0; delta < 3; delta++)
                {
                    int boundaryInd = boundaryConditions[boundary] * 3 + delta;
                    int leftBound = boundaryInd < globalMatrix.getBandWidth() ? 0 : boundaryInd - globalMatrix.getBandWidth() + 1;
                    int rightBound = boundaryInd + globalMatrix.getBandWidth() < globalMatrix.Dimension ? boundaryInd + globalMatrix.getBandWidth() : globalMatrix.Dimension;

                    for (int columnInd = leftBound; columnInd < rightBound; columnInd++)
                    {
                        if (!(boundaryInd).Equals(columnInd))
                        {
                            globalMatrix.setElement(boundaryInd, columnInd, neitralMatrix);
                        }
                    }

                    /*globalMatrix[boundaryInd, boundaryInd][0, 1] = 0;
                    globalMatrix[boundaryInd, boundaryInd][0, 2] = 0;
                    globalMatrix[boundaryInd, boundaryInd][1, 2] = 0;
                    globalMatrix[boundaryInd, boundaryInd][1, 0] = 0;
                    globalMatrix[boundaryInd, boundaryInd][2, 0] = 0;
                    globalMatrix[boundaryInd, boundaryInd][2, 1] = 0;*/

                    if (delta == 0)
                    {
                        rightSide[boundaryConditions[boundary]] = defaultVector;
                    }

                }

            }
            
        }
    }
}
