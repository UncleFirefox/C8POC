using System.Collections;
namespace C8POC.Interfaces
{
    public interface IGraphicsPlugin : IPlugin
    {
        /// <summary>
        /// Action to Draw
        /// </summary>
        void Draw(BitArray graphics);
    }
}