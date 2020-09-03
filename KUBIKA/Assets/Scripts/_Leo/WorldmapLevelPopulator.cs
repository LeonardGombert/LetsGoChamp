using Kubika.Game;
using UnityEngine;
using UnityEngine.EventSystems;

public class WorldmapLevelPopulator : MonoBehaviour, IDragHandler, IEndDragHandler
{
    [SerializeField] GameObject levelPrefab;
    Transform LevelListTransfrom;
    LevelsManager levelsManager;
    public float percentThreshold = 0.2f;

    public int currentWorld = 1;

    // Start is called before the first frame update
    void Start()
    {
        levelsManager = LevelsManager.instance;
        LevelListTransfrom = transform.GetChild(0).GetChild(0).GetChild(0);

        PopulateScreen(1);
    }

    public void OnDrag(PointerEventData eventData)
    {

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float percentage = (eventData.pressPosition.x - eventData.position.x) / Screen.width;
        if (Mathf.Abs(percentage) >= percentThreshold)
        {
            foreach (Transform child in LevelListTransfrom) Destroy(child.gameObject);

            if (percentage > 0 && currentWorld < (int)Biomes.Count)
            {
                currentWorld++;
            }

            else if (percentage < 0 && currentWorld > 1)
            {
                currentWorld--;
            }

            Debug.Log("Swiped to next page");
            Debug.Log("Current World is " + currentWorld);
            
            PopulateScreen(currentWorld);
        }
        else Debug.Log("Failed swipe");
    }

    // populate the list with the levels in the current world
    void PopulateScreen(int _currentWorld)
    {
        foreach (var item in levelsManager.gameMasterList)
        {
            if (item.kubicode.Contains("Worl" + _currentWorld))
            {
                GameObject newObj = Instantiate(levelPrefab, Vector3.zero, Quaternion.identity, LevelListTransfrom);
                LevelInstantiator level = newObj.GetComponent<LevelInstantiator>();
                level.FillValues(item.levelName, item.difficulty, item.kubicode);
            }

            // if you're reading items of the next world, stop the foreach loop to save on time
            else if (item.kubicode.Contains("World" + (_currentWorld + 1))) break;
        }
    }
}
