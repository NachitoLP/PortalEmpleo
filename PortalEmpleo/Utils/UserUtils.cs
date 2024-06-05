namespace PortalEmpleo.Utils
{
    public static class UserUtils
    {
        public static int CalculateAge(DateTime user_birth_date)
        {
            DateTime actualDate = DateTime.Today;
            int user_age = actualDate.Year - user_birth_date.Year;

            if (user_birth_date > actualDate.AddYears(-user_age))
                user_age--;
            return user_age;
        }
    }
}
