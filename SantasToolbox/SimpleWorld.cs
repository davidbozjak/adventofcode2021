namespace SantasToolbox
{
    public class SimpleWorld<T> : IWorld
        where T : IWorldObject
    {
        public IEnumerable<IWorldObject> WorldObjects => this.worldObjects.Cast<IWorldObject>();

        private readonly List<T> worldObjects;

        public SimpleWorld(IEnumerable<T> objects)
        {
            this.worldObjects = objects.ToList();
        }
    }
}
