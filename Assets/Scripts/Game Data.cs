[System.Serializable]
public class GameData
{
    public int money;
    public int level;
    public int tableCount = 3;

    public GameData(int level, int money, int tableCount)
    {
        this.money = money;
        this.level = level;
        this.tableCount = tableCount;
    }

}