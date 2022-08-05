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

        #region Load and Save Entities

        private string format = "json";

        [HorizontalGroup("Load Entities", order: 0)]
        [FilePath(Extensions = "$format"), PropertySpace, LabelWidth(70)]
        public string EntitiesFile;

        [HorizontalGroup("Load Entities", order: 0)]
        [Button, PropertySpace(SpaceAfter = 20)]
        [DisableIf(nameof(FileIsNotValid))]
        private void Load()
        {
            var contexts = Context.DeserializeContexs(File.ReadAllText(EntitiesFile));
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

        [PropertyOrder(1), Button(ButtonSizes.Large)]
        [DisableIf(nameof(DataIsNotValid))]
        private void Save()
        {
            var path = UnityEditor.EditorUtility.SaveFilePanel(
                title: $"Save Contexts and Entities as {format.ToUpper()}",
                directory: UnityEngine.Application.dataPath,
                defaultName: $"Entities.{format}",
                extension: format);
            if (string.IsNullOrEmpty(path)) return;

            File.WriteAllText(path, Context.SerializeContextsData(contextsData: ContextsData.ToArray()));
        }

        private bool FileIsNotValid()
            => string.IsNullOrEmpty(EntitiesFile)
            || !File.Exists(EntitiesFile);

        private bool DataIsNotValid()
            => ContextsData.Count < 1
            || !ContextsData.TrueForAll(contextData => contextData.IsValid());

        #endregion

        #region Entities

        [PropertySpace(20), PropertyOrder(2), LabelText(nameof(Contexts))]
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