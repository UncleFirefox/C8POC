// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WindowsOpcodeMapService.cs" company="AlFranco">
//   Albert Rodriguez Franco 2013
// </copyright>
// <summary>
//   Defines the WindowsOpcodeMapService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace C8POC.WinFormsUI.Services
{
    using System;
    using System.Collections.Generic;

    using C8POC.Interfaces;

    /// <summary>
    /// The windows opcode map service.
    /// </summary>
    public class WindowsOpcodeMapService : IOpcodeMapService
    {
        /// <summary>
        /// Gets or sets the opcode processor.
        /// </summary>
        public IOpcodeProcessor OpcodeProcessor { get; set; }

        /// <summary>
        /// The instruction map.
        /// </summary>
        private Dictionary<ushort, Action<IMachineState>> instructionMap = new Dictionary<ushort, Action<IMachineState>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsOpcodeMapService"/> class.
        /// </summary>
        /// <param name="opcodeProcessor">
        /// The opcode processor.
        /// </param>
        public WindowsOpcodeMapService(IOpcodeProcessor opcodeProcessor)
        {
            this.OpcodeProcessor = opcodeProcessor;
            this.SetUpInstructionMap();
        }

        /// <summary>
        /// The get instruction map.
        /// </summary>
        /// <returns>
        /// The <see cref="Dictionary"/>.
        /// </returns>
        public Dictionary<ushort, Action<IMachineState>> GetInstructionMap()
        {
            return instructionMap;
        }

        /// <summary>
        /// Sets the instruction map up for fast opcode decoding
        /// </summary>
        private void SetUpInstructionMap()
        {
            this.instructionMap.Add(0x0000, this.GoToRoutineStartingWithZero);
            this.instructionMap.Add(0x00E0, this.OpcodeProcessor.ClearScreen);
            this.instructionMap.Add(0x00EE, this.OpcodeProcessor.ReturnFromSubRoutine);
            this.instructionMap.Add(0x1000, this.OpcodeProcessor.Jump);
            this.instructionMap.Add(0x2000, this.OpcodeProcessor.CallAtAdress);
            this.instructionMap.Add(0x3000, this.OpcodeProcessor.SkipNextInstructionIfRegisterEqualsImmediate);
            this.instructionMap.Add(0x4000, this.OpcodeProcessor.SkipNextInstructionIfRegisterNotEqualsImmediate);
            this.instructionMap.Add(0x5000, this.OpcodeProcessor.SkipNextInstructionIfRegisterEqualsRegister);
            this.instructionMap.Add(0x6000, this.OpcodeProcessor.LoadValueIntoRegister);
            this.instructionMap.Add(0x7000, this.OpcodeProcessor.AddValueIntoRegister);
            this.instructionMap.Add(0x8000, this.GoToArithmeticLogicInstruction);
            this.instructionMap.Add(0x8001, this.OpcodeProcessor.OrRegistersIntoRegister);
            this.instructionMap.Add(0x8002, this.OpcodeProcessor.AndRegistersIntoRegiter);
            this.instructionMap.Add(0x8003, this.OpcodeProcessor.ExclusiveOrIntoRegister);
            this.instructionMap.Add(0x8004, this.OpcodeProcessor.AddRegistersIntoRegister);
            this.instructionMap.Add(0x8005, this.OpcodeProcessor.SubstractRegisters);
            this.instructionMap.Add(0x8006, this.OpcodeProcessor.ShiftRegisterRight);
            this.instructionMap.Add(0x8007, this.OpcodeProcessor.SubstractRegistersReverse);
            this.instructionMap.Add(0x800E, this.OpcodeProcessor.ShiftRegisterLeft);
            this.instructionMap.Add(0x9000, this.OpcodeProcessor.SkipNextInstructionIfRegisterNotEqualsRegister);
            this.instructionMap.Add(0xA000, this.OpcodeProcessor.LoadIntoIndexRegister);
            this.instructionMap.Add(0xB000, this.OpcodeProcessor.JumpToV0PlusImmediate);
            this.instructionMap.Add(0xC000, this.OpcodeProcessor.LoadRandomIntoRegister);
            this.instructionMap.Add(0xD000, this.OpcodeProcessor.DrawSprite);
            this.instructionMap.Add(0xE000, this.GoToSkipRegisterInstruction);
            this.instructionMap.Add(0xE09E, this.OpcodeProcessor.SkipNextInstructionIfRegisterEqualsKeyPressed);
            this.instructionMap.Add(0xE0A1, this.OpcodeProcessor.SkipNextInstructionIfRegisterNotEqualsKeyPressed);
            this.instructionMap.Add(0xF000, this.GoToMemoryOperationInstruction);
            this.instructionMap.Add(0xF007, this.OpcodeProcessor.LoadTimerValueIntoRegister);
            this.instructionMap.Add(0xF00A, this.OpcodeProcessor.LoadKeyIntoRegister);
            this.instructionMap.Add(0xF015, this.OpcodeProcessor.LoadRegisterIntoDelayTimer);
            this.instructionMap.Add(0xF018, this.OpcodeProcessor.LoadRegisterIntoSoundTimer);
            this.instructionMap.Add(0xF01E, this.OpcodeProcessor.AddRegisterToIndexRegister);
            this.instructionMap.Add(0xF029, this.OpcodeProcessor.LoadFontSpriteLocationFromValueInRegister);
            this.instructionMap.Add(0xF033, this.OpcodeProcessor.LoadBcdRepresentationFromRegister);
            this.instructionMap.Add(0xF055, this.OpcodeProcessor.LoadAllRegistersFromValueInRegister);
            this.instructionMap.Add(0xF065, this.OpcodeProcessor.LoadFromValueInRegisterIntoAllRegisters);
        }

        #region Internal Instruction Map Set

        /// <summary>
        /// Discriminates further into the instruction mapper for instructions starting with 0x0
        /// </summary>
        /// <param name="machineState">
        /// The machine State.
        /// </param>
        private void GoToRoutineStartingWithZero(IMachineState machineState)
        {
            var fetchedOpcode = machineState.CurrentOpcode & 0xF0FF;

            if (fetchedOpcode == 0x0000)
            {
                this.OpcodeProcessor.JumpToRoutineAtAdress(machineState);
            }
            else
            {
                this.instructionMap[(ushort)fetchedOpcode](machineState);
            }
        }

        /// <summary>
        /// Discriminates further into the instruction mapper for instructions starting with 0x8
        /// </summary>
        /// <param name="machineState">
        /// The machine State.
        /// </param>
        private void GoToArithmeticLogicInstruction(IMachineState machineState)
        {
            var filteredOpcode = (ushort)(machineState.CurrentOpcode & 0xF00F);

            if (filteredOpcode == 0x8000)
            {
                this.OpcodeProcessor.LoadRegisterIntoRegister(machineState);
            }
            else
            {
                this.instructionMap[filteredOpcode](machineState);
            }
        }

        /// <summary>
        /// Discriminates instructions starting with 0xE
        /// </summary>
        /// <param name="machineState">
        /// The machine State.
        /// </param>
        private void GoToSkipRegisterInstruction(IMachineState machineState)
        {
            this.instructionMap[(ushort)(machineState.CurrentOpcode & 0xF0FF)](machineState);
        }

        /// <summary>
        /// Discriminates better for instructionmap opcodes starting with 0xF
        /// </summary>
        /// <param name="machineState">
        /// The machine State.
        /// </param>
        private void GoToMemoryOperationInstruction(IMachineState machineState)
        {
            this.instructionMap[(ushort)(machineState.CurrentOpcode & 0xF0FF)](machineState);
        }

        #endregion
    }
}
