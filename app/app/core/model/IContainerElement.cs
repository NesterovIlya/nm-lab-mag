using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app.core.model
{
    interface IContainerElement
    {
        void transpose();
        bool isNeutralElement();
        IContainerElement getNeutralElememt();
    }
}
