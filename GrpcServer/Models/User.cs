namespace GrpcServer.Models
{
    public class User(string id, string userName, bool isActive)
    {
        public string Id { get; } = id;
        public string UserName { get; } = userName;
        public bool IsActive { get; } = isActive;
        

        public override string ToString()
        {
            return $"User [Id={Id}, UserName={UserName}, IsActive={IsActive}]";
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (obj is null || GetType() != obj.GetType()) return false;
            var other = (User)obj;
            return Id == other.Id && UserName == other.UserName;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, UserName);
        }
    }
}
