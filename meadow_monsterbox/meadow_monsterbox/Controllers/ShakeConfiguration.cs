using System;

namespace meadow_monsterbox.Controllers
{
    public class ShakeConfiguration
    {
        private static readonly Random _random = new Random();

        public int BeginIterations { get; set; } = 25;

        public int EndIterations { get; set; } = 50;

        public int GetIterations()
        {
            return _random.Next(BeginIterations, EndIterations);
        }

        public int BeginDelay { get; set; } = 50;

        public int EndDelay { get; set; } = 75;

        public int GetDelay()
        {
            return _random.Next(BeginDelay, EndDelay);
        }
    }
}