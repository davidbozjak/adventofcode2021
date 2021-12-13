var points = new InputProvider<PaperPoint?>("Input.txt", GetPoint).Where(w => w != null).Cast<PaperPoint>().ToList();
var world = new FoldableWorld(points);
var printer = new WorldPrinter();

world.FoldX(655);

var visiblePointsAfterFirstFold = world.WorldObjects.Count();

world.FoldY(447);
world.FoldX(327);
world.FoldY(223);
world.FoldX(163);
world.FoldY(111);
world.FoldX(81);
world.FoldY(55);
world.FoldX(40);
world.FoldY(27);
world.FoldY(13);
world.FoldY(6);

printer.Print(world);
Console.WriteLine($"Part 1: {visiblePointsAfterFirstFold}");

static bool GetPoint(string? input, out PaperPoint? value)
{
    value = null;

    if (input == null) return false;

    var numbers = input.Split(',', StringSplitOptions.RemoveEmptyEntries)
        .Select(w => int.Parse(w))
        .ToArray();

    value = new PaperPoint(numbers[0], numbers[1]);

    return true;
}