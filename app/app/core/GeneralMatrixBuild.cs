using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app.core
{
    class GeneralMatrixBuild
    {
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

            for(int i = 0; i < 4; i++)
            {
                for(int j = i; j < 4; j++)
                {
                    //как будто Сережа уже сделал и я такой оп и сделал
                    MatrixDimension3 matrix = new MatrixDimension3();
                    
                    int globali = ElementsMap.ElementDecomposition[element.id, i];
                    int globalj = ElementsMap.ElementDecomposition[element.id, j];
                    matr[globali, globalj] = matr[globali, globalj] + matrix;
                }
            }

        }

    }

}
