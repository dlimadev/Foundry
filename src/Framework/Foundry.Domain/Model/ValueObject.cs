namespace Foundry.Domain.Model
{
    /// <summary>
    /// An abstract base class for Value Objects. Value Objects are objects that
    /// are defined by their attributes, not by a unique identity.
    /// This base class provides a standard implementation for equality comparison.
    /// </summary>
    public abstract class ValueObject
    {
        /// <summary>
        /// When overridden in a derived class, gets the components that define the value of the object.
        /// </summary>
        /// <returns>An IEnumerable of the value-defining components.</returns>
        protected abstract IEnumerable<object> GetEqualityComponents();

        public override bool Equals(object? obj)
        {
            if (obj == null || obj.GetType() != GetType())
            {
                return false;
            }

            var other = (ValueObject)obj;
            return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
        }

        public override int GetHashCode()
        {
            return GetEqualityComponents()
                .Select(x => x != null ? x.GetHashCode() : 0)
                .Aggregate((x, y) => x ^ y);
        }

        public static bool operator ==(ValueObject a, ValueObject b)
        {
            if (a is null && b is null) return true;
            if (a is null || b is null) return false;
            return a.Equals(b);
        }

        public static bool operator !=(ValueObject a, ValueObject b) => !(a == b);
    }
}