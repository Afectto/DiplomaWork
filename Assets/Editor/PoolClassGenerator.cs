using System;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public class PoolClassGenerator : EditorWindow
{
    private string _className = "NewClass";

    [MenuItem("Tools/Pool Class Generator")]
    public static void ShowWindow()
    {
        GetWindow<PoolClassGenerator>("Class Pool Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Generate Pool Classes", EditorStyles.boldLabel);
        
        _className = EditorGUILayout.TextField("Pool Class Name", _className);

        if (GUILayout.Button("Generate Pool Classes"))
        {
            GenerateClasses(_className);
        }
    }

    private void GenerateClasses(string namePool)
    {
        string poolPath = "Assets/Scripts/Pools";
        string folderPath = Path.Combine(poolPath, namePool);
        
        if (!AssetDatabase.IsValidFolder(poolPath))
        {
            AssetDatabase.CreateFolder("Assets/Scripts", "Pools");
        }

        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder(poolPath, namePool);
        }

        CreateClass(folderPath, namePool, TypeClass.Object);
        CreateClass(folderPath, namePool + "Pool", TypeClass.Pool);
        CreateClass(folderPath, namePool + "Spawner", TypeClass.Spawner);
        
        UpdateGameInstaller(namePool);
        
        AssetDatabase.Refresh();
    }
    
    private void UpdateGameInstaller(string poolClassName)
    {
        string gameInstallerPath = "Assets/Scripts/Installers/GameInstaller.cs";
    
        if (!File.Exists(gameInstallerPath))
        {
            Debug.LogError("GameInstaller.cs not found!");
            return;
        }

        string[] lines = File.ReadAllLines(gameInstallerPath);
        string newMethod = $@"

    private void {poolClassName}()
    {{
        Container
            .Bind<ObjectPool<{poolClassName}>>()
            .To<{poolClassName}Pool>()
            .AsSingle();

        Container
            .BindInterfacesAndSelfTo<{poolClassName}Spawner>()
            .FromComponentsInHierarchy()
            .AsSingle();
    }}";

        bool methodExists = lines.Any(line => line.Contains($"private void {poolClassName}()"));
    
        int insertIndex = Array.FindLastIndex(lines, line => line.Contains("}"));

        if (!methodExists && insertIndex != -1)
        {
            using (StreamWriter writer = new StreamWriter(gameInstallerPath, false))
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    writer.WriteLine(lines[i]);
                    if (i == insertIndex - 1)
                    {
                        writer.WriteLine(newMethod);
                    }
                }
            }
            
            AddMethodCallToInstallBindings(gameInstallerPath, poolClassName);
            AssetDatabase.ImportAsset(gameInstallerPath);
        }
    }
    
    private void AddMethodCallToInstallBindings(string gameInstallerPath, string poolClassName)
    {
        string[] lines = File.ReadAllLines(gameInstallerPath);
        string installBindingsMethod = "InstallBindings";

        int insertIndex = Array.FindIndex(lines, line => line.Contains(installBindingsMethod));
    
        if (insertIndex != -1)
        {
            for (int i = insertIndex; i < lines.Length; i++)
            {
                if (lines[i].Contains("}"))
                {
                    lines[i - 1] += $"\n        {poolClassName}();";
                    break;
                }
            }
        
            File.WriteAllLines(gameInstallerPath, lines);
        }
    }
    
    private void CreateClass(string folderPath, string poolClassName, TypeClass typeClass)
    {
        string classPath = Path.Combine(folderPath, $"{poolClassName}.cs");
        poolClassName = poolClassName.Replace("Pool", "").Replace("Spawner", "");
        
        using (StreamWriter sw = File.CreateText(classPath))
        {
            sw.WriteLine(GetClassContent(poolClassName, typeClass));
        }

        AssetDatabase.ImportAsset(classPath);
    }

    private string GetClassContent(string poolClassName, TypeClass typeClass)
    {
        return typeClass switch
        {
            TypeClass.Object => PooledObject(poolClassName),
            TypeClass.Pool => Pool(poolClassName),
            TypeClass.Spawner => Spawner(poolClassName),
            _ => string.Empty,
        };
    }

    private enum TypeClass
    {
        Object,
        Pool,
        Spawner
    }

    private string PooledObject(string poolClassName)
    {
        return $@"
using System;
using UnityEngine;

public class {poolClassName} : MonoBehaviour, IPooledObject
{{ 
    public event Action<IPooledObject> OnNeedReturnToPool;

    public void GetInit()
    {{
        // Initialization Logic
    }}

    public void CreateInit()
    {{
        // Creation Logic
    }}        
    public void TriggerOnNeedReturnToPool()
    {{
        OnNeedReturnToPool?.Invoke(this);
    }}
}}";
    }
    
    private string Pool(string poolClassName)
    {
        return $@"
using UnityEngine;

public class {poolClassName}Pool : ObjectPool<{poolClassName}>
{{
    public new void Initialize({poolClassName} objectPrefab, Transform parentPool = null)
    {{
        base.Initialize(objectPrefab, parentPool);
    }}
}}";
    }

    private string Spawner(string poolClassName)
    {
        return $@"
using Zenject;

public class {poolClassName}Spawner : ItemSpawners<{poolClassName}>
{{
    private {poolClassName}Pool _{poolClassName.ToLower()}ItemPool;
    protected override ObjectPool<{poolClassName}> ItemPool => _{poolClassName.ToLower()}ItemPool;
    
    [Inject]
    protected override void Inject(ObjectPool<{poolClassName}> pool)
    {{ 
        _{poolClassName.ToLower()}ItemPool = pool as {poolClassName}Pool; 
        _{poolClassName.ToLower()}ItemPool.Initialize(itemPrefab.Value as {poolClassName}, transform);
    }}

    private void OnEnable()
    {{
        StartCoroutine(SpawnObjects());
    }}

    protected override void InitializeItem(IPooledObject item)
    {{
        // Item Initialization Logic
    }}
}}";
    }
}
