namespace SmakenziBot.BuildOrder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BroodWar.Api;
    using Prerequisities;
    using Steps;
    using UnitType = BroodWar.Api.Enum.UnitType;

    public class BuildOrderScheduler
    {
        private readonly BuildOrderSteps _buildOrderSteps;
        private readonly StepExecutor _stepExecutor;
        private readonly IDictionary<Step, int> _lastExecutionFrame = new Dictionary<Step, int>(); // TODO: GROWS FOREVER
        private const int RetryIntervalFrames = 200;

        public BuildOrderScheduler(IObservable<UnitType> trainingStarted, IObservable<UnitType> constructionStarted, TerrainStrategy terrainStrategy)
        {
            _buildOrderSteps = new BuildOrderSteps(terrainStrategy);
            _stepExecutor = new StepExecutor(trainingStarted, constructionStarted, terrainStrategy);
        }

        public void OnFrame()
        {
            var (current, next) = _buildOrderSteps.Get;
            var canExecuteNext = CanExecuteNext(current, next);
            Check(current);
            if (canExecuteNext) Check(next);
        }

        private void Check(Step s)
        {
            if (!s.AllPrerequisitesMet()) return;
            var shouldRetry = ShouldRetry(s);
            if (IsExecuting(s) && !shouldRetry) return;

            var text = shouldRetry ? "Retrying" : "Executing";
            Game.Write($"{text} step: {s}");
            _stepExecutor.Execute(s);
            _lastExecutionFrame[s] = Game.FrameCount;
        }

        private bool ShouldRetry(Step s) => IsExecuting(s) && Game.FrameCount - _lastExecutionFrame[s] > RetryIntervalFrames;
        private bool IsExecuting(Step s) => _lastExecutionFrame.ContainsKey(s);

        private bool CanExecuteNext(Step current, Step next)
        {
            if (next is CancelConstructBuildingStep) return false;
            var currentResourcePrereqs = current.Prerequisites.OfType<ResourcePrerequisite>().FirstOrDefault();
            if (currentResourcePrereqs == null) return next.AllPrerequisitesMet();
            var nextResourcePrereqs = next.Prerequisites.OfType<ResourcePrerequisite>().FirstOrDefault();
            if (nextResourcePrereqs == null) return next.AllPrerequisitesMet();

            return HasEnoughResources(currentResourcePrereqs.Minerals + nextResourcePrereqs.Minerals,
                       currentResourcePrereqs.Gas + nextResourcePrereqs.Gas) && next.AllPrerequisitesMet();
        }

        private bool HasEnoughResources(int minerals, int gas) => Game.Self.Minerals >= minerals && Game.Self.Gas >= gas;
    }
}