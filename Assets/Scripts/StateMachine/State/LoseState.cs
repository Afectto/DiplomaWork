using UnityEngine;

public class LoseState : IGameState
{
    public void Enter()
    {
        Time.timeScale = 0;
        var panel =  FindObjectWithTag("LosePanel");
        FindObjectWithTag("LosePanel")?.SetActive(true);
    }

    public void Exit()
    {
        Time.timeScale = 1;
        GameObject.FindGameObjectWithTag("LosePanel")?.SetActive(false);
    }
    
    private GameObject FindObjectWithTag(string tag)
    {
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.CompareTag(tag))
            {
                return obj;
            }
        }
        return null;
    }
}