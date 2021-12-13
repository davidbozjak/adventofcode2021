class FoldableWorld : IWorld
{
    public IEnumerable<IWorldObject> WorldObjects => this.objects;
    private readonly List<PaperPoint> objects = new();

    public FoldableWorld(IEnumerable<PaperPoint> worldObjects)
    {
        this.objects.AddRange(worldObjects);
    }

    public void FoldX(int x)
    {
        var right = this.objects.Where(w => w.X > x);

        var pointsToAdd = new List<PaperPoint>();

        foreach (var point in right)
        {
            var newX = x - (point.X - x);

            pointsToAdd.Add(new PaperPoint(newX, point.Y));
        }

        this.objects.AddRange(pointsToAdd.Where(w => !this.objects.Any(ww => w.X == ww.X && w.Y == ww.Y)));
        this.objects.RemoveAll(w => right.Contains(w));
    }

    public void FoldY(int y)
    {
        var above = this.objects.Where(w => w.Y > y);

        var pointsToAdd = new List<PaperPoint>();

        foreach (var point in above)
        {
            var newY = y - (point.Y - y);

            pointsToAdd.Add(new PaperPoint(point.X, newY));
        }

        this.objects.AddRange(pointsToAdd.Where(w => !this.objects.Any(ww => w.X == ww.X && w.Y == ww.Y)));
        this.objects.RemoveAll(w => above.Contains(w));
    }
}