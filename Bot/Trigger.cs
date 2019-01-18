namespace SmakenziBot
{
    using System;
    using System.Linq;
    using BroodWar.Api;
    using NBWTA.Utils;

    public class Trigger
    {
        private readonly Func<bool> _activation;
        private readonly Action _act;
        private readonly bool _triggerOnce;

        public Trigger(Func<bool> activation, Action act, bool triggerOnce = false)
        {
            _activation = activation;
            _act = act;
            _triggerOnce = triggerOnce;
        }

        public void OnFrame()
        {
            if (!_activation()) return;
            _act();
            if (_triggerOnce) IsCompleted = true;
        }

        public bool IsCompleted { get; private set; }
    }

    public abstract class BwAction
    {
        public abstract void Execute();
    }

    public class MoveUnits : BwAction
    {
        private readonly Func<Unit, bool> _predicate;
        private readonly int _count;
        private readonly Position _destination;

        public MoveUnits(Func<Unit, bool> predicate, int count, Position destination)
        {
            _predicate = predicate;
            _count = count;
            _destination = destination;
        }

        public override void Execute()
        {
            Game.Self.Units.Where(_predicate).Take(_count).ForEach(u => u.Move(_destination, false));
        }
    }
}