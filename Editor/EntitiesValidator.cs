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
using Newtonsoft.Json;

namespace ECS
{
    public class EntitiesValidator : OdinEditorWindow
    {
        [UnityEditor.MenuItem("ECS/Entities Validator", priority = 1)]
        private static void OpenWindow()
        {
            var window = GetWindow<EntitiesValidator>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(700, 700);
        }

        #region Validate Entities

        private string format = "json";

        [HorizontalGroup("Validate Entities", order: 0)]
        [FilePath(Extensions = "$format"), PropertySpace, LabelWidth(70)]
        public string EntitiesFile;

        private static readonly JsonSerializerSettings serializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
        };

        [HorizontalGroup("Validate Entities", order: 0)]
        [Button, PropertySpace(SpaceAfter = 10)]
        [DisableIf(nameof(FileIsNotValid))]
        private void Validate()
        {
            FileIsValid.Valid = true;
            ContextsAreValid.Valid = true;
            EntitiesAreValid.Valid = true;
            ComponentsAreValid.Valid = true;

            ContextData[] contexts;

            // Verify that the file can be deserialized
            try
            {
                contexts = JsonConvert.DeserializeObject<ContextData[]>(File.ReadAllText(EntitiesFile), serializerSettings);
            }
            catch (Exception exception)
            {
                FileIsValid.Issue = $"File [{EntitiesFile}] is not valid: {exception.Message}";
                FileIsValid.Valid = false;
                return;
            }

            for (int c = 0; c < contexts.Length; c++)
            {
                var context = contexts[c];
                // Verify that Context is valid
                if (string.IsNullOrEmpty(context.Context))
                {
                    ContextsAreValid.Issue = $"Context {context.Context} at position [{c}] is not valid!";
                    ContextsAreValid.Valid = false;
                    return;
                }
                // Verify that Context exists
                if (Context.GetContext(context.Context) == null)
                {
                    ContextsAreValid.Issue = $"Context {context.Context} at position [{c}] doesn't exist!";
                    ContextsAreValid.Valid = false;
                    return;
                };

                // Verify that Entities are valid
                if (context.Entities == null)
                {
                    EntitiesAreValid.Issue = $"Context [{context.Context}] has invalid Entities!";
                    EntitiesAreValid.Valid = false;
                    return;
                }

                // Verify that there are Entities
                if (context.Entities.Length < 1)
                {
                    EntitiesAreValid.Issue = $"Context [{context.Context}] doesn't have any Entity!";
                    EntitiesAreValid.Valid = false;
                    return;
                }

                for (int e = 0; e < context.Entities.Length; e++)
                {
                    var components = context.Entities[e];

                    // Verify that Components are valid
                    if (components == null)
                    {
                        ComponentsAreValid.Issue = $"Context [{context.Context}] - Entity [{e}] has invalid Components!";
                        ComponentsAreValid.Valid = false;
                        return;
                    }

                    // Verify that there are Components
                    if (components.Length < 1)
                    {
                        ComponentsAreValid.Issue = $"Context [{context.Context}] - Entity [{e}] doesn't have any Component!";
                        ComponentsAreValid.Valid = false;
                        return;
                    }

                    // Verify that Components are valid
                    for (int i = 0; i < components.Length; i++)
                    {
                        if (components[i] == null)
                        {
                            ComponentsAreValid.Issue = $"Context [{context.Context}] - Entity [{e}] - Component [{i}] is not valid!";
                            ComponentsAreValid.Valid = false;
                            return;
                        }
                    }

                    // Verify that there are no duplicate components
                    for (int i = 0; i < components.Length; i++)
                    {
                        var componentTypeCount = 0;
                        foreach (var otherComponent in components)
                        {
                            if (otherComponent.GetType() == components[i].GetType())
                            {
                                componentTypeCount++;
                            }
                            if (componentTypeCount > 1)
                            {
                                ComponentsAreValid.Issue = $"Context [{context.Context}] - Entity [{e}] has duplicate [{components[i].GetType()}] Component!";
                                ComponentsAreValid.Valid = false;
                                return;
                            }
                        }
                    }
                }
            }
        }

        [GUIColor("@this.FileIsValid.GetColor()")]
        public Condition FileIsValid = new Condition();

        [GUIColor("@this.ContextsAreValid.GetColor()")]
        public Condition ContextsAreValid = new Condition();

        [GUIColor("@this.EntitiesAreValid.GetColor()")]
        public Condition EntitiesAreValid = new Condition();

        [GUIColor("@this.ComponentsAreValid.GetColor()")]
        public Condition ComponentsAreValid = new Condition();

        private bool FileIsNotValid()
            => string.IsNullOrEmpty(EntitiesFile)
            || !File.Exists(EntitiesFile);

        #endregion
    }
}

#endif