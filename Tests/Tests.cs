#if TEST && ECS
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ECS
{
    public static class Tests
    {
        /// <summary>
        /// Timeout in Milliseconds until a task is deemed as finished.
        /// </summary>
        public const int Timeout = 1000;

        /// <summary>
        /// Interval in milliseconds at which the task will check its condition
        /// until <see cref="Timeout"/> is reached.
        /// </summary>
        public const int Interval = 100;

        /// <summary>
        /// Scene index containing <see cref="LifeCycleController"/>.
        /// </summary>
        private const string lifeCycleScene = "LifeCycle";

        /// <summary>
        /// Will load the Backend Secene and initialize backend service providers.
        /// </summary>
        public static async UniTask Setup()
        {
            Assert.AreEqual(true, Application.CanStreamedLevelBeLoaded(lifeCycleScene)
                , "SCENE IS NOT VALID, ADD IT IN BUILD SETTINGS!");

            if (!SceneManager.GetSceneByName(lifeCycleScene).IsValid())
            {
                await SceneManager.LoadSceneAsync(lifeCycleScene);
            };
        }
    }
}
#endif