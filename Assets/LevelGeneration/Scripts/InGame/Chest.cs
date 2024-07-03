using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chest : MonoBehaviour
{
    private SpriteRenderer pickupRenderer;

    private SpriteRenderer itemRenderer;
    public float delay = 1f;

    // Start is called before the first frame update
    void Start()
    {
        pickupRenderer = GetComponent<SpriteRenderer>();
        pickupRenderer.enabled = false;

        itemRenderer = transform.Find("ItemSprite").GetComponent<SpriteRenderer>();
        itemRenderer.enabled = false;
        StartCoroutine(ShowItem());
    }

    IEnumerator ShowItem()
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Enable the image GameObject
        itemRenderer.enabled = true;
    }

    void Update()
    {

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            pickupRenderer.enabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            pickupRenderer.enabled = false;
        }
    }
}
