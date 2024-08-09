using System;
using AStar;

namespace Sources
{
    [Serializable]
    public class Animal
    {
        public Position Position { get; set; }
        public Position Target { get; set; }

        public Animal(Position position, Position target)
        {
            Position = position;
            Target = target;
        }
    }
}