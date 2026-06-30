using UnityEditor;
using UnityEngine;
using System.IO;

namespace Wellz.Utils.Editor {
    public static class ScriptTemplateMenu {
        private const string PackageFolderName = "com.wellz.utils"; // Altere para o 'name' exato do seu package.json

        // Menu para o MonoBehaviour Customizado
        [MenuItem("Assets/Create/Wellz Utils/Custom MonoBehaviour", false, 20)]
        public static void CreateCustomMonoBehaviour() {
            string templatePath = $"Packages/{PackageFolderName}/ScriptTemplates/20-Wellz Utils__Custom MonoBehaviour-ExampleMono.cs.txt";
            CreateScriptFromTemplate(templatePath, "NewExampleMono.cs");
        }

        // Menu para a Classe C# Comum
        [MenuItem("Assets/Create/Wellz Utils/Custom Class", false, 21)]
        public static void CreateCustomClass() {
            string templatePath = $"Packages/{PackageFolderName}/ScriptTemplates/21-Wellz Utils__Custom Class-ExampleClass.cs.txt";
            CreateScriptFromTemplate(templatePath, "NewExampleClass.cs");
        }

        // NOVO: Menu para o ScriptableObject Customizado
        [MenuItem("Assets/Create/Wellz Utils/Custom ScriptableObject", false, 22)]
        public static void CreateCustomScriptableObject() {
            string templatePath = $"Packages/{PackageFolderName}/ScriptTemplates/22-Wellz Utils__Custom ScriptableObject-ScriptableObject.cs.txt";
            CreateScriptFromTemplate(templatePath, "NewScriptableObject.cs");
        }

        // Método auxiliar para evitar repetição de código
        private static void CreateScriptFromTemplate(string templatePath, string defaultFileName) {
            if (File.Exists(templatePath)) {
                ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, defaultFileName);
            } else {
                Debug.LogError($"[Wellz Utils] Template não encontrado no caminho: {templatePath}");
            }
        }
    }
}