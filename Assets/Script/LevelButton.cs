using UnityEngine;

public class LevelButton : MonoBehaviour
{
    [SerializeField] private string levelSceneName; 

    public void LoadThisLevel()
    {
        SceneTransitionManager.Instance.LoadLevel(levelSceneName);
    }
}
