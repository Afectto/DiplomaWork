using Newtonsoft.Json;
using UnityEngine;

[System.Serializable]
public class MyVector3
{
    public float x;
    public float y;
    public float z;
    
    public MyVector3(Vector3 vector3)
    {
        x = vector3.x;
        y = vector3.y;
        z = vector3.z;
    }
    
    [JsonConstructor]
    public MyVector3(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Vector3 ToVector3() => new Vector3(x, y, z);

    public Vector2Int ToVector2Int() => new Vector2Int(Mathf.RoundToInt(x), Mathf.RoundToInt(y));
    
    public static implicit operator Vector3(MyVector3 myVector)
    {
        return new Vector3(myVector.x, myVector.y, myVector.z);
    }    
    
    public static implicit operator MyVector3(Vector3 vector)
    {
        return new MyVector3(vector);
    }
    
    public static implicit operator Vector2Int(MyVector3 myVector)
    {
        return new Vector2Int(Mathf.RoundToInt(myVector.x), Mathf.RoundToInt(myVector.y));
    }

    public static implicit operator MyVector3(Vector2Int vector)
    {
        return new MyVector3(vector.x, vector.y, 0);
    }
    
    public override string ToString() => $"({x:F2}, {y:F2}, {z:F2})";
}