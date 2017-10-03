using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Battleship
{
    public class Gameboard : GameArt
    {

        public string owner;
        private PointF _origin;
        private float _scale;
        private PointF _newOrigin { get; set; }
        private float _newScale { get; set; }
        SolidBrush BackgroundBrush;
        SolidBrush TileBrush;
        SolidBrush FontBrush;
        Pen outline;

        public List<GamePiece> pieces;
        public RectangleF[] rectPieces = new RectangleF[100];
        public PointF origin
        {
            get
            {
                return this._origin;
            }
            set
            {
                this.currentArea.Location = value;
                this._origin = value;
                this.needsRedraw = true;
            }
        }

        public float scale
        {
         get
            {
                return this._scale;
            }
         set
            {
                this.currentArea.Width = value * 100;
                this.currentArea.Height = value * 100;
                this._scale = value;
                this.needsRedraw = true;
            }
        }
        public Color backColor { get; set; }
        public List<Color> stateColors = new List<Color>();

        public string[] H_Labels = new string[10]
        { "1","2","3","4","5","6","7","8","9","10" };
        public string[] V_Labels = new string[10]
        { "A","B","C","D","E","F","G","H","I","J",};

        public Gameboard(string _owner,Color backColor)
        {
            this.owner = _owner;
            this.origin = new PointF();
            this.pieces = new List<GamePiece>(100);
            this.scale = 3;
            this.origin = new PointF(20,20);
            this.needsRedraw = true;
            this.backColor = backColor;
            BackgroundBrush = new SolidBrush(this.backColor);
            TileBrush = new SolidBrush(this.backColor);
            FontBrush = new SolidBrush(Color.White);
            outline = new Pen(Color.FromArgb(255, 30, 30, 30), 2);

            this.stateColors.Add(backColor);
            this.stateColors.Add(Color.FromArgb(150,Color.Black));
            this.stateColors.Add(Color.Gray);
            this.stateColors.Add(Color.DarkRed);
            this.stateColors.Add(Color.White);

            for (int i = 0; i < 101; i++)
            {
                pieces.Add(new GamePiece(i));
                pieces[i].drawState = 0;
                pieces[i].boardPiece = 0;
            }
            rectRedraw(this.origin, this.scale);

        }


        public void MoveTo(float X, float Y, float newScale)//will move the board on next redraw
        {
            this._newOrigin = new PointF(X, Y);
            this._newScale = newScale;
            this.needsRedraw = true;
        }
        public void MoveTo(PointF point, float newScale)//will move the board on next redraw
        {
            this._newOrigin = point;
            this._newScale = newScale;
            this.needsRedraw = true;
        }

        public GamePiece GetPieceAtIndex(int index)
        {
            return pieces[index];
        }

        public Color GetGamePieceColor(int index)
        {
            return stateColors[GetPieceAtIndex(index).drawState];
        }

        public int GetGamePieceState(int index)
        {
            return GetPieceAtIndex(index).drawState;
        }

        public void SetGamePieceState(List<GamePiece> GPList, int state)
        {
            foreach (GamePiece GP in GPList)
            {
                GP.drawState = state;
            }
        }


        public void SetGamePieceState(GamePiece GP, int state)
        {
            GP.drawState = state;
        }

        public List<GamePiece> Get_AbovePieces(GamePiece G, int dist)
        {
            List<GamePiece> retList = new List<GamePiece>(10);
            retList.Add(G);
            if ((G.idex - (10 * dist)) < 1)
            {
                return null;
            }
            else
            {
                for (int i = 1; i <= dist; i++)
                {
                    retList.Add(pieces[G.idex - (i * 10)]);
                }
            }
            return retList;
        }

        public List<GamePiece> Get_BelowPieces(GamePiece G, int dist)
        {
            List<GamePiece> retList = new List<GamePiece>(10);
            retList.Add(G);
            if ((G.idex + (10 * dist)) > 100)
            {
                return null;
            }
            else
            {
                for (int i = 1; i <= dist; i++)
                {
                    retList.Add(pieces[G.idex + (i * 10)]);
                }
            }
            return retList;
        }

        public List<GamePiece> Get_LeftPieces(GamePiece G, int dist)
        {
            List<GamePiece> retList = new List<GamePiece>(10);
            retList.Add(G);
            for (int i = G.idex - 1; i >= G.idex - dist; i--)
            {
                if (i % 10 == 0)
                {
                    return null;
                }
                retList.Add(pieces[i]);
            }
            return retList;
        }

        public List<GamePiece> Get_RightPieces(GamePiece G, int dist)
        {
            List<GamePiece> retList = new List<GamePiece>(10);
            retList.Add(G);
            for (int i = G.idex + 1; i <= G.idex + dist; i++)
            {
                if ((i-1) % 10 == 0)
                {
                    return null;
                }
                retList.Add(pieces[i]);
            }
            return retList;
        }

        public void rectRedraw(PointF Orgin, float Scale)
        {
            for (int i = 1; i <= 100; i++)
            {

                int newSize = (int)(10 * Scale);
                int newBoardSize = (int)(100 * Scale);

                PointF position = new PointF(((i - 1) % 10) * newSize, (newSize * ((i - 1) / 10)));
                SizeF offset = new SizeF(position);
                PointF newPoint = PointF.Add(Orgin, offset);
                RectangleF square = new RectangleF(newPoint, new SizeF(newSize, newSize));
                rectPieces[i - 1] = square;
            }
        }

        public class GamePiece
        {
            public int idex { get; }
            public int drawState { get; set; }
            public int boardPiece { get; set; }
            public int pieceDirection { get; set; }
            public GamePiece(int idex)
            {
                this.idex = idex;
            }



        }

        //DRAWING//
        public override void Draw(Graphics g)
        {
            rectRedraw(_newOrigin, _newScale);
            //set clipping region to bounds
            //g.Clip = r;

            //Draw base color
            g.FillRectangle(BackgroundBrush, currentArea);
            //Draw Specific states
            for (int i = 1; i <= 100; i++)
            {
                //draw indivibual rectangles
                if (GetGamePieceState(i) != 0)
                {
                    Color fillColor = this.GetGamePieceColor(i);
                    TileBrush.Color = fillColor;
                    g.FillRectangle(TileBrush, rectPieces[i - 1]);
                }

            }
            //drawMainBox
            g.DrawRectangle(outline, Rectangle.Round(currentArea));
            //drawGrid
            for (int i = 1; i <= 10; i++)
            {
                g.DrawLine(outline, rectPieces[i - 1].Location, new PointF(rectPieces[i - 1].X, (rectPieces[i - 1].Y + currentArea.Width)));
                g.DrawLine(outline, rectPieces[(i - 1)*10].Location, new PointF((rectPieces[(i - 1) * 10].X + currentArea.Width), rectPieces[(i - 1) * 10].Y));
                g.DrawString(H_Labels[i - 1], new Font("Consolas", scale * 4,FontStyle.Bold), FontBrush, new PointF(rectPieces[i - 1].X + (scale*(5F/3F)), currentArea.Y - (scale*7)));
                g.DrawString(V_Labels[i - 1], new Font("Consolas", scale * 4, FontStyle.Bold), FontBrush, new PointF(currentArea.X - (scale * 7), rectPieces[(i - 1) * 10].Y + (scale * (5F / 3F))));
            }

            //Update Scale and Origin
            this.scale = _newScale;
            this.origin = _newOrigin;
            this.needsRedraw = false;
        }
        
    }
}
