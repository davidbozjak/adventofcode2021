var parser = new SingleLineStringInputParser<int>(int.TryParse, str => str.Split(",", StringSplitOptions.RemoveEmptyEntries));
var input = new InputProvider<int>("Input.txt", parser.GetValue).ToList();

Dictionary<int, int> sumOfStepsToAlignForX = new();
Dictionary<int, int> sumOfFuelToAlignForX = new();

int min = input.Min();
int max = input.Max();

for (int i = min; i <= max; i++)
{
    sumOfStepsToAlignForX[i] = input.Sum(x => Math.Abs(x - i));
    sumOfFuelToAlignForX[i] = input.Sum(x => linearSum(Math.Abs(x - i)));
}

Console.WriteLine($"Part 1: {sumOfStepsToAlignForX.Values.Min()}");
Console.WriteLine($"Part 2: {sumOfFuelToAlignForX.Values.Min()}");

int linearSum(int final)
{
    int sum = 0;
    for (int i = 0; i <= final; i++)
        sum += i;
    return sum;
}