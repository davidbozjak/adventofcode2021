var input = new InputProvider<InputOutputPair?>("Input.txt", ParseRow)
    .Where(w => w != null).Cast<InputOutputPair>().ToList();

int countOfOneOfTheUniqueOutputs = 0;

foreach (var pair in input)
{
    foreach (var output in pair.Outputs)
    {
        if (output.Length == 2 || // renders 1
            output.Length == 4 || // renders 4
            output.Length == 3 || // renders 7
            output.Length == 7)   // renders 8
            countOfOneOfTheUniqueOutputs++;
    }
}

Console.WriteLine($"Part 1: {countOfOneOfTheUniqueOutputs} occurances of 1/4/7/8");

// permanent list to be used for removal by elimination!
var charList = new List<char>() { 'a', 'b', 'c', 'd', 'e', 'f', 'g' };
var possibleCombinations = charList.GetAllOrdersOfList()
    .Select(w => new string(w.ToArray()))
    .ToList();

int sumOfScreenValues = 0;

foreach (var pair in input)
{
    var remainingCombinations = possibleCombinations.ToList();

    var allSegments = pair.Inputs.Concat(pair.Outputs).ToList();

    foreach (var segment in allSegments)
    {
        if (segment.Length == 2)
        {
            // must be digit 1
            remainingCombinations = remainingCombinations.Where(w => segment[0] == w[2] || segment[1] == w[2]).ToList();
            remainingCombinations = remainingCombinations.Where(w => segment[0] == w[5] || segment[1] == w[5]).ToList();
                                                                      
        }                                                             
        else if (segment.Length == 4)                                 
        {        
            // must be digit 4
            remainingCombinations = remainingCombinations.Where(w => segment[0] == w[1] || segment[1] == w[1] || segment[2] == w[1] || segment[3] == w[1]).ToList();
            remainingCombinations = remainingCombinations.Where(w => segment[0] == w[2] || segment[1] == w[2] || segment[2] == w[2] || segment[3] == w[2]).ToList();
            remainingCombinations = remainingCombinations.Where(w => segment[0] == w[3] || segment[1] == w[3] || segment[2] == w[3] || segment[3] == w[3]).ToList();
            remainingCombinations = remainingCombinations.Where(w => segment[0] == w[5] || segment[1] == w[5] || segment[2] == w[5] || segment[3] == w[5]).ToList();
        }                                                                                                        
        else if (segment.Length == 3)                                                                            
        {     
            // must be digit 7
            remainingCombinations = remainingCombinations.Where(w => segment[0] == w[0] || segment[1] == w[0] || segment[2] == w[0]).ToList();
            remainingCombinations = remainingCombinations.Where(w => segment[0] == w[2] || segment[1] == w[2] || segment[2] == w[2]).ToList();
            remainingCombinations = remainingCombinations.Where(w => segment[0] == w[5] || segment[1] == w[5] || segment[2] == w[5]).ToList();
        }
        else if (segment.Length == 5)
        {
            //can be 2, 3, 5   
            remainingCombinations = remainingCombinations.Where(w => 
                (!segment.Contains(w[1]) && !segment.Contains(w[5])) || // special 2
                (!segment.Contains(w[1]) && !segment.Contains(w[4])) || //special 3
                (!segment.Contains(w[2]) && !segment.Contains(w[4])) // special 5
                ).ToList();
        }
        else if (segment.Length == 6)
        {
            //can be 0, 6, 9
            remainingCombinations = remainingCombinations.Where(w =>
                !segment.Contains(w[3]) || // special 0
                !segment.Contains(w[2]) || //special 6
                !segment.Contains(w[4]) // special 9
                ).ToList();
        }
    }

    if (remainingCombinations.Count > 1) 
        throw new Exception();

    var code = remainingCombinations[0];
    var digits = new Dictionary<string, int>
    {
        {GetStringFromChars(code[0], code[1], code[2], code[4], code[5], code[6]), 0},
        {GetStringFromChars(code[2], code[5]), 1},
        {GetStringFromChars(code[0], code[2], code[3], code[4], code[6]), 2},
        {GetStringFromChars(code[0], code[2], code[3], code[5], code[6]), 3},
        {GetStringFromChars(code[1], code[2], code[3], code[5]), 4},
        {GetStringFromChars(code[0], code[1], code[3], code[5], code[6]), 5},
        {GetStringFromChars(code[0], code[1], code[3], code[4], code[5], code[6]), 6},
        {GetStringFromChars(code[0], code[2], code[5]), 7},
        {GetStringFromChars(code[0], code[1], code[2], code[3], code[4], code[5], code[6]), 8},
        {GetStringFromChars(code[0], code[1], code[2], code[3], code[5], code[6]), 9},
    };

    int value = 0;

    for (int i = pair.Outputs.Length - 1; i >= 0; i--)
    {
        var output = pair.Outputs[i];

        string digitKey = digits.Keys
            .Where(w => w.Length == output.Length)
            .Where(w => w.All(ww => output.Contains(ww))).First();
        int digit = digits[digitKey];

        digit *= (int)Math.Pow(10, pair.Outputs.Length - 1 - i);

        value += digit;
    }

    sumOfScreenValues += value;
}

Console.WriteLine($"Part 2: Sum of all outputs is {sumOfScreenValues}");

static string GetStringFromChars(params char[] chars)
{
    return new string(chars);
}

static bool ParseRow(string? input, out InputOutputPair? value)
{
    value = null;

    if (string.IsNullOrWhiteSpace(input)) return false;

    var inputOutput = input.Split('|', StringSplitOptions.RemoveEmptyEntries);

    var inputSegments = inputOutput[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);
    var outputSegments = inputOutput[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);

    value = new InputOutputPair(inputSegments, outputSegments);

    return true;
}

class InputOutputPair
{
    public string[] Inputs { get; init; }
    public string[] Outputs { get; init; }

    public InputOutputPair(string[] inputs, string[] outputs)
    {
        this.Inputs = inputs;
        this.Outputs = outputs;
    }
}