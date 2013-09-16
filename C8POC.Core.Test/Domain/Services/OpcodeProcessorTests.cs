// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OpcodeProcessorTests.cs" company="AlFranco">
//   Albert Rodriguez Franco 2013
// </copyright>
// <summary>
//   Defines the OpcodeProcessorTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace C8POC.Core.Test.Domain.Services
{
    using System.Linq;

    using C8POC.Core.Domain.Services;

    using Xunit;

    /// <summary>
    /// Tests for opcode processor
    /// </summary>
    public class OpcodeProcessorTests
    {
        /// <summary>
        /// Tests the ignored function
        /// </summary>
        [Fact()]
        public void JumpToRoutineAtAdressTest()
        {
            // Arrange
            var sut = this.GetDefaultOpcodeProcessor();
            var machineState = FixtureUtils.DefaultMachineState();

            // Act
            sut.JumpToRoutineAtAdress(machineState);

            // Assert
            Assert.True(!machineState.Stack.Any());
        }

        [Fact()]
        public void ClearScreenTest()
        {
            // Arrange
            var sut = this.GetDefaultOpcodeProcessor();
            var machineState = FixtureUtils.DefaultMachineState();

            // Act
            sut.ClearScreen(machineState);

            // Assert
            Assert.True(!machineState.Graphics.Cast<bool>().Any(x => x));
        }

        [Fact()]
        public void ReturnFromSubRoutineTest()
        {
            // Arrange
            var sut = this.GetDefaultOpcodeProcessor();
            var machineState = FixtureUtils.DefaultMachineState();
            machineState.Stack.Push(0x305);

            // Act
            sut.ReturnFromSubRoutine(machineState);

            // Assert
            Assert.Equal(machineState.ProgramCounter, 0x305);
            Assert.True(machineState.Stack.Count == 0);
        }

        [Fact()]
        public void JumpTest()
        {
            // Arrange
            var sut = this.GetDefaultOpcodeProcessor();
            var machineState = FixtureUtils.DefaultMachineState();
            machineState.CurrentOpcode = 0x1234;

            // Act
            sut.Jump(machineState);

            // Assert
            Assert.Equal(machineState.ProgramCounter, 0x234);
        }

        [Fact()]
        public void CallAtAdressTest()
        {
            // Arrange
            var sut = this.GetDefaultOpcodeProcessor();
            var machineState = FixtureUtils.DefaultMachineState();
            machineState.ProgramCounter = 0x555;
            machineState.CurrentOpcode = 0x2345;

            // Act
            sut.CallAtAdress(machineState);

            // Assert
            Assert.Equal(machineState.ProgramCounter, 0x345);
            Assert.Equal(machineState.Stack.Peek(), 0x555);
        }

        [Fact()]
        public void SkipNextInstructionIfRegisterEqualsImmediateTest()
        {
            // Arrange
            var sut = this.GetDefaultOpcodeProcessor();
            var machineState = FixtureUtils.DefaultMachineState();
            machineState.ProgramCounter = 0x500;
            machineState.CurrentOpcode = 0x3456;
            machineState.VRegisters[4] = 0x56;

            // Act
            sut.SkipNextInstructionIfRegisterEqualsImmediate(machineState);

            // Assert
            Assert.Equal(machineState.ProgramCounter, 0x502);
        }

        [Fact()]
        public void SkipNextInstructionIfRegisterNotEqualsImmediateTest()
        {
            // Arrange
            var sut = this.GetDefaultOpcodeProcessor();
            var machineState = FixtureUtils.DefaultMachineState();
            machineState.ProgramCounter = 0x500;
            machineState.CurrentOpcode = 0x4456;
            machineState.VRegisters[4] = 0x57;

            // Act
            sut.SkipNextInstructionIfRegisterNotEqualsImmediate(machineState);

            // Assert
            Assert.Equal(machineState.ProgramCounter, 0x502);
        }

        [Fact()]
        public void SkipNextInstructionIfRegisterEqualsRegisterTest()
        {
            // Arrange
            var sut = this.GetDefaultOpcodeProcessor();
            var machineState = FixtureUtils.DefaultMachineState();
            machineState.ProgramCounter = 0x500;
            machineState.CurrentOpcode = 0x5450;
            machineState.VRegisters[4] = 0x57;
            machineState.VRegisters[5] = 0x57;

            // Act
            sut.SkipNextInstructionIfRegisterEqualsRegister(machineState);

            // Assert
            Assert.Equal(machineState.ProgramCounter, 0x502);
        }

        [Fact()]
        public void LoadValueIntoRegisterTest()
        {
            // Arrange
            var sut = this.GetDefaultOpcodeProcessor();
            var machineState = FixtureUtils.DefaultMachineState();
            machineState.CurrentOpcode = 0x6450;
            machineState.VRegisters[4] = 0x00;

            // Act
            sut.LoadValueIntoRegister(machineState);

            // Assert
            Assert.Equal(machineState.VRegisters[4], 0x50);
        }

        [Fact()]
        public void AddValueIntoRegisterTest()
        {
            // Arrange
            var sut = this.GetDefaultOpcodeProcessor();
            var machineState = FixtureUtils.DefaultMachineState();
            machineState.CurrentOpcode = 0x7630;
            machineState.VRegisters[6] = 0x25;

            // Act
            sut.AddValueIntoRegister(machineState);

            // Assert
            Assert.Equal(machineState.VRegisters[6], 0x30 + 0x25);
        }

        [Fact()]
        public void LoadRegisterIntoRegisterTest()
        {
            // Arrange
            var sut = this.GetDefaultOpcodeProcessor();
            var machineState = FixtureUtils.DefaultMachineState();
            machineState.CurrentOpcode = 0x8230;
            machineState.VRegisters[3] = 0x80;

            // Act
            sut.LoadRegisterIntoRegister(machineState);

            // Assert
            Assert.Equal(machineState.VRegisters[2], 0x80);
        }

        [Fact()]
        public void OrRegistersIntoRegisterTest()
        {
            // Arrange
            var sut = this.GetDefaultOpcodeProcessor();
            var machineState = FixtureUtils.DefaultMachineState();
            machineState.CurrentOpcode = 0x8231;

            machineState.VRegisters[2] = 0x82;
            machineState.VRegisters[3] = 0x85;

            // Act
            sut.OrRegistersIntoRegister(machineState);

            // Assert
            Assert.Equal(machineState.VRegisters[2], 0x82 | 0x85);
        }

        [Fact()]
        public void AndRegistersIntoRegiterTest()
        {
            // Arrange
            var sut = this.GetDefaultOpcodeProcessor();
            var machineState = FixtureUtils.DefaultMachineState();
            machineState.CurrentOpcode = 0x8232;

            machineState.VRegisters[2] = 0x82;
            machineState.VRegisters[3] = 0x85;

            // Act
            sut.AndRegistersIntoRegiter(machineState);

            // Assert
            Assert.Equal(machineState.VRegisters[2], 0x82 & 0x85);
        }

        [Fact()]
        public void ExclusiveOrIntoRegisterTest()
        {
            // Arrange
            var sut = this.GetDefaultOpcodeProcessor();
            var machineState = FixtureUtils.DefaultMachineState();
            machineState.CurrentOpcode = 0x8233;

            machineState.VRegisters[2] = 0x82;
            machineState.VRegisters[3] = 0x85;

            // Act
            sut.ExclusiveOrIntoRegister(machineState);

            // Assert
            Assert.Equal(machineState.VRegisters[2], 0x82 ^ 0x85);
        }

        [Fact()]
        public void AddRegistersIntoRegisterTest()
        {
            Assert.True(false, "not implemented yet");
        }

        [Fact()]
        public void SubstractRegistersTest()
        {
            Assert.True(false, "not implemented yet");
        }

        [Fact()]
        public void ShiftRegisterRightTest()
        {
            Assert.True(false, "not implemented yet");
        }

        [Fact()]
        public void SubstractRegistersReverseTest()
        {
            Assert.True(false, "not implemented yet");
        }

        [Fact()]
        public void ShiftRegisterLeftTest()
        {
            Assert.True(false, "not implemented yet");
        }

        [Fact()]
        public void SkipNextInstructionIfRegisterNotEqualsRegisterTest()
        {
            Assert.True(false, "not implemented yet");
        }

        [Fact()]
        public void LoadIntoIndexRegisterTest()
        {
            Assert.True(false, "not implemented yet");
        }

        [Fact()]
        public void JumpToV0PlusImmediateTest()
        {
            Assert.True(false, "not implemented yet");
        }

        [Fact()]
        public void LoadRandomIntoRegisterTest()
        {
            Assert.True(false, "not implemented yet");
        }

        [Fact()]
        public void DrawSpriteTest()
        {
            Assert.True(false, "not implemented yet");
        }

        [Fact()]
        public void SkipNextInstructionIfRegisterEqualsKeyPressedTest()
        {
            Assert.True(false, "not implemented yet");
        }

        [Fact()]
        public void SkipNextInstructionIfRegisterNotEqualsKeyPressedTest()
        {
            Assert.True(false, "not implemented yet");
        }

        [Fact()]
        public void LoadTimerValueIntoRegisterTest()
        {
            Assert.True(false, "not implemented yet");
        }

        [Fact()]
        public void LoadKeyIntoRegisterTest()
        {
            Assert.True(false, "not implemented yet");
        }

        [Fact()]
        public void LoadRegisterIntoDelayTimerTest()
        {
            Assert.True(false, "not implemented yet");
        }

        [Fact()]
        public void LoadRegisterIntoSoundTimerTest()
        {
            Assert.True(false, "not implemented yet");
        }

        [Fact()]
        public void AddRegisterToIndexRegisterTest()
        {
            Assert.True(false, "not implemented yet");
        }

        [Fact()]
        public void LoadFontSpriteLocationFromValueInRegisterTest()
        {
            Assert.True(false, "not implemented yet");
        }

        [Fact()]
        public void LoadBcdRepresentationFromRegisterTest()
        {
            Assert.True(false, "not implemented yet");
        }

        [Fact()]
        public void LoadAllRegistersFromValueInRegisterTest()
        {
            Assert.True(false, "not implemented yet");
        }

        [Fact()]
        public void LoadFromValueInRegisterIntoAllRegistersTest()
        {
            Assert.True(false, "not implemented yet");
        }

        #region Helpers

        /// <summary>
        /// Gets a default opcodeprocessor for our tests
        /// </summary>
        /// <returns>
        /// The <see cref="OpcodeProcessor"/>.
        /// </returns>
        private OpcodeProcessor GetDefaultOpcodeProcessor()
        {
            return new OpcodeProcessor();
        }

        #endregion
    }
}