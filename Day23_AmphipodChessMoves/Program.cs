using System.Diagnostics;
using System.Drawing;

var amberPlayer = new Player('A', 1, 2);
var bronzePlayer = new Player('B', 10, 4);
var copperPlayer = new Player('C', 100, 6);
var desertPlayer = new Player('D', 1000, 8);

// Example data:
//var initialWorld = new Burrow(new[]
//{
//    (2, 1, bronzePlayer),
//    (2, 2, amberPlayer),
//    (4, 1, copperPlayer),
//    (4, 2, desertPlayer),
//    (6, 1, bronzePlayer),
//    (6, 2, copperPlayer),
//    (8, 1, desertPlayer),
//    (8, 2, amberPlayer),
//});

// Assignment data:
var initialWorld = new Burrow(new[]
{
    (2, 1, bronzePlayer),
    (2, 2, copperPlayer),
    (4, 1, desertPlayer),
    (4, 2, desertPlayer),
    (6, 1, copperPlayer),
    (6, 2, bronzePlayer),
    (8, 1, amberPlayer),
    (8, 2, amberPlayer),
});

//var almostIdealWorld = new Burrow(new[]
//{
//    (2, 1, bronzePlayer),
//    (2, 2, amberPlayer),
//    (4, 1, amberPlayer),
//    (4, 2, bronzePlayer),
//    (6, 1, copperPlayer),
//    (6, 2, copperPlayer),
//    (8, 1, desertPlayer),
//    (8, 2, desertPlayer),
//});

var idealWorld = new Burrow(new[]
{
    (2, 1, amberPlayer),
    (2, 2, amberPlayer),
    (4, 1, bronzePlayer),
    (4, 2, bronzePlayer),
    (6, 1, copperPlayer),
    (6, 2, copperPlayer),
    (8, 1, desertPlayer),
    (8, 2, desertPlayer),
});

int maxInRightRoom = int.MinValue;

var printer = new WorldPrinter();
printer.Print(initialWorld);

var possibleMovesCache = new UniqueFactory<Burrow, List<Burrow>>(burrow => burrow.GetPossibleMoves().ToList());
var path = AStarPathfinder.FindPath<Burrow>(initialWorld, idealWorld, GetHeuristicScore, possibleMovesCache.GetOrCreateInstance);

//Console.WriteLine("Path found");
//Console.ReadKey();

//if (path == null) throw new Exception();

//foreach (var step in path)
//{
//    printer.Print(step);
//    Console.ReadKey();
//}

Console.WriteLine($"Part 1: { path.Sum(w => w.Cost)}");

int GetHeuristicScore(Burrow burrow)
{
    var playersNotInTheRightRoom = burrow.OccupiedSpaces.Where(w => w.X != w.Player.DestinationX).ToList();

    var playersInTheRightRoom = 8 - playersNotInTheRightRoom.Count;
    if (playersInTheRightRoom > maxInRightRoom)
    {
        maxInRightRoom = playersInTheRightRoom;
        Console.WriteLine($"{DateTime.Now} found space for {playersInTheRightRoom}");
    }

    int total = 0;

    foreach (var player in playersNotInTheRightRoom)
    {
        total += Math.Abs(player.X - player.Player.DestinationX) * player.Player.CostMultiplyer;
    }

    return total;
}

[DebuggerDisplay("{string.Concat(this.OccupiedSpaces)}")]
class Burrow : IWorld, INode, IEquatable<Burrow>
{
    private readonly Dictionary<(int x, int y), Space> spaces;
    private readonly Cached<IReadOnlyCollection<Space>> readOnlySpacesWrapper;
    public IEnumerable<IWorldObject> WorldObjects => this.readOnlySpacesWrapper.Value;

    public IEnumerable<Space> OccupiedSpaces => this.spaces.Values.Where(w => w.IsOccupied);

    public int Cost { get; init; }

    public Burrow()
    {
        this.spaces = new Dictionary<(int x, int y), Space>()
        {
            { (0, 0), new Space(0, 0, null) },
            { (1, 0), new Space(1, 0, null) },
            { (2, 0), new Space(2, 0, null) },
            { (3, 0), new Space(3, 0, null) },
            { (4, 0), new Space(4, 0, null) },
            { (5, 0), new Space(5, 0, null) },
            { (6, 0), new Space(6, 0, null) },
            { (7, 0), new Space(7, 0, null) },
            { (8, 0), new Space(8, 0, null) },
            { (9, 0), new Space(9, 0, null) },
            { (10, 0), new Space(10, 0, null) },
            { (2, 1), new Space(2, 1, null) },
            { (2, 2), new Space(2, 2, null) },
            { (4, 1), new Space(4, 1, null) },
            { (4, 2), new Space(4, 2, null) },
            { (6, 1), new Space(6, 1, null) },
            { (6, 2), new Space(6, 2, null) },
            { (8, 1), new Space(8, 1, null) },
            { (8, 2), new Space(8, 2, null) },
        };

        this.readOnlySpacesWrapper = new Cached<IReadOnlyCollection<Space>>(() => this.spaces.Values);
    }

