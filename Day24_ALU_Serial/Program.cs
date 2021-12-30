var programLines = new InputProvider<Instruction?>("Input.txt", GetInstruction)
    .Where(w => w != null).Cast<Instruction>().ToList();

var fullSerialNumberValidator = new ALU(programLines);

var digitValidators = SplitInstructionIntoSubComputers(programLines);

// use memoization to prune dead branches.
// Fun fact, since part 1 and part 2 are working on the same search space it doesn't have to be cleared between runs, furhter speeding up part 2 (Unfortunately part2 was already way faster)
HashSet<(int digit, long previousOutput)> deadBranchLog = new();

var maxNumber = Search(0, 0, 0, 9, 0, w => w - 1);

if (maxNumber == null) throw new Exception();

//validate number using full ALU
(_, _, _, long outputPart1) = fullSerialNumberValidator.RunProgramForInput(maxNumber.ToString().ToCharArray().Select(w => (long)(w - '0')).GetEnumerator());

if (outputPart1 != 0) throw new Exception();

Console.WriteLine($"Part 1: {maxNumber}");

var minNumber = Search(0, 0, 0, 1, 10, w => w + 1);

if (minNumber == null) throw new Exception();

//validate number using full ALU
(_, _, _, long outputPart2) = fullSerialNumberValidator.RunProgramForInput(minNumber.ToString().ToCharArray().Select(w => (long)(w - '0')).GetEnumerator());

if (outputPart2 != 0) throw new Exception();

Console.WriteLine($"Part 2: {minNumber}");

long? Search(int digit, long previousOutput, long currentNumber, int inputStartValue, int inputEndValue, Func<int, int> inputIteratorFunc)
{
    if (deadBranchLog.Contains((digit, previousOutput)))
        return null;

    if (digit == 14)
    {
        if (previousOutput == 0) 
            return currentNumber;
        else return null;
    }

    var validator = digitValidators[digit];

    for (int input = inputStartValue; input != inputEndValue; input = inputIteratorFunc(input))
    {
        (_, _, _, long output) = validator.RunProgramForInput(0, 0, 0, previousOutput, new List<long> { input }.GetEnumerator());
        var newNumber = (currentNumber * 10) + input;
        var result = Search(digit + 1, output, newNumber, inputStartValue, inputEndValue, inputIteratorFunc);

        if (result != null)
            return result;
    }

    deadBranchLog.Add((digit, previousOutput));

    return null;
}

static ALU[] SplitInstructionIntoSubComputers(IReadOnlyList<Instruction> programLines)
{
    var computers = new List<ALU>();
    int indexOfLastInputCommand = 0;

    for (int i = 1; i <= programLines.Count; i++)
    {
        if (i == programLines.Count || programLines[i].Type == InstructionType.Input)
        {
            computers.Add(new ALU(programLines.Skip(indexOfLastInputCommand).Take(i - indexOfLastInputCommand)));
            indexOfLastInputCommand = i;
        }
    }

    if (computers.Count != 14) throw new Exception("Domain knowledge, serial number is 14 digits long");

    return computers.ToArray();
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