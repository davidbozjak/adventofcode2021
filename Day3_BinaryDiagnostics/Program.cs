var input = new InputProvider<string?>("Input.txt", GetString).Where(w => w != null).Cast<string>().ToList();

var length = input[0].Length;

var gammaBinary = new int[length];
var epsilonBinary = new int[length];

var oxygenNumbersToEliminate = input.ToList();
var co2NumbersToEliminate = input.ToList();

for (int i = 0; i < length; i++)
{
    int noOf0 = 0;
    int noOf1 = 0;

    foreach (var line in input)
    {
        if (line[i] == '0') noOf0++;
        else noOf1++;
    }

    if (noOf0 > noOf1)
    {
        gammaBinary[i] = 0;
        epsilonBinary[i] = 1;
    }
    else
    {
        gammaBinary[i] = 1;
        epsilonBinary[i] = 0;
    }
}

for (int i = 0; i < length && oxygenNumbersToEliminate.Count > 1; i++)
{
    int noOf0 = 0;
    int noOf1 = 0;

    foreach (var line in oxygenNumbersToEliminate)
    {
        if (line[i] == '0') noOf0++;
        else noOf1++;
    }

    var mostCommon = noOf0 > noOf1 ? 0 : 1;

    oxygenNumbersToEliminate =
            oxygenNumbersToEliminate.Where(w => w[i] == mostCommon.ToString()[0])
            .ToList();
}

for (int i = 0; i < length && co2NumbersToEliminate.Count > 1; i++)
{
    int noOf0 = 0;
    int noOf1 = 0;

    foreach (var line in co2NumbersToEliminate)
    {
        if (line[i] == '0') noOf0++;
        else noOf1++;
    }

    var leastCommon = noOf0 > noOf1 ? 1 : 0;

    co2NumbersToEliminate =
            co2NumbersToEliminate.Where(w => w[i] == leastCommon.ToString()[0])
            .ToList();
}

int gamma = GetIntFromBoolArray(gammaBinary);
int epsilon = GetIntFromBoolArray(epsilonBinary);
int oxygen = GetIntFromBoolArray(oxygenNumbersToEliminate[0].ToCharArray().Select(w => int.Parse(w.ToString())).ToArray());
int co2 = GetIntFromBoolArray(co2NumbersToEliminate[0].ToCharArray().Select(w => int.Parse(w.ToString())).ToArray());

Console.WriteLine($"Part 1: {gamma * epsilon}");
Console.WriteLine($"Part 2: {oxygen * co2}");

int GetIntFromBoolArray(int[] input)
{
    int result = 0;
    for (int i = input.Length - 1, pow = 0; i >= 0; i--, pow++)
    {
        result += input[i] * (int)Math.Pow(2, pow);
    }
    return result;
}

static bool GetString(string? input, out string value)
{
    value = input ?? string.Empty;

    return !string.IsNullOrWhiteSpace(input);
}