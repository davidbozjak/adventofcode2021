var numbers = new InputProvider<PairNumber?>("Input.txt", GetPairNumber)
    .Where(w => w != null)
    .Cast<PairNumber>().ToList();

var addedNumber = numbers[0];

foreach (var number in numbers.Skip(1))
{
    addedNumber = addedNumber + number;
}

Console.WriteLine($"Part 1: {addedNumber.GetMagnitude()}");

long maxValue = long.MinValue;

int numberCount = numbers.Count;

for (int i = 0; i < numberCount; i++)
{
    for (int j = 0; j < numberCount; j++)
    {
        if (i == j) continue;

        var freshNumbers = new InputProvider<PairNumber?>("Input.txt", GetPairNumber)
            .Where(w => w != null)
            .Cast<PairNumber>().ToList();

        var number1 = freshNumbers[i];
        var number2 = freshNumbers[j];

        var addition = number1 + number2;
        var magnitudeAfterAddition = addition.GetMagnitude();

        if (magnitudeAfterAddition > maxValue)
        {
            maxValue = magnitudeAfterAddition;
        }
    }
}

Console.WriteLine($"Part 2: Max: {maxValue}");

static bool GetPairNumber(string? input, out PairNumber? value)
{
    value = null;

    if (string.IsNullOrWhiteSpace(input)) return false;

    value = new PairNumber(input, null);

    return true;
}