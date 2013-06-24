// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BreakPointStates.cs" company="AlFranco">
//   Albert Rodriguez Franco 2013
// </copyright>
// <summary>
//   Defines the states a breakpoint can be
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace C8POC.WinFormsUI.Disassembly
{
    /// <summary>
    /// The break point states.
    /// </summary>
    public enum BreakPointStates
    {
        /// <summary>
        /// No breakpoint
        /// </summary>
        None,

        /// <summary>
        /// When a breakpoint is set (red dot)
        /// </summary>
        Set,

        /// <summary>
        /// When a breakpoint is hit (red dot with yellow arrow)
        /// </summary>
        Hit,

        /// <summary>
        /// When a line is being debugged using step over or step into (yellow arrow)
        /// </summary>
        Step
    }
}
