public class ShowTutorial : ISaveData
{
    public bool isNeedShow;
    public bool isNeedShowBuy;

    public ShowTutorial(string _)
    {
        isNeedShow = true;
        isNeedShowBuy = true;
    }
}