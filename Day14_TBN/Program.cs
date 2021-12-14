using System.Text;
using System.Text.RegularExpressions;

var input = new InputProvider<InsertionRule?>("Input.txt", GetInsertionRule)
    .Where(w => w != null).Cast<InsertionRule>().ToList();

var count = input.Count;

//string polymer = "NNCB";
string polymer = "OOBFPNOPBHKCCVHOBCSO";

for (int step = 0; step < 40; step++)
{
    var builder = new StringBuilder(polymer);

    foreach (var insertionRule in input)
    {
        //var indexes = new List<int>();
        bool hasFoundFirstIndex = false;
        for (int i = 0; i < builder.Length; i++)
        {
            if (builder[i] < 'A' || builder[i] > 'Z') continue;

            //if (indexes.Count == 1)
            if (hasFoundFirstIndex)
            {
                if (builder[i] == insertionRule.SecondChar)
                {
                    builder.Insert(i, insertionRule.InsertedChart);
                }
                //indexes.Clear();
                hasFoundFirstIndex = false;
            }

            //if (indexes.Count == 0 && builder[i] == insertionRule.FirstChar)
            if (!hasFoundFirstIndex && builder[i] == insertionRule.FirstChar)
            {
                //indexes.Add(i); 
                hasFoundFirstIndex = true;
            }
            else
            {
                //indexes.Clear();
                hasFoundFirstIndex = false;
            }
        }
    }

    polymer = builder.ToString().ToUpper();
    //Console.WriteLine($"After step {step + 1} length {polymer.Length}: {polymer}");
    Console.WriteLine($"After step {step + 1} length {polymer.Length}");
}

var letterFrequency = polymer.ToCharArray().GroupBy(w => w).ToDictionary(w => w.Key, w => w.Count());
Console.WriteLine($"Part 1: {letterFrequency.Values.Max() - letterFrequency.Values.Min()}");

static bool GetInsertionRule(string? input, out InsertionRule? value)
{
    value = null;

    if (input == null) return false;

    var parts = input.Split(" -> ", StringSplitOptions.RemoveEmptyEntries);

    if (parts.Length != 2) throw new Exception();

    //var repalcementString = parts[0][..1] + parts[1] + parts[0][1..];

    //value = new InsertionRule(parts[0], repalcementString);

    var letters = new Regex("[A-Z]").Matches(input).Select(w => w.Value[0]).ToList();

    if (letters.Count != 3) throw new Exception();

    value = new InsertionRule(letters[0], letters[1], (char)(letters[2] + 32));

    return true;
}

//record InsertionRule(string Base, string ReplacementString);
record InsertionRule(char FirstChar, char SecondChar, char InsertedChart);