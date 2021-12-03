var input = new InputProvider<string?>("Input.txt", GetString).Where(w => w != null).Cast<string>().ToList();

var length = input[0].Length;

var gammaBinary = new int[length];

for (int i = 0; i < length; i++)
{
    (int noOf0, int noOf1) = CountBitValuesInArrayAtIndex(i, input);

    gammaBinary[i] = noOf0 > noOf1 ? 0 : 1;
}

int gamma = GetIntFromBoolArray(gammaBinary);
int epsilon = GetIntFromBoolArray(InvertBoolArray(gammaBinary));

Console.WriteLine($"Part 1: {gamma * epsilon}");

var oxygenNumbersToEliminate = input.ToList();
var co2NumbersToEliminate = input.ToList();

var oxygenBinary = PruneListByBitwiseSelection(oxygenNumbersToEliminate, (noOf0, noOf1) => noOf0 > noOf1 ? 0 : 1);
int oxygen = GetIntFromBoolArray(oxygenBinary);

var co2Binary = PruneListByBitwiseSelection(oxygenNumbersToEliminate, (noOf0, noOf1) => noOf0 > noOf1 ? 1 : 0);
var co2 = GetIntFromBoolArray(co2Binary);

Console.WriteLine($"Part 2: {oxygen * co2}");

int[] PruneListByBitwiseSelection(IList<string> list, Func<int, int, int> criteriaSelector)
{
    for (int i = 0; i < length && list.Count > 1; i++)
    {
        (int noOf0, int noOf1) = CountBitValuesInArrayAtIndex(i, list);

        var valueToMatch = criteriaSelector(noOf0, noOf1) == 1 ? '1' : '0';

        list = list.Where(w => w[i] == valueToMatch)
                .ToList();
    }

    if (list.Count != 1) throw new Exception();

    return list[0].ToCharArray().Select(w => int.Parse(w.ToString())).ToArray();
}

(int noOf0, int noOf1) CountBitValuesInArrayAtIndex(int index, IList<string> list)
{
    int noOf1 = list.Select(w => w[index] == '1' ? 1 : 0).Sum();
    int noOf0 = list.Count - noOf1;

    return (noOf0, noOf1);
}

int[] InvertBoolArray(int[] input)
{
    var result = new int[input.Length];

    for (int i = 0; i < input.Length; i++)
    {
        result[i] = input[i] == 1 ? 0 : 1;
    }

    return result;
}

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