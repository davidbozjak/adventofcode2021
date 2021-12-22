using System.Drawing;
using System.Text.RegularExpressions;

var instructions = new InputProvider<Instruction?>("Input.txt", GetInstruction)
    .Where(w => w != null)
    .Cast<Instruction>()
    .ToList();

var factory = new UniqueFactory<(int x, int y, int z), Bit>(w => new Bit(w.x, w.y, w.z));

Console.WriteLine(instructions.Count);

foreach (var instruction in instructions)
{
    for (int x = instruction.MinX; x <= instruction.MaxX; x++)
    {
        if (x < -50 || x > 50) continue;

        for (int y = instruction.MinY; y <= instruction.MaxY; y++)
        {
            if (y < -50 || y > 50) continue;

            for (int z = instruction.MinZ; z <= instruction.MaxZ; z++)
            {
                if (z < -50 || z > 50) continue;

                var bit = factory.GetOrCreateInstance((x, y, z));
                bit.Value = instruction.On;
            }
        }
    }
}

Console.WriteLine($"Part 1: {factory.AllCreatedInstances.Where(w => w.Value).Count()}");

static bool GetInstruction(string? input, out Instruction? value)
{
    value = null;

    if (string.IsNullOrWhiteSpace(input)) return false;

    Regex numRegex = new(@"-?\d+");

    bool on = input.StartsWith("on");
    var numbers = numRegex.Matches(input)
        .Select(w => int.Parse(w.Value))
        .ToArray();

    if (numbers.Length != 6) throw new Exception();

    value = new Instruction(on, numbers[0], numbers[1], numbers[2], numbers[3], numbers[4], numbers[5]);

    return true;
}

record Instruction(bool On, int MinX, int MaxX, int MinY, int MaxY, int MinZ, int MaxZ);

class Bit : IWorldObject
{
    public Point Position { get; init; }

    public char CharRepresentation => this.Value ? '¤' : '.';

    public int X => this.Position.X;

    public int Y => this.Position.Y;

    public int Z { get; init; }

    public bool Value { get; set; }

    public Bit(int x, int y, int z)
    {
        this.Position = new Point(x, y);
        this.Z = z;
    }
}