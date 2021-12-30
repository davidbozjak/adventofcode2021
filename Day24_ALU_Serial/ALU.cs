class ALU
{
    private readonly Instruction[] instructions;

    public ALU(IEnumerable<Instruction> instructions)
    {
        this.instructions = instructions.ToArray();
    }

    public (int w, int x, int y, int z) RunProgramForInput(IEnumerator<int> inputs)
    {
        int[] registers = new int[4];

        foreach (var instruction in instructions)
        {
            var register1Id = instruction.Register1 - 'w';
            var register2 = -1;
            var register2explicitValue = false;

            if (instruction.Register2 != null)
            {
                register2explicitValue = int.TryParse(instruction.Register2, out register2);

                if (!register2explicitValue)
                {
                    register2 = instruction.Register2[0] - 'w';
                }
            }

            switch (instruction.Type)
            {
                case InstructionType.Input:
                    HandleInputInstruction(ref registers[register1Id], inputs);
                    break;
                case InstructionType.Add:
                    HandleAddInstruction(ref registers[register1Id], register2explicitValue ? register2 : registers[register2]);
                    break;
                case InstructionType.Multiply:
                    HandleAddInstruction(ref registers[register1Id], register2explicitValue ? register2 : registers[register2]);
                    break;
                case InstructionType.Divide:
                    HandleDivideInstruction(ref registers[register1Id], register2explicitValue ? register2 : registers[register2]);
                    break;
                case InstructionType.Modulo:
                    HandleModuloInstruction(ref registers[register1Id], register2explicitValue ? register2 : registers[register2]);
                    break;
                case InstructionType.Equals:
                    HandleEqualizeInstruction(ref registers[register1Id], register2explicitValue ? register2 : registers[register2]);
                    break;
                default:
                    throw new Exception();
            }
        }

        return (registers[0], registers[1], registers[2], registers[3]);
    }

    private void HandleInputInstruction(ref int register, IEnumerator<int> inputs)
    {
        if (!inputs.MoveNext()) 
            throw new Exception("Expected more inputs than were provided");

        register = inputs.Current;
    }

    private void HandleAddInstruction(ref int register, int otherRegister)
    {
        register = register + otherRegister;
    }

    private void HandleMultiplyInstruction(ref int register, int otherRegister)
    {
        register = register * otherRegister;
    }

    private void HandleDivideInstruction(ref int register, int otherRegister)
    {
        register = register / otherRegister;
    }

    private void HandleModuloInstruction(ref int register, int otherRegister)
    {
        register = register % otherRegister;
    }

    private void HandleEqualizeInstruction(ref int register, int otherRegister)
    {
        register = register == otherRegister ? 1 : 0;
    }
}