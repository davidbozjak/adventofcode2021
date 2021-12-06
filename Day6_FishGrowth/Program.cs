using System.Diagnostics;

var parser = new SingleLineStringInputParser<int>(int.TryParse, str => str.Split(",", StringSplitOptions.RemoveEmptyEntries));
var input = new InputProvider<int>("Input.txt", parser.GetValue).ToList();

var ageDict = input.GroupBy(w => w).ToDictionary(w => w.Key, w => (long)w.Count());

for (int i = 8; i >= 0; i--)
{
    if (!ageDict.ContainsKey(i))
        ageDict[i] = 0;
}

var stopwatch = Stopwatch.StartNew();

int simulateFor = 256;
for (int day = 1; day <= simulateFor; day++)
{
    var multiPlyingGenerationSize = ageDict[0];
    var spawn = multiPlyingGenerationSize;
    
    for (int i = 0; i < 8; i++)
    {
        ageDict[i] = ageDict[i + 1];
    }

    ageDict[6] = ageDict[6] + multiPlyingGenerationSize;
    ageDict[8] = spawn;

    //Console.WriteLine($"School size after {day} days: {ageDict.Sum(w => w.Value)}");
}

stopwatch.Stop();

Console.WriteLine($"School size after {simulateFor} days: {ageDict.Sum(w => w.Value)}");
Console.WriteLine($"Elapsed miliseconds: {stopwatch.ElapsedMilliseconds} - {stopwatch.ElapsedTicks} ticks");