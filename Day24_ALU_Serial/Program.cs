﻿var programLines = new InputProvider<Instruction?>("Input.txt", GetInstruction)
    .Where(w => w != null).Cast<Instruction>().ToList();

var fullSerialNumberValidator = new ALU(programLines);

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

// use memoization to prune dead branches and speed up computation
HashSet<(int digit, long previousOutput)> deadBranchLog = new();

var maxNumber = SearchForMaxResult(0, 0, 0);

if (maxNumber == null) throw new Exception();

//validate number using full ALU
(_, _, _, long outputPart1) = fullSerialNumberValidator.RunProgramForInput(0, 0, 0, 0, maxNumber.ToString().ToCharArray().Select(w => (long)(w - '0')).GetEnumerator());

if (outputPart1 != 0) throw new Exception();

Console.WriteLine($"Part 1: {maxNumber}");

deadBranchLog = new();

var minNumber = SearchForMinResult(0, 0, 0);

if (minNumber == null) throw new Exception();

//validate number using full ALU
(_, _, _, long outputPart2) = fullSerialNumberValidator.RunProgramForInput(0, 0, 0, 0, minNumber.ToString().ToCharArray().Select(w => (long)(w - '0')).GetEnumerator());

if (outputPart2 != 0) throw new Exception();

Console.WriteLine($"Part 2: {minNumber}");

long? SearchForMaxResult(int digit, long previousOutput, long currentNumber)
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

    for (int input = 9; input > 0; input--)
    {
        (_, _, _, long output) = validator.RunProgramForInput(0, 0, 0, previousOutput, new List<long> { input }.GetEnumerator());
        var newNumber = (currentNumber * 10) + input;
        var result = SearchForMaxResult(digit + 1, output, newNumber);

        if (result != null)
            return result;
    }

    deadBranchLog.Add((digit, previousOutput));

    return null;
}

long? SearchForMinResult(int digit, long previousOutput, long currentNumber)
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

    for (int input = 1; input <= 9; input++)
    {
        (_, _, _, long output) = validator.RunProgramForInput(0, 0, 0, previousOutput, new List<long> { input }.GetEnumerator());
        var newNumber = (currentNumber * 10) + input;
        var result = SearchForMinResult(digit + 1, output, newNumber);

        if (result != null)
            return result;
    }

    deadBranchLog.Add((digit, previousOutput));

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