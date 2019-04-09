using System.Drawing;
using System.Numerics;

namespace Interfaces
{
    interface IFrame
    {
        // положение относительно центра сцены
        Vector2 position { get; set; }
        
        // положение на экране
        Vector2 screenposition {get;set;}

        // размер
        Vector2 size { get; set; }

        Image image { get; set; }

        Vector2 draw();
    }
}
