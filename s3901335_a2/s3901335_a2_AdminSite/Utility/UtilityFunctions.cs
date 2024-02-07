namespace s3901335_a2_AdminSite.Utility;

public static class UtilityFunctions
{
    public static Boolean hasMoreThanTwoDecimalPlaces(this decimal value)
    {
        decimal fractionalPart = value - Math.Floor(value);

        return fractionalPart.ToString().Length > 4;
    }
}