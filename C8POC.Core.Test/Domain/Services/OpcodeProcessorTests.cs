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
            Assert.True(false, "not implemented yet");
        }

        [Fact()]
        public void JumpTest()
        {
            Assert.True(false, "not implemented yet");
        }

        [Fact()]
        public void CallAtAdressTest()
        {
            Assert.True(false, "not implemented yet");
        }

        [Fact()]
        public void SkipNextInstructionIfRegisterEqualsImmediateTest()
        {
            Assert.True(false, "not implemented yet");
        }

        [Fact()]
        public void SkipNextInstructionIfRegisterNotEqualsImmediateTest()
        {
            Assert.True(false, "not implemented yet");
        }

        [Fact()]
        public void SkipNextInstructionIfRegisterEqualsRegisterTest()
        {
            Assert.True(false, "not implemented yet");
        }

        [Fact()]
        public void LoadValueIntoRegisterTest()
        {
            Assert.True(false, "not implemented yet");
        }

        [Fact()]
        public void AddValueIntoRegisterTest()
        {
            Assert.True(false, "not implemented yet");
        }

        [Fact()]
        public void LoadRegisterIntoRegisterTest()
        {
            Assert.True(false, "not implemented yet");
        }

        [Fact()]
        public void OrRegistersIntoRegisterTest()
        {
            Assert.True(false, "not implemented yet");
        }

        [Fact()]
        public void AndRegistersIntoRegiterTest()
        {
            Assert.True(false, "not implemented yet");
        }

        [Fact()]
        public void ExclusiveOrIntoRegisterTest()
        {
            Assert.True(false, "not implemented yet");
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