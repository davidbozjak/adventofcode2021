var amberPlayer = new Player('A', 1, 2);
var bronzePlayer = new Player('B', 10, 4);
var copperPlayer = new Player('C', 100, 6);
var desertPlayer = new Player('D', 1000, 8);

// Example data Part1:
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

//Example data Part2:
//var initialWorld = new Burrow(new[]
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

//Part 1 Assignment data:
//var initialWorld = new Burrow(new[]
//{
//    (2, 1, bronzePlayer),
//    (2, 2, copperPlayer),
//    (4, 1, desertPlayer),
//    (4, 2, desertPlayer),
//    (6, 1, copperPlayer),
//    (6, 2, bronzePlayer),
//    (8, 1, amberPlayer),
//    (8, 2, amberPlayer),
//});

//Part 2 Assignment data:
var initialWorld = new Burrow(new[]
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

//Part 1 final state:
//var idealWorld = new Burrow(new[]
//{
//    (2, 1, amberPlayer),
//    (2, 2, amberPlayer),
//    (4, 1, bronzePlayer),
//    (4, 2, bronzePlayer),
//    (6, 1, copperPlayer),
//    (6, 2, copperPlayer),
//    (8, 1, desertPlayer),
//    (8, 2, desertPlayer),
//});

//Part 2 final state:
var idealWorld = new Burrow(new[]
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

int minInWrongRoom = int.MaxValue;

var printer = new WorldPrinter();
printer.Print(initialWorld);

var possibleMovesCache = new UniqueFactory<Burrow, List<Burrow>>(burrow => burrow.GetPossibleMoves().ToList());
var path = AStarPathfinder.FindPath<Burrow>(initialWorld, idealWorld, GetHeuristicScore, possibleMovesCache.GetOrCreateInstance);

//Console.WriteLine("Path found");
//Console.ReadKey();

if (path == null) throw new Exception();

//foreach (var step in path)
//{
//    printer.Print(step);
//    Console.ReadKey();
//}

Console.WriteLine($"Part 1: { path.Sum(w => w.Cost)}");

int GetHeuristicScore(Burrow burrow)
{
    var playersNotInTheRightRoom = burrow.OccupiedSpaces.Where(w => w.X != w.Player.DestinationX).ToList();

    if (playersNotInTheRightRoom.Count < minInWrongRoom)
    {
        minInWrongRoom = playersNotInTheRightRoom.Count;

        var printer = new WorldPrinter();
        printer.Print(burrow);

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

record Player(char Name, int CostMultiplyer, int DestinationX);