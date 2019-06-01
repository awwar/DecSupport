using Models;
using System;

namespace Interfaces
{
    internal interface IHandler <T>
    {
        Action Redraw { get; set; }
        T[] Handle();
    }
}
