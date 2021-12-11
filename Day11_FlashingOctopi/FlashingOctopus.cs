using System.Drawing;

class FlashingOctopus : IWorldObject
{
    public int X => this.Position.X;
    public int Y => this.Position.Y;

    public Point Position { get; init; }

    public int Energy { get; private set; }

    public char CharRepresentation => this.Energy.ToString()[0];

    public int Z => 0;

    public readonly List<FlashingOctopus> neighbours = new();

    private bool hasFlashedThisStep = false;

    public FlashingOctopus(int x, int y, int initialEnergy)
    {
        this.Position = new Point(x, y);
        this.Energy = initialEnergy;
    }

    public void AddNeighbour(FlashingOctopus octopus)
    {
        if (octopus == this) throw new Exception();

        this.neighbours.Add(octopus);
    }

    public int Step()
    {
        this.Energy++;

        if (this.Energy > 9 && !this.hasFlashedThisStep)
        {
            this.hasFlashedThisStep = true;
            return 1 + this.neighbours.Select(w => w.Step()).Sum();
        }

        return 0;
    }

    public void ConcludeStep()
    {
        this.hasFlashedThisStep = false;
        
        if (this.Energy > 9)
            this.Energy = 0;
    }
}