var amberPlayer = new Player('A', 1, 2);
var bronzePlayer = new Player('B', 10, 4);
var copperPlayer = new Player('C', 100, 6);
var desertPlayer = new Player('D', 1000, 8);

// Example data Part1:
//var initialWorldPart1 = new Burrow(new[]
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

Burrow.EmptyBurrow = new KeyValuePair<(int x, int y), Space>[]
{
    KeyValuePair.Create((0, 0), new Space(0, 0, null)),
    KeyValuePair.Create((1, 0), new Space(1, 0, null)),
    KeyValuePair.Create((2, 0), new Space(2, 0, null)),
    KeyValuePair.Create((3, 0), new Space(3, 0, null)),
    KeyValuePair.Create((4, 0), new Space(4, 0, null)),
    KeyValuePair.Create((5, 0), new Space(5, 0, null)),
    KeyValuePair.Create((6, 0), new Space(6, 0, null)),
    KeyValuePair.Create((7, 0), new Space(7, 0, null)),
    KeyValuePair.Create((8, 0), new Space(8, 0, null)),
    KeyValuePair.Create((9, 0), new Space(9, 0, null)),
    KeyValuePair.Create((10, 0), new Space(10, 0, null)),
    KeyValuePair.Create((2, 1), new Space(2, 1, null)),
    KeyValuePair.Create((2, 2), new Space(2, 2, null)),
    KeyValuePair.Create((4, 1), new Space(4, 1, null)),
    KeyValuePair.Create((4, 2), new Space(4, 2, null)),
    KeyValuePair.Create((6, 1), new Space(6, 1, null)),
    KeyValuePair.Create((6, 2), new Space(6, 2, null)),
    KeyValuePair.Create((8, 1), new Space(8, 1, null)),
    KeyValuePair.Create((8, 2), new Space(8, 2, null))
};

