var points = new InputProvider<PaperPoint?>("Input.txt", GetPoint).Where(w => w != null).Cast<PaperPoint>().ToList();
var world = new FoldableWorld(points);
var printer = new WorldPrinter();

var instructions = new InputProvider<Instruction?>("Instruction.txt", GetInstruction).Where(w => w != null).Cast<Instruction>().ToList();

ExecuteInstruction(instructions[0], world);

var visiblePointsAfterFirstFold = world.WorldObjects.Count();

for (int i = 1; i < instructions.Count; i++)
{
    ExecuteInstruction(instructions[i], world);
}

printer.Print(world);
Console.WriteLine($"Part 1: {visiblePointsAfterFirstFold}");

static void ExecuteInstruction(Instruction instruction, FoldableWorld world)
{
    if (instruction.IsVertical)
    {
        world.FoldY(instruction.Value);
    }
    else
    {
        world.FoldX(instruction.Value);
    }
}

static bool GetPoint(string? input, out PaperPoint? value)
{
    value = null;

    if (string.IsNullOrWhiteSpace(input)) return false;

    var numbers = input.Split(',', StringSplitOptions.RemoveEmptyEntries)
        .Select(w => int.Parse(w))
        .ToArray();

    value = new PaperPoint(numbers[0], numbers[1]);

    return true;
}

static bool GetInstruction(string? input, out Instruction? value)
{
    value = null;

    if (string.IsNullOrWhiteSpace(input)) return false;

    var isVertical = input.Contains('y');
    var intValue = int.Parse(input[(input.LastIndexOf('=') + 1)..]);

    value = new Instruction(isVertical, intValue);
    return true;
}

record Instruction(bool IsVertical, int Value);