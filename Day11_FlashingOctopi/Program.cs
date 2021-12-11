var input = new InputProvider<int[]>("Input.txt", ParseRow).ToList();
var octopi = new List<FlashingOctopus>();

for (int y = 0; y < input.Count; y++)
{
    for (int x = 0; x < input[0].Length; x++)
    {
        octopi.Add(new FlashingOctopus(x, y, input[y][x]));
    }
}

for (int y = 0; y < input.Count; y++)
{
    for (int x = 0; x < input[0].Length; x++)
    {
        var octopous = octopi.Where(w => w.X == x && w.Y == y).First();

        octopi.Where(w => 
            ((Math.Abs(w.X - octopous.X) + Math.Abs(w.Y - octopous.Y)) == 1) ||
            (Math.Abs(w.X - octopous.X) == 1 && Math.Abs(w.Y - octopous.Y) == 1))
            .ToList()
            .ForEach(w => octopous.AddNeighbour(w));
    }
}

var world = new SimpleWorld<FlashingOctopus>(octopi);
var printer = new WorldPrinter();

int totalFlashes = 0;
int step = 0;
for (; step < 100; step++)
{
    int flashesThisStep = ProcessWorldStep();

    totalFlashes += flashesThisStep;

    Console.WriteLine($"After {step + 1} steps: {totalFlashes}");
}

Console.WriteLine($"Part 1: {totalFlashes}");

for (; ; step++)
{
    int flashesThisStep = ProcessWorldStep();

    if (flashesThisStep == octopi.Count)
    {
        Console.WriteLine($"Part 2: First simoultanious flash {step + 1}");
        break;
    }
}

int ProcessWorldStep()
{
    int flashesThisStep = 0;
    
    foreach (var octopous in octopi)
    {
        flashesThisStep += octopous.Step();
    }

    octopi.ForEach(w => w.ConcludeStep());

    return flashesThisStep;
}

static bool ParseRow(string? input, out int[] row)
{
    row = Array.Empty<int>();

    if (string.IsNullOrWhiteSpace(input)) return false;

    row = input.ToCharArray().Select(w => w - '0').ToArray();

    return true;
}