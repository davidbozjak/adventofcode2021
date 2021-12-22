
var die = new DeterministicDie(100);
var player1 = new Player(1, 4, die);
var player2 = new Player(2, 5, die);

var players = new[] { player1, player2 };
int currentPlayerIndex = 0;
var currentPlayer = players[currentPlayerIndex];

while (players.All(w => w.Score < 1000))
{
    currentPlayer.Move();
    //Console.WriteLine(currentPlayer);

    currentPlayerIndex++;
    if (currentPlayerIndex >= players.Length)
        currentPlayerIndex = 0;
    currentPlayer = players[currentPlayerIndex];
}

Console.WriteLine($"Part 1: {currentPlayer.Score * die.NumberOfRolls}");


Dictionary<(int positionPlayer1, int scorePlayer1, int positionPlayer2, int scorePlayer2, int currentPlayer, int noOfThrowsRemaining), (long winPlayer1, long winPlayer2)> knownStates = new();

var scores = DiracPlay(4, 0, 5, 0, 0, 3);

Console.WriteLine($"Part 2: {scores.winPlayer1} vs {scores.winPlayer2} wins. Max: {Math.Max(scores.winPlayer1, scores.winPlayer2)}");

(long winPlayer1, long winPlayer2) DiracPlay(int positionPlayer1, int scorePlayer1, int positionPlayer2, int scorePlayer2, int currentPlayer, int noOfThrowsRemaining)
{
    if (knownStates.ContainsKey((positionPlayer1, scorePlayer1, positionPlayer2, scorePlayer2, currentPlayer, noOfThrowsRemaining)))
        return knownStates[(positionPlayer1, scorePlayer1, positionPlayer2, scorePlayer2, currentPlayer, noOfThrowsRemaining)];

    long winPlayer1 = 0, winPlayer2 = 0;

    if (noOfThrowsRemaining == 0)
    {
        if (currentPlayer == 0)
        {
            scorePlayer1 += positionPlayer1;

            if (scorePlayer1 >= 21)
            {
                return (1, 0);
            }

            currentPlayer = 1;
        }
        else
        {
            scorePlayer2 += positionPlayer2;

            if (scorePlayer2 >= 21)
            {
                return (0, 1);
            }

            currentPlayer = 0;
        }

        noOfThrowsRemaining = 3;
    }

    if (currentPlayer == 0)
    {
        var positionAfter1 = AdjustPosition(positionPlayer1, 1);
        var resultAfterThrow1 = DiracPlay(positionAfter1, scorePlayer1, positionPlayer2, scorePlayer2, currentPlayer, noOfThrowsRemaining - 1);

        var positionAfter2 = AdjustPosition(positionPlayer1, 2);
        var resultAfterThrow2 = DiracPlay(positionAfter2, scorePlayer1, positionPlayer2, scorePlayer2, currentPlayer, noOfThrowsRemaining - 1);

        var positionAfter3 = AdjustPosition(positionPlayer1, 3);
        var resultAfterThrow3 = DiracPlay(positionAfter3, scorePlayer1, positionPlayer2, scorePlayer2, currentPlayer, noOfThrowsRemaining - 1);

        winPlayer1 = resultAfterThrow1.winPlayer1 + resultAfterThrow2.winPlayer1 + resultAfterThrow3.winPlayer1;
        winPlayer2 = resultAfterThrow1.winPlayer2 + resultAfterThrow2.winPlayer2 + resultAfterThrow3.winPlayer2;

        knownStates[(positionPlayer1, scorePlayer1, positionPlayer2, scorePlayer2, currentPlayer, noOfThrowsRemaining)] = (winPlayer1, winPlayer2);

        return (winPlayer1, winPlayer2);
    }
    else
    {
        var positionAfter1 = AdjustPosition(positionPlayer2, 1);
        var resultAfterThrow1 = DiracPlay(positionPlayer1, scorePlayer1, positionAfter1, scorePlayer2, currentPlayer, noOfThrowsRemaining - 1);

        var positionAfter2 = AdjustPosition(positionPlayer2, 2);
        var resultAfterThrow2 = DiracPlay(positionPlayer1, scorePlayer1, positionAfter2, scorePlayer2, currentPlayer, noOfThrowsRemaining - 1);

        var positionAfter3 = AdjustPosition(positionPlayer2, 3);
        var resultAfterThrow3 = DiracPlay(positionPlayer1, scorePlayer1, positionAfter3, scorePlayer2, currentPlayer, noOfThrowsRemaining - 1);

        winPlayer1 = resultAfterThrow1.winPlayer1 + resultAfterThrow2.winPlayer1 + resultAfterThrow3.winPlayer1;
        winPlayer2 = resultAfterThrow1.winPlayer2 + resultAfterThrow2.winPlayer2 + resultAfterThrow3.winPlayer2;

        knownStates[(positionPlayer1, scorePlayer1, positionPlayer2, scorePlayer2, currentPlayer, noOfThrowsRemaining)] = (winPlayer1, winPlayer2);

        return (winPlayer1, winPlayer2);
    }

    static int AdjustPosition(int initialPosition, int thrownValue)
    {
        var newPosition = initialPosition + thrownValue;
        while (newPosition > 10)
        {
            newPosition -= 10;
        }

        return newPosition;
    }
}

class DeterministicDie
{
    private int currentValue = 0;
    private readonly int noOfSides;

    public int NumberOfRolls { get; private set; }

    public DeterministicDie(int noOfSides)
    {
        this.noOfSides = noOfSides;
        this.currentValue = 1;
    }

    public int Roll()
    {
        var value = this.currentValue;

        currentValue++;
        if (currentValue > noOfSides)
           this.currentValue = 1;

        NumberOfRolls++;

        return value;
    }
}

class Player
{
    public int Position { get; private set; }

    public int Score { get; private set; }

    public int Id { get; init; }

    private readonly DeterministicDie die;

    public Player(int id, int position, DeterministicDie die)
    {
        this.Id = id;
        this.Position = position;
        this.die = die;
        this.Score = 0;
    }

    public void Move()
    {
        var newPosition = this.Position;

        for (int i = 0; i < 3; i++)
        {
            newPosition += die.Roll();
            while (newPosition > 10)
            {
                newPosition = newPosition - 10;
            }
        }

        if (newPosition < 0) throw new Exception();
        if (newPosition > 10) throw new Exception();

        this.Score += newPosition;
        this.Position = newPosition;
    }

    public override string ToString()
    {
        return $"Player {Id} Score: {Score}";
    }
}