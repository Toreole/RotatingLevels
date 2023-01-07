using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    [Tag, SerializeField]
    private string playerTag;
    [Scene, SerializeField]
    private string nextScene;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.CompareTag(playerTag))
        {
            collider.GetComponent<PlayerController>().enabled = false;
            SceneManager.LoadScene(nextScene);
            Debug.Log("Level Complete");
        }
    }
}
