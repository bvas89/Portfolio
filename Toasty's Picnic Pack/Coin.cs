
public class Coin : Collectible
{
    public override void Collect()
    {
        Actions.Game.GetPoint(GameData.Data.Points.Coin);
        Destroy(gameObject);
    }
}
