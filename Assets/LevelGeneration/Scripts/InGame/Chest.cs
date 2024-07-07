using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chest : MonoBehaviour
{
    private SpriteRenderer pickupRenderer;

    public float delay = 1f;

    private bool isPlayerInTrigger = false;
    [SerializeField] private GameObject unlockCanvas;
    [SerializeField] private GameObject coinCanvas;

    //SLOT DATA//
    [SerializeField] private WeaponInfo weaponInfo;
    public bool isAssaultWeapon;
    public bool isRangeWeapon;

    private bool slotInUse;

    [SerializeField] private int price;

    [SerializeField] private GameObject toolbarSlot;

    // Start is called before the first frame update
    void Start()
    {
        pickupRenderer = GetComponent<SpriteRenderer>();
        pickupRenderer.enabled = false;

        unlockCanvas.SetActive(false);

        coinCanvas = GameObject.Find("CoinCanvas");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the tag of the trigger collided with is Exit.
        if (other.tag == "Player")
        {
            unlockCanvas.SetActive(true);
            isPlayerInTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Reset the flag when the player leaves the trigger zone.
        if (other.tag == "Player")
        {
            unlockCanvas.SetActive(false);
            isPlayerInTrigger = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the player is in the trigger zone and the F key is pressed.
        if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.F))
        {
            if (isAssaultWeapon && coinCanvas.GetComponent<CoinManager>().currentGold >= price)
            {
                coinCanvas.GetComponent<CoinManager>().DecreaseCurrentGold(price);
                toolbarSlot = GameObject.Find("AssaultToolbar");
                EquipGear(weaponInfo);
                Destroy(this.gameObject);
            }
            else if (isRangeWeapon && coinCanvas.GetComponent<CoinManager>().currentGold >= price)
            {
                coinCanvas.GetComponent<CoinManager>().DecreaseCurrentGold(price);
                toolbarSlot = GameObject.Find("RangeToolbar");
                toolbarSlot.GetComponent<ToolbarSlot>();
                EquipGear(weaponInfo);
                Destroy(this.gameObject);
            }
        }
    }

    public void EquipGear(WeaponInfo weaponInfo)
    {
        if (slotInUse)
        {
            UnEquipGear();
        }

        this.weaponInfo = weaponInfo;

        if (toolbarSlot != null)
        {
            toolbarSlot.GetComponent<ToolbarSlot>().weaponInfo = weaponInfo;
            toolbarSlot.GetComponent<ToolbarSlot>().slotSprite.GetComponent<Image>().sprite = weaponInfo.weaponSprite;
        }

        GameObject.Find("ActiveToolbar").GetComponent<ActiveToolbar>().ChangeActiveWeapon();

        slotInUse = true;
    }

    private void UnEquipGear()
    {
        this.weaponInfo = null;

        if (toolbarSlot != null)
        {
            toolbarSlot.GetComponent<ToolbarSlot>().weaponInfo = null;
            toolbarSlot.GetComponent<ToolbarSlot>().slotSprite.GetComponent<Image>().sprite = null;
        }
        GameObject.Find("ActiveToolbar").GetComponent<ActiveToolbar>().ChangeActiveWeapon();

        slotInUse = false;
    }
}
