using System;

namespace meadow_monsterbox.Controllers
{
    internal abstract class InitalizedBaseController : IInitalize
    {
        protected bool initialized = false;

        public virtual void Initialize()
        {
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine($"{GetType().Name} is initialized.");
        }
    }

    internal interface IInitalize
    {
        void Initialize();
    }
}