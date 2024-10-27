namespace CartAPI.Business.Services
{
    public static class CacheKeys
    {
        public static class Cart
        {
            private const string Prefix = "Cart";
            public static string GetKey(string userId) => $"{Prefix}:{userId}";
        }
        public static class Product // yapım aşamasında
        {
            private const string Prefix = "Product";
            public static string GetKey(string userId) => $"{Prefix}:{userId}";
        }
        public static class User // yapım aşamasında
        {
            private const string Prefix = "User";
            public static string GetKey(string userId) => $"{Prefix}:{userId}";
        }
    }
}
