#if ODIN_INSPECTOR && ECS
using System.IO;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System.Collections.Generic;
using Entitas;
using System;

namespace ECS
{
    public class EntitiesEditor : OdinEditorWindow
    {
        [UnityEditor.MenuItem("ECS/Entities Editor", priority = 1)]
        private static void OpenWindow()
        {
            var window = GetWindow<EntitiesEditor>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(700, 700);
        }

        #region Load Entities

        [FoldoutGroup("Load Entities"), PropertySpace]
        [FilePath(Extensions = ".json")]
        [InfoBox("Json file containing collection of contexts and their entities.", InfoMessageType.None)]
        public string EntitiesDataPath;

        private bool DataPathIsValid() => !string.IsNullOrEmpty(EntitiesDataPath);

        [FoldoutGroup("Load Entities"), PropertySpace(20), Button(ButtonSizes.Large)]
        [ShowIf(nameof(DataPathIsValid))]
        private void LoadEntities()
            => ContextsData.AddRange(Context.DeserializeContexs(File.ReadAllText(EntitiesDataPath)));

        [PropertySpace(20), Button(ButtonSizes.Large)]
        [ShowIf(nameof(DataPathIsValid))]
        private void SaveEntities()
            => ContextsData.AddRange(Context.DeserializeContexs(File.ReadAllText(EntitiesDataPath)));

        #endregion

        #region Entities

        [PropertySpace(20)]
        [ListDrawerSettings(CustomAddFunction = nameof(AddDefaultContext))]
        public List<ContextData> ContextsData = new List<ContextData>();

        private ContextData AddDefaultContext()
        {
            var contextData = new ContextData();

            if (Contexts.sharedInstance.allContexts.Length > 0)
            {
                // Add first context if any
                var firstContext = Contexts.sharedInstance.allContexts[0].contextInfo;
                contextData.Context = firstContext.name;
                // Create entity
                contextData.Entities = new IComponent[1][];
                // Add first component to entity if any
                if (firstContext.componentTypes.Length > 0)
                {
                    contextData.Entities[0] = new IComponent[]
                    {
                        (IComponent)Activator.CreateInstance(firstContext.componentTypes[0])
                    };
                };
            }

            return contextData;
        }

        #endregion
    }
}

#endif