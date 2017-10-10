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
        int[] Ship1 = new int[6] { -1, 0, 0, 0, 0, 0 };
        int[] Ship2 = new int[5] { -1, 0, 0, 0, 0 };
        int[] Ship3 = new int[4] { -1, 0, 0, 0 };
        int[] Ship4 = new int[4] { -1, 0, 0, 0 };
        int[] Ship5 = new int[3] { -1, 0, 0 };
        List<int[]> Ships = new List<int[]>(5);
        /*************
         * Brushes
         *************/
        static internal SolidBrush FontBrush;
        static internal Pen OuterPen;
       

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

        public Gameboard(string _owner,Color backColor, Panel initPanel)
        {
            this.owner = _owner;
            this.origin = new PointF();
            this.pieces = new List<GamePiece>(100);
            this.scale = initPanel.Width / 100;
            this.origin = initPanel.Location;
            this._newScale = initPanel.Width / 100;
            this._newOrigin = initPanel.Location;

            Ships.Add(Ship1);
            Ships.Add(Ship2);
            Ships.Add(Ship3);
            Ships.Add(Ship4);
            Ships.Add(Ship5);

            this.needsRedraw = true;
            this.backColor = backColor;
            FontBrush = new SolidBrush(Color.FromArgb(100,Color.Black));
            OuterPen = new Pen(backColor, 8);

            this.stateColors.Add(backColor);
            this.stateColors.Add(Color.FromArgb(150,Color.Black));
            this.stateColors.Add(Color.Gray);
            this.stateColors.Add(Color.DarkRed);
            this.stateColors.Add(Color.White);

            for (int i = 0; i < 101; i++)
            {
                pieces.Add(new GamePiece(i));
                pieces[i].drawState = 0;
                pieces[i].Ship = 0;
            }
            rectRedraw(this.origin, this.scale);

        }

        public void AddShipLocation(List<GamePiece> pieces)
        { 
            foreach (int[] S in Ships)
            {
                if (S[0] != -1)
                {
                    continue;
                } 
                for (int i = 0; i < pieces.Count(); i++)
                {
                    S[i+1] = pieces[i].idex;
                }
                S[0] = 1; //Ship is ready
                return;
            }
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
            public int Ship { get; set; }
            public int pieceDirection { get; set; }
            public GamePiece(int idex)
            {
                this.idex = idex;
            }



        }

        //DRAWING//

        private RectangleF OffsetRect(RectangleF rect, float px)
        {
            return new RectangleF(rect.X - px, rect.Y - px, rect.Width + (px*2), rect.Height + (px * 2));
        }

        public override void Draw(Graphics g)
        {
            rectRedraw(_newOrigin, _newScale);

            //Draw base color

            Color darker_BG = Color.FromArgb(backColor.A, (int)(backColor.R * .75F), (int)(backColor.G * .75F), (int)(backColor.B * .75F));
            LinearGradientBrush BoardGradient = new LinearGradientBrush(
                new PointF(currentArea.X, currentArea.Y),
                new PointF(currentArea.Right, currentArea.Y),
                backColor,
                darker_BG);

            g.FillRectangle(BoardGradient, OffsetRect(currentArea,1));

            //drawMainBox
            GraphicsPath outlinePath = new GraphicsPath();
            outlinePath.StartFigure();
                outlinePath.AddRectangle(OffsetRect(currentArea,4));

            OuterPen.LineJoin = LineJoin.Bevel;
            OuterPen.Alignment = PenAlignment.Outset;
            g.DrawPath(OuterPen, outlinePath);

            //Draw Specific states
            for (int i = 1; i <= 100; i++)
            {
                //DrawBoardDetails
                float offset = -(rectPieces[0].Width / 4);
                RectangleF innerElipse = OffsetRect(rectPieces[i - 1], offset);

                LinearGradientBrush innerBoardGradient = new LinearGradientBrush(
                    new PointF(innerElipse.X, innerElipse.Y),
                    new PointF(innerElipse.Right, innerElipse.Bottom),
                    darker_BG,
                    backColor);
                g.FillEllipse(innerBoardGradient, innerElipse);

                //draw indivibual rectangles
                if (GetGamePieceState(i) != 0)
                {
                    Color fillColor = this.GetGamePieceColor(i);

                    Color darker_fillColor = Color.FromArgb(fillColor.A, (int)(fillColor.R * .7F), (int)(fillColor.G * .7F), (int)(fillColor.B * .7F));
                    LinearGradientBrush OuterPeg = new LinearGradientBrush(
                        new PointF(rectPieces[i - 1].X, rectPieces[i - 1].Y),
                        new PointF(rectPieces[i - 1].Right, rectPieces[i - 1].Y),
                        fillColor,
                        darker_fillColor);

                    LinearGradientBrush InnerPeg = new LinearGradientBrush(
                        new PointF(rectPieces[i - 1].X, rectPieces[i - 1].Y),
                        new PointF(rectPieces[i - 1].Right, rectPieces[i - 1].Y),
                        darker_fillColor,
                        fillColor);

                    g.FillEllipse(OuterPeg, rectPieces[i - 1]);
                    g.FillEllipse(InnerPeg, innerElipse);

                } 

            }
            

            //drawLabels
            for (int i = 1; i <= 10; i++)
            {
                g.DrawString(H_Labels[i - 1], new Font("Consolas", scale * 4,FontStyle.Bold), FontBrush, new PointF(rectPieces[i - 1].X + (scale*(6F/3F)), currentArea.Y - (scale*8.3F)));
                g.DrawString(V_Labels[i - 1], new Font("Consolas", scale * 4, FontStyle.Bold), FontBrush, new PointF(currentArea.X - (scale * 8F), rectPieces[(i - 1) * 10].Y + (scale * (6F / 3F))));
            }
            

            //Update Scale and Origin
            this.scale = _newScale;
            this.origin = _newOrigin;
            this.needsRedraw = false;
        }

        public void DrawShips(Graphics G, Panel P)
        {
            RectangleF ShipContainer = new RectangleF(P.Location, new SizeF(P.Width, P.Height));
            Color C = Color.FromArgb(100, Color.WhiteSmoke);
            SolidBrush S = new SolidBrush(backColor);
            G.FillRectangle(S, ShipContainer);
            List<RectangleF> pegList = new List<RectangleF>();
            Point offset = new Point(14,3);

            int drawOrder = 0;

            foreach (int[] Ship in Ships)
            {
                for (int i = 1; i < Ship.Length; i++)
                {
                    pegList.Add(
                        new RectangleF(
                            new Point(P.Location.X + offset.X,P.Location.Y+offset.Y), 
                            new SizeF(25, 25)));

                    /**************
                     * Draw Elipses
                     **************/
                    Color fillColor = this.GetGamePieceColor(Ship[i]);
                    Color darker_fillColor = Color.FromArgb(fillColor.A, (int)(fillColor.R * .7F), (int)(fillColor.G * .7F), (int)(fillColor.B * .7F));
                    LinearGradientBrush OuterPeg = new LinearGradientBrush(
                        new PointF(pegList[i - 1].X, pegList[i - 1].Y),
                        new PointF(pegList[i - 1].Right, pegList[i - 1].Y),
                        fillColor,
                        darker_fillColor);
                    G.FillEllipse(OuterPeg, pegList[drawOrder]);
                    G.DrawEllipse(new Pen(Color.Black), pegList[drawOrder]);
                    drawOrder++;

                    offset.X += 25;
                }
                offset.X = 14;
                offset.Y += 30;
            }

            //G.DrawRectangles(new Pen(Color.Black), pegList.ToArray());



        }
        
    }
}
