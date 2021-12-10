var input = new InputProvider<string>("Input.txt", GetString).ToList();

var corruptedScore = new Dictionary<char, int>()
{
    { ')', 3 },
    { ']', 57 },
    { '}', 1197 },
    { '>', 25137 },
};

var missingScore = new Dictionary<char, int>()
{
    { ')', 1 },
    { ']', 2 },
    { '}', 3 },
    { '>', 4 },
};

var sumCorrupterChars = 0;
var missingScores = new List<long>();

foreach (var line in input)
{
    var stack = new Stack<char>();
    var corrputedChar = GetCorruptedCharacter(line, stack);

    if (corrputedChar > 0)
    {
        sumCorrupterChars += corruptedScore[corrputedChar];
    }
    else
    {
        long score = 0;

        foreach (char c in stack.ToArray())
        {
            var missing = c switch
            {
                '(' => ')',
                '[' => ']',
                '{' => '}',
                '<' => '>',
                _ => throw new Exception()
            };

            score = score * 5 + missingScore[missing];
        }

        missingScores.Add(score);
    }
}

Console.WriteLine($"Part 1: {sumCorrupterChars}");
Console.WriteLine($"Part 2: {missingScores.OrderBy(w => w).Skip(missingScores.Count / 2).First()}");

static char GetCorruptedCharacter(string input, Stack<char> chunks)
{
    if (input.Length == 0) return (char)0;

    var next = input[0];

    if (next == '(' || next == '[' ||next == '{' || next == '<')
    {
        chunks.Push(next);        
    }

    if (next == ')' || next == ']' || next == '}' || next == '>')
    {
        var mostRecentChunk = chunks.Pop();

        if ((mostRecentChunk == '(' && next != ')') ||
            (mostRecentChunk == '[' && next != ']') ||
            (mostRecentChunk == '{' && next != '}') ||
            (mostRecentChunk == '<' && next != '>'))
            return next;
    }

    return GetCorruptedCharacter(input[1..], chunks);
}

static bool GetString(string? input, out string? value)
{
    value = null;

    if (input == null) return false;

    value = input ?? string.Empty;

    return true;
}