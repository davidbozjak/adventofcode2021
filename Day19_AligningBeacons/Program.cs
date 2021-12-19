using System.Drawing;
using System.Text.RegularExpressions;
using System.Diagnostics;

var inputParser = new InputProvider<string?>("Input.txt", GetString) { EndAtEmptyLine = false };
var scannerParser = new MultiLineParser<ScannedWorld>(() => new ScannedWorld(), (world, value) => world.AddScannedRow(value));
var scanners = scannerParser.AddRange(inputParser);

Console.WriteLine($"Scanners: {scanners.Count}");
scanners[0].PrintAllDirections();

static bool GetString(string? input, out string? value)
{
    value = null;

    if (input == null) return false;

    value = input ?? string.Empty;

    return true;
}

[DebuggerDisplay("{this.Id}: {this.ObjectCount}")]
class ScannedWorld : IWorld
{
    public IEnumerable<IWorldObject> WorldObjects => this.cachedReadOnlyBeacons.Value;

    private readonly List<Beacon> beacons = new();
    private readonly Cached<IReadOnlyCollection<Beacon>> cachedReadOnlyBeacons;

    public int Id { get; private set; }

    public int ObjectCount => this.beacons.Count;

    public ScannedWorld()
    {
        this.cachedReadOnlyBeacons = new Cached<IReadOnlyCollection<Beacon>>(() => this.beacons.AsReadOnly());
    }

    public ScannedWorld(IList<Beacon> initialBeacons)
        :this()
    {
        this.beacons.AddRange(initialBeacons);
    }

    public ScannedWorld? MatchWorld(ScannedWorld other)
    {
        var matched = new List<Beacon>();

        for (int rotation = 0; rotation < 24; rotation++)
        {

        }

        if (matched.Count >= 12)
            return new ScannedWorld(matched);
        else return null;
    }

    public void PrintAllDirections()
    {
        for (int rotation = 0; rotation < 24; rotation++)
        {
            Console.WriteLine($"Rotation {rotation}");

            foreach (var beacon in this.beacons)
                Console.WriteLine(beacon.GetInOrientation(rotation));
        }
    }

    public void AddScannedRow(string input)
    {
        if (input.Contains("scanner"))
        {
            Regex numRegex = new(@"\d+");
            this.Id = int.Parse(numRegex.Match(input).Value);
        }
        else
        {
            var values = input.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(w => int.Parse(w))
                .ToArray();

            this.beacons.Add(new Beacon(values[0], values[1], values[2]));
        }
    }
}

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
    
}