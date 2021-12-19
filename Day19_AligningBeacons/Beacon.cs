using System.Diagnostics;
using System.Drawing;

[DebuggerDisplay("{this.ToString()}")]
class Beacon : IWorldObject
{
    public Point Position { get; init; }

    public char CharRepresentation => 'B';

    public int X => this.Position.X;

    public int Y => this.Position.Y;

    public int Z { get; init; }

    public Beacon(int x, int y, int z)
    {
        this.Position = new Point(x, y);
        this.Z = z;

        this.allOrientations = new Cached<Beacon[]>(() => this.GetInAllOrientations().ToArray());
    }

    public Beacon GetInOrientation(int orientation)
    {
        if (orientation < 0) throw new Exception();
        else if (orientation > 23) throw new Exception();

        return this.allOrientations.Value[orientation];
    }

    public Beacon Transform(int x, int y, int z)
    {
        return new Beacon(this.X + x, this.Y + y, this.Z + z);
    }

    public (int x, int y, int z) GetDistancesTo(Beacon other)
    {
        return (this.X - other.X, this.Y - other.Y, this.Z - other.Z);
    }

    private readonly Cached<Beacon[]> allOrientations;

    private IEnumerable<Beacon> GetInAllOrientations()
    {
        foreach (var orientedBeacon in GetAllOrientations(this))
        {
            foreach (var rotatedOrientedBeacon in GetAllRotations(orientedBeacon))
                yield return rotatedOrientedBeacon;
        }
    }

    private static IEnumerable<Beacon> GetAllOrientations(Beacon beacon)
    {
        var current = beacon;

        for (int i = 0; i < 3; i++)
        {
            yield return current;
            yield return new Beacon(-current.X, -current.Y, current.Z);

            current = new Beacon(current.Y, current.Z, current.X);
        }
    }

    private static IEnumerable<Beacon> GetAllRotations(Beacon beacon)
    {
        var current = beacon;

        for (int i = 0; i < 4; i++)
        {
            yield return current;

            current = new Beacon(current.X, -current.Z, current.Y);
        }
    }

    public override string ToString() => $"({X},{Y},{Z})";

    public override bool Equals(object? obj)
    {
        if (obj == null) return false;

        Beacon other = obj as Beacon;

        if (other == null) return false;

        return this.X == other.X && this.Y == other.Y && this.Z == other.Z;
    }

    public override int GetHashCode()
    {
        return X.GetHashCode() * Y.GetHashCode() * Z.GetHashCode();
    }
}