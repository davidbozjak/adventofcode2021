using System.Drawing;

class PaperPoint : IWorldObject
{
    public Point Position { get; init; }

    public int X => this.Position.X;
    public int Y => this.Position.Y;
    public int Z => 0;

    public char CharRepresentation => '#';

    public PaperPoint(int x, int y)
    {
        this.Position = new Point(x, y);
    }
}
