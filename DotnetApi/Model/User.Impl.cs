namespace DotnetApi.Model;

public partial class User : IEquatable<User>
{
    public bool Equals(User? other)
    {
        if (other is null) return false;

        if (ReferenceEquals(this, other)) return true;

        return other.Id == Id
               && other.FirstName == FirstName
               && other.LastName == LastName;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;

        if (ReferenceEquals(this, obj)) return true;

        if (obj.GetType() != GetType()) return false;

        return Equals((User)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, FirstName, LastName);
    }
}