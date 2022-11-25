using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class UIManager_BH : MonoBehaviour
{
    public static UIManager_BH instance;

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
    [SerializeField] private Button inventoryCloseBtn;

    private bool isTabPressed;
    private bool inventoryState;

    //Fishing
    [SerializeField] GameObject pressEImage;
    [SerializeField] Text fishName;
    [SerializeField] Image fishImage;
    PlayerFishing player;
    ItemData[] fish;

    void Start()
    {
        player = GetComponent<PlayerFishing>();

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
        KeyInput();

        Open(bagInventory);
        OpenEImage();

        CloseInventory();
    }

    void KeyInput()
    {
        isTabPressed = Input.GetKeyDown(KeyCode.Tab);
    }

    void Open(GameObject ui)
    {
        if (isTabPressed)
        {
            isTabPressed = false;
            inventoryState = !inventoryState;
            ui.SetActive(inventoryState);
        }
    }

    void OpenEImage()
    {
        pressEImage.SetActive(player.eImageActivate);
    }

    //������ �̸��� ��������Ʈ�� fishnumber��� random int ���� �־ ����
    public void OpenSuccessImage()
    {
        fish = player.fish;

        int fishNumber = Random.Range(0, fish.Length);
        Debug.Log("Success! Caught " + fish[fishNumber]);
        
        if (GetComponent<PlayerInventory>().itemList.Count <= GetComponent<PlayerInventory>().MAXITEM)
        {
            GetComponent<Encyclopedia>().itemData = fish[fishNumber];
            GetComponent<Encyclopedia>().GainItem();
            //GetComponent<PlayerInventory>().itemList.Add(fish[fishNumber]);
        }
        else
        {
            Debug.Log("인벤토리 가득참");
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
            }
        }
    }
}
