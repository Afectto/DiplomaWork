using UnityEngine;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using TaskAsync = System.Threading.Tasks.Task;

public class CodeExecutor : MonoBehaviour
{    
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TextMeshProUGUI textTask;
    [SerializeField] private TextMeshProUGUI resultTask;
    private Button _button;

    private string _paramInExecute;
    private Task _selectedTask;

    public event Action OnTaskComplete;

    private void Awake()
    {
        _button = GetComponent<Button>();

        async void Call()
        {
            _button.onClick.RemoveListener(Call);
            await ExecuteCode();
            _button.onClick.AddListener(Call);
        }

        _button.onClick.AddListener(Call);
        _button.onClick.AddListener(() =>
        {
            transform
                .DOShakeScale(0.25f, 0.1f, 10, 90)
                .SetUpdate(true);
            SoundManager.Play(SoundType.Click);
        });
    }

    private void OnEnable()
    {
    }

    public void SetTask(Task newTask)
    {
        _selectedTask = newTask;
        ShowTask();
    }
    
    private void ShowTask()
    {
        textTask.text = _selectedTask.text;
        
        inputField.text = $@"
using UnityEngine;
using System;

public class DynamicCode
{{
{_selectedTask.baseCode}
    
}}";
        resultTask.text = "";
    }
    
    private async TaskAsync ExecuteCode()
    {
        resultTask.text = "Waiting..."; 
        resultTask.color = Color.yellow;
        
        string code = inputField.text;
        var compilation = await TaskAsync.Run(() => CompileCode(code));
        
        bool success = await TaskAsync.Run(() => EmitAndExecute(compilation, _selectedTask.tests));

        if (success)
        {
            resultTask.text = "Код успешно выполнен."; 
            resultTask.color = Color.green;
            OnTaskComplete?.Invoke();
        }
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

    private bool EmitAndExecute(CSharpCompilation compilation, List<Test> tests)
    {
        using var ms = new MemoryStream();
        var result = compilation.Emit(ms);

        if (!result.Success)
        {
            var errors = result.Diagnostics
                .Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
                .Select(diagnostic => 
                {
                    var lineSpan = diagnostic.Location.GetLineSpan();
                    var lineNumber = lineSpan.StartLinePosition.Line + 1; // Получаем номер строки
                    return $"Ошибка в строке {lineNumber}: {diagnostic.GetMessage()}";
                });

            UnityMainThreadDispatcher.Enqueue(() =>
            {
                resultTask.text = "Ошибки компиляции:\n" + string.Join("\n", errors);
                resultTask.color = Color.red;
            });
            return false;
        }

        ms.Seek(0, SeekOrigin.Begin);
        var assembly = Assembly.Load(ms.ToArray());
        var type = assembly.GetType("DynamicCode");
        var instance = Activator.CreateInstance(type);
        var method = type.GetMethod("main");

        if (method != null)
        {
            foreach (var test in tests)
            {           
                var inputs = test.input.Split(',').Select(x => x.Trim()).ToArray();
                object[] parameters = null;
                if (test.inputTypes[0].StartsWith("[") && test.inputTypes[0].EndsWith("]"))
                {
                    parameters = new object[1];
                    parameters[0] = ConvertInputsToArray(inputs, test.inputTypes);
                }
                else
                {
                    var typedInputs = ConvertInputs(inputs, test.inputTypes);
                    parameters = new object[typedInputs.Length];
                    for (var index = 0; index < typedInputs.Length; index++)
                    {
                        var param = typedInputs[index];
                        parameters[index] = param;
                    }
                }

                var actualResult = method.Invoke(instance, parameters);

                if (actualResult is Array objectArrayResult)
                {
                    actualResult = ConvertToString(objectArrayResult);
                }
                
                if (!ValidateResult(actualResult, test.expected))
                {
                    UnityMainThreadDispatcher.Enqueue(() =>
                    {
                        resultTask.text = $"Тест не пройден:" +
                                          $" входные данные '{test.input}'" +
                                          $", ожидаемый результат '{test.expected}'" +
                                          $", актуальный результат '{actualResult}'";
                        resultTask.color = Color.red;
                    });
                    return false;
                }
            }
            return true;
        }
        else
        {
            Debug.LogError("Метод 'main' не найден.");
            return false;
        }
    }

    private string ConvertToString(Array array)
    {
        string rez = "";

        for (var index = 0; index < array.Length; index++)
        {
            rez += array.GetValue(index);
            if (index < array.Length - 1)
            {
                rez += ", ";
            }
        }

        return rez;
    }

    private object ConvertInputsToArray(string[] inputs, List<string> inputTypes)
    {
        if (inputTypes.Count != 1)
        {
            Debug.LogError("Неверное количество типов. Ожидается один тип для массива.");
            return null;
        }

        string innerType = inputTypes[0].Trim('[', ']');
        switch (innerType.ToLower())
        {
            case "double":
                return inputs.Select(x => Convert.ToDouble(x)).ToArray();
            case "float":
                return inputs.Select(x => Convert.ToSingle(x)).ToArray();
            case "int":
                return inputs.Select(x => Convert.ToInt32(x)).ToArray();
            case "string":
                return inputs; // В данном случае, массив строк уже готов
            default:
                Debug.LogError($"Неизвестный тип массива {innerType} для входных данных.");
                return null;
        }
    }
    
    private object[] ConvertInputs(string[] inputs, List<string> inputTypes)
    {
        var convertedInputs = new object[inputs.Length];

        for (int i = 0; i < inputs.Length; i++)
        {
            convertedInputs[i] = ChooseType(inputs[i], inputTypes[i]);
        }

        return convertedInputs;
    }


    private object ChooseType(string input, string inputTypes)
    {
        object convertedInputs = null;
        switch (inputTypes.ToLower())
        {
            case "int":
                convertedInputs = Convert.ToInt32(input);
                break;
            case "double":
                convertedInputs = Convert.ToDouble(input);
                break;
            case "string":
                convertedInputs = input;
                break;
            default:
                Debug.LogError($"Неизвестный тип {inputTypes} для входных данных.");
                break;
        }

        return convertedInputs;
    }
    private bool ValidateResult(object actual, string expected)
    {
        return actual?.ToString() == expected;
    }
}

