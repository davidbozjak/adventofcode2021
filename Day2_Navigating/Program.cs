var input = new InputProvider<Instruction?>("Input.txt", Parse).ToList();

var instructions = input.Where(w => w != null).ToList();

int depth = 0;
int x = 0;

foreach (var instruction in instructions)
{
    switch (instruction.Direction)
    {
        case Instruction.Directions.Down:
            depth += instruction.Steps;
            break;
        case Instruction.Directions.Up:
            depth -= instruction.Steps;
            break;
        case Instruction.Directions.Forward:
            x += instruction.Steps;
            break;
        default: throw new Exception();
    }
}

Console.WriteLine($"Part 1: {depth * x}");

int aim = 0;
depth = 0;
x = 0;

foreach (var instruction in instructions)
{
    switch (instruction.Direction)
    {
        case Instruction.Directions.Down:
            aim += instruction.Steps;
            break;
        case Instruction.Directions.Up:
            aim -= instruction.Steps;
            break;
        case Instruction.Directions.Forward:
            x += instruction.Steps;
            depth += aim * instruction.Steps;
            break;
        default: throw new Exception();
    }
}

Console.WriteLine($"Part 2: {depth * x}");

static bool Parse(string? input, out Instruction? value)
{
    value = null;

    if (!string.IsNullOrWhiteSpace(input))
    {
        value = new Instruction();

        if (input.StartsWith("forward"))
        {
            value.Direction = Instruction.Directions.Forward;
        }
        else if (input.StartsWith("up"))
        {
            value.Direction = Instruction.Directions.Up;
        }
        else if (input.StartsWith("down"))
        {
            value.Direction = Instruction.Directions.Down;
        }
        else throw new Exception();

        value.Steps = int.Parse(input.Substring(input.LastIndexOf(" ") + 1));

        return true;
    }
    else
    {
        return false;
    }
}

class Instruction
{
    public enum Directions { Forward, Down, Up }
    
    public Directions Direction { get; set; }

    public int Steps { get; set; }
}