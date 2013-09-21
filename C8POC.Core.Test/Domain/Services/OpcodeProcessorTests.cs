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
        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
        public void AddRegistersIntoRegisterTest()
        {
            // Arrange
            var sut = this.GetDefaultOpcodeProcessor();
            var machineState = FixtureUtils.DefaultMachineState();
            machineState.CurrentOpcode = 0x8234;

            machineState.VRegisters[2] = 0xFF;
            machineState.VRegisters[3] = 0xFF;

            // Act
            sut.AddRegistersIntoRegister(machineState);

            // Assert
            Assert.Equal(machineState.VRegisters[2], 0xFF + 0xFF);
            Assert.Equal(machineState.VRegisters[0xF], 0x1);
        }

        [Fact]
        public void SubstractRegistersTest()
        {
            // Arrange
            var sut = this.GetDefaultOpcodeProcessor();
            var machineState = FixtureUtils.DefaultMachineState();
            machineState.CurrentOpcode = 0x8235;

            machineState.VRegisters[2] = 0xE0;
            machineState.VRegisters[3] = 0x10;

            // Act
            sut.SubstractRegisters(machineState);

            // Assert
            Assert.Equal(machineState.VRegisters[2], 0xE0 - 0x10);
            Assert.Equal(machineState.VRegisters[0xF], 0x1);
        }

        [Fact]
        public void ShiftRegisterRightTest()
        {
            // Arrange
            var sut = this.GetDefaultOpcodeProcessor();
            var machineState = FixtureUtils.DefaultMachineState();
            machineState.CurrentOpcode = 0x8236;

            machineState.VRegisters[2] = 0xE001;
            machineState.VRegisters[3] = 0xF001;

            // Act
            sut.ShiftRegisterRight(machineState);

            // Assert
            Assert.Equal(machineState.VRegisters[2], 0xF001 / 2);
            Assert.Equal(machineState.VRegisters[0xF], 0x1);
        }

        [Fact]
        public void SubstractRegistersReverseTest()
        {
            // Arrange
            var sut = this.GetDefaultOpcodeProcessor();
            var machineState = FixtureUtils.DefaultMachineState();
            machineState.CurrentOpcode = 0x8237;

            machineState.VRegisters[2] = 0xE001;
            machineState.VRegisters[3] = 0xFF01;

            // Act
            sut.SubstractRegistersReverse(machineState);

            // Assert
            Assert.Equal(machineState.VRegisters[2], 0xFF01 - 0xE001);
            Assert.Equal(machineState.VRegisters[0xF], 0x1);
        }

        [Fact]
        public void ShiftRegisterLeftTest()
        {
            // Arrange
            var sut = this.GetDefaultOpcodeProcessor();
            var machineState = FixtureUtils.DefaultMachineState();
            machineState.CurrentOpcode = 0x823E;

            machineState.VRegisters[2] = 0x8010;
            machineState.VRegisters[3] = 0x80FF;

            // Act
            sut.ShiftRegisterLeft(machineState);

            // Assert
            Assert.Equal(machineState.VRegisters[2], 0x80FF >> 1);
            Assert.Equal(machineState.VRegisters[0xF], 0x1);
        }

        [Fact]
        public void SkipNextInstructionIfRegisterNotEqualsRegisterTest()
        {
            // Arrange
            var sut = this.GetDefaultOpcodeProcessor();
            var machineState = FixtureUtils.DefaultMachineState();
            machineState.CurrentOpcode = 0x9238;
            machineState.ProgramCounter = 0x300;

            machineState.VRegisters[2] = 0x8010;
            machineState.VRegisters[3] = 0x80FF;

            // Act
            sut.SkipNextInstructionIfRegisterNotEqualsRegister(machineState);

            // Assert
            Assert.Equal(machineState.ProgramCounter, 0x302);
        }

        [Fact]
        public void LoadIntoIndexRegisterTest()
        {
            // Arrange
            var sut = this.GetDefaultOpcodeProcessor();
            var machineState = FixtureUtils.DefaultMachineState();
            machineState.CurrentOpcode = 0xA327;

            // Act
            sut.LoadIntoIndexRegister(machineState);

            // Assert
            Assert.Equal(machineState.IndexRegister, 0x327);
        }

        [Fact]
        public void JumpToV0PlusImmediateTest()
        {
            // Arrange
            var sut = this.GetDefaultOpcodeProcessor();
            var machineState = FixtureUtils.DefaultMachineState();
            machineState.CurrentOpcode = 0xB327;
            machineState.VRegisters[0x0] = 0x1111;

            // Act
            sut.JumpToV0PlusImmediate(machineState);

            // Assert
            Assert.Equal(machineState.ProgramCounter, 0x1111 + 0x0327);
        }

        [Fact]
        public void LoadRandomIntoRegisterTest()
        {
            // Arrange
            var sut = this.GetDefaultOpcodeProcessor();
            var machineState = FixtureUtils.DefaultMachineState();
            machineState.CurrentOpcode = 0xC320;
            machineState.VRegisters[3] = 0x1111;

            // Act
            sut.LoadRandomIntoRegister(machineState);

            // Assert
            Assert.NotEqual(machineState.VRegisters[3], 0x1111);
        }

        [Fact]
        public void DrawSpriteTest()
        {
            Assert.True(false, "not implemented yet");
        }

        [Fact]
        public void SkipNextInstructionIfRegisterEqualsKeyPressedTest()
        {
            // Arrange
            var sut = this.GetDefaultOpcodeProcessor();
            var machineState = FixtureUtils.DefaultMachineState();
            machineState.CurrentOpcode = 0xE29E;
            machineState.ProgramCounter = 0x300;

            machineState.VRegisters[0x2] = 0x9;
            machineState.Keys[0x9] = true;

            // Act
            sut.SkipNextInstructionIfRegisterEqualsKeyPressed(machineState);

            // Assert
            Assert.Equal(machineState.ProgramCounter, 0x302);
        }

        [Fact]
        public void SkipNextInstructionIfRegisterNotEqualsKeyPressedTest()
        {
            // Arrange
            var sut = this.GetDefaultOpcodeProcessor();
            var machineState = FixtureUtils.DefaultMachineState();
            machineState.CurrentOpcode = 0xE2A1;
            machineState.ProgramCounter = 0x300;

            machineState.VRegisters[0x2] = 0x9;
            machineState.Keys[0x9] = false;

            // Act
            sut.SkipNextInstructionIfRegisterNotEqualsKeyPressed(machineState);

            // Assert
            Assert.Equal(machineState.ProgramCounter, 0x302);
        }

        [Fact]
        public void LoadTimerValueIntoRegisterTest()
        {
            // Arrange
            var sut = this.GetDefaultOpcodeProcessor();
            var machineState = FixtureUtils.DefaultMachineState();
            machineState.CurrentOpcode = 0xF407;

            machineState.VRegisters[0x4] = 0x15;
            machineState.DelayTimer = 0x40;

            // Act
            sut.LoadTimerValueIntoRegister(machineState);

            // Assert
            Assert.Equal(machineState.VRegisters[0x4], machineState.DelayTimer);
        }

        [Fact]
        public void LoadKeyIntoRegisterTest()
        {
            // Arrange
            var sut = this.GetDefaultOpcodeProcessor();
            var machineState = FixtureUtils.DefaultMachineState();
            machineState.ProgramCounter = 0x402;
            machineState.CurrentOpcode = 0xF40A;
            machineState.VRegisters[0x4] = 0x0;
            machineState.Keys.SetAll(false);

            // Act
            sut.LoadKeyIntoRegister(machineState);

            // Assert
            Assert.Equal(machineState.VRegisters[0x4], 0x0);
            Assert.Equal(machineState.ProgramCounter, 0x400);

            // Arrange 2nd time, simulate key pressed
            machineState.Keys[0x3] = true;
            machineState.ProgramCounter = 0x402;

            // Act
            sut.LoadKeyIntoRegister(machineState);

            // Assert
            Assert.Equal(machineState.VRegisters[0x4], 0x3);
            Assert.Equal(machineState.ProgramCounter, 0x402);
        }

        [Fact]
        public void LoadRegisterIntoDelayTimerTest()
        {
            // Arrange
            var sut = this.GetDefaultOpcodeProcessor();
            var machineState = FixtureUtils.DefaultMachineState();
            machineState.CurrentOpcode = 0xF415;

            machineState.VRegisters[0x4] = 0x15;
            machineState.DelayTimer = 0x40;

            // Act
            sut.LoadRegisterIntoDelayTimer(machineState);

            // Assert
            Assert.Equal(machineState.DelayTimer, machineState.VRegisters[0x4]);
        }

        [Fact]
        public void LoadRegisterIntoSoundTimerTest()
        {
            // Arrange
            var sut = this.GetDefaultOpcodeProcessor();
            var machineState = FixtureUtils.DefaultMachineState();
            machineState.CurrentOpcode = 0xF418;

            machineState.VRegisters[0x4] = 0x15;
            machineState.SoundTimer = 0x40;

            // Act
            sut.LoadRegisterIntoSoundTimer(machineState);

            // Assert
            Assert.Equal(machineState.SoundTimer, machineState.VRegisters[0x4]);
        }

        [Fact]
        public void AddRegisterToIndexRegisterTest()
        {
            // Arrange
            var sut = this.GetDefaultOpcodeProcessor();
            var machineState = FixtureUtils.DefaultMachineState();
            machineState.CurrentOpcode = 0xF41E;

            machineState.VRegisters[0x4] = 0x15;
            machineState.IndexRegister = 0x40;

            // Act
            sut.AddRegisterToIndexRegister(machineState);

            // Assert
            Assert.Equal(machineState.IndexRegister, 0x15 + 0x40);
        }

        [Fact]
        public void LoadFontSpriteLocationFromValueInRegisterTest()
        {
            // Arrange
            var sut = this.GetDefaultOpcodeProcessor();
            var machineState = FixtureUtils.DefaultMachineState();
            machineState.CurrentOpcode = 0xF429;

            machineState.VRegisters[0x4] = 0x7;

            // Act
            sut.LoadFontSpriteLocationFromValueInRegister(machineState);

            // Assert
            Assert.Equal(machineState.IndexRegister, 5 * 0x7);
        }

        [Fact]
        public void LoadBcdRepresentationFromRegisterTest()
        {
            // Arrange
            var sut = this.GetDefaultOpcodeProcessor();
            var machineState = FixtureUtils.DefaultMachineState();
            machineState.CurrentOpcode = 0xF433;

            machineState.VRegisters[0x4] = 125;

            // Act
            sut.LoadBcdRepresentationFromRegister(machineState);

            // Assert
            Assert.Equal(machineState.Memory[machineState.IndexRegister], 1);
            Assert.Equal(machineState.Memory[machineState.IndexRegister + 1], 2);
            Assert.Equal(machineState.Memory[machineState.IndexRegister + 2], 5);
        }

        [Fact]
        public void LoadAllRegistersFromValueInRegisterTest()
        {
            // Arrange
            var sut = this.GetDefaultOpcodeProcessor();
            var machineState = FixtureUtils.DefaultMachineState();
            machineState.CurrentOpcode = 0xF455;

            machineState.VRegisters[0x0] = 0x12;
            machineState.VRegisters[0x1] = 0x13;
            machineState.VRegisters[0x2] = 0x14;
            machineState.VRegisters[0x3] = 0x15;
            machineState.VRegisters[0x4] = 0x16;

            // Act
            sut.LoadAllRegistersFromValueInRegister(machineState);

            // Assert
            Assert.Equal(machineState.Memory[machineState.IndexRegister], 0x12);
            Assert.Equal(machineState.Memory[machineState.IndexRegister + 1], 0x13);
            Assert.Equal(machineState.Memory[machineState.IndexRegister + 2], 0x14);
            Assert.Equal(machineState.Memory[machineState.IndexRegister + 3], 0x15);
            Assert.Equal(machineState.Memory[machineState.IndexRegister + 4], 0x16);
        }

        [Fact]
        public void LoadFromValueInRegisterIntoAllRegistersTest()
        {
            // Arrange
            var sut = this.GetDefaultOpcodeProcessor();
            var machineState = FixtureUtils.DefaultMachineState();
            machineState.CurrentOpcode = 0xF265;
            machineState.IndexRegister = 0x204;

            machineState.Memory[machineState.IndexRegister] = 0x10;
            machineState.Memory[machineState.IndexRegister + 1] = 0x11;
            machineState.Memory[machineState.IndexRegister + 2] = 0x12;

            // Act
            sut.LoadFromValueInRegisterIntoAllRegisters(machineState);

            // Assert
            Assert.Equal(machineState.Memory[machineState.IndexRegister], machineState.VRegisters[0]);
            Assert.Equal(machineState.Memory[machineState.IndexRegister + 1], machineState.VRegisters[1]);
            Assert.Equal(machineState.Memory[machineState.IndexRegister + 2], machineState.VRegisters[2]);
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