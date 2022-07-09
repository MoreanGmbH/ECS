namespace Morean
{
    public class LifeCycleController : UnityEngine.MonoBehaviour
    {
        private LifeCycle lifeCycle;

        /// <summary>
        /// Construct <see cref="LifeCycle"/>.
        /// </summary>
        private void Awake() => lifeCycle = new LifeCycle();

        /// <summary>
        /// Initialize <see cref="LifeCycle"/>.
        /// </summary>
        private void OnEnable() => lifeCycle.Initialize();

        /// <summary>
        /// Update <see cref="LifeCycle"/>.
        /// </summary>
        private void Update() => lifeCycle.Execute();

        /// <summary>
        /// Cleanup <see cref="LifeCycle"/>.
        /// </summary>
        private void LateUpdate() => lifeCycle.Cleanup();

        /// <summary>
        /// Tear down <see cref="LifeCycle"/>.
        /// </summary>
        private void OnDisable() => lifeCycle.TearDown();

        /// <summary>
        /// Tear down <see cref="LifeCycle"/>.
        /// </summary>
        private void OnDestroy() => lifeCycle.TearDown();

        /// <summary>
        /// Tear down <see cref="LifeCycle"/>.
        /// </summary>
        private void OnApplicationQuit() => lifeCycle.TearDown();
    } 
}