#if ODIN_INSPECTOR && ECS
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System.Collections.Generic;
using System.IO;

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

        private string format = "json";

        [HorizontalGroup("Scan Directory", order: 0)]
        [FolderPath(AbsolutePath = true), PropertySpace, LabelWidth(90)]
        public string ScanDirectory;

        [HorizontalGroup("Scan Directory", order: 0)]
        [Button, PropertySpace(SpaceAfter = 10)]
        [DisableIf(nameof(DirectoryIsNotValid))]
        private void Scan()
        {
            foreach (var file in Directory.EnumerateFiles(ScanDirectory, $"*.{format}", SearchOption.AllDirectories))
            {
                Validators.Add(new EntitiesFileValidator() { EntitiesFile = file });
            }
        }

        [Button(ButtonSizes.Large), PropertyOrder(1), PropertySpace(20)]
        [DisableIf("@Validators.Count == 0")]
        public void ValidateAll() => Validators.ForEach(validator => validator.Validate());

        [HideReferenceObjectPicker, PropertyOrder(2), PropertySpace(20)]
        [ListDrawerSettings(CustomAddFunction = nameof(AddValidator))]
        public List<EntitiesFileValidator> Validators = new List<EntitiesFileValidator>();

        private void AddValidator() => Validators.Add(new EntitiesFileValidator());

        private bool DirectoryIsNotValid()
            => string.IsNullOrEmpty(ScanDirectory)
            || !Directory.Exists(ScanDirectory);
    }
}
#endif