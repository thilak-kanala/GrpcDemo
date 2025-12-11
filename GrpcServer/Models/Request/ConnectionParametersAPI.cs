namespace GrpcServer.Models.Request
{
    public class ConnectionParametersApi(string baseUrl, string username, string password)
    {
        public string BaseUrl { get; set; } = baseUrl;
        public string Username { get; set; } = username;
        public string Password { get; set; } = password;

        public override string ToString()
        {
            return $"ConnectionParameters [BaseUrl={BaseUrl}, Username={Username}, Password=******]";
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (obj is null || GetType() != obj.GetType()) return false;
            var other = (ConnectionParametersApi)obj;
            return BaseUrl == other.BaseUrl && Username == other.Username && Password == other.Password;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(BaseUrl, Username, Password);
        }
    }
}

