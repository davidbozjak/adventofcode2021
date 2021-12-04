class BingoBoard
{
    private readonly List<List<int>> board = new();
    private readonly List<int> calledNumbers = new();

    public void AddRow(string row)
    {
        var numbers = row.Split(" ", StringSplitOptions.RemoveEmptyEntries);
        board.Add(numbers.Select(w => int.Parse(w)).ToList());
    }

    public bool MarkNumber(int calledNumber)
    {
        calledNumbers.Add(calledNumber);

        return IsBoardComplete();
    }

    public bool IsBoardComplete()
    {
        foreach (var row in board)
        {
            bool isRowComplete = true;
            foreach (var number in row)
            {
                if (!calledNumbers.Contains(number))
                {
                    isRowComplete = false;
                    break;
                }
            }

            if (isRowComplete) return true;
        }

        for (int column = 0; column < board[0].Count; column++)
        {
            bool isColumnComplete = true;
            for (int row = 0; row < board[0].Count; row++)
            {
                if (!calledNumbers.Contains(board[row][column]))
                {
                    isColumnComplete = false;
                    break;
                }
            }

            if (isColumnComplete) return true;
        }

        return false;
    }

    public int SumUnmarkedNumbers()
    {
        return board
            .SelectMany(w => w)
            .Where(w => !calledNumbers.Contains(w))
            .Sum();
    }

    public void Reset()
    {
        this.calledNumbers.Clear();
    }
}