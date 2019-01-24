namespace SmakenziBot.BuildOrder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BroodWar.Api;
    using NBWTA.Utils;
    using Prerequisities;
    using Steps;
    using UnitType = BroodWar.Api.Enum.UnitType;

    public class BuildOrderScheduler
    {
        private readonly Queue<Step> _buildOrderQueue = new Queue<Step>();
        private readonly BuildOrderStepsAdapter _buildOrderSteps;
        private readonly StepExecutor _stepExecutor;
        private readonly IDictionary<Step, int> _executingStepsFrame = new Dictionary<Step, int>();
        private IReadOnlyCollection<Step> ExecutingSteps => _executingStepsFrame.Keys.ToList();
        private bool IsExecuting(Step s) => _executingStepsFrame.ContainsKey(s);
        private const int RetryIntervalFrames = 200;
        private const int ParallelExecutionsCount = 3;
        private const int QueueCount = 5;

        public BuildOrderScheduler(IObservable<UnitType> trainingStarted, IObservable<UnitType> constructionStarted, AnalyzedMapExtra analyzedMapExtra, GameInfo gameInfo)
        {
            var buildOrder = new BuildOrderSteps(analyzedMapExtra);
            //var buildOrder = new BuildOrderZvT();
            _buildOrderSteps = new BuildOrderStepsAdapter(buildOrder);
            _stepExecutor = new StepExecutor(trainingStarted, constructionStarted, analyzedMapExtra, gameInfo);
        }

        public void OnFrame()
        {
            if (Game.FrameCount % 5 != 0) return;
            RemoveCompletedSteps();
            EnsureQueueCount();
            OrderExecution();
            OrderRetry();
        }

        private void RemoveCompletedSteps()
        {
            ExecutingSteps.Where(k => k.IsCompleted).ForEach(k => _executingStepsFrame.Remove(k));
        }

        private void EnsureQueueCount()
        {
            if (_buildOrderQueue.Count >= QueueCount) return;
            var nextStep = _buildOrderSteps.Get.current;
            if (_buildOrderQueue.Contains(nextStep) || IsExecuting(nextStep)) return;
            _buildOrderQueue.Enqueue(nextStep);
        }

        private void OrderExecution()
        {
            if (ExecutingSteps.Count >= ParallelExecutionsCount) return;
            if (!_buildOrderQueue.Any()) return;
            var nextStep = _buildOrderQueue.Peek();
            if (CanExecuteNextStep(ExecutingSteps, nextStep) && nextStep.AllPrerequisitesMet())
                Execute(_buildOrderQueue.Dequeue());
        }

        private void OrderRetry()
        {
            ExecutingSteps.Where(RetryFramesElapsed).ForEach(step => Execute(step, isRetry: true));
        }

        private void Execute(Step step, bool isRetry = false)
        {
            var action = isRetry ? "Retrying" : "Executing";
            Game.Write($"{action} step: {step}");
            _stepExecutor.Execute(step);
            _executingStepsFrame[step] = Game.FrameCount;
        }

        private bool RetryFramesElapsed(Step s) => Game.FrameCount - _executingStepsFrame[s] > RetryIntervalFrames;

        private bool CanExecuteNextStep(IEnumerable<Step> pendingSteps, Step next)
        {
            if (next is CancelConstructBuildingStep && ExecutingSteps.Any()) return false;
            var currentResourcePrereqs = pendingSteps.SelectMany(s => s.Prerequisites).OfType<ResourcePrerequisite>().ToList();
            if (!currentResourcePrereqs.Any()) return next.AllPrerequisitesMet();
            var nextResourcePrereqs = next.Prerequisites.OfType<ResourcePrerequisite>().FirstOrDefault();
            if (nextResourcePrereqs == null) return next.AllPrerequisitesMet();

            return HasEnoughResources(currentResourcePrereqs.Sum(r => r.Minerals) + nextResourcePrereqs.Minerals,
                       currentResourcePrereqs.Sum(r => r.Gas) + nextResourcePrereqs.Gas)
                   && next.AllPrerequisitesMet();
        }

        private static bool HasEnoughResources(int minerals, int gas) => Game.Self.Minerals >= minerals && Game.Self.Gas >= gas;
    }
}