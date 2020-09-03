using UnityEngine;
using UnityEngine.EventSystems;

public class PanelDragger : MonoBehaviour, IDragHandler, IEndDragHandler 
{
    public float percentThreshold = 0.2f;
    Vector3 panelLocation;

    private void Start()
    {
        panelLocation = transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        float difference = eventData.pressPosition.x - eventData.position.x;
        transform.position = panelLocation - new Vector3(difference, 0 , 0);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float percentage = (eventData.pressPosition.x - eventData.position.x) / Screen.width;
        if (Mathf.Abs(percentage) >= percentThreshold)
        {
            Vector3 newLocation = panelLocation;
            if (percentage > 0)
            {
                newLocation += new Vector3(-Screen.width, 0, 0);
            }
            else if (percentage < 0)
            {
                newLocation += new Vector3(Screen.width, 0, 0);
            }

            transform.position = newLocation;
            panelLocation = newLocation;
            Debug.Log("Swiped to next page");
        }
        else
        {
            transform.position = panelLocation;
            Debug.Log("Failed swipe");
        }
    }
}
