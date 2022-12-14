using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class UIManager : MonoBehaviourPun
{
    public static UIManager instance;
    
    private void Awake()
    {
        if (instance != null)
        {
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    [SerializeField] private GameObject bagInventory;
    [SerializeField] private GameObject equipmentInventory;
    [SerializeField] private GameObject encyclopediaInventory;
    [SerializeField] private GameObject toolTip;
    [SerializeField] private Button inventoryCloseBtn;
    
    private bool isTabPressed;
    private bool inventoryState;
    
    //Fishing
    [SerializeField] GameObject pressEImage;
    [SerializeField] Text fishName;
    [SerializeField] Image fishImage;
    PlayerController player;
 
    void Start()
    {
        player = GetComponent<PlayerController>();
        
        inventoryCloseBtn.onClick.AddListener((() =>
        {
            inventoryState = false;

            bagInventory.SetActive(false);
            equipmentInventory.SetActive(false);
            encyclopediaInventory.SetActive(false);
            pressEImage.SetActive(false);
        }));
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine) return;
        
        KeyInput();

        Open(bagInventory, equipmentInventory);
        OpenEImage();
        
        CloseInventory();
    }

    void KeyInput()
    {
        isTabPressed = Input.GetKeyDown(KeyCode.Tab);
    }

    void Open(GameObject bag, GameObject equipment)
    {
        if (isTabPressed)
        {
            isTabPressed = false;

            if (inventoryState)
            {
                bag.SetActive(false);
                equipment.SetActive(false);
                toolTip.SetActive(false);
            }
            else
            {
                bag.SetActive(true);
            }
            
            inventoryState = !inventoryState;
        }
    }
    
    void OpenEImage()
    {
        pressEImage.SetActive(player.eImageActivate);
    }
    
    //?????????????????? ?????????????? ???????????????????????????????? fishnumber????????? random int ???????????? ???????? ????????????
    public void OpenSuccessImage(GameObject fish)
    {
        if (GetComponent<PlayerInventory>().itemList.Count <= GetComponent<PlayerInventory>().MAXITEM)
        {
            GetComponent<Encyclopedia>().itemData = fish.GetComponent<Item>().item;
            GetComponent<Encyclopedia>().GainItem();

            GameObject temp = Instantiate(fish);
            
            GetComponent<PlayerInventory>().AddItem(temp);
        }
        else
        {
            Debug.Log("???????????? ?????????");
        }
    }

    void CloseInventory()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                inventoryState = false;
                bagInventory.SetActive(false);
                equipmentInventory.SetActive(false);
                encyclopediaInventory.SetActive(false);
                toolTip.SetActive(false);
            }
        }
    }
}