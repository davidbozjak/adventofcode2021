var parser = new SingleLineStringInputParser<int>(int.TryParse, str => str.Split(",", StringSplitOptions.RemoveEmptyEntries));
var input = new InputProvider<int>("Input.txt", parser.GetValue).ToList();

var school = input.Select(w => new Lanterfish(w)).ToList();

int simulateFor = 256;
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
Console.ReadKey();

class Lanterfish
{
    public int Age { private set; get; }

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