namespace C8POC.Interfaces
{
    /// <summary>
    /// Interface that any sound plugin should implement
    /// </summary>
    public interface ISoundPlugin : IPlugin
    {
        /// <summary>
        /// Action raised to generate a sound
        /// </summary>
        void GenerateSound();
    }
}