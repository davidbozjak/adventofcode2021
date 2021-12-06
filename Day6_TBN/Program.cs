var parser = new SingleLineStringInputParser<int>(int.TryParse, str => str.Split(",", StringSplitOptions.RemoveEmptyEntries));
var input = new InputProvider<int>("Input.txt", parser.GetValue).ToList();

var school = input.Select(w => new Lanterfish(w)).ToList();

//NaiveSimulatePart1();

var ageDict = school.GroupBy(w => w.Age).ToDictionary(w => w.Key, w => (long)w.Count());

for (int i = 8; i >= 0; i--)
{
    if (!ageDict.ContainsKey(i))
        ageDict[i] = 0;
}

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

    Console.WriteLine($"School size after {day} days: {ageDict.Sum(w => w.Value)}");
}

Console.WriteLine($"Part 2: {school.Count} after {simulateFor} days");

void NaiveSimulatePart1()
{
    int simulateFor = 80;
    for (int day = 0; day < simulateFor; day++)
    {
        List<Lanterfish> spawn = new();

        foreach (var fish in school)
        {
            var newFish = fish.SimulateDay();

            if (newFish != null)
            {
                spawn.Add(newFish);
            }
        }

        school.AddRange(spawn);
        Console.WriteLine($"School size after {day} days: {school.Count}");
    }

    Console.WriteLine($"Part 1: {school.Count} after {simulateFor} days");
}

class Lanterfish
{
    public int Age { set; get; }

    public Lanterfish(int initialAge)
    {
        Age = initialAge;
    }

    public Lanterfish? SimulateDay()
    {
        this.Age--;

        if (this.Age < 0)
        {
            this.Age = 6;
            return new Lanterfish(8);
        }

        return null;
    }
}