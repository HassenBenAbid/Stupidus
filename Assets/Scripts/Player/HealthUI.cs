using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUI : MonoBehaviour
{
    private const int HEARTS_NUMBER = 5;

    [SerializeField] private GameObject heart;
    [SerializeField] private Transform heartPosition;

    private List<GameObject> heartsList;
    private int currentIndex;

    public void setHearts(int activeHearts)
    {
        if (heartsList == null)
        {
            heartsList = new List<GameObject>();
            for (int i = 0; i < HEARTS_NUMBER; i++)
            {
                Vector2 newPosition = new Vector2(heartPosition.position.x + (i * 25), heartPosition.position.y);
                GameObject currentHeart = Instantiate(heart, newPosition, Quaternion.identity);
                currentHeart.transform.SetParent(this.transform);
                heartsList.Add(currentHeart);
                if (i > activeHearts - 1)
                {
                    currentHeart.SetActive(false);
                }
            }

            currentIndex = activeHearts - 1;
        }

    }

    public void heartHit(int damage)
    {
        for(int i=0; i<damage; i++)
        {
            heartsList[currentIndex].SetActive(false);
            currentIndex--;
        }
    }

    public void addHeart()
    {
        currentIndex++;
        if (currentIndex <= HEARTS_NUMBER - 1)
        {
            heartsList[currentIndex].SetActive(true);
        }
    }
}
