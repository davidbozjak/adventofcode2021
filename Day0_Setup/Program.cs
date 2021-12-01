//Using AoC2017 Day1 as a test run

var parser = new SingleLineStringInputParser<int>(int.TryParse, str => str.ToCharArray().Select(w => w.ToString()).ToArray());
var input = new InputProvider<int>("Input.txt", parser.GetValue).ToList();

Console.WriteLine($"Part 1: {SumIfEqual(input)}");
Console.WriteLine($"Part 2: {SumIfEqual(input, input.Count / 2)}");

static int SumIfEqual(IList<int> input, int offset = 1)
{
    int sum = 0;

    for (int i = 0; i < input.Count; i++)
    {
        int indexToMatch = i + offset;
        indexToMatch = indexToMatch >= input.Count ? indexToMatch - input.Count : indexToMatch;

        if (input[i] == input[indexToMatch])
            sum += input[indexToMatch];
    }

    return sum;
}