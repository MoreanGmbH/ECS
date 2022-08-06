#if ODIN_INSPECTOR && ECS
using System.IO;
using Sirenix.OdinInspector;
using System;
using Newtonsoft.Json;

namespace ECS
{
    public class EntitiesFileValidator
    {
        private string format = "json";
        private static readonly JsonSerializerSettings serializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
        };

        private bool triedValidating = false;
        private bool isValid = false;
        private bool canFix = false;

        ContextData[] contexts;

        [HorizontalGroup("Validate Entities", order: 0)]
        [FilePath(Extensions = "$format"), PropertySpace, LabelWidth(70)]
        [OnValueChanged("@this.triedValidating = this.isValid = false")]
        public string EntitiesFile;

        [GUIColor("@this.FileIsValid.GetColor()")]
        [ShowIf(nameof(CanShowConditions))]
        public Condition FileIsValid = new Condition();

        [GUIColor("@this.ContextsAreValid.GetColor()")]
        [ShowIf(nameof(CanShowConditions))]
        public Condition ContextsAreValid = new Condition();

        [GUIColor("@this.EntitiesAreValid.GetColor()")]
        [ShowIf(nameof(CanShowConditions))]
        public Condition EntitiesAreValid = new Condition();

        [GUIColor("@this.ComponentsAreValid.GetColor()")]
        [ShowIf(nameof(CanShowConditions))]
        public Condition ComponentsAreValid = new Condition();

        private bool CanShowConditions() => !FileIsNotValid() && triedValidating;

        private bool FileIsNotValid()
            => string.IsNullOrEmpty(EntitiesFile)
            || !File.Exists(EntitiesFile);

        [HorizontalGroup("Validate Entities", order: 0)]
        [Button, PropertySpace(SpaceAfter = 10)]
        [DisableIf(nameof(FileIsNotValid))]
        public void Validate()
        {
            if (FileIsNotValid()) return;

            triedValidating = true;
            isValid = false;
            canFix = true;

            FileIsValid.Valid = true;
            ContextsAreValid.Valid = true;
            EntitiesAreValid.Valid = true;
            ComponentsAreValid.Valid = true;

            contexts = null;

            // Verify that the file can be deserialized
            try
            {
                contexts = JsonConvert.DeserializeObject<ContextData[]>(File.ReadAllText(EntitiesFile), serializerSettings);
            }
            catch (Exception exception)
            {
                FileIsValid.Issue = $"File [{EntitiesFile}] is not valid: {exception.Message}";
                FileIsValid.Valid = false;
                canFix = false;
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
                    canFix = false;
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

            isValid = true;
        }

        [Button(ButtonSizes.Large), PropertySpace(20)]
        [ShowIf("@triedValidating && !isValid && canFix")]
        private void Fix()
        {
            var entitiesEditor = new EntitiesEditor();
            entitiesEditor.ContextsData.AddRange(contexts);
            entitiesEditor.Show();
        }
    }
}
#endif