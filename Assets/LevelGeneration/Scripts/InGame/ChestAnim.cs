using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestAnim : MonoBehaviour
{
    [SerializeField] private GameObject itemSpite;

    // Start is called before the first frame update
    void Start()
    {
        itemSpite.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void ShowItem()
    {
        itemSpite.SetActive(true);
    }
}
