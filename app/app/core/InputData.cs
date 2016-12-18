using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app.core
{
    /*
     * Входные данные задачи
     */
    public class InputData
    {
        // Шаги по осям ОХ, OY, OZ
        public double hx { get; private set; }
        public double hy { get; private set; }
        public double hz { get; private set; }

        // Число шагов по осям OX, OY, OZ
        public int Nx { get; private set; }
        public int Ny { get; private set; }
        public int Nz { get; private set; }

        // Модуль Юнга
        public double elasticityModulus { get; private set; }

        // Коэффициент Пуассона
        public double poissonRatio { get; private set; }

        // Плотность
        public double density { get; private set; }

        // Запрашиваемое количество итераций, для которых будет расчитано решение
        public int iterationsCount { get; private set; }

        // Список номеров точек, являющихся граничными
        public int[] boundaryConditions { get; private set; }
       

        public InputData(
            double _hx, 
            double _hy, 
            double _hz, 
            int _Nx, 
            int _Ny, 
            int _Nz, 
            double _elasticityModulus, 
            double _poissonRatio, 
            double _density, 
            int _iterationsCount, 
            int[] _boundaryConditions
        )
        {
            hx = _hx;
            hy = _hy;
            hz = _hz;
            Nx = _Nx;
            Ny = _Ny;
            Nz = _Nz;
            elasticityModulus = _elasticityModulus;
            poissonRatio = _poissonRatio;
            density = _density;
            iterationsCount = _iterationsCount;
            boundaryConditions = _boundaryConditions;
        }
    }

}
