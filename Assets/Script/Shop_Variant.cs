using UnityEngine;

public class Shop_Bread : Shop_Item {
    public Shop_Bread()    {
        ID = "Test_Item_Bread";
        Name = "Bread";
        Desc = "Cuma Roti doang.";
        Icon = Resources.Load<Sprite>("Icons/Bread");
        AvailableStock = 10;
        Stack = 3;

        ReqValue = 1;
        ReqItem = "Gold";
    }
    public override void OnBuy()
    {
        GameManager.instance?.Bread.Gain(SelectedValue * Stack);
    }
}
