using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
    public Joystick joystick;

    public float speed;

    private bool isLeft = false;

    public Button PickUpButton;

    #region Верхние счетчики

    [Header("Тексты")]
    public Text countFood;
    private int  _countFood;
    public Text countBrokenBottle;
    private int  _countBrokenBottle;
    public Text countPackets;
    private int  _countPackets;
    public Text countCover;
    private int  _countCover;

    public Text AlertLabel;
    private int maxItemPerType = 4;
    
    // money
    public Text countMoney;
    private int _countMoney = 0;

    #endregion

    //public int wasteInArea = 0; 
    public List<WasteOfType> wastesInArea; 
    
    // animation
    [Header("Аниматоры")]
    public Animator bodyAnimator;
    public Animator bagAnimator;
    public Animator handAnimator;
    public Animator sosAnimator;
    public Animator stickAnimator;
    private Vector3 lastFixedPosition;
    
    //bafs
    private float defaultRadius;
    
    // shop
    [Header("UI")]
    public GameObject ShopButton;
    public GameObject UIShop;
    public GameObject UIGroup; // for hide

    private void Start()
    {
        flipX();
        defaultRadius = GetComponent<CircleCollider2D>().radius;
        UpdateUi();
    }

    public void FixedUpdate()
    {
        // walk
        Vector2 direction = Vector2.up * joystick.Vertical + Vector2.right * joystick.Horizontal;

        if (direction.x > 0.1 && isLeft)
            flipX();
        if (direction.x < -0.1 && !isLeft)
            flipX();

        #region animations

        if (transform.position != lastFixedPosition)
        {
            bodyAnimator.SetBool("isWalk", true);
            bagAnimator.SetBool("isWalk", true);
            handAnimator.SetBool("isWalk", true);
            sosAnimator.SetBool("isWalk", true);
            stickAnimator.SetBool("isWalk", true);
        }
        else
        {
            bodyAnimator.SetBool("isWalk", false);
            bagAnimator.SetBool("isWalk", false);
            sosAnimator.SetBool("isWalk", false);
            handAnimator.SetBool("isWalk", false);
            stickAnimator.SetBool("isWalk", false);
        }

        float speedAnim = new Vector2(joystick.Horizontal, joystick.Vertical).magnitude;
        bodyAnimator.SetFloat("speed", speedAnim * 0.4f);
        bagAnimator.SetFloat("speed", speedAnim * 0.4f);
        sosAnimator.SetFloat("speed", speedAnim);
        stickAnimator.SetFloat("speed", speedAnim);

        #endregion
        
        lastFixedPosition = transform.position;
        transform.Translate(direction * speed * Time.fixedDeltaTime);
    }

    void flipX()
    {
        isLeft = !isLeft;
        Vector3 rotated = transform.localScale;
        rotated.x *= -1;
        transform.localScale = rotated;
    }

    #region Enter/Exit Triggers

    IEnumerator OnTriggerEnter2DCoroutine(Collider2D other)
    {
        switch (other.gameObject.tag)
        {
            case "Waste":
                var wasteOfType = other.gameObject.GetComponent<WasteOfType>();
                
                if (!wastesInArea.Contains(wasteOfType)) // чтобы 100% избежать дубликатов
                {
                    wastesInArea.Add(wasteOfType);

                    wastesInArea.Last().SetActiveSprite();

                    PickUpButton.gameObject.SetActive(true); 
                }
                break;
            
            case "Shop":
                other.gameObject.GetComponent<WasteOfType>().SetActiveSprite();
                ShopButton.SetActive(true);
                break;
        }
        yield break;
    }
    IEnumerator OnTriggerExit2DCoroutine(Collider2D other)
    {
        switch (other.gameObject.tag)
        {
            case "Waste":
                var current = other.gameObject.GetComponent<WasteOfType>();
                wastesInArea.Remove(current);
                current.SetInactiveSprite();

                //PickUpButton.gameObject.SetActive(false);
                break;
            
            case "Shop":
                other.gameObject.GetComponent<WasteOfType>().SetInactiveSprite();
                ShopButton.SetActive(false);
                break;
        }
        
        if(wastesInArea.Count == 0) 
            PickUpButton.gameObject.SetActive(false);
        else
            PickUpButton.gameObject.SetActive(true);
        
        yield break;
    }
    
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        StartCoroutine(OnTriggerEnter2DCoroutine(other));
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        StartCoroutine(OnTriggerExit2DCoroutine(other));
    }

    #endregion


    WasteOfType ClearOneWaste()
    {
        WasteOfType type = null;
        foreach (var waste in wastesInArea)
        {
            switch (waste.Type)
            {
                case WasteOfType.Types.Food:
                    if (_countFood + 1 <= maxItemPerType)
                        type = waste;
                    break;
                case WasteOfType.Types.BrokenBottle:
                    if (_countBrokenBottle + 1 <= maxItemPerType)
                        type = waste;
                    break;
                case WasteOfType.Types.Packet:
                    if (_countPackets + 1 <= maxItemPerType)
                        type = waste;
                    break;
                case WasteOfType.Types.Cover:
                    type = waste;
                    break;
            }
        }

        if (type == null)
        {
            AlertLabel.text = "Нет места для них, сдайте!";
        }
        else
        {
            stickAnimator.SetBool("isPick", true);
            handAnimator.SetBool("isPick", true);
            sosAnimator.SetBool("isPick", true);
            
            switch (type.Type)
            {
                case WasteOfType.Types.Food:
                    _countFood++;
                    break;
                case WasteOfType.Types.BrokenBottle:
                    _countBrokenBottle++;
                    break;
                case WasteOfType.Types.Packet:
                    _countPackets++;
                    break;
                case  WasteOfType.Types.Cover:
                    _countCover++;
                    break;
            } 
        }

        return type;
    }
    
    IEnumerator PickUpAnimationCoroutine()
    {
        PickUpButton.gameObject.SetActive(false);

        /**/
        var waste = ClearOneWaste();
        
        if (waste == null)
        {
            yield break;
        }

        /* поворот в сторону мусора */
        if (Math.Abs(waste.gameObject.transform.position.x) - Math.Abs(transform.position.x) < 0)
        //if (Math.Abs(transform.position.x) - Math.Abs(waste.gameObject.transform.position.x) > 0)
        { // право
            if(isLeft)
                flipX();
        }
        else
        { // лево
            if(!isLeft)
                flipX();
        }
        
        yield return new WaitForSeconds(0.2f); // продолжить примерно через 100ms

        var currentWaste = wastesInArea.FindLast(o=>o == waste);

        // add pick up item to up ui label
        UpdateUi();
        
        // remove waste (gameobject)
        wastesInArea.Remove(currentWaste);
        DestroyImmediate(currentWaste.gameObject);

        // set active button for next pick up
        if(wastesInArea.Count == 0) 
            PickUpButton.gameObject.SetActive(false);
        else
            PickUpButton.gameObject.SetActive(true);
        /**/
        
        
        
        stickAnimator.SetBool("isPick", false);
        sosAnimator.SetBool("isPick", false);
        handAnimator.SetBool("isPick", false);
        
        yield return null;
    }
    IEnumerator AllPickUpAnimationCoroutine()
    {
        PickUpButton.gameObject.SetActive(false);

        do
        {
            var waste = ClearOneWaste();
            
            if (waste == null)
            {
                yield break;
            }
            
            /* поворот в сторону мусора */
            if (Math.Abs(waste.gameObject.transform.position.x) - Math.Abs(transform.position.x) < 0)
                //if (Math.Abs(transform.position.x) - Math.Abs(waste.gameObject.transform.position.x) > 0)
            { // право
                if(isLeft)
                    flipX();
            }
            else
            { // лево
                if(!isLeft)
                    flipX();
            }
            
            yield return new WaitForSeconds(0.2f); // продолжить примерно через 200ms

            var currentWaste = wastesInArea.FindLast(o=>o == waste);

            // add pick up item to up ui label
            UpdateUi();
        
            // remove waste (gameobject)
            wastesInArea.Remove(currentWaste);
            DestroyImmediate(currentWaste.gameObject);
        } while (wastesInArea.Count != 0);

        // set active button for next pick up
        if(wastesInArea.Count == 0) 
            PickUpButton.gameObject.SetActive(false);
        else
            PickUpButton.gameObject.SetActive(true);
        /**/
        
        
        
        stickAnimator.SetBool("isPick", false);
        sosAnimator.SetBool("isPick", false);
        handAnimator.SetBool("isPick", false);
        
        yield return null;
    }
    
    public void PickUp()
    {
        if(haveSos)
            StartCoroutine(AllPickUpAnimationCoroutine());
        else
            StartCoroutine(PickUpAnimationCoroutine());
    }

    void UpdateUi()
    {
        countFood.text = $"{_countFood}/{maxItemPerType}";
        countBrokenBottle.text = $"{_countBrokenBottle}/{maxItemPerType}";
        countPackets.text = $"{_countPackets}/{maxItemPerType}";
        countCover.text = $"{_countCover}к";
        countMoney.text = $"{_countMoney}₽";
    }

    #region shop
    
    [Header("Ларек")]
    public GameObject hand;
    public GameObject handWithStick;
    public GameObject handWithSos;
    public GameObject bagGameObject;

    // расценки, чо
    private int foodCost = 10;
    private int packetCost = 5;
    private int brokenBottleCost = 15;
    private int coverCost = 7;


    public void drop_all() // echange waste to money
    {
        var beforeMoney = _countMoney;
        
        _countMoney += _countFood * foodCost;
        _countMoney += _countPackets * packetCost;
        _countMoney += _countBrokenBottle * brokenBottleCost;
        _countMoney += _countCover * coverCost;

        // обнуляем
        _countFood = 0;
        _countPackets = 0;
        _countBrokenBottle = 0;
        _countCover = 0;

        if(_countMoney > beforeMoney)
            AlertLabel.text = "Пойдет, жду еще!";
        else
            AlertLabel.text = "Слабенько, давай еще на поиски";

        UpdateUi();
    }

    public void CloseOrShowShop()
    {
        AlertLabel.text = "";
        if (ShopButton.activeSelf)
        {
            UIGroup.SetActive(false);
            UIShop.SetActive(true);
            ShopButton.SetActive(false);
        }
        else
        {
            UIGroup.SetActive(true);
            UIShop.SetActive(false);
            ShopButton.SetActive(true);
        }
    }


    private bool haveStick;
    private bool haveSos;
    private bool haveBag;
    
    // TODO выставь цены
    private int bagCost = 20;
    private int stickCost = 40;
    private int sosCost = 80; // 60

    public void buyBag()
    {
        if (haveBag)
        {
            AlertLabel.text = "У вас уже есть мешок!";
        }
        else
        {
            if (_countMoney < bagCost)
            {
                AlertLabel.text = "Не хватает деняк!(";
                return;
            }

            /* покупаем */
            bagGameObject.SetActive(true);
            haveBag = true;

            // баф
            maxItemPerType *= 2;
            //
            AlertLabel.text = "Вместимость увеличена!";
            
            // вычитаем деньги!11 :D
            _countMoney -= bagCost;
            
            UpdateUi();
        }
    }
    public void buyStick()
    {
        if (haveStick)
        {
            AlertLabel.text = "У вас уже есть пика!";
        }
        else
        {
            if (haveSos)
            {
                AlertLabel.text = "У вас уже есть мусоросборщик!";
            }
            else
            {
                if (_countMoney < stickCost)
                {
                    AlertLabel.text = "Не хватает деняк!(";
                    return;
                }
                
                /* покупаем */
                hand.SetActive(false);
                handWithStick.SetActive(true);
                handWithSos.SetActive(false);
                haveStick = true;

                // баф
                GetComponent<CircleCollider2D>().radius = defaultRadius + 1;
                //
                
                AlertLabel.text = "Радиус захвата стал больше!";
                
                // вычитаем деньги!11 :D
                _countMoney -= stickCost;
                
                UpdateUi();
            }
        }
    }
    public void buySos()
    {
        if (haveSos)
        {
            AlertLabel.text = "Уже куплено!";
        }
        else
        {
            if (_countMoney < sosCost)
            {
                AlertLabel.text = "Не хватает деняк!(";
                return;
            }
            
            /* покупаем */
            hand.SetActive(false);
            handWithStick.SetActive(false);
            handWithSos.SetActive(true);
            haveSos = true;
            
            // баф
            GetComponent<CircleCollider2D>().radius = defaultRadius + 1;
            //
            
            AlertLabel.text = "Вперед покорять мусоор!";
            
            // вычитаем деньги!11 :D
            _countMoney -= sosCost;
            
            UpdateUi();
        }
    }

    #endregion
}
