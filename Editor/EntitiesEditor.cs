#if ODIN_INSPECTOR && ECS
using System.IO;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System.Collections.Generic;
using Entitas;
using System;
using System.Linq;

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
        {
            var contexts = Context.DeserializeContexs(File.ReadAllText(EntitiesDataPath));
            foreach (var context in contexts)
            {
                var contextId = -1;
                for (int i = 0; i < ContextsData.Count; i++)
                {
                    if (context.Context == ContextsData[i].Context)
                    {
                        contextId = i;
                        break;
                    }
                }

                // Add entities to matching context
                if (contextId != -1)
                {
                    var contextData = ContextsData[contextId];
                    contextData.Entities = contextData.Entities.Union(context.Entities).ToArray();
                    ContextsData[contextId] = contextData;
                }
                else
                {
                    ContextsData.Add(context);
                }
            }
        }

        [PropertySpace(20), Button(ButtonSizes.Large)]
        [ShowIf(nameof(DataPathIsValid))]
        private void SaveEntities() { }

        #endregion

        #region Entities

        [PropertySpace(20)]
        [LabelText(nameof(Contexts))]
        [ValueDropdown(nameof(AvailableContexts), ExcludeExistingValuesInList = true, DrawDropdownForListElements = false)]
        public List<ContextData> ContextsData = new List<ContextData>();

        private ValueDropdownList<ContextData> AvailableContexts()
        {
            var contexts = Contexts.sharedInstance.allContexts
                // Skip existing contexts and those without any component
                .Where(context => !ContextsData.Any(contextData => contextData.Context == context.contextInfo.name)
                    && context.contextInfo.componentTypes.Length > 0)
                .Select(context => new ContextData()
                {
                    Context = context.contextInfo.name,
                    Entities = new IComponent[][]
                    {
                        new IComponent[] { (IComponent)Activator.CreateInstance(context.contextInfo.componentTypes[0]) }
                    }
                });

            var valueDropdownList = new ValueDropdownList<ContextData>();
            foreach (var context in contexts)
            {
                valueDropdownList.Add(context.Context, context);
            }
            return valueDropdownList;
        }

        #endregion
    }
}

#endif