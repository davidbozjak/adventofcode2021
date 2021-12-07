var parser = new SingleLineStringInputParser<int>(int.TryParse, str => str.Split(",", StringSplitOptions.RemoveEmptyEntries));
var input = new InputProvider<int>("Input.txt", parser.GetValue).ToList();

Dictionary<int, int> sumOfStepsToAlignForX = new();
Dictionary<int, int> sumOfFuelToAlignForX = new();

int min = input.Min();
int max = input.Max();

var stopwatch = System.Diagnostics.Stopwatch.StartNew();

for (int i = min; i <= max; i++)
{
    sumOfStepsToAlignForX[i] = input.Sum(x => Math.Abs(x - i));
    sumOfFuelToAlignForX[i] = input.Sum(x => linearSum(Math.Abs(x - i)));
}

stopwatch.Stop();

Console.WriteLine($"Part 1: {sumOfStepsToAlignForX.Values.Min()}");
Console.WriteLine($"Part 2: {sumOfFuelToAlignForX.Values.Min()}");
Console.WriteLine($"Duration: {stopwatch.ElapsedMilliseconds} ms");

int linearSum(int final)
{
    return final * (final + 1) / 2;
}