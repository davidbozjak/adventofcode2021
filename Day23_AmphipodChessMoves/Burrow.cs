using System.Diagnostics;

[DebuggerDisplay("{string.Concat(this.OccupiedSpaces)}")]
class Burrow : IWorld, INode, IEquatable<Burrow>
{
    public static IEnumerable<KeyValuePair<(int x, int y), Space>> EmptyBurrow;

    private readonly IReadOnlyDictionary<(int x, int y), Space> spaces;
    private readonly Cached<IReadOnlyCollection<Space>> readOnlySpacesWrapper;
    public IEnumerable<IWorldObject> WorldObjects => this.readOnlySpacesWrapper.Value;

    private readonly Cached<int> cachedMaxYValue;
    private readonly Cached<IList<Space>> cachedOccupiedSpaces;
    public IEnumerable<Space> OccupiedSpaces => this.cachedOccupiedSpaces.Value;

    public int Cost { get; }

    public Burrow()
    {
        this.spaces = new Dictionary<(int x, int y), Space>(EmptyBurrow);

        this.readOnlySpacesWrapper = new Cached<IReadOnlyCollection<Space>>(() => this.spaces.Values.ToList());
        this.cachedOccupiedSpaces = new Cached<IList<Space>>(() => this.spaces.Values.Where(w => w.IsOccupied).ToList());
        this.cachedMaxYValue = new Cached<int>(() => this.spaces.Values.Select(w => w.Y).Max());
    }

    public Burrow(IEnumerable<(int x, int y, Player p)> occupiedSpaces)
        :this()
    {
        var spacesToModify = new Dictionary<(int x, int y), Space>(EmptyBurrow);

        foreach ((int x, int y, Player p) in occupiedSpaces)
        {
            if (!this.spaces.ContainsKey((x, y)))
                throw new Exception();

            spacesToModify[(x, y)] = spacesToModify[(x, y)].Occpy(p);
        }

        this.spaces = spacesToModify;
    }

    private Burrow(Burrow clone, (int x, int y) poz, (int x, int y) previouslyOccupiedSpace, Player player, int cost)
        :this()
    {
        var spacesToModify = new Dictionary<(int x, int y), Space>(EmptyBurrow);

        foreach (var space in clone.OccupiedSpaces)
        {
            spacesToModify[(space.X, space.Y)] = this.spaces[(space.X, space.Y)].Occpy(space.Player);
        }

        spacesToModify[previouslyOccupiedSpace] = spacesToModify[previouslyOccupiedSpace].Occpy(null);
        spacesToModify[poz] = spacesToModify[poz].Occpy(player);

        this.spaces = spacesToModify;
        this.Cost = cost;
    }

    public Burrow SetPlayer((int x, int y) poz, (int x, int y) previouslyOccupiedSpace, Player player, int cost)
    {
        return new Burrow(this, poz, previouslyOccupiedSpace, player, cost);
    }

    public IEnumerable<Burrow> GetPossibleMoves()
    {
        var spaceWithPlayer = this.OccupiedSpaces
            .Select(w => ((w.X, w.Y), w.Player));

        foreach ((var currentSpace, var player) in spaceWithPlayer)
        {
            //check if this player is done, if yes do not move it
            if (currentSpace.Y > 0 && currentSpace.X == player.DestinationX)
            {
                bool allBelowOK = true;

                for (int y = this.cachedMaxYValue.Value; y > currentSpace.Y; y--)
                {
                    if (!this.spaces[(currentSpace.X, y)].IsOccupied || (currentSpace.X != this.spaces[(currentSpace.X, y)].Player.DestinationX))
                    {
                        allBelowOK = false;
                        break;
                    }
                }

                if (allBelowOK) 
                    continue;
            }

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

                    bool skipTileBecauseRoomHasWrongMembers = false;
                    for (int y = 1; y <= this.cachedMaxYValue.Value; y++)
                    {
                        if (this.spaces[(tile.x, y)].IsOccupied && this.spaces[(tile.x, y)].Player.DestinationX != tile.x)
                        {
                            skipTileBecauseRoomHasWrongMembers = true;
                            break;
                        }
                    }

                    if (skipTileBecauseRoomHasWrongMembers)
                    {
                        continue;   // handle rule: "AND that room contains no amphipods which do not also have that room as their own destination."
                    }

                    bool skipTileBecauseNotAtBottom = false;
                    for (int y = 1; y < this.cachedMaxYValue.Value; y++)
                    {
                        if (tile.y == y && !this.spaces[(tile.x, y + 1)].IsOccupied)
                        {
                            skipTileBecauseNotAtBottom = true;
                            break;
                        }
                    }

                    if (skipTileBecauseNotAtBottom)
                    {
                        continue;   // enforce that they always go to the bottom of the room first
                    }
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
