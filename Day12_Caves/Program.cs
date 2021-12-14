var input = new InputProvider<Link?>("Input.txt", ParseLink).Where(w => w != null).Cast<Link>().ToList();
var factory = new UniqueFactory<string, Cave>(name => new Cave(name));

foreach (var link in input)
{
    var startCave = factory.GetOrCreateInstance(link.Start);
    var endCave = factory.GetOrCreateInstance(link.End);

    startCave.AddNeighbour(endCave);
    endCave.AddNeighbour(startCave);
}

var start = factory.AllCreatedInstances.Where(w => w.Name == "start").First();
var end = factory.AllCreatedInstances.Where(w => w.Name == "end").First();

var part1Stopwatch = System.Diagnostics.Stopwatch.StartNew();

var allPaths = new List<List<Cave>>();
Explore(start, end, new List<Cave>(), allPaths, false);
part1Stopwatch.Stop();

Console.WriteLine($"Part 1: {allPaths.Count} in {part1Stopwatch.ElapsedMilliseconds} ms");

var part2Stopwatch = System.Diagnostics.Stopwatch.StartNew();

var allPathsWithAdvancedRules = new List<List<Cave>>();
Explore(start, end, new List<Cave>(), allPathsWithAdvancedRules, true);
part2Stopwatch.Stop();

Console.WriteLine($"Part 2: {allPathsWithAdvancedRules.Count} in {part2Stopwatch.ElapsedMilliseconds} ms");

void Explore(Cave current, Cave endNode, List<Cave> path, List<List<Cave>> completedPaths, bool enhancedRules)
{
    if (path.Contains(current) && !current.IsBigCave)
    {
        if (!enhancedRules) return;

        if (current == start) return;
        if (current == end) return;

        if (path.Where(w => !w.IsBigCave).GroupBy(w => w).Select(w => w.Count()).Any(w => w > 1)) 
            return;
    }

    path = path.ToList(); // make a copy to not ruin backtracking
    path.Add(current);

    if (current == endNode)
    {
        if (!completedPaths.Any(w => ArePathsTheSame(path, w)))
        {
            completedPaths.Add(path);
        }

        return;
    }

    foreach (var neighbour in current.Neighbours)
    {
        Explore(neighbour, endNode, path, completedPaths, enhancedRules);
    }
}

static bool ArePathsTheSame(List<Cave> path1, List<Cave> path2)
{
    if (path1.Count != path2.Count) return false;

    for (int i = 0; i < path1.Count; i++)
    {
        if (path1[i] != path2[i]) return false;
    }

    return true;
}

static bool ParseLink(string? input, out Link? value)
{
    value = null;

    if (string.IsNullOrWhiteSpace(input)) return false;

    var parts = input.Split('-', StringSplitOptions.RemoveEmptyEntries);
    value = new Link(parts[0], parts[1]);

    return true;
}

record Link(string Start, string End);