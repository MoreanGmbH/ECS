#if TEST && MOREAN_ECS
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Morean.ECS
{
    public static class TestExtensions
    {
        /// <summary>
        /// Timeout in Milliseconds until a task is deemed as finished.
        /// </summary>
        private const int timeout = 1000;

        /// <summary>
        /// Interval in milliseconds at which the task will check its condition
        /// until <see cref="timeout"/> is reached.
        /// </summary>
        private const int interval = 100;

        /// <summary>
        /// Scene index containing <see cref="LifeCycleController"/>.
        /// </summary>
        private const string lifeCycleScene = nameof(LifeCycle);

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

        /// <summary>
        /// Try validating <paramref name="condition"/> until <see cref="timeout"/>.
        /// </summary>
        /// <param name="condition"><see cref="delegate"/> that returns bool.</param>
        /// <param name="message">Message to show in case the <paramref name="condition"/> fails.</param>
        /// <returns>True if <paramref name="condition"/> is valid, False otherwise.</returns>
        public static async UniTask Validate(Func<bool> condition, string message)
        {
            var startTime = DateTime.Now;
            while (!condition())
            {
                await UniTask.Delay(interval);
                if ((DateTime.Now - startTime).TotalMilliseconds > timeout)
                {
                    break;
                }
            }
            Assert.IsTrue(condition(), message);
        }


        /// <summary>
        /// Try validating <paramref name="condition"/> until <see cref="timeout"/>.
        /// </summary>
        /// <param name="condition"><see cref="UniTask"/> <see cref="delegate"/> that returns bool.</param>
        /// <param name="message">Message to show in case the <paramref name="condition"/> fails.</param>
        /// <returns>True if <paramref name="condition"/> is valid, False otherwise.</returns>
        public static async UniTask Validate(Func<UniTask<bool>> condition, string message)
        {
            var startTime = DateTime.Now;
            var passed = await condition.Invoke();
            while (!passed)
            {
                await UniTask.Delay(interval);
                if ((DateTime.Now - startTime).TotalMilliseconds > timeout)
                {
                    break;
                }

                passed = await condition.Invoke();
            }
            Assert.IsTrue(passed, message);
        }
    }
}
#endif