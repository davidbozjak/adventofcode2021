using System.Diagnostics;
using System.Drawing;

[DebuggerDisplay("{this.ToString()}")]
class Space : IWorldObject
{
    public Point Position {get; init;}

    public char CharRepresentation => this.Player?.Name ?? '.';

    public int X => this.Position.X;

    public int Y => this.Position.Y;

    public int Z => 0;

    public Player? Player { get; init; }
    public bool IsOccupied => this.Player != null;

    public Space(int x, int y, Player? player)
    {
        this.Position = new Point(x, y);
        this.Player = player;
    }

    public Space Occpy(Player? player)
    {
        return new Space(this.X, this.Y, player);
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Space other)
        {
            return false;
        }

        if (this.Position != other.Position) return false;
        if (this.Player != other.Player) return false;

        return true;
    }

    public override int GetHashCode()
    {
        return this.Position.GetHashCode() * 
            (this.Player?.GetHashCode() ?? 1);
    }

    public override string ToString()
    => $"({X}, {Y}) {this.CharRepresentation}";
}
