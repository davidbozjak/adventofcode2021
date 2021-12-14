using System.Text.RegularExpressions;

var input = new InputProvider<InsertionRule?>("Input.txt", GetInsertionRule)
    .Where(w => w != null).Cast<InsertionRule>().ToList();

var count = input.Count;

string polymer = "OOBFPNOPBHKCCVHOBCSO";

var dict = new Dictionary<string, long>();
var letterFrequency = polymer.ToCharArray().GroupBy(w => w).ToDictionary(w => w.Key, w => (long)w.Count());

for (int i = 1; i < polymer.Length; i++)
{
    var key = $"{polymer[i - 1]}{polymer[i]}";
    if (!dict.ContainsKey(key))
        dict[key] = 1;
    else dict[key]++;
}

for (int step = 0; step < 40; step++)
{
    var nextStepDict = new Dictionary<string, long>();

    foreach (var rule in input)
    {
        var key = $"{rule.FirstChar}{rule.SecondChar}";

        if (!dict.ContainsKey(key)) continue;

        var newKey1 = $"{rule.FirstChar}{rule.InsertedChar}";
        var newKey2 = $"{rule.InsertedChar}{rule.SecondChar}";

        if (!nextStepDict.ContainsKey(newKey1)) nextStepDict[newKey1] = 0;
        if (!nextStepDict.ContainsKey(newKey2)) nextStepDict[newKey2] = 0;

        nextStepDict[newKey1] += dict[key];
        nextStepDict[newKey2] += dict[key];

        if (!letterFrequency.ContainsKey(rule.InsertedChar)) letterFrequency[rule.InsertedChar] = 0;
        letterFrequency[rule.InsertedChar] += dict[key];
    }

    dict = nextStepDict;
    Console.WriteLine($"Step {step + 1}: Letter frequency Max : {letterFrequency.Values.Max()} Min: {letterFrequency.Values.Min()} Diff: {letterFrequency.Values.Max() - letterFrequency.Values.Min()}");
}

static bool GetInsertionRule(string? input, out InsertionRule? value)
{
    value = null;

    if (input == null) return false;

    var parts = input.Split(" -> ", StringSplitOptions.RemoveEmptyEntries);

    if (parts.Length != 2) throw new Exception();

    var letters = new Regex("[A-Z]").Matches(input).Select(w => w.Value[0]).ToList();

    if (letters.Count != 3) throw new Exception();

    value = new InsertionRule(letters[0], letters[1], letters[2]);

    return true;
}

record InsertionRule(char FirstChar, char SecondChar, char InsertedChar);