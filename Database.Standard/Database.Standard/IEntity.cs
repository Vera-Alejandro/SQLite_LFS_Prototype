
namespace Interstates.Control.Database
{
    /// <summary>
    /// Represents an entity object from a database.
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// The state of the database object.
        /// </summary>
        EntityState EntityState { get; set;}
    }
}
