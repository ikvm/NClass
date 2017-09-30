﻿// // NClass - Free class diagram editor
// // Copyright (C) 2016 Georgi Baychev
// // 
// // This program is free software; you can redistribute it and/or modify it under 
// // the terms of the GNU General Public License as published by the Free Software 
// // Foundation; either version 3 of the License, or (at your option) any later version.
// // 
// // This program is distributed in the hope that it will be useful, but WITHOUT 
// // ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS 
// // FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// //
// // You should have received a copy of the GNU General Public License along with 
// // this program; if not, write to the Free Software Foundation, Inc., 
// // 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using NClass.Core;
using NClass.DiagramEditor.Diagrams;
using NClass.DiagramEditor.Diagrams.Shapes;

namespace NClass.DiagramEditor.UseCaseDiagram.Shapes
{
    public class ActorShape : Shape
    {
        private Actor actor;
        private const int DefaultWidth = 75;
        private const int DefaultHeight = 150;
        private const int PaddingSize = 5;
        private readonly Size defaultSize = new Size(DefaultWidth, DefaultHeight);
        private const int Proportion = 2; // Height / Width


        public ActorShape(Actor entity) : base(entity)
        {
            actor = entity;
            this.MinimumSize = defaultSize;
        }

        public override void Draw(IGraphics g, bool onScreen, Style style)
        {
            DrawSurface(g, onScreen, style);
            DrawText(g, onScreen, style);
        }

        private void DrawText(IGraphics graphics, bool onScreen, Style style)
        {
            var stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.Trimming = StringTrimming.EllipsisCharacter;
            //graphics.DrawRectangle(new Pen(Color.Black), new Rectangle(this.Left, this.Top, this.Width, this.Height) );
            //graphics.DrawRectangle(new Pen(Color.Black), GetTextRectangle(graphics, style) );
            graphics.DrawString(
                                Entity.Name,
                                style.StaticMemberFont,
                                new SolidBrush(style.ActorTextColor),
                                GetTextRectangle(graphics, style),
                                stringFormat);
        }

        private void DrawSurface(IGraphics graphics, bool onScreen, Style style)
        {
            var actorPen = new Pen(style.ActorColor);
            var headSize = this.Width / 2 - PaddingSize;
            var headRectangle = new Rectangle(this.Left + PaddingSize + headSize / 2, this.Top + PaddingSize, headSize, headSize);
            // directly beyond the head
            var bodyStart = new Point(headRectangle.X + headRectangle.Width / 2, headRectangle.Bottom);
            // 2/3 of the length;
            var bodyEnd = new Point(headRectangle.X + headRectangle.Width / 2, headRectangle.Bottom + (this.Height + PaddingSize * 2) / 3);
            var leftLeg = new Point(bodyEnd.X - headRectangle.Width / 3, bodyEnd.Y + ((bodyEnd.Y - bodyStart.Y) / 2));
            var rightLeg = new Point(bodyEnd.X + headRectangle.Width / 3, bodyEnd.Y + ((bodyEnd.Y - bodyStart.Y) / 2));

            var handsStart = new Point(headRectangle.X - headRectangle.Width / 6, headRectangle.Bottom + ((bodyEnd.Y - bodyStart.Y) / 10));
            var handsEnd = new Point(headRectangle.X + headRectangle.Width + headRectangle.Width / 6, headRectangle.Bottom + ((bodyEnd.Y - bodyStart.Y) / 10));

            // head
            graphics.DrawEllipse(actorPen,
                                 headRectangle.X,
                                 headRectangle.Y,
                                 headRectangle.Width,
                                 headRectangle.Height);
            // body
            graphics.DrawLine(actorPen, bodyStart, bodyEnd);
            // legs
            graphics.DrawLine(actorPen, bodyEnd, leftLeg);
            graphics.DrawLine(actorPen, bodyEnd, rightLeg);
            // hands
            graphics.DrawLine(actorPen, handsStart, handsEnd);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            var change = e.Change;
            Debug.WriteLine(change);
            if (Math.Abs(change.Width) > Math.Abs(change.Height))
            {
                this.size.Height = Proportion * this.Width;
            }
            else
            {
                this.size.Width = this.Height / Proportion;
            }
            base.OnResize(e);
        }

       protected override Size DefaultSize
        {
            get { return defaultSize; }
        }
        public override IEntity Entity {
            get
            {
                return this.actor;
            }
        }
        protected override int GetBorderWidth(Style style)
        {
            //FIXME
            return style.CommentBorderWidth;
        }

        protected override float GetRequiredWidth(Graphics g, Style style)
        {
            return Width;
        }

        private Rectangle GetTextRectangle(IGraphics g, Style style)
        {
            SizeF textSize = g.MeasureString(this.Entity.Name, style.ActorFont);
            int left = this.Left + PaddingSize;
            int top = (int)Math.Ceiling(this.Bottom - textSize.Height - PaddingSize);
            int width = this.Width - 2 * PaddingSize;
            int height = (int)Math.Ceiling(textSize.Height + PaddingSize);
            return new Rectangle(left, top, width, height);
        }

        protected override bool CloneEntity(IDiagram diagram)
        {
            var useCaseDiagram = diagram as UseCaseDiagram;
            if (useCaseDiagram == null)
                return false;

            return useCaseDiagram.InsertActor(actor.Clone());
        }

        public static Rectangle GetOutline(Style style)
        {
            return new Rectangle(0, 0, DefaultWidth, DefaultHeight);
        }
    }
}