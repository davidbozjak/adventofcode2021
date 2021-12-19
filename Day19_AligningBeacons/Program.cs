using System.Diagnostics;
using System.Drawing;
using System.Text.RegularExpressions;

var inputParser = new InputProvider<string?>("Input.txt", GetString) { EndAtEmptyLine = false };
var scannerParser = new MultiLineParser<ScannedWorld>(() => new ScannedWorld(), (world, value) => world.AddScannedRow(value));
var scanners = scannerParser.AddRange(inputParser);

var masterScanner = scanners[0];
var unmatched = scanners.Skip(1).ToList();

while (unmatched.Count > 0)
{
    foreach (var world in unmatched)
    {
        var result = masterScanner.ExtendWorld(world);
        if (result)
        {
            unmatched.Remove(world);
            break;
        }
    }
}

Console.WriteLine($"Part 1: {masterScanner.WorldObjects.Count()}");

long maxManhattanDistance = long.MinValue;

foreach (var scanner1 in scanners)
{
    foreach (var scanner2 in scanners)
    {
        int distance = Math.Abs(scanner1.X - scanner2.X) + Math.Abs(scanner1.Y - scanner2.Y) + Math.Abs(scanner1.Z - scanner2.Z);

        if (distance > maxManhattanDistance)
        {
            maxManhattanDistance = distance;
        }
    }
}

Console.WriteLine($"Part 2: {maxManhattanDistance}");

static bool GetString(string? input, out string? value)
{
    value = null;

    if (input == null) return false;

    value = input ?? string.Empty;

    return true;
}

[DebuggerDisplay("{this.Id}: {this.ObjectCount}")]
class ScannedWorld : IWorld, IWorldObject
{
    public IEnumerable<IWorldObject> WorldObjects => this.cachedReadOnlyBeacons.Value;

    private readonly List<Beacon> beacons = new();
    private readonly Cached<IReadOnlyCollection<Beacon>> cachedReadOnlyBeacons;

    public int Id { get; private set; }

    public int ObjectCount => this.beacons.Count;

    public char CharRepresentation => 'S';

    public Point Position { get; private set; }

    public int X => this.Position.X;

    public int Y => this.Position.Y;

    public int Z { get; private set; }

    public Dictionary<(int x, int y, int z), (Beacon, Beacon)> BeaconDistances = new();

    public ScannedWorld()
    {
        this.cachedReadOnlyBeacons = new Cached<IReadOnlyCollection<Beacon>>(() => this.beacons.AsReadOnly());
        this.Position = new Point(0, 0);
        this.Z = 0;
    }

    public void SetPosition(int x, int y, int z)
    {
        this.Position = new Point(x, y);
        this.Z = z;
    }

    public bool ExtendWorld(ScannedWorld other)
    {
        for (int orientation = 0; orientation < 24; orientation++)
        {
            var points = other.GetOrientedBeacons(orientation).ToList();

            var matched = new HashSet<Beacon>();

            foreach (var point1 in points)
            {
                foreach (var point2 in points)
                {
                    if (point1 == point2) continue;

                    var distance = point1.GetDistancesTo(point2);

                    if (this.BeaconDistances.ContainsKey(distance))
                    {
                        matched.Add(point1);
                        matched.Add(point2);

                        if (matched.Count >= 12)
                        {
                            (var ownPoint1, var ownPoint2) = this.BeaconDistances[distance];
                            var transform = ownPoint1.GetDistancesTo(point1);

                            // they match! now apply the transform to all points and incorporate them in your map!

                            foreach (var point in points)
                            {
                                AddBeacon(point.Transform(transform.x, transform.y, transform.z));
                            }

                            other.SetPosition(transform.x, transform.y, transform.z);

                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }

    public IEnumerable<Beacon> GetOrientedBeacons(int orientation)
    {
        return this.beacons.Select(w => w.GetInOrientation(orientation));
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

            AddBeacon(new Beacon(values[0], values[1], values[2]));
        }
    }

    private void AddBeacon(Beacon beacon)
    {
        if (!this.beacons.Contains(beacon))
        {
            this.beacons.Add(beacon);
            AddBeaconDistances(beacon);
        }
    }

    private void AddBeaconDistances(Beacon newBeacon)
    {
        foreach (var beacon in this.beacons)
        {
            if (beacon == newBeacon) continue;

            this.BeaconDistances.Add(beacon.GetDistancesTo(newBeacon), (beacon, newBeacon));
            this.BeaconDistances.Add(newBeacon.GetDistancesTo(beacon), (newBeacon, beacon));
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