var programLines = new InputProvider<Instruction?>("Input.txt", GetInstruction)
    .Where(w => w != null).Cast<Instruction>().ToList();

var computer = new ALU(programLines);
var output = computer.RunProgramForInput(new List<int> { 1, 2, 3 }.GetEnumerator());

Console.WriteLine(programLines.Count);

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