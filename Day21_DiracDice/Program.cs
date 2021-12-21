
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