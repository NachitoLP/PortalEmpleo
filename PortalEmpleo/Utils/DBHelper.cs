namespace PortalEmpleo.Utils
{
    public static class DBHelper
    {
        public static string GetConnectionString()
        {
            string server = "FX-NB-001\\MSSQLSERVER02";
            string database = "PortalEmpleo";
            return $"Data Source={server};Initial Catalog={database};Integrated Security=True;TrustServerCertificate=True";
        }
    }
}
