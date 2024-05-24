using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class UpgradeSelect : MonoBehaviour
{
    //====ITEM DATA====//
    public string itemName;
    public int quantity;
    public Sprite itemSprite;
    public bool isFull;
    public string itemDescription;
    public Sprite emptySprite;
    public ItemType itemType;
    public WeaponInfo weaponInfo;

    [SerializeField] private TMP_Text itemDescriptionNameText;
    [SerializeField] private TMP_Text itemDescriptionText;

    //====ITEM SLOT====//
    [SerializeField] Image itemImage;

    //====EQUIP SLOT====//
    [SerializeField]
    EquippedSlot assaultWeaponSlot,
        magicWeaponSlot, rangeWeaponSlot;


    public GameObject selectedShader;
    public bool thisItemSelected;

    private InventoryManager inventoryManager;
    private EquipmentSOLibrary equipmentSOLibrary;

    [SerializeField] public ToolbarSlot toolbarSlot;
    private bool slotInUse;
    [SerializeField] public Image slotImage;

    private void Start()
    {
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
        equipmentSOLibrary = GameObject.Find("InventoryCanvas").GetComponent<EquipmentSOLibrary>();

        //Update Image
        this.itemSprite = weaponInfo.weaponSprite;
        itemImage.sprite = itemSprite;
    }

    public int AddItem(string itemName, int quantity, Sprite itemSprite, string itemDescription, ItemType itemType, WeaponInfo weaponInfo)
    {
        if (isFull)
        {
            return quantity;
        }

        //Update Weapon Info
        this.weaponInfo = weaponInfo;

        //Update Item Type
        this.itemType = itemType;

        //Update Name
        this.itemName = itemName;

        //Update Image
        this.itemSprite = itemSprite;
        itemImage.sprite = itemSprite;

        //Update Description
        this.itemDescription = itemDescription;

        //Update Quantity
        this.quantity = 1;
        isFull = true;

        return 0;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftClick();
        }
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            //OnRightClick();
        }
    }

    public void OnLeftClick()
    {
        if (isFull)
        {
            if (thisItemSelected)
            {
                EquipGear();
            }
            else
            {
                inventoryManager.DeselectAllSlots();
                selectedShader.SetActive(true);
                thisItemSelected = true;
                itemDescriptionNameText.text = itemName;
                itemDescriptionText.text = itemDescription;
            }
        }
        else
        {
            inventoryManager.DeselectAllSlots();
            selectedShader.SetActive(true);
            thisItemSelected = true;
        }
    }

    private void EquipGear()
    {
        if (itemType == ItemType.assaultWeapon)
            assaultWeaponSlot.EquipGear(itemSprite, itemName, itemDescription, weaponInfo);
        if (itemType == ItemType.magicWeapon)
            magicWeaponSlot.EquipGear(itemSprite, itemName, itemDescription, weaponInfo);
        if (itemType == ItemType.rangeWeapon)
            rangeWeaponSlot.EquipGear(itemSprite, itemName, itemDescription, weaponInfo);

        EmptySlot();
    }

    public void EquipGear(Sprite itemSprite, string itemName, string itemDescription, WeaponInfo weaponInfo)
    {
        if (slotInUse)
        {
            UnEquipGear();
        }

        //UPDATE IMAGE
        this.itemSprite = itemSprite;
        slotImage.sprite = this.itemSprite;

        //UPDATE DATA
        this.itemName = itemName;
        this.itemDescription = itemDescription;

        this.weaponInfo = weaponInfo;

        if (toolbarSlot != null)
        {
            toolbarSlot.weaponInfo = weaponInfo;
            toolbarSlot.slotSprite.GetComponent<Image>().sprite = slotImage.sprite;
        }

        GameObject.Find("ActiveToolbar").GetComponent<ActiveToolbar>().ChangeActiveWeapon();

        slotInUse = true;
    }

    private void UnEquipGear()
    {
        inventoryManager.DeselectAllSlots();

        string itemNameToUnequip = this.itemName;
        string itemDescriptionToUnequip = this.itemDescription;
        Sprite itemSpriteToUnequip = this.itemSprite;

        inventoryManager.AddItem(itemNameToUnequip, 1, itemSpriteToUnequip, itemDescriptionToUnequip, itemType, weaponInfo);

        this.itemSprite = emptySprite;
        slotImage.sprite = this.emptySprite;

        this.itemName = "";
        this.itemDescription = "";
        this.weaponInfo = null;

        if (toolbarSlot != null)
        {
            toolbarSlot.weaponInfo = null;
            toolbarSlot.slotSprite.GetComponent<Image>().sprite = slotImage.sprite;
        }
        GameObject.Find("ActiveToolbar").GetComponent<ActiveToolbar>().ChangeActiveWeapon();

        slotInUse = false;
    }

    private void EmptySlot()
    {
        //Update Weapon Info
        this.weaponInfo = null;

        //Update Item Type
        this.itemType = ItemType.consumeable;

        //Update Name
        this.itemName = "";

        //Update Image
        this.itemSprite = emptySprite;
        itemImage.sprite = itemSprite;

        //Update Description
        this.itemDescription = "";

        //Update Quantity
        this.quantity = 0;

        itemImage.sprite = emptySprite;
        itemDescriptionNameText.text = "";
        itemDescriptionText.text = "";
        isFull = false;
    }

    public void OnRightClick()
    {
        GameObject itemToDrop = new GameObject(itemName);
        Item newItem = itemToDrop.AddComponent<Item>();
        newItem.quantity = 1;
        newItem.itemName = itemName;
        newItem.sprite = itemSprite;
        newItem.itemDescription = itemDescription;
        newItem.itemType = this.itemType;


        SpriteRenderer sr = itemToDrop.AddComponent<SpriteRenderer>();
        sr.sprite = itemSprite;
        sr.sortingOrder = 0;
        sr.sortingLayerName = "Default";

        itemToDrop.AddComponent<CapsuleCollider2D>();
        Rigidbody2D rb2D = itemToDrop.AddComponent<Rigidbody2D>();
        rb2D.gravityScale = 0f;

        itemToDrop.transform.position = GameObject.FindWithTag("Player").transform.position
            + new Vector3(1f, 0, 0);
        //itemToDrop.transform.localScale = new Vector3(1, 1, 1);

        this.quantity -= 1;
        if (this.quantity <= 0)
        {
            EmptySlot();
        }
    }
}