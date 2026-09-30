using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FoodSlot : MonoBehaviour
{
    private Image imgFood;

    private Color normalColor = new Color(1f, 1f, 1f, 1f);
    private Color fadeColor = new Color(1f, 1f, 1f, 0.6f);

    private GrillStation grillCtrl;

    void Awake()
    {
        imgFood = this.transform.GetChild(0).GetComponent<Image>();
        imgFood.gameObject.SetActive(false);
        grillCtrl = this.transform.parent.parent.GetComponent<GrillStation>();
    }

    public void OnSetSlot(Sprite spr)
    {
        imgFood.gameObject.SetActive(true);
        imgFood.sprite = spr;
        imgFood.SetNativeSize();
    }

    public void OnActiveFood(bool active)
    {
        imgFood.gameObject.SetActive(active);
        imgFood.color = normalColor;
    }

    public void OnFadeFood()
    {
        this.OnActiveFood(true);
        imgFood.color = fadeColor;
    }

    public void OnHideFood()
    {
        this.OnActiveFood(false);
        imgFood.color = normalColor;
    }

    public void OnCheckMerge()
    {
        grillCtrl?.OnCheckMerge();
    }

    public void OnPrepareItem(Image img)
    {
        this.OnSetSlot(img.sprite);
        imgFood.color = normalColor;
        imgFood.transform.position = img.transform.position;
        imgFood.transform.localScale = img.transform.localScale;
        imgFood.transform.localEulerAngles = img.transform.localEulerAngles;

        imgFood.transform.DOLocalMove(Vector3.zero, 0.3f);
        imgFood.transform.DOScale(new Vector3(1.6f, 1.6f, 1f), 0.3f);
        imgFood.transform.DORotate(Vector3.zero, 0.2f);
    }

    public void OnCheckPrepareTray()
    {
        grillCtrl?.OnCheckPrepareTray();
    }

    public void DoShake()
    {
        imgFood.transform.DOShakePosition(0.5f, 10f, 10, 180f);
    }

    public FoodSlot GetSlotNull => grillCtrl != null ? grillCtrl.GetSlotNull() : null;

    public bool HasFood => imgFood.gameObject.activeInHierarchy && imgFood.color == normalColor;
    public Sprite GetSpriteFood => imgFood.sprite;
}