    public Burrow(IEnumerable<(int x, int y, Player p)> occupiedSpaces)
        :this()
    {
        foreach ((int x, int y, Player p) in occupiedSpaces)
        {
            if (!this.spaces.ContainsKey((x, y)))
                throw new Exception();

            this.spaces[(x, y)] = this.spaces[(x, y)].Occpy(p);
        }
    }

    private Burrow(Burrow clone)
        :this()
    {
        foreach (var space in clone.OccupiedSpaces)
        {
            this.spaces[(space.X, space.Y)] = this.spaces[(space.X, space.Y)].Occpy(space.Player);
        }
    }

    public Burrow SetPlayer((int x, int y) poz, (int x, int y) previouslyOccupiedSpace, Player player, int cost)
    {
        var newBurrow = new Burrow(this) { Cost = cost };

        newBurrow.spaces[previouslyOccupiedSpace] = newBurrow.spaces[previouslyOccupiedSpace].Occpy(null);
        newBurrow.spaces[poz] = newBurrow.spaces[poz].Occpy(player);
        
        return newBurrow;
    }

    public IEnumerable<Burrow> GetPossibleMoves()
    {
        var spaceWithPlayer = this.OccupiedSpaces
            .Select(w => ((w.X, w.Y), w.Player));

        foreach ((var currentSpace, var player) in spaceWithPlayer)
        {
            var allReachableTiles = GetAllReachableTilesForPlayer(currentSpace, player, new List<(int x, int y)>());

            foreach((var tile, var cost) in allReachableTiles)
            {
                if (tile.y == 0)
                {
                    if (new[] { 2, 4, 6, 8 }.Contains(tile.x)) 
                        continue; // handle rule: "Amphipods will never stop on the space immediately outside any room."
                }

                if (currentSpace.Y == 0)
                {
                    if (tile.y == 0)
                        continue; // handle rule: "Once an amphipod stops moving in the hallway, it will stay in that spot until it can move into a room."
                }

                if (tile.y > 0)
                {
                    if (tile.x != player.DestinationX)
                        continue; // handle rule: "Amphipods will never move from the hallway into a room unless that room is their destination room"

                    if ((this.spaces[(tile.x, 1)].IsOccupied && this.spaces[(tile.x, 1)].Player.DestinationX != tile.x) ||
                        (this.spaces[(tile.x, 2)].IsOccupied && this.spaces[(tile.x, 2)].Player.DestinationX != tile.x))
                        continue; // handle rule: "AND that room contains no amphipods which do not also have that room as their own destination."

                    if (tile.y == 1 && !this.spaces[(tile.x, 2)].IsOccupied)
                        continue; // enforce that they always go to the bottom of the room first
                }

                yield return this.SetPlayer(tile, currentSpace, player, cost);
            }
        }
    }

    private IEnumerable<((int x, int y), int cost)> GetAllReachableTilesForPlayer((int currentX, int currentY) poz, Player player, List<(int x, int y)> path)
    {
        var options = new[]
        {
            (poz.currentX - 1, poz.currentY),
            (poz.currentX + 1, poz.currentY),
            (poz.currentX, poz.currentY - 1),
            (poz.currentX, poz.currentY + 1)
        };

        foreach (var option in options)
        {
            if (!this.spaces.ContainsKey(option)) continue;
            if (this.spaces[option].IsOccupied) continue;
            if (path.Contains(option)) continue;

            var newPath = path.ToList();
            newPath.Add(option);
            
            yield return (option, newPath.Count * player.CostMultiplyer);

            foreach (var rez in GetAllReachableTilesForPlayer(option, player, newPath))
            {
                yield return rez;
            }
        }
    }

    public override int GetHashCode()
    {
        var spaceMultiplier = 1;
        foreach (var space in this.OccupiedSpaces)
        {
            spaceMultiplier *= space.GetHashCode();
        }
        return spaceMultiplier;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Burrow other)
        {
            return false;
        }

        foreach (var occupiedSpace in this.OccupiedSpaces)
        {
            if (!other.OccupiedSpaces.Contains(occupiedSpace)) return false;
        }

        return true;
    }

    public bool Equals(Burrow? other)
    {
        return this.Equals((object)other);
    }

    public static bool operator ==(Burrow burrow1, Burrow burrow2)
    {
        if ((burrow1 is null) || (burrow2 is null))
            return Object.Equals(burrow1, burrow2);

        return burrow1.Equals(burrow2);
    }

    public static bool operator !=(Burrow burrow1, Burrow burrow2)
    {
        return !(burrow1 == burrow2);
    }

    public override string ToString()
    {
        return string.Concat(this.OccupiedSpaces);
    }
}

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

record Player(char Name, int CostMultiplyer, int DestinationX);