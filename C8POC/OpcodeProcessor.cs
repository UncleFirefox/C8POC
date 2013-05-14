// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OpcodeProcessor.cs" company="AlFranco">
//   Albert Rodriguez Franco 2013
// </copyright>
// <summary>
//   Interprets instructions working with a given MachineState
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace C8POC
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using C8POC.Interfaces;

    /// <summary>
    /// Interprets instructions working with a given MachineState
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Mnemotechnical stuff is obviously not recognized by StyleCop")]
    public class OpcodeProcessor : IOpcodeProcessor
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="OpcodeProcessor"/> class. 
        /// Constructor injecting a machine state
        /// </summary>
        /// <param name="machineState">
        /// A machine state
        /// </param>
        public OpcodeProcessor(IMachineState machineState)
        {
            this.MachineState = machineState;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpcodeProcessor"/> class. 
        /// Constructor that will potentially have a machine state injected later
        /// </summary>
        public OpcodeProcessor()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the machine state in which the instructions will operate
        /// </summary>
        public IMachineState MachineState { get; set; }

        #endregion

        #region Instruction Set

        // Instruction set taken from http://devernay.free.fr/hacks/chip8/C8TECH10.HTM#00E0

        /// <summary>
        /// 0nnn - SYS addr
        /// Jump to a machine code routine at nnn.
        /// This instruction is only used on the old computers on which Chip-8 was originally implemented. 
        /// It is ignored by modern interpreters.
        /// </summary>
        public void JumpToRoutineAtAdress()
        {
            // IGNORE! xD
        }

        /// <summary>
        /// 00E0 - CLS
        /// Clear the display.
        /// </summary>
        public void ClearScreen()
        {
            this.MachineState.Graphics.SetAll(false);
            this.MachineState.IsDrawFlagSet = true;
        }

        /// <summary>
        /// 00EE - RET
        /// Return from a subroutine.
        /// The interpreter sets the program counter to the address at the top of the machineState.Stack, 
        /// then subtracts 1 from the machineState.Stack pointer
        /// </summary>
        public void ReturnFromSubRoutine()
        {
            this.MachineState.ProgramCounter = this.MachineState.Stack.Pop();
        }

        /// <summary>
        /// 1nnn - JP addr
        /// Jump to location nnn.
        /// The interpreter sets the program counter to nnn.
        /// </summary>
        public void Jump()
        {
            this.MachineState.ProgramCounter = (ushort)(this.MachineState.CurrentOpcode & 0x0FFF);
        }

        /// <summary>
        /// 2nnn - CALL addr
        /// Call subroutine at nnn.
        /// The interpreter increments the machineState.Stack pointer, then puts the current PC on the top of the machineState.Stack. 
        /// The PC is then set to nnn.
        /// </summary>
        public void CallAtAdress()
        {
            // Program counter will be increased right after the instruction fetch 
            // So theres no need to increase the program counter before pushing
            this.MachineState.Stack.Push(this.MachineState.ProgramCounter);

            this.MachineState.ProgramCounter = (ushort)(this.MachineState.CurrentOpcode & 0x0FFF);
        }

        /// <summary>
        /// 3xkk - SE Vx, byte
        /// Skip next instruction if Vx = kk.
        /// The interpreter compares register Vx to kk, and if they are equal, increments the program counter by 2.
        /// </summary>
        public void SkipNextInstructionIfRegisterEqualsImmediate()
        {
            if (this.MachineState.VRegisters[this.MachineState.XRegisterFromCurrentOpcode] ==
                (this.MachineState.CurrentOpcode & 0x00FF))
            {
                this.MachineState.IncreaseProgramCounter();
            }
        }

        /// <summary>
        /// 4xkk - SNE Vx, byte
        /// Skip next instruction if Vx != kk.
        /// The interpreter compares register Vx to kk, and if they are not equal, increments the program counter by 2.
        /// </summary>
        public void SkipNextInstructionIfRegisterNotEqualsImmediate()
        {
            if (this.MachineState.VRegisters[this.MachineState.XRegisterFromCurrentOpcode] !=
                (this.MachineState.CurrentOpcode & 0x00FF))
            {
                this.MachineState.IncreaseProgramCounter();
            }
        }

        /// <summary>
        /// 5xy0 - SE Vx, Vy
        /// Skip next instruction if Vx = Vy.
        /// The interpreter compares register Vx to register Vy, and if they are equal, increments the program counter by 2.
        /// </summary>
        public void SkipNextInstructionIfRegisterEqualsRegister()
        {
            if (this.MachineState.VRegisters[this.MachineState.XRegisterFromCurrentOpcode] ==
                this.MachineState.VRegisters[this.MachineState.YRegisterFromCurrentOpcode])
            {
                this.MachineState.IncreaseProgramCounter();
            }
        }

        /// <summary>
        /// 6xkk - LD Vx, byte
        /// Set Vx = kk.
        /// The interpreter puts the value kk into register Vx.
        /// </summary>
        public void LoadValueIntoRegister()
        {
            this.MachineState.VRegisters[this.MachineState.XRegisterFromCurrentOpcode] =
                (ushort)(this.MachineState.CurrentOpcode & 0x00FF);
        }

        /// <summary>
        /// 7xkk - ADD Vx, byte
        /// Set Vx = Vx + kk.
        /// Adds the value kk to the value of register Vx, then stores the result in Vx.
        /// </summary>
        public void AddValueIntoRegister()
        {
            this.MachineState.VRegisters[this.MachineState.XRegisterFromCurrentOpcode] +=
                (ushort)(this.MachineState.CurrentOpcode & 0x00FF);
        }

        /// <summary>
        /// 8xy0 - LD Vx, Vy
        /// Set Vx = Vy.
        /// Stores the value of register Vy in register Vx.
        /// </summary>
        public void LoadRegisterIntoRegister()
        {
            this.MachineState.VRegisters[this.MachineState.XRegisterFromCurrentOpcode] +=
                this.MachineState.VRegisters[this.MachineState.YRegisterFromCurrentOpcode];
        }

        /// <summary>
        /// 8xy1 - OR Vx, Vy
        /// Set Vx = Vx OR Vy.
        /// Performs a bitwise OR on the values of Vx and Vy, then stores the result in Vx. 
        /// A bitwise OR compares the corrseponding bits from two values, and if either bit is 1, 
        /// then the same bit in the result is also 1. Otherwise, it is 0. 
        /// </summary>
        public void OrRegistersIntoRegister()
        {
            this.MachineState.VRegisters[this.MachineState.XRegisterFromCurrentOpcode] |=
                this.MachineState.VRegisters[this.MachineState.YRegisterFromCurrentOpcode];
        }

        /// <summary>
        /// 8xy2 - AND Vx, Vy
        /// Set Vx = Vx AND Vy.
        /// Performs a bitwise AND on the values of Vx and Vy, then stores the result in Vx. 
        /// A bitwise AND compares the corrseponding bits from two values, and if both bits are 1, 
        /// then the same bit in the result is also 1. Otherwise, it is 0.
        /// </summary>
        public void AndRegistersIntoRegiter()
        {
            this.MachineState.VRegisters[this.MachineState.XRegisterFromCurrentOpcode] &=
                this.MachineState.VRegisters[this.MachineState.YRegisterFromCurrentOpcode];
        }

        /// <summary>
        /// 8xy3 - XOR Vx, Vy
        /// Set Vx = Vx XOR Vy.
        /// Performs a bitwise exclusive OR on the values of Vx and Vy, then stores the result in Vx. 
        /// An exclusive OR compares the corrseponding bits from two values, and if the bits are not both the same, 
        /// then the corresponding bit in the result is set to 1. Otherwise, it is 0. 
        /// </summary>
        public void ExclusiveOrIntoRegister()
        {
            this.MachineState.VRegisters[this.MachineState.XRegisterFromCurrentOpcode] ^=
                this.MachineState.VRegisters[this.MachineState.YRegisterFromCurrentOpcode];
        }

        /// <summary>
        /// 8xy4 - ADD Vx, Vy
        /// Set Vx = Vx + Vy, set VF = carry.
        /// The values of Vx and Vy are added together. If the result is greater than 8 bits (i.e., > 255,) VF is set to 1, 
        /// otherwise 0. Only the lowest 8 bits of the result are kept, and stored in Vx.
        /// </summary>
        public void AddRegistersIntoRegister()
        {
            this.MachineState.VRegisters[this.MachineState.XRegisterFromCurrentOpcode] +=
                this.MachineState.VRegisters[this.MachineState.YRegisterFromCurrentOpcode];

            if (this.MachineState.VRegisters[this.MachineState.XRegisterFromCurrentOpcode] > 0xFF)
            {
                this.MachineState.VRegisters[0xF] = 1;
            }
            else
            {
                this.MachineState.VRegisters[0xF] = 0;
            }
        }

        /// <summary>
        /// 8xy5 - SUB Vx, Vy
        /// Set Vx = Vx - Vy, set VF = NOT borrow.
        /// If Vx > Vy, then VF is set to 1, otherwise 0. Then Vy is subtracted from Vx, and the results stored in Vx.
        /// </summary>
        public void SubstractRegisters()
        {
            if (this.MachineState.VRegisters[this.MachineState.XRegisterFromCurrentOpcode] >
                this.MachineState.VRegisters[this.MachineState.YRegisterFromCurrentOpcode])
            {
                this.MachineState.VRegisters[0xF] = 1;
            }
            else
            {
                this.MachineState.VRegisters[0xF] = 0;
            }

            this.MachineState.VRegisters[this.MachineState.XRegisterFromCurrentOpcode] -=
                this.MachineState.VRegisters[this.MachineState.YRegisterFromCurrentOpcode];
        }

        /// <summary>
        /// 8xy6 - SHR Vx {, Vy}
        /// Set Vx = Vx SHR 1.
        /// If the least-significant bit of Vx is 1, then VF is set to 1, otherwise 0. Then Vx is divided by 2.
        /// </summary>
        public void ShiftRegisterRight()
        {
            if ((this.MachineState.CurrentOpcode & 0x000F) == 1)
            {
                this.MachineState.VRegisters[0xF] = 1;
            }
            else
            {
                this.MachineState.VRegisters[0xF] = 0;
            }

            this.MachineState.VRegisters[this.MachineState.XRegisterFromCurrentOpcode] >>= 1;
        }

        /// <summary>
        /// 8xy7 - SUBN Vx, Vy
        /// Set Vx = Vy - Vx, set VF = NOT borrow.
        /// If Vy > Vx, then VF is set to 1, otherwise 0. Then Vx is subtracted from Vy, and the results stored in Vx.
        /// </summary>
        public void SubstractRegistersReverse()
        {
            if (this.MachineState.VRegisters[this.MachineState.YRegisterFromCurrentOpcode]
                > this.MachineState.VRegisters[this.MachineState.XRegisterFromCurrentOpcode])
            {
                this.MachineState.VRegisters[0xF] = 1;
            }
            else
            {
                this.MachineState.VRegisters[0xF] = 0;
            }

            this.MachineState.VRegisters[this.MachineState.XRegisterFromCurrentOpcode] =
                (ushort)
                (this.MachineState.VRegisters[this.MachineState.YRegisterFromCurrentOpcode]
                 - this.MachineState.VRegisters[this.MachineState.XRegisterFromCurrentOpcode]);
        }

        /// <summary>
        /// 8xyE - SHL Vx {, Vy}
        /// Set Vx = Vx SHL 1.
        /// If the most-significant bit of Vx is 1, then VF is set to 1, otherwise to 0. Then Vx is multiplied by 2.
        /// </summary>
        public void ShiftRegisterLeft()
        {
            if ((this.MachineState.VRegisters[this.MachineState.XRegisterFromCurrentOpcode] & 0xF000) == 0x1000)
            {
                this.MachineState.VRegisters[0xF] = 1;
            }
            else
            {
                this.MachineState.VRegisters[0xF] = 0;
            }

            this.MachineState.VRegisters[this.MachineState.XRegisterFromCurrentOpcode] <<= 1;
        }

        /// <summary>
        /// 9xy0 - SNE Vx, Vy
        /// Skip next instruction if Vx != Vy.
        /// The values of Vx and Vy are compared, and if they are not equal, the program counter is increased by 2.
        /// </summary>
        public void SkipNextInstructionIfRegisterNotEqualsRegister()
        {
            if (this.MachineState.VRegisters[this.MachineState.XRegisterFromCurrentOpcode] !=
                this.MachineState.VRegisters[this.MachineState.YRegisterFromCurrentOpcode])
            {
                this.MachineState.IncreaseProgramCounter();
            }
        }

        /// <summary>
        /// Annn - LD I, addr
        /// Set I = nnn.
        /// The value of register I is set to nnn.
        /// </summary>
        public void LoadIntoIndexRegister()
        {
            this.MachineState.IndexRegister = (ushort)(this.MachineState.CurrentOpcode & 0x0FFF);
        }

        /// <summary>
        /// Bnnn - JP V0, addr
        /// Jump to location nnn + V0.
        /// The program counter is set to nnn plus the value of V0.
        /// </summary>
        public void JumpToV0PlusImmediate()
        {
            this.MachineState.ProgramCounter = (ushort)(this.MachineState.VRegisters[0] + (this.MachineState.CurrentOpcode & 0x0FFF));
        }

        /// <summary>
        /// Cxkk - RND Vx, byte
        /// Set Vx = random byte AND kk.
        /// The interpreter generates a random number from 0 to 255, which is then ANDed with the value kk. 
        /// The results are stored in Vx. See instruction 8xy2 for more information on AND.
        /// </summary>
        public void LoadRandomIntoRegister()
        {
            var randomnumber = (ushort)new Random().Next(0, 255);

            this.MachineState.VRegisters[this.MachineState.XRegisterFromCurrentOpcode] =
                (ushort)(randomnumber & (this.MachineState.CurrentOpcode & 0x00FF));
        }

        /// <summary>
        /// Dxyn - DRW Vx, Vy, nibble
        /// Display n-byte sprite starting at machineState.Memory location I at (Vx, Vy), set VF = collision.
        /// The interpreter reads n bytes from machineState.Memory, starting at the address stored in I. 
        /// These bytes are then displayed as sprites on screen at coordinates (Vx, Vy). 
        /// Sprites are XORed onto the existing screen. 
        /// If this causes any pixels to be erased, VF is set to 1, otherwise it is set to 0. 
        /// If the sprite is positioned so part of it is outside the coordinates of the display, it wraps around to the opposite side of the screen. 
        /// See instruction 8xy3 for more information on XOR, and section 2.4, Display, for more information on the Chip-8 screen and sprites.
        /// </summary>
        public void DrawSprite()
        {
            var numbytes = (ushort)(this.MachineState.CurrentOpcode & 0x000F);
            var positionX = this.MachineState.VRegisters[this.MachineState.XRegisterFromCurrentOpcode];
            var positionY = this.MachineState.VRegisters[this.MachineState.YRegisterFromCurrentOpcode];

            for (var rowNum = 0; rowNum < numbytes; rowNum++)
            {
                ushort currentpixel = this.MachineState.Memory[this.MachineState.IndexRegister + rowNum];

                // We assume sprites are always 8 pixels wide
                for (var colNum = 0; colNum < 8; colNum++) 
                {
                    if ((currentpixel & (0x80 >> colNum)) != 0)
                    {
                        int positioninGraphics = (positionX + colNum +
                                                  ((positionY + rowNum) * C8Constants.ResolutionWidth))
                                                 % (C8Constants.ResolutionWidth * C8Constants.ResolutionHeight);

                        // Make sure we get a value inside boundaries
                        if (this.MachineState.Graphics[positioninGraphics])
                        {
                            // Collision!
                            this.MachineState.VRegisters[0xF] = 1;
                        }

                        this.MachineState.Graphics[positioninGraphics] ^= true;
                    }
                }

                this.MachineState.IsDrawFlagSet = true;
            }
        }

        /// <summary>
        /// Ex9E - SKP Vx
        /// Skip next instruction if key with the value of Vx is pressed.
        /// Checks the keyboard, and if the key corresponding to the value of Vx is currently in the down position, 
        /// PC is increased by 2.
        /// </summary>
        public void SkipNextInstructionIfRegisterEqualsKeyPressed()
        {
            if (this.MachineState.Keys[this.MachineState.VRegisters[this.MachineState.XRegisterFromCurrentOpcode]])
            {
                this.MachineState.IncreaseProgramCounter();
            }
        }

        /// <summary>
        /// ExA1 - SKNP Vx
        /// Skip next instruction if key with the value of Vx is not pressed.
        /// Checks the keyboard, and if the key corresponding to the value of Vx is currently in the up position, 
        /// PC is increased by 2.
        /// </summary>
        public void SkipNextInstructionIfRegisterNotEqualsKeyPressed()
        {
            if (!this.MachineState.Keys[this.MachineState.VRegisters[this.MachineState.XRegisterFromCurrentOpcode]])
            {
                this.MachineState.IncreaseProgramCounter();
            }
        }

        /// <summary>
        /// Fx07 - LD Vx, DT
        /// Set Vx = delay timer value.
        /// The value of DT is placed into Vx.
        /// </summary>
        public void LoadTimerValueIntoRegister()
        {
            this.MachineState.VRegisters[this.MachineState.XRegisterFromCurrentOpcode] = this.MachineState.DelayTimer;
        }

        /// <summary>
        /// Fx0A - LD Vx, K
        /// Wait for a key press, store the value of the key in Vx.
        /// All execution stops until a key is pressed, then the value of that key is stored in Vx.
        /// </summary>
        public void LoadKeyIntoRegister()
        {
            // TODO What do I do with this wait for key?
            /*while (!machineState.Keys.OfType<bool>().Any(x => x))
            {

            }*/
        }

        /// <summary>
        /// Fx15 - LD DT, Vx
        /// Set delay timer = Vx.
        /// DT is set equal to the value of Vx.
        /// </summary>
        public void LoadRegisterIntoDelayTimer()
        {
            this.MachineState.DelayTimer = this.MachineState.VRegisters[this.MachineState.XRegisterFromCurrentOpcode];
        }

        /// <summary>
        /// Fx18 - LD ST, Vx
        /// Set sound timer = Vx.
        /// ST is set equal to the value of Vx.
        /// </summary>
        public void LoadRegisterIntoSoundTimer()
        {
            this.MachineState.SoundTimer = this.MachineState.VRegisters[this.MachineState.XRegisterFromCurrentOpcode];
        }

        /// <summary>
        /// Fx1E - ADD I, Vx
        /// Set I = I + Vx.
        /// The values of I and Vx are added, and the results are stored in I.
        /// </summary>
        public void AddRegisterToIndexRegister()
        {
            this.MachineState.IndexRegister += this.MachineState.VRegisters[this.MachineState.XRegisterFromCurrentOpcode];
        }

        /// <summary>
        /// Fx29 - LD F, Vx
        /// Set I = location of sprite for digit Vx.
        /// The value of I is set to the location for the hexadecimal sprite corresponding to the value of Vx. 
        /// See section 2.4, Display, for more information on the Chip-8 hexadecimal font.
        /// </summary>
        public void LoadFontSpriteLocationFromValueInRegister()
        {
            // Comments from NGEmu about this opcode
            /*Mmm... basically, set the machineState.Memory pointer I to the location of the character of the hexadecimal stored in register VX. 
              So... if it's FA29, you look in register VA and see what value it holds. 
              If it's F129, you look in register V1 and see what value it holds.
              You can assume that the value stored in those registers only goes from 0x0 to 0xF. 
              Now... your emulator should have a table of sprite data already preset somewhere in the machineState.Memory (preferrably in the locations before 0x200). 
              These are sprite of characters from 0 to F, and they are 4x5 each in dimension. 
              Which means... mmm... the number 0 should be somewhat like this in binary data:

              1111 0000
              1001 0000
              1001 0000
              1001 0000
              1111 0000

              If you have to ask, that's because the draw instruction draws 8 bits at a time.*/

            ushort character = this.MachineState.VRegisters[this.MachineState.XRegisterFromCurrentOpcode];

            // We assume that the value for character goes from 0x0 to 0xF and that each digit has size 5 (5 rows per digit)
            // Fonts are loaded starting at 0x0
            this.MachineState.IndexRegister = (ushort)(5 * character);
        }

        /// <summary>
        /// Fx33 - LD B, Vx
        /// Store BCD representation of Vx in machineState.Memory locations I, I+1, and I+2.
        /// The interpreter takes the decimal value of Vx, 
        /// and places the hundreds digit in machineState.Memory at location in I, 
        /// the tens digit at location I+1, 
        /// and the ones digit at location I+2.
        /// </summary>
        public void LoadBcdRepresentationFromRegister()
        {
            this.MachineState.Memory[this.MachineState.IndexRegister] =
                (byte)(this.MachineState.VRegisters[this.MachineState.XRegisterFromCurrentOpcode] / 100); // Hundreds

            this.MachineState.Memory[this.MachineState.IndexRegister + 1] =
                (byte)((this.MachineState.VRegisters[this.MachineState.XRegisterFromCurrentOpcode] / 10) % 10); // Tens

            this.MachineState.Memory[this.MachineState.IndexRegister + 2] =
                (byte)(this.MachineState.VRegisters[this.MachineState.XRegisterFromCurrentOpcode] % 10); // Ones
        }

        /// <summary>
        /// Fx55 - LD [I], Vx
        /// Store registers V0 through Vx in machineState.Memory starting at location I.
        /// The interpreter copies the values of registers V0 through Vx into machineState.Memory, starting at the address in I.
        /// </summary>
        public void LoadAllRegistersFromValueInRegister()
        {
            for (int i = 0; i <= this.MachineState.XRegisterFromCurrentOpcode; i++)
            {
                this.MachineState.Memory[this.MachineState.IndexRegister + i] = (byte)this.MachineState.VRegisters[i];
            }
        }

        /// <summary>
        /// Fx65 - LD Vx, [I]
        /// Read registers V0 through Vx from machineState.Memory starting at location I.
        /// The interpreter reads values from machineState.Memory starting at location I into registers V0 through Vx.
        /// </summary>
        public void LoadFromValueInRegisterIntoAllRegisters()
        {
            for (int i = 0; i <= this.MachineState.XRegisterFromCurrentOpcode; i++)
            {
                this.MachineState.VRegisters[i] = this.MachineState.Memory[this.MachineState.IndexRegister + i];
            }
        }

        #endregion
    }
}
