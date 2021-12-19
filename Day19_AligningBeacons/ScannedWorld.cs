using System.Diagnostics;
using System.Drawing;
using System.Text.RegularExpressions;

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
