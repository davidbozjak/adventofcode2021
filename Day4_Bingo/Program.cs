var numbersInput = new InputProvider<string?>("Numbers.txt", GetString).ToList();
var numbersCalled = numbersInput[0].Split(",", StringSplitOptions.RemoveEmptyEntries)
    .Select(w => int.Parse(w))
    .ToList();

var boardsInputParser = new InputProvider<string?>("Boards.txt", GetString) { EndAtEmptyLine = false };
var boardsInput = boardsInputParser.ToList();
var boardsParser = new MultiLineParser<BingoBoard>(() => new BingoBoard(), (board, value) => board.AddRow(value));
var boards = boardsParser.AddRange(boardsInput);

Console.WriteLine($"Number of boards parsed: {boards.Count}");

CallNumbersUntilFirstBoardComplete(boards, numbersCalled);

foreach (var board in boards)
{
    board.Reset();
}

CallNumbersUntilLastBoardCompletes(boards, numbersCalled);

static void CallNumbersUntilFirstBoardComplete(IList<BingoBoard> boards, IList<int> numbersCalled)
{
    foreach (var number in numbersCalled)
    {
        foreach (var board in boards)
        {
            if (board.MarkNumber(number))
            {
                Console.WriteLine($"Part 1: {number * board.SumUnmarkedNumbers()}");
                return;
            }
        }
    }
}

static void CallNumbersUntilLastBoardCompletes(IList<BingoBoard> boards, IList<int> numbersCalled)
{
    var uncompletedBoards = boards.ToList();

    foreach (var number in numbersCalled)
    {
        foreach (var board in boards)
        {
            if (board.MarkNumber(number))
            {
                uncompletedBoards.Remove(board);

                if (uncompletedBoards.Count == 0)
                {
                    Console.WriteLine($"Part 2: {number * board.SumUnmarkedNumbers()}");
                    return;
                }
            }
        }
    }
}

static bool GetString(string? input, out string? value)
{
    value = null;

    if (input == null) return false;

    value = input ?? string.Empty;

    return true;
}