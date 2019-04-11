using System.Collections.Generic;

namespace Interfaces
{
    interface IHandler
    {
        List<IFrame> draw();
        void handle();
    }
}