//Part 1 Assignment data:
var initialWorldPart1 = new Burrow(new[]
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

var idealWorldPart1 = new Burrow(new[]
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

int minInWrongRoom = int.MaxValue;

var possibleMovesCachePart1 = new UniqueFactory<Burrow, List<Burrow>>(burrow => burrow.GetPossibleMoves().ToList());
var pathPart1 = AStarPathfinder.FindPath<Burrow>(initialWorldPart1, idealWorldPart1, GetHeuristicScore, possibleMovesCachePart1.GetOrCreateInstance);

if (pathPart1 == null) throw new Exception();

Burrow.EmptyBurrow = new KeyValuePair<(int x, int y), Space>[]
{
    KeyValuePair.Create((0, 0), new Space(0, 0, null)),
    KeyValuePair.Create((1, 0), new Space(1, 0, null)),
    KeyValuePair.Create((2, 0), new Space(2, 0, null)),
    KeyValuePair.Create((3, 0), new Space(3, 0, null)),
    KeyValuePair.Create((4, 0), new Space(4, 0, null)),
    KeyValuePair.Create((5, 0), new Space(5, 0, null)),
    KeyValuePair.Create((6, 0), new Space(6, 0, null)),
    KeyValuePair.Create((7, 0), new Space(7, 0, null)),
    KeyValuePair.Create((8, 0), new Space(8, 0, null)),
    KeyValuePair.Create((9, 0), new Space(9, 0, null)),
    KeyValuePair.Create((10, 0), new Space(10, 0, null)),
    KeyValuePair.Create((2, 1), new Space(2, 1, null)),
    KeyValuePair.Create((2, 2), new Space(2, 2, null)),
    KeyValuePair.Create((2, 3), new Space(2, 3, null)),
    KeyValuePair.Create((2, 4), new Space(2, 4, null)),
    KeyValuePair.Create((4, 1), new Space(4, 1, null)),
    KeyValuePair.Create((4, 2), new Space(4, 2, null)),
    KeyValuePair.Create((4, 3), new Space(4, 3, null)),
    KeyValuePair.Create((4, 4), new Space(4, 4, null)),
    KeyValuePair.Create((6, 1), new Space(6, 1, null)),
    KeyValuePair.Create((6, 2), new Space(6, 2, null)),
    KeyValuePair.Create((6, 3), new Space(6, 3, null)),
    KeyValuePair.Create((6, 4), new Space(6, 4, null)),
    KeyValuePair.Create((8, 1), new Space(8, 1, null)),
    KeyValuePair.Create((8, 2), new Space(8, 2, null)),
    KeyValuePair.Create((8, 3), new Space(8, 3, null)),
    KeyValuePair.Create((8, 4), new Space(8, 4, null))
};

minInWrongRoom = int.MaxValue;

var initialWorldPart2 = new Burrow(new[]
{
    (2, 1, bronzePlayer),
    (2, 2, desertPlayer),
    (2, 3, desertPlayer),
    (2, 4, copperPlayer),
    (4, 1, desertPlayer),
    (4, 2, copperPlayer),
    (4, 3, bronzePlayer),
    (4, 4, desertPlayer),
    (6, 1, copperPlayer),
    (6, 2, bronzePlayer),
    (6, 3, amberPlayer),
    (6, 4, bronzePlayer),
    (8, 1, amberPlayer),
    (8, 2, amberPlayer),
    (8, 3, copperPlayer),
    (8, 4, amberPlayer),
});

//Example data Part2:
//var initialWorldPart2 = new Burrow(new[]
//{
//    (2, 1, bronzePlayer),
//    (2, 2, desertPlayer),
//    (2, 3, desertPlayer),
//    (2, 4, amberPlayer),
//    (4, 1, copperPlayer),
//    (4, 2, copperPlayer),
//    (4, 3, bronzePlayer),
//    (4, 4, desertPlayer),
//    (6, 1, bronzePlayer),
//    (6, 2, bronzePlayer),
//    (6, 3, amberPlayer),
//    (6, 4, copperPlayer),
//    (8, 1, desertPlayer),
//    (8, 2, amberPlayer),
//    (8, 3, copperPlayer),
//    (8, 4, amberPlayer),
//});

var idealWorldPart2 = new Burrow(new[]
{
    (2, 1, amberPlayer),
    (2, 2, amberPlayer),
    (2, 3, amberPlayer),
    (2, 4, amberPlayer),
    (4, 1, bronzePlayer),
    (4, 2, bronzePlayer),
    (4, 3, bronzePlayer),
    (4, 4, bronzePlayer),
    (6, 1, copperPlayer),
    (6, 2, copperPlayer),
    (6, 3, copperPlayer),
    (6, 4, copperPlayer),
    (8, 1, desertPlayer),
    (8, 2, desertPlayer),
    (8, 3, desertPlayer),
    (8, 4, desertPlayer),
});

var possibleMovesCachePart2 = new UniqueFactory<Burrow, List<Burrow>>(burrow => burrow.GetPossibleMoves().ToList());
var pathPart2 = AStarPathfinder.FindPath<Burrow>(initialWorldPart2, idealWorldPart2, GetHeuristicScore, possibleMovesCachePart2.GetOrCreateInstance);

if (pathPart2 == null) throw new Exception();

Console.WriteLine($"Part 1: { pathPart1.Sum(w => w.Cost)}");
Console.WriteLine($"Part 2: { pathPart2.Sum(w => w.Cost)}");

Console.WriteLine("Visualize path Part 1");
Console.ReadKey();
VisualizeMovement(pathPart1);

Console.WriteLine("Visualize path Part 2");
Console.ReadKey();
VisualizeMovement(pathPart2);

int GetHeuristicScore(Burrow burrow)
{
    var playersNotInTheRightRoom = burrow.OccupiedSpaces.Where(w => w.X != w.Player.DestinationX).ToList();


    if (playersNotInTheRightRoom.Count < minInWrongRoom)
    {
        minInWrongRoom = playersNotInTheRightRoom.Count;
#if DEBUG
        var printer = new WorldPrinter();
        printer.Print(burrow);
#endif
        Console.WriteLine($"{DateTime.Now}: {minInWrongRoom} left");
    }

    int total = 0;

    foreach (var player in playersNotInTheRightRoom)
    {
        total += Math.Abs(player.X - player.Player.DestinationX) * player.Player.CostMultiplyer;

        total += (player.Y + 1) * player.Player.CostMultiplyer;
    }

    return total;
}

void VisualizeMovement(IEnumerable<Burrow> path)
{
    var printer = new WorldPrinter();

    foreach (var step in path)
    {
        printer.Print(step);
        Console.ReadKey();
    }
}

record Player(char Name, int CostMultiplyer, int DestinationX);