class ALU
{
    private readonly Instruction[] instructions;

    public ALU(IEnumerable<Instruction> instructions)
    {
        this.instructions = instructions.ToArray();
    }

    public (long w, long x, long y, long z) RunProgramForInput(long initialW, long initialX, long initialY, long initialZ, IEnumerator<long> inputs)
    {
        long[] registers = new[] { initialW, initialX, initialY, initialZ };

        Instruction? prevInstruction;
        int stepCounter = 0;
        foreach (var instruction in instructions)
        {
            stepCounter++;
            long register1Id = instruction.Register1 - 'w';
            long register2 = -1;
            var register2explicitValue = false;

            if (instruction.Register2 != null)
            {
                register2explicitValue = long.TryParse(instruction.Register2, out register2);

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
                    HandleMultiplyInstruction(ref registers[register1Id], register2explicitValue ? register2 : registers[register2]);
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

            prevInstruction = instruction;
        }

        return (registers[0], registers[1], registers[2], registers[3]);
    }

    private static void HandleInputInstruction(ref long register, IEnumerator<long> inputs)
    {
        if (!inputs.MoveNext()) 
            throw new Exception("Expected more inputs than were provided");

        register = inputs.Current;
    }

    private static void HandleAddInstruction(ref long register, long otherRegister)
    {
        register = register + otherRegister;
    }

    private static void HandleMultiplyInstruction(ref long register, long otherRegister)
    {
        register = register * otherRegister;
    }

    private static void HandleDivideInstruction(ref long register, long otherRegister)
    {
        register = register / otherRegister;
    }

    private static void HandleModuloInstruction(ref long register, long otherRegister)
    {
        register = register % otherRegister;
    }

    private static void HandleEqualizeInstruction(ref long register, long otherRegister)
    {
        register = register == otherRegister ? 1 : 0;
    }
}