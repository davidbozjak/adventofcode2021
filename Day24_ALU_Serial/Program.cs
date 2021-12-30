var programLines = new InputProvider<Instruction?>("Input.txt", GetInstruction)
    .Where(w => w != null).Cast<Instruction>().ToList();

//var computer = new ALU(programLines);
//(long w, long x, long y, long z) = computer.RunProgramForInput(new List<long> { 1, 3, 5, 7, 9, 2, 4, 6, 8, 9, 9, 9, 9, 9 }.GetEnumerator());

//Console.WriteLine($"w: {w} x: {x} y: {y} z: {z}");

var digitValidators = new List<ALU>();
int indexOfLastInputCommand = 0;

for (int i = 1; i <= programLines.Count; i++)
{
    if (i == programLines.Count || programLines[i].Type == InstructionType.Input)
    {
        digitValidators.Add(new ALU(programLines.Skip(indexOfLastInputCommand).Take(i - indexOfLastInputCommand)));
        indexOfLastInputCommand = i;
    }
}

if (digitValidators.Count != 14) throw new Exception("Domain knowledge, serial number is 14 digits long");

Dictionary<(int digit, long previousOutput), long?> memcache = new();

var maxNumber = SearchForMaxResult(0, 0, 0);

if (maxNumber == null) throw new Exception();

Console.WriteLine($"Part 1: {maxNumber}");

long? SearchForMaxResult(int digit, long previousOutput, long currentNumber)
{
    if (memcache.ContainsKey((digit, previousOutput)))
        return memcache[(digit, previousOutput)];

    if (digit == 14)
    {
        if (previousOutput == 0) 
            return currentNumber;
        else return null;
    }

    var validator = digitValidators[digit];

    for (int input = 9; input > 0; input--)
    {
        (_, _, _, long output) = validator.RunProgramForInput(0, 0, 0, previousOutput, new List<long> { input }.GetEnumerator());
        var newNumber = (currentNumber * 10) + input;
        var result = SearchForMaxResult(digit + 1, output, newNumber);

        if (result != null)
            return result;
    }

    memcache[(digit, previousOutput)] = null;

    return null;
}

static bool GetInstruction(string? input, out Instruction? value)
{
    value = null;

    if (input == null) return false;

    var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

    var type = parts[0] switch
    {
        "inp" => InstructionType.Input,
        "add" => InstructionType.Add,
        "mul" => InstructionType.Multiply,
        "div" => InstructionType.Divide,
        "mod" => InstructionType.Modulo,
        "eql" => InstructionType.Equals,
        _ => throw new Exception()
    };

    var register1 = parts[1][0];
    string? register2 = parts.Length == 3 ? parts[2] : null;

    if (type == InstructionType.Input &&
        register2 != null) throw new Exception();

    if (new[] { InstructionType.Add, InstructionType.Multiply, InstructionType.Divide, InstructionType.Modulo, InstructionType.Equals }.Contains(type) &&
        register2 == null) throw new Exception();

    value = new Instruction(type, register1, register2);

    return true;
}

enum InstructionType 
{ 
    Input,
    Add,
    Multiply,
    Divide,
    Modulo,
    Equals
}

record Instruction(InstructionType Type, char Register1, string? Register2);