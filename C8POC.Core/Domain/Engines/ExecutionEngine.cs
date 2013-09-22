// -----------------------------------------------------------------------
// <copyright file="ExecutionEngine.cs" company="AlFranco">
// Albert Rodriguez Franco 2013
// </copyright>
// -----------------------------------------------------------------------

namespace C8POC.Core.Domain.Engines
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;

    using C8POC.Core.Properties;
    using C8POC.Interfaces.Domain.Engines;
    using C8POC.Interfaces.Domain.Entities;
    using C8POC.Interfaces.Domain.Services;

    /// <summary>
    /// Represents the engine in charge of the execution
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    public class ExecutionEngine : IExecutionEngine
    {
        /// <summary>
        /// Contains the mapping between an opcode and the function that should be executed
        /// </summary>
        private readonly Dictionary<ushort, Action<IMachineState>> instructionMap =
            new Dictionary<ushort, Action<IMachineState>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionEngine"/> class.
        /// </summary>
        /// <param name="opcodeMapService">
        /// The opcode map service.
        /// </param>
        public ExecutionEngine(IOpcodeMapService opcodeMapService)
        {
            this.OpcodeMapService = opcodeMapService;
            this.instructionMap = this.OpcodeMapService.GetInstructionMap();
        }

        /// <summary>
        /// Gets or sets the mediator.
        /// </summary>
        public IEngineMediator EngineMediator { get; set; }

        /// <summary>
        /// Gets or sets the opcode map service.
        /// </summary>
        public IOpcodeMapService OpcodeMapService { get; set; }

        /// <summary>
        /// Sets the mediator for the engine
        /// </summary>
        /// <param name="engineMediator">
        /// The engine mediator.
        /// </param>
        public void SetMediator(IEngineMediator engineMediator)
        {
            this.EngineMediator = engineMediator;
        }

        /// <summary>
        /// Starts the emulation using a delegate
        /// </summary>
        public void StartEmulatorExecution()
        {
            this.ExecutionLoop();
        }

        /// <summary>
        /// The emulator task.
        /// </summary>
        private void ExecutionLoop()
        {
            var cycleStopWatch = new Stopwatch();
            var millisecondsperframe = 1.0 / Settings.Default.FramesPerSecond * 1000.0;

            // Execution loop
            while (this.EngineMediator.IsRunning)
            {
                cycleStopWatch.Restart();

                this.EmulateFrame();

                // Adjust the speed of the frame in case it goes too fast
                Thread.Sleep((int)Math.Max(0.0, millisecondsperframe - cycleStopWatch.ElapsedMilliseconds));
            }
        }

        /// <summary>
        /// Emulates a frame
        /// </summary>
        private void EmulateFrame()
        {
            for (var cycleNum = 0; cycleNum < Settings.Default.CyclesPerFrame; cycleNum++)
            {
                this.EmulateCycle();

                // Check in the middle of the frame if we should stop
                if (!this.EngineMediator.IsRunning)
                {
                    return;
                }
            }

            if (this.EngineMediator.MachineState.IsDrawFlagSet)
            {
                this.EngineMediator.DrawGraphics();
                this.EngineMediator.MachineState.IsDrawFlagSet = false;
            }
        }

        /// <summary>
        /// Emulates a cycle
        /// </summary>
        private void EmulateCycle()
        {
            // Get Opcode located at program counter
            this.EngineMediator.MachineState.FetchOpcode();

            // Processes the opcode
            this.ProcessOpcode();

            // Update timers
            this.UpdateTimers();
        }

        /// <summary>
        /// Timer update every cycle
        /// </summary>
        private void UpdateTimers()
        {
            if (this.EngineMediator.MachineState.DelayTimer > 0)
            {
                --this.EngineMediator.MachineState.DelayTimer;
            }

            if (this.EngineMediator.MachineState.SoundTimer > 0)
            {
                if (this.EngineMediator.MachineState.SoundTimer == 1)
                {
                    this.EngineMediator.GenerateSound();
                }

                --this.EngineMediator.MachineState.SoundTimer;
            }
        }

        /// <summary>
        /// Obtains the opcode and executes the action associated to it
        /// </summary>
        private void ProcessOpcode()
        {
            try
            {
                this.instructionMap[(ushort)(this.EngineMediator.MachineState.CurrentOpcode & 0xF000)](this.EngineMediator.MachineState);
            }
            catch (KeyNotFoundException)
            {
                throw new Exception(
                    string.Format("Instruction with Opcode {0:X} is not implemented", this.EngineMediator.MachineState.CurrentOpcode));
            }
        }
    }
}
