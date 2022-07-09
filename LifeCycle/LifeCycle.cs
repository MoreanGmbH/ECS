namespace Morean
{
    public class LifeCycle
    {
        private readonly Contexts contexts;

        /// <summary>
        /// All systems and sub systems.
        /// </summary>
        private readonly Entitas.Systems systems;

        private bool isTearingDown = false;

        /// <summary>
        /// Inject all features and systems
        /// </summary>
        public LifeCycle()
        {
            contexts = Contexts.sharedInstance;
            systems = new Features(contexts);
        }

        /// <summary>
        /// This calls Initialize() on all systems.
        /// </summary>
        public void Initialize()
        {
            isTearingDown = false;

            // Reactivate reactive systems in case of a LifyCycle restart
            systems.ActivateReactiveSystems();

            systems.Initialize();
        }

        /// <summary>
        /// This calls Execute() on all systems.
        /// </summary>
        public void Execute() => systems.Execute();

        /// <summary>
        /// This calls Cleanup() on all systems.
        /// </summary>
        public void Cleanup() => systems.Cleanup();

        /// <summary>
        /// Tear down life cycle by cleaning up systems, destroying entites and cleanning up contexts.
        /// </summary>
        public async void TearDown()
        {
            if (isTearingDown) return;

            isTearingDown = true;

            systems.TearDown();
            systems.DeactivateReactiveSystems();

            await System.Threading.Tasks.Task.Delay(1000);

            // Destroys all entities and resets creationIndex back to 0
            contexts.Reset();
        }
    }
}