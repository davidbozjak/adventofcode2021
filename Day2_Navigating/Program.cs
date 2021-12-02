using Day2_Navigating;

var input = new InputProvider<Instruction?>("Input.txt", Parse).ToList();

var instructions = input.Where(w => w != null).Cast<Instruction>().ToList();

int depth = 0;
int x = 0;

foreach (var instruction in instructions)
{
    switch (instruction.Direction)
    {
        case Directions.Down:
            depth += instruction.Steps;
            break;
        case Directions.Up:
            depth -= instruction.Steps;
            break;
        case Directions.Forward:
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
        case Directions.Down:
            aim += instruction.Steps;
            break;
        case Directions.Up:
            aim -= instruction.Steps;
            break;
        case Directions.Forward:
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
        value = new Instruction
        {
            Direction = input[..input.IndexOf(" ")] switch
            {
                "forward" => Directions.Forward,
                "up" => Directions.Up,
                "down" => Directions.Down,
                _ => throw new Exception()
            },

            Steps = int.Parse(input[input.LastIndexOf(" ")..])
        };

        return true;
    }
    else
    {
        return false;
    }
}