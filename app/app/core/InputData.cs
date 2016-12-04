using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app.core
{
    public class InputData
    {
        public double hx { get; set; }
        public double hy { get; set; }
        public double hz { get; set; }
        public double elasticityModulus { get; set; }
        public double poissonRatio { get; set; }
        public double density { get; set; }
        public double iterationsCount { get; set; }
        public int[] boundaryConditions { get; set; }
        public int Nx { get; set; }
        public int Ny { get; set; }
        public int Nz { get; set; }

        public InputData(double _hx, double _hy, double _hz, int _Nx, int _Ny, int _Nz, double _elasticityModulus, double _poissonRatio, double _density, double _iterationsCount, int[] _boundaryConditions)
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
