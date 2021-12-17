using System.Drawing;

//var targetAreaMinX = 20;
//var targetAreaMaxX = 30;
//var targetAreaMinY = -10;
//var targetAreaMaxY = -5;

var targetAreaMinX = 277;
var targetAreaMaxX = 318;
var targetAreaMinY = -92;
var targetAreaMaxY = -53;

var maxY = int.MinValue;

var list = new HashSet<(int x, int y)>();

int minInitialX = -30;
int maxInitialX = 30;
int minInitialY = -30;
int maxInitialY = 30;
int maxSteps = 100;

bool restart = true;
while (restart)
{
    restart = false;
    for (int initialX = minInitialX; initialX < maxInitialX; initialX++)
    {
        for (int initialY = minInitialY; initialY < maxInitialY; initialY++)
        {
            var probe = new Probe(initialX, initialY);

            bool hasHit = false;

            for (int step = 0; step < maxSteps; step++)
            {
                probe.MakeStep();

                if (probe.X >= targetAreaMinX && probe.X <= targetAreaMaxX
                    && probe.Y >= targetAreaMinY && probe.Y <= targetAreaMaxY)
                {
                    hasHit = true;

                    if (!list.Contains((initialX, initialY)) && list.Count > 0)
                    {
                        var maxXFound = list.Select(w => w.x).Max();
                        var minXFound = list.Select(w => w.x).Min();
                        var maxYFound = list.Select(w => w.y).Max();
                        var minYFound = list.Select(w => w.y).Min();

                        if (initialX < minXFound || initialX > maxXFound
                            || initialY < minYFound || initialY > maxYFound)
                        {
                            minInitialX -= 5;
                            maxInitialX += 5;
                            minInitialY -= 5;
                            maxInitialY += 5;
                            restart = true;
                        }
                    }

                    list.Add((initialX, initialY));

                    if (step > (double)maxSteps * 0.80)
                    {
                        maxSteps += 100;
                    }

                    break;
                }

                if (probe.Y < targetAreaMinY)
                {
                    break;
                }
            }

            if (hasHit && probe.MaxY > maxY)
            {
                maxY = probe.MaxY;
            }
        }
    }
}

Console.WriteLine($"Part1: {maxY}");
Console.WriteLine($"Part2: {list.Count}");

class Probe : IWorldObject
{
    public Point Position => new Point(this.X, this.Y);

    public char CharRepresentation => '#';

    public int X { get; private set; }
    public int Y { get; private set; }

    public int Z => 0;

    public int VelocityX { get; private set; }
    public int VelocityY { get; private set; }

    public int MaxY { get; private set; } = int.MinValue;

    public Probe(int initialVelocityX, int initialVelocityY)
    {
        this.VelocityX = initialVelocityX;
        this.VelocityY = initialVelocityY;
    }

    public void MakeStep()
    {
        this.X += VelocityX;
        this.Y += VelocityY;
        

        if (this.VelocityX != 0)
            this.VelocityX = this.VelocityX > 0 ? this.VelocityX - 1 : this.VelocityX + 1;

        this.VelocityY -= 1;

        if (this.Y > this.MaxY)
            this.MaxY = this.Y;
    }
}