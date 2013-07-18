// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MachineDisassemblerInterceptor.cs" company="AlFranco">
//   Albert Rodriguez Franco 2013
// </copyright>
// <summary>
//   Defines the MachineDisassemblerInterceptor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace C8POC.WinFormsUI.Disassembly
{
    using System;
    using System.Threading;
    using System.Windows.Forms;

    using C8POC.Interfaces;
    using C8POC.WinFormsUI.Forms;

    using Castle.DynamicProxy;

    /// <summary>
    /// Intercepts the calls in instructions and displays what's happening in real time
    /// </summary>
    public class MachineDisassemblerInterceptor : IInterceptor
    {
        /// <summary>
        /// The disassembler form.
        /// </summary>
        private readonly DisassemblerForm disassemblerForm;

        /// <summary>
        /// Initializes a new instance of the <see cref="MachineDisassemblerInterceptor"/> class.
        /// </summary>
        /// <param name="disassemblerForm">
        /// The disassembler form.
        /// </param>
        public MachineDisassemblerInterceptor(DisassemblerForm disassemblerForm)
        {
            this.disassemblerForm = disassemblerForm;
            this.disassemblerForm.Show();
        }

        /// <summary>
        /// Intercepts the call in machine processing
        /// </summary>
        /// <param name="invocation">
        /// Intercepted action invocation
        /// </param>
        public void Intercept(IInvocation invocation)
        {
            var machineState = invocation.GetArgumentValue(0) as IMachineState;

            // Bind the grid with the opcodes if its empty
            if (this.disassemblerForm.IsOpcodeGridViewEmpty())
            {
                this.disassemblerForm.Invoke(new Action(() => this.disassemblerForm.BindOpcodeList(machineState)));
            }

            // Refresh the timer status since it's incremented after a cycle
            this.disassemblerForm.Invoke(
               new Action(() => this.disassemblerForm.RefreshDisassemblerStatus(machineState)));

            var currentRowPosition = this.disassemblerForm.GetGridRowPositionFromProgramCounter(
                machineState.ProgramCounter - 2);

            var debugAction = DebugOptions.Continue;

            if (this.disassemblerForm.HasHitBreakPoint(currentRowPosition))
            {
                this.disassemblerForm.SetHitState(currentRowPosition);
            }

            var wasPreviouslyDebugging = this.disassemblerForm.IsLineDebugging(currentRowPosition);

            if (wasPreviouslyDebugging)
            {
                debugAction = this.WaitForAction();
            }

            invocation.Proceed();

            var nextRowPosition = this.disassemblerForm.GetGridRowPositionFromProgramCounter(
                machineState.ProgramCounter);

            if (wasPreviouslyDebugging)
            {
                this.disassemblerForm.Invoke(
                    new Action(() => this.disassemblerForm.CleanUpDebuggedLine(currentRowPosition)));

                if (debugAction == DebugOptions.StepOver)
                {
                    this.disassemblerForm.Invoke(
                        new Action(() => this.disassemblerForm.SetStepOverState(nextRowPosition)));
                }
            }

            this.disassemblerForm.Invoke(
                new Action(() => this.disassemblerForm.RefreshDisassemblerStatus(machineState)));
        }

        /// <summary>
        /// Waits for keys step over (F10) or continue (F5)
        /// </summary>
        /// <returns>
        /// The <see cref="DebugOptions"/>.
        /// </returns>
        private DebugOptions WaitForAction()
        {
            var wait = new ManualResetEvent(false);
            var keyPressed = Keys.None;
            
            KeyEventHandler keyUpEvent = delegate(object sender, KeyEventArgs args)
                {
                    if (args.KeyCode != Keys.F10 && args.KeyCode != Keys.F5)
                    {
                        return;
                    }

                    keyPressed = args.KeyCode;
                    wait.Set();
                };

            this.disassemblerForm.KeyUp += keyUpEvent;

            wait.WaitOne();

            this.disassemblerForm.KeyUp -= keyUpEvent;

            return keyPressed == Keys.F10 ? DebugOptions.StepOver : DebugOptions.Continue;
        }
    }
}
