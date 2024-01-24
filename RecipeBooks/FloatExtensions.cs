namespace RecipeBooks
{
    public static  class FloatExtensions
    {
        // ************************
        public static bool Between(this float nbr, float low, float high)
        {
            if (nbr < low)
            {
                return false;
            }

            if (nbr > high)
            {
                return false;
            }

            return true;
        }
    }
    // ************************

}
