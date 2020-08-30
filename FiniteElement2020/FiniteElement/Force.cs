using DongUtility;

namespace FiniteElement
{
    /// <summary>
    /// An abstract base class for all forces
    /// The AddForce() function applies the force to all relevant projectiles for a given tick
    /// </summary>
    abstract public class Force
    {
        /// <summary>
        /// Applies to force to relevant projectiles.
        /// Should be called once per clock tick.
        /// </summary>
        abstract public void AddForce();
    }
}
