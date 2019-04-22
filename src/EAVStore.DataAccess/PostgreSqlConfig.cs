namespace EAVStore.DataAccess
{
    public class PostgreSqlConfig
    {
        public string HostAddress { get; set; }

        public string HostPort { get; set; }

        public string Database { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string ToConnectionString() {
            return $"Host={HostAddress};Port={HostPort};Database={Database};Username={UserName};Password={Password};";
        }
    }
}