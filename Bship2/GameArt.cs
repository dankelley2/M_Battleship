using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Battleship
{
    public abstract class GameArt
    {

        public static List<GameArt> Drawable = new List<GameArt>();
        public abstract void Draw(Graphics g);
        public bool needsRedraw;
        public RectangleF currentArea;
        public GameArt()
        {
            Drawable.Add(this);
        }
    }
    
}
