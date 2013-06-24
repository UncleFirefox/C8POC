// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DebugOptions.cs" company="AlFranco">
//   Albert Rodriguez Franco 2013
// </copyright>
// <summary>
//   Defines the DebugOptions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace C8POC.WinFormsUI.Disassembly
{
    /// <summary>
    /// Options available once a break point is hit
    /// </summary>
    public enum DebugOptions
    {
        /// <summary>
        /// Continues the execution (equivalent to F5 in Visual Studio)
        /// </summary>
        Continue,

        /// <summary>
        /// Goes to next instruction (equivalent to F10 in Visual Studio)
        /// </summary>
        StepOver
    }
}
