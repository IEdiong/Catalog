namespace Catalog.Domain.Common;

public abstract class Entity : IEquatable<Entity>
{
    public Guid Id { get; protected set; }
    
    protected Entity(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Id cannot be empty", nameof(id));
        Id = id;
    }
    
    protected Entity() { }

    public bool Equals(Entity? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return GetType() == other.GetType() && Id.Equals(other.Id);
    }

    public override bool Equals(object? obj) => Equals(obj as Entity);

    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(Entity? left, Entity? right) => left?.Equals(right) ?? right is null;

    public static bool operator !=(Entity? left, Entity? right) => !(left == right);
}