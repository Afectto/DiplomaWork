using System;
using System.Collections.Generic;

[Serializable]
public class Task
{
    public int id;
    public string text;
    public string baseCode;
    public List<Test> tests;
    public int difficulty;
}