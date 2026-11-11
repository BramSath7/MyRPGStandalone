using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Item_Combat : MonoBehaviour {
    public static UI_Item_Combat instance;
    public Image Coin;
    public Image Bread;
    public TextMeshProUGUI CoinTxt;
    public TextMeshProUGUI BreadTxt;
    private void Awake() {
        instance = this;
        var coin = transform.Find("PnlCoin");
        var bread = transform.Find("PnlBread");
        Coin = coin.Find("ImgCoin").GetComponent<Image>();
        CoinTxt = coin.Find("TxtCoin").GetComponent<TextMeshProUGUI>();
        Bread = bread.Find("ImgBread").GetComponent<Image>();
        BreadTxt = bread.Find("TxtBread").GetComponent<TextMeshProUGUI>();
        GameNotify.OnGameLoaded += Init;
    }

    private void OnDestroy()
    {
        GameNotify.OnGameLoaded -= Init;
    }
    public void Init() {
        var coin = GameManager.instance.Coin;
        var bread = GameManager.instance.Bread;

        Coin.sprite = coin.Icon;
        Bread.sprite = bread.Icon;

        CoinTxt.text = coin.Current.ToString();
        BreadTxt.text = bread.Current.ToString();

        coin.OnChanged += value => { CoinTxt.text = $"Coin: {value}x"; };
        bread.OnChanged += value => { BreadTxt.text = $"Bread: {value}x"; };
    }
}
