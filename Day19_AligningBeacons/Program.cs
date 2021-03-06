var inputParser = new InputProvider<string?>("Input.txt", GetString) { EndAtEmptyLine = false };
var scannerParser = new MultiLineParser<ScannedWorld>(() => new ScannedWorld(), (world, value) => world.AddScannedRow(value));
var scanners = scannerParser.AddRange(inputParser);

var part1Stopwatch = System.Diagnostics.Stopwatch.StartNew();

var masterScanner = scanners[0];
var unmatched = scanners.Skip(1).ToList();

while (unmatched.Count > 0)
{
    foreach (var world in unmatched)
    {
        var result = masterScanner.ExtendWorld(world);
        if (result)
        {
            unmatched.Remove(world);
            break;
        }
    }
}
part1Stopwatch.Stop();

Console.WriteLine($"Part 1: {masterScanner.WorldObjects.Count()}, done in {part1Stopwatch.ElapsedMilliseconds}ms");

var part2Stopwatch = System.Diagnostics.Stopwatch.StartNew();

long maxManhattanDistance = long.MinValue;

foreach (var scanner1 in scanners)
{
    foreach (var scanner2 in scanners)
    {
        int distance = Math.Abs(scanner1.X - scanner2.X) + Math.Abs(scanner1.Y - scanner2.Y) + Math.Abs(scanner1.Z - scanner2.Z);

        if (distance > maxManhattanDistance)
        {
            maxManhattanDistance = distance;
        }
    }
}

part2Stopwatch.Stop();

Console.WriteLine($"Part 2: {maxManhattanDistance}, done in {part2Stopwatch.ElapsedMilliseconds}ms");

static bool GetString(string? input, out string? value)
{
    value = null;

    if (input == null) return false;

    value = input ?? string.Empty;

    return true;
}