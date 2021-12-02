namespace Day2_Navigating
{
    public enum Directions { Forward, Down, Up }

    record class Instruction
    {
        public Directions Direction { get; init; }

        public int Steps { get; init; }
    }
}
