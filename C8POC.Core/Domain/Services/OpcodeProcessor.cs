// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OpcodeProcessor.cs" company="AlFranco">
//   Albert Rodriguez Franco 2013
// </copyright>
// <summary>
//   Interprets instructions working with a given MachineState
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace C8POC.Core.Domain.Services
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using C8POC.Core.Infrastructure;
    using C8POC.Interfaces.Domain.Entities;
    using C8POC.Interfaces.Domain.Services;

    /// <summary>
    /// Interprets instructions working with a given MachineState
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
        Justification = "Mnemotechnical stuff is obviously not recognized by StyleCop")]
    public class OpcodeProcessor : IOpcodeProcessor
    {
        #region Instruction Set

        // Instruction set taken from http://devernay.free.fr/hacks/chip8/C8TECH10.HTM#00E0

        /// <summary>
        /// 0nnn - SYS addr
        /// Jump to a machine code routine at nnn.
        /// This instruction is only used on the old computers on which Chip-8 was originally implemented. 
        /// It is ignored by modern interpreters.
        /// </summary>
        /// <param name="machineState">
        /// The machine State.
        /// </param>
        public void JumpToRoutineAtAdress(IMachineState machineState)
        {
            // IGNORE! xD
        }

        /// <summary>
        /// 00E0 - CLS
        /// Clear the display.
        /// </summary>
        /// <param name="machineState">
        /// The machine State.
        /// </param>
        public void ClearScreen(IMachineState machineState)
        {
            machineState.Graphics.SetAll(false);
            machineState.IsDrawFlagSet = true;
        }

        /// <summary>
        /// 00EE - RET
        /// Return from a subroutine.
        /// The interpreter sets the program counter to the address at the top of the machineState.Stack, 
        /// then subtracts 1 from the machineState.Stack pointer
        /// </summary>
        /// <param name="machineState">
        /// The machine State.
        /// </param>
        public void ReturnFromSubRoutine(IMachineState machineState)
        {
            machineState.ProgramCounter = machineState.Stack.Pop();
        }

        /// <summary>
        /// 1nnn - JP addr
        /// Jump to location nnn.
        /// The interpreter sets the program counter to nnn.
        /// </summary>
        /// <param name="machineState">
        /// The machine State.
        /// </param>
        public void Jump(IMachineState machineState)
        {
            machineState.ProgramCounter = (ushort)(machineState.CurrentOpcode & 0x0FFF);
        }

        /// <summary>
        /// 2nnn - CALL addr
        /// Call subroutine at nnn.
        /// The interpreter increments the machineState.Stack pointer, then puts the current PC on the top of the machineState.Stack. 
        /// The PC is then set to nnn.
        /// </summary>
        /// <param name="machineState">
        /// The machine State.
        /// </param>
        public void CallAtAdress(IMachineState machineState)
        {
            // Program counter will be increased right after the instruction fetch 
            // So theres no need to increase the program counter before pushing
            machineState.Stack.Push(machineState.ProgramCounter);

            machineState.ProgramCounter = (ushort)(machineState.CurrentOpcode & 0x0FFF);
        }

        /// <summary>
        /// 3xkk - SE Vx, byte
        /// Skip next instruction if Vx = kk.
        /// The interpreter compares register Vx to kk, and if they are equal, increments the program counter by 2.
        /// </summary>
        /// <param name="machineState">
        /// The machine State.
        /// </param>
        public void SkipNextInstructionIfRegisterEqualsImmediate(IMachineState machineState)
        {
            if (machineState.VRegisters[machineState.XRegisterFromCurrentOpcode]
                == (machineState.CurrentOpcode & 0x00FF))
            {
                machineState.IncreaseProgramCounter();
            }
        }

        /// <summary>
        /// 4xkk - SNE Vx, byte
        /// Skip next instruction if Vx != kk.
        /// The interpreter compares register Vx to kk, and if they are not equal, increments the program counter by 2.
        /// </summary>
        /// <param name="machineState">
        /// The machine State.
        /// </param>
        public void SkipNextInstructionIfRegisterNotEqualsImmediate(IMachineState machineState)
        {
            if (machineState.VRegisters[machineState.XRegisterFromCurrentOpcode]
                != (machineState.CurrentOpcode & 0x00FF))
            {
                machineState.IncreaseProgramCounter();
            }
        }

        /// <summary>
        /// 5xy0 - SE Vx, Vy
        /// Skip next instruction if Vx = Vy.
        /// The interpreter compares register Vx to register Vy, and if they are equal, increments the program counter by 2.
        /// </summary>
        /// <param name="machineState">
        /// The machine State.
        /// </param>
        public void SkipNextInstructionIfRegisterEqualsRegister(IMachineState machineState)
        {
            if (machineState.VRegisters[machineState.XRegisterFromCurrentOpcode]
                == machineState.VRegisters[machineState.YRegisterFromCurrentOpcode])
            {
                machineState.IncreaseProgramCounter();
            }
        }

        /// <summary>
        /// 6xkk - LD Vx, byte
        /// Set Vx = kk.
        /// The interpreter puts the value kk into register Vx.
        /// </summary>
        /// <param name="machineState">
        /// The machine State.
        /// </param>
        public void LoadValueIntoRegister(IMachineState machineState)
        {
            machineState.VRegisters[machineState.XRegisterFromCurrentOpcode] =
                (ushort)(machineState.CurrentOpcode & 0x00FF);
        }

        /// <summary>
        /// 7xkk - ADD Vx, byte
        /// Set Vx = Vx + kk.
        /// Adds the value kk to the value of register Vx, then stores the result in Vx.
        /// </summary>
        /// <param name="machineState">
        /// The machine State.
        /// </param>
        public void AddValueIntoRegister(IMachineState machineState)
        {
            machineState.VRegisters[machineState.XRegisterFromCurrentOpcode] +=
                (ushort)(machineState.CurrentOpcode & 0x00FF);
        }

        /// <summary>
        /// 8xy0 - LD Vx, Vy
        /// Set Vx = Vy.
        /// Stores the value of register Vy in register Vx.
        /// </summary>
        /// <param name="machineState">
        /// The machine State.
        /// </param>
        public void LoadRegisterIntoRegister(IMachineState machineState)
        {
            machineState.VRegisters[machineState.XRegisterFromCurrentOpcode] =
                machineState.VRegisters[machineState.YRegisterFromCurrentOpcode];
        }

        /// <summary>
        /// 8xy1 - OR Vx, Vy
        /// Set Vx = Vx OR Vy.
        /// Performs a bitwise OR on the values of Vx and Vy, then stores the result in Vx. 
        /// A bitwise OR compares the corrseponding bits from two values, and if either bit is 1, 
        /// then the same bit in the result is also 1. Otherwise, it is 0. 
        /// </summary>
        /// <param name="machineState">
        /// The machine State.
        /// </param>
        public void OrRegistersIntoRegister(IMachineState machineState)
        {
            machineState.VRegisters[machineState.XRegisterFromCurrentOpcode] |=
                machineState.VRegisters[machineState.YRegisterFromCurrentOpcode];
        }

        /// <summary>
        /// 8xy2 - AND Vx, Vy
        /// Set Vx = Vx AND Vy.
        /// Performs a bitwise AND on the values of Vx and Vy, then stores the result in Vx. 
        /// A bitwise AND compares the corrseponding bits from two values, and if both bits are 1, 
        /// then the same bit in the result is also 1. Otherwise, it is 0.
        /// </summary>
        /// <param name="machineState">
        /// The machine State.
        /// </param>
        public void AndRegistersIntoRegiter(IMachineState machineState)
        {
            machineState.VRegisters[machineState.XRegisterFromCurrentOpcode] &=
                machineState.VRegisters[machineState.YRegisterFromCurrentOpcode];
        }

        /// <summary>
        /// 8xy3 - XOR Vx, Vy
        /// Set Vx = Vx XOR Vy.
        /// Performs a bitwise exclusive OR on the values of Vx and Vy, then stores the result in Vx. 
        /// An exclusive OR compares the corrseponding bits from two values, and if the bits are not both the same, 
        /// then the corresponding bit in the result is set to 1. Otherwise, it is 0. 
        /// </summary>
        /// <param name="machineState">
        /// The machine State.
        /// </param>
        public void ExclusiveOrIntoRegister(IMachineState machineState)
        {
            machineState.VRegisters[machineState.XRegisterFromCurrentOpcode] ^=
                machineState.VRegisters[machineState.YRegisterFromCurrentOpcode];
        }

        /// <summary>
        /// 8xy4 - ADD Vx, Vy
        /// Set Vx = Vx + Vy, set VF = carry.
        /// The values of Vx and Vy are added together. If the result is greater than 8 bits (i.e., &gt; 255,) VF is set to 1, 
        /// otherwise 0. Only the lowest 8 bits of the result are kept, and stored in Vx.
        /// </summary>
        /// <param name="machineState">
        /// The machine State.
        /// </param>
        public void AddRegistersIntoRegister(IMachineState machineState)
        {
            machineState.VRegisters[machineState.XRegisterFromCurrentOpcode] +=
                machineState.VRegisters[machineState.YRegisterFromCurrentOpcode];

            if (machineState.VRegisters[machineState.XRegisterFromCurrentOpcode] > 0xFF)
            {
                machineState.VRegisters[0xF] = 1;
            }
            else
            {
                machineState.VRegisters[0xF] = 0;
            }
        }

        /// <summary>
        /// 8xy5 - SUB Vx, Vy
        /// Set Vx = Vx - Vy, set VF = NOT borrow.
        /// If Vx &gt; Vy, then VF is set to 1, otherwise 0. Then Vy is subtracted from Vx, and the results stored in Vx.
        /// </summary>
        /// <param name="machineState">
        /// The machine State.
        /// </param>
        public void SubstractRegisters(IMachineState machineState)
        {
            if (machineState.VRegisters[machineState.XRegisterFromCurrentOpcode]
                > machineState.VRegisters[machineState.YRegisterFromCurrentOpcode])
            {
                machineState.VRegisters[0xF] = 1;
            }
            else
            {
                machineState.VRegisters[0xF] = 0;
            }

            machineState.VRegisters[machineState.XRegisterFromCurrentOpcode] -=
                machineState.VRegisters[machineState.YRegisterFromCurrentOpcode];
        }

        /// <summary>
        /// 8xy6 - SHR Vx {, Vy}
        /// Set Vx = Vy SHR 1.
        /// If the least-significant bit of Vy is 1, then VF is set to 1, otherwise 0. Then Vx has the value of Vy divided by 2.
        /// </summary>
        /// <param name="machineState">
        /// The machine State.
        /// </param>
        public void ShiftRegisterRight(IMachineState machineState)
        {
            if ((machineState.VRegisters[machineState.YRegisterFromCurrentOpcode] & 0x0001) != 0x0)
            {
                machineState.VRegisters[0xF] = 1;
            }
            else
            {
                machineState.VRegisters[0xF] = 0;
            }

            machineState.VRegisters[machineState.XRegisterFromCurrentOpcode] =
                (ushort)(machineState.VRegisters[machineState.YRegisterFromCurrentOpcode] >> 1);
        }

        /// <summary>
        /// 8xy7 - SUBN Vx, Vy
        /// Set Vx = Vy - Vx, set VF = NOT borrow.
        /// If Vy &gt; Vx, then VF is set to 1, otherwise 0. Then Vx is subtracted from Vy, and the results stored in Vx.
        /// </summary>
        /// <param name="machineState">
        /// The machine State.
        /// </param>
        public void SubstractRegistersReverse(IMachineState machineState)
        {
            if (machineState.VRegisters[machineState.YRegisterFromCurrentOpcode]
                > machineState.VRegisters[machineState.XRegisterFromCurrentOpcode])
            {
                machineState.VRegisters[0xF] = 1;
            }
            else
            {
                machineState.VRegisters[0xF] = 0;
            }

            machineState.VRegisters[machineState.XRegisterFromCurrentOpcode] =
                (ushort)
                (machineState.VRegisters[machineState.YRegisterFromCurrentOpcode]
                 - machineState.VRegisters[machineState.XRegisterFromCurrentOpcode]);
        }

        /// <summary>
        /// 8xyE - SHL Vx {, Vy}
        /// Set Vx = Vy SHL 1.
        /// If the most-significant bit of Vy is 1, then VF is set to 1, otherwise to 0. Then Vx has the value of Vy multiplied by 2.
        /// </summary>
        /// <param name="machineState">
        /// The machine State.
        /// </param>
        public void ShiftRegisterLeft(IMachineState machineState)
        {
            if ((machineState.VRegisters[machineState.YRegisterFromCurrentOpcode] & 0x8000) != 0x0)
            {
                machineState.VRegisters[0xF] = 1;
            }
            else
            {
                machineState.VRegisters[0xF] = 0;
            }

            machineState.VRegisters[machineState.XRegisterFromCurrentOpcode] =
                (ushort)(machineState.VRegisters[machineState.YRegisterFromCurrentOpcode] >> 1);
        }

        /// <summary>
        /// 9xy0 - SNE Vx, Vy
        /// Skip next instruction if Vx != Vy.
        /// The values of Vx and Vy are compared, and if they are not equal, the program counter is increased by 2.
        /// </summary>
        /// <param name="machineState">
        /// The machine State.
        /// </param>
        public void SkipNextInstructionIfRegisterNotEqualsRegister(IMachineState machineState)
        {
            if (machineState.VRegisters[machineState.XRegisterFromCurrentOpcode]
                != machineState.VRegisters[machineState.YRegisterFromCurrentOpcode])
            {
                machineState.IncreaseProgramCounter();
            }
        }

        /// <summary>
        /// Annn - LD I, addr
        /// Set I = nnn.
        /// The value of register I is set to nnn.
        /// </summary>
        /// <param name="machineState">
        /// The machine State.
        /// </param>
        public void LoadIntoIndexRegister(IMachineState machineState)
        {
            machineState.IndexRegister = (ushort)(machineState.CurrentOpcode & 0x0FFF);
        }

        /// <summary>
        /// Bnnn - JP V0, addr
        /// Jump to location nnn + V0.
        /// The program counter is set to nnn plus the value of V0.
        /// </summary>
        /// <param name="machineState">
        /// The machine State.
        /// </param>
        public void JumpToV0PlusImmediate(IMachineState machineState)
        {
            machineState.ProgramCounter = (ushort)(machineState.VRegisters[0] + (machineState.CurrentOpcode & 0x0FFF));
        }

        /// <summary>
        /// Cxkk - RND Vx, byte
        /// Set Vx = random byte AND kk.
        /// The interpreter generates a random number from 0 to 255, which is then ANDed with the value kk. 
        /// The results are stored in Vx. See instruction 8xy2 for more information on AND.
        /// </summary>
        /// <param name="machineState">
        /// The machine State.
        /// </param>
        public void LoadRandomIntoRegister(IMachineState machineState)
        {
            var randomnumber = (ushort)new Random().Next(0, 255);

            machineState.VRegisters[machineState.XRegisterFromCurrentOpcode] =
                (ushort)(randomnumber & (machineState.CurrentOpcode & 0x00FF));
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
        /// <param name="machineState">
        /// The machine State.
        /// </param>
        public void DrawSprite(IMachineState machineState)
        {
            var numbytes = (ushort)(machineState.CurrentOpcode & 0x000F);
            var positionX = machineState.VRegisters[machineState.XRegisterFromCurrentOpcode];
            var positionY = machineState.VRegisters[machineState.YRegisterFromCurrentOpcode];

            for (var rowNum = 0; rowNum < numbytes; rowNum++)
            {
                ushort currentpixel = machineState.Memory[machineState.IndexRegister + rowNum];

                // We assume sprites are always 8 pixels wide
                for (var colNum = 0; colNum < 8; colNum++)
                {
                    if ((currentpixel & (0x80 >> colNum)) != 0)
                    {
                        int positioninGraphics = (positionX + colNum
                                                  + ((positionY + rowNum) * C8Constants.ResolutionWidth))
                                                 % (C8Constants.ResolutionWidth * C8Constants.ResolutionHeight);

                        // Make sure we get a value inside boundaries
                        if (machineState.Graphics[positioninGraphics])
                        {
                            // Collision!
                            machineState.VRegisters[0xF] = 1;
                        }

                        machineState.Graphics[positioninGraphics] ^= true;
                    }
                }

                machineState.IsDrawFlagSet = true;
            }
        }

        /// <summary>
        /// Ex9E - SKP Vx
        /// Skip next instruction if key with the value of Vx is pressed.
        /// Checks the keyboard, and if the key corresponding to the value of Vx is currently in the down position, 
        /// PC is increased by 2.
        /// </summary>
        /// <param name="machineState">
        /// The machine State.
        /// </param>
        public void SkipNextInstructionIfRegisterEqualsKeyPressed(IMachineState machineState)
        {
            if (machineState.Keys[machineState.VRegisters[machineState.XRegisterFromCurrentOpcode]])
            {
                machineState.IncreaseProgramCounter();
            }
        }

        /// <summary>
        /// ExA1 - SKNP Vx
        /// Skip next instruction if key with the value of Vx is not pressed.
        /// Checks the keyboard, and if the key corresponding to the value of Vx is currently in the up position, 
        /// PC is increased by 2.
        /// </summary>
        /// <param name="machineState">
        /// The machine State.
        /// </param>
        public void SkipNextInstructionIfRegisterNotEqualsKeyPressed(IMachineState machineState)
        {
            if (!machineState.Keys[machineState.VRegisters[machineState.XRegisterFromCurrentOpcode]])
            {
                machineState.IncreaseProgramCounter();
            }
        }

        /// <summary>
        /// Fx07 - LD Vx, DT
        /// Set Vx = delay timer value.
        /// The value of DT is placed into Vx.
        /// </summary>
        /// <param name="machineState">
        /// The machine State.
        /// </param>
        public void LoadTimerValueIntoRegister(IMachineState machineState)
        {
            machineState.VRegisters[machineState.XRegisterFromCurrentOpcode] = machineState.DelayTimer;
        }

        /// <summary>
        /// Fx0A - LD Vx, K
        /// Wait for a key press, store the value of the key in Vx.
        /// All execution stops until a key is pressed, then the value of that key is stored in Vx.
        /// </summary>
        /// <param name="machineState">
        /// The machine State.
        /// </param>
        public void LoadKeyIntoRegister(IMachineState machineState)
        {
            if (!machineState.Keys.OfType<bool>().Any(x => x))
            {
                machineState.ProgramCounter -= 2;
                return;
            }

            for (var i = 0; i < machineState.Keys.Count; i++)
            {
                if (machineState.Keys[i])
                {
                    machineState.VRegisters[machineState.XRegisterFromCurrentOpcode] = (ushort)i;
                    return;
                }
            }
        }

        /// <summary>
        /// Fx15 - LD DT, Vx
        /// Set delay timer = Vx.
        /// DT is set equal to the value of Vx.
        /// </summary>
        /// <param name="machineState">
        /// The machine State.
        /// </param>
        public void LoadRegisterIntoDelayTimer(IMachineState machineState)
        {
            machineState.DelayTimer = machineState.VRegisters[machineState.XRegisterFromCurrentOpcode];
        }

        /// <summary>
        /// Fx18 - LD ST, Vx
        /// Set sound timer = Vx.
        /// ST is set equal to the value of Vx.
        /// </summary>
        /// <param name="machineState">
        /// The machine State.
        /// </param>
        public void LoadRegisterIntoSoundTimer(IMachineState machineState)
        {
            machineState.SoundTimer = machineState.VRegisters[machineState.XRegisterFromCurrentOpcode];
        }

        /// <summary>
        /// Fx1E - ADD I, Vx
        /// Set I = I + Vx.
        /// The values of I and Vx are added, and the results are stored in I.
        /// </summary>
        /// <param name="machineState">
        /// The machine State.
        /// </param>
        public void AddRegisterToIndexRegister(IMachineState machineState)
        {
            machineState.IndexRegister += machineState.VRegisters[machineState.XRegisterFromCurrentOpcode];
        }

        /// <summary>
        /// Fx29 - LD F, Vx
        /// Set I = location of sprite for digit Vx.
        /// The value of I is set to the location for the hexadecimal sprite corresponding to the value of Vx. 
        /// See section 2.4, Display, for more information on the Chip-8 hexadecimal font.
        /// </summary>
        /// <param name="machineState">
        /// The machine State.
        /// </param>
        public void LoadFontSpriteLocationFromValueInRegister(IMachineState machineState)
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

            ushort character = machineState.VRegisters[machineState.XRegisterFromCurrentOpcode];

            // We assume that the value for character goes from 0x0 to 0xF and that each digit has size 5 (5 rows per digit)
            // Fonts are loaded starting at 0x0
            machineState.IndexRegister = (ushort)(5 * character);
        }

        /// <summary>
        /// Fx33 - LD B, Vx
        /// Store BCD representation of Vx in machineState.Memory locations I, I+1, and I+2.
        /// The interpreter takes the decimal value of Vx, 
        /// and places the hundreds digit in machineState.Memory at location in I, 
        /// the tens digit at location I+1, 
        /// and the ones digit at location I+2.
        /// </summary>
        /// <param name="machineState">
        /// The machine State.
        /// </param>
        public void LoadBcdRepresentationFromRegister(IMachineState machineState)
        {
            machineState.Memory[machineState.IndexRegister] =
                (byte)(machineState.VRegisters[machineState.XRegisterFromCurrentOpcode] / 100); // Hundreds

            machineState.Memory[machineState.IndexRegister + 1] =
                (byte)((machineState.VRegisters[machineState.XRegisterFromCurrentOpcode] / 10) % 10); // Tens

            machineState.Memory[machineState.IndexRegister + 2] =
                (byte)(machineState.VRegisters[machineState.XRegisterFromCurrentOpcode] % 10); // Ones
        }

        /// <summary>
        /// Fx55 - LD [I], Vx
        /// Store registers V0 through Vx in machineState.Memory starting at location I.
        /// The interpreter copies the values of registers V0 through Vx into machineState.Memory, starting at the address in I.
        /// </summary>
        /// <param name="machineState">
        /// The machine State.
        /// </param>
        public void LoadAllRegistersFromValueInRegister(IMachineState machineState)
        {
            for (int i = 0; i <= machineState.XRegisterFromCurrentOpcode; i++)
            {
                machineState.Memory[machineState.IndexRegister + i] = (byte)machineState.VRegisters[i];
            }
        }

        /// <summary>
        /// Fx65 - LD Vx, [I]
        /// Read registers V0 through Vx from machineState.Memory starting at location I.
        /// The interpreter reads values from machineState.Memory starting at location I into registers V0 through Vx.
        /// </summary>
        /// <param name="machineState">
        /// The machine State.
        /// </param>
        public void LoadFromValueInRegisterIntoAllRegisters(IMachineState machineState)
        {
            for (int i = 0; i <= machineState.XRegisterFromCurrentOpcode; i++)
            {
                machineState.VRegisters[i] = machineState.Memory[machineState.IndexRegister + i];
            }
        }

        #endregion
    }
}
