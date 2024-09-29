using UnityEngine;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine.UI;

public class CodeExecutor : MonoBehaviour
{    
    [SerializeField] private TMP_InputField inputField;
    private Button _button;

    private string _paramInExecute;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(ExecuteCode);
    }
    
    // public void ExecuteCode()
    // {
    //     string code = $@"
    //     public class DynamicCode
    //     {{
    //         public void Execute(Player player)
    //         {{
    //             {inputField.text}
    //         }}
    //     }}";
    //     // @" TODO: пример
    //     // public class DynamicCode
    //     // {
    //     //     public void Execute()
    //     //     {
    //     //         int k = 10;
    //     //         int l = 0;
    //     //         for(int i = 0; i < k; i++)
    //     //         {
    //     //             l += i;
    //     //         }
    //     //         UnityEngine.Debug.Log(l);
    //     //     }
    //     // }"
    //     var syntaxTrees = new[] { CSharpSyntaxTree.ParseText(code) };
    //
    //     // Включаем ссылки на сборки, необходимые для компиляции
    //     var references = AppDomain.CurrentDomain.GetAssemblies()
    //         .Where(assembly => !assembly.IsDynamic && !string.IsNullOrEmpty(assembly.Location))
    //         .Select(assembly => MetadataReference.CreateFromFile(assembly.Location))
    //         .ToList();
    //
    //     // Добавляем конкретные зависимости, необходимые для Unity 
    //     references.Add(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));
    //
    //     var compilation = CSharpCompilation.Create(
    //         "DynamicAssembly",
    //         syntaxTrees,
    //         references,
    //         new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
    //
    //     using (var ms = new MemoryStream())
    //     {
    //         var result = compilation.Emit(ms);
    //
    //         if (!result.Success)
    //         {
    //             foreach (var diagnostic in result.Diagnostics)
    //             {
    //                 Debug.LogError(diagnostic.ToString());
    //             }
    //             return;
    //         }
    //
    //         ms.Seek(0, SeekOrigin.Begin);
    //
    //         var assembly = Assembly.Load(ms.ToArray());
    //         var type = assembly.GetType("DynamicCode");
    //         var instance = Activator.CreateInstance(type);
    //         var method = type.GetMethod("Execute");
    //         
    //         if (method != null)
    //         {
    //             Player player = FindObjectOfType<Player>();
    //             method.Invoke(instance, new object[] { player });
    //         }
    //         else
    //         {
    //             Debug.LogError("Метод 'Execute' не найден.");
    //         }
    //     }
    // }
    
    private void ExecuteCode()
    {
        string code = GenerateDynamicCode(inputField.text);

        var compilation = CompileCode(code);
        
        if (EmitAndExecute(compilation))
        {
            Debug.Log("Код успешно выполнен.");
        }
    }

    private string GenerateDynamicCode(string userInput)
    {
        return $@"
        public class DynamicCode
        {{
            public void Execute(Player player)
            {{
                {userInput}
            }}
        }}";
    }

    private CSharpCompilation CompileCode(string code)
    {
        var syntaxTrees = new[] { CSharpSyntaxTree.ParseText(code) };

        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(assembly => !assembly.IsDynamic && !string.IsNullOrEmpty(assembly.Location))
            .Select(assembly => MetadataReference.CreateFromFile(assembly.Location))
            .ToList();

        references.Add(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));
        references.Add(MetadataReference.CreateFromFile(typeof(Debug).Assembly.Location));

        return CSharpCompilation.Create(
            "DynamicAssembly",
            syntaxTrees,
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
    }

    private bool EmitAndExecute(CSharpCompilation compilation)
    {
        using (var ms = new MemoryStream())
        {
            var result = compilation.Emit(ms);

            if (!result.Success)
            {
                foreach (var diagnostic in result.Diagnostics)
                {
                    Debug.LogError(diagnostic.ToString());
                }
                return false;
            }

            ms.Seek(0, SeekOrigin.Begin);
            var assembly = Assembly.Load(ms.ToArray());
            var type = assembly.GetType("DynamicCode");
            var instance = Activator.CreateInstance(type);
            var method = type.GetMethod("Execute");

            if (method != null)
            {
                Player player = FindObjectOfType<Player>();
                method.Invoke(instance, new object[] { player });
                return true;
            }
            else
            {
                Debug.LogError("Метод 'Execute' не найден.");
                return false;
            }
        }
    }

}