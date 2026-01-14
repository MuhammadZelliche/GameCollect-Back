namespace ProjetSecret.Utils
{
  public static class RarityHelper
  {
    private static readonly Random _random = new Random();

    public static string GetRandomRarete()
    {
      var roll = _random.Next(100);

      if (roll < 65) return "Commun";      // 65%
      if (roll < 90) return "Rare";        // 25%
      return "Très Rare";                  // 10%
    }
  }

}

