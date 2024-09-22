using UnityEngine;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using TMPro;
using UnityEngine.UI;

public class CodeExecutor : MonoBehaviour
{    
    [SerializeField] private TMP_InputField inputField;
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(ExecuteCode);
    }
    
    public void ExecuteCode()
    {
        string code = $@"
        public class DynamicCode
        {{
            public void Execute()
            {{
                {inputField.text
            }
        }}";
        // @" TODO: пример
        // public class DynamicCode
        // {
        //     public void Execute()
        //     {
        //         int k = 10;
        //         int l = 0;
        //         for(int i = 0; i < k; i++)
        //         {
        //             l += i;
        //         }
        //         UnityEngine.Debug.Log(l);
        //     }
        // }"
        var syntaxTrees = new[] { CSharpSyntaxTree.ParseText(code) };

        // Включаем ссылки на сборки, необходимые для компиляции
        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(assembly => !assembly.IsDynamic && !string.IsNullOrEmpty(assembly.Location))
            .Select(assembly => MetadataReference.CreateFromFile(assembly.Location))
            .ToList();

        // Добавляем конкретные зависимости, необходимые для Unity 
        references.Add(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));

        var compilation = CSharpCompilation.Create(
            "DynamicAssembly",
            syntaxTrees,
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        using (var ms = new MemoryStream())
        {
            var result = compilation.Emit(ms);

            if (!result.Success)
            {
                foreach (var diagnostic in result.Diagnostics)
                {
                    Debug.LogError(diagnostic.ToString());
                }
                return;
            }

            ms.Seek(0, SeekOrigin.Begin);

            var assembly = Assembly.Load(ms.ToArray());
            var type = assembly.GetType("DynamicCode");
            var instance = Activator.CreateInstance(type);
            var method = type.GetMethod("Execute");
            method.Invoke(instance, null);
        }
    }
}