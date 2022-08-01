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
            => ContextsData.AddRange(Context.DeserializeContexs(File.ReadAllText(EntitiesDataPath)));

        [PropertySpace(20), Button(ButtonSizes.Large)]
        [ShowIf(nameof(DataPathIsValid))]
        private void SaveEntities()
            => ContextsData.AddRange(Context.DeserializeContexs(File.ReadAllText(EntitiesDataPath)));

        #endregion

        #region Entities

        [PropertySpace(20)]
        [LabelText(nameof(Contexts))]
        [ValueDropdown(nameof(AvailableContexts), ExcludeExistingValuesInList = true, DrawDropdownForListElements = false)]
        public List<ContextData> ContextsData = new List<ContextData>();

        private ValueDropdownList<ContextData> AvailableContexts()
        {
            var contexts = Contexts.sharedInstance.allContexts
                .Where(context => context.contextInfo.componentTypes.Length > 0)
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