// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisassemblerForm.cs" company="AlFranco">
//   Albert Rodriguez Franco 2013
// </copyright>
// <summary>
//   Defines the DisassemblerForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace C8POC.WinFormsUI.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;

    using C8POC.Core.Infrastructure;
    using C8POC.Interfaces;
    using C8POC.Interfaces.Domain.Entities;
    using C8POC.WinFormsUI.Disassembly;

    using Castle.Core.Internal;

    /// <summary>
    /// Form in which disassembly will take place
    /// </summary>
    public partial class DisassemblerForm : Form
    {
        /// <summary>
        /// The list of buttons for easy access
        /// </summary>
        private readonly IEnumerable<Panel> buttonList;

        /// <summary>
        /// The list of registers for easy access
        /// </summary>
        private readonly IEnumerable<TextBox> registerList;

        /// <summary>
        /// The list of values in the stack for easy access
        /// </summary>
        private readonly IEnumerable<TextBox> stackList;

        /// <summary>
        /// Internal list of values that will be bound to the data grid view
        /// </summary>
        private IList<Tuple<string, string, string>> opcodeList;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisassemblerForm"/> class.
        /// </summary>
        public DisassemblerForm()
        {
            this.InitializeComponent();
            this.opcodeList = new List<Tuple<string, string, string>>();
            this.buttonList = this.GetInnerControlsOfType<Panel>(this.groupBoxKeys);
            this.registerList = this.GetInnerControlsOfType<TextBox>(this.groupBoxRegisters);
            this.stackList = this.GetInnerControlsOfType<TextBox>(this.groupBoxStack);

            this.EmptyBreakPointColumn();
            this.KeyPreview = true;
        }


        /// <summary>
        /// Disables the close button
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                var myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | 0x200;
                return myCp;
            }
        }

        /// <summary>
        /// The refresh disassembler status.
        /// </summary>
        /// <param name="machineState">
        /// The monitored machine state
        /// </param>
        public void RefreshDisassemblerStatus(IMachineState machineState)
        {
            this.SetButtonStatus(machineState);
            this.SetRegisterValues(machineState);
            this.SetStackValues(machineState);
            this.SetTimerValues(machineState);
            this.SetProgramCounter(machineState);
            this.SetDrawFlag(machineState);
        }

        /// <summary>
        /// Gets the grid position from the program counter
        /// </summary>
        /// <param name="programCounter">
        /// Program counter value
        /// </param>
        /// <returns>
        /// Grid position equivalent
        /// </returns>
        public int GetGridRowPositionFromProgramCounter(int programCounter)
        {
            // One opcode is 2 bytes of the memory
            return (programCounter - C8Constants.StartRomAddress);
        }

        /// <summary>
        /// Indicates if the current line has to stop for debugging
        /// </summary>
        /// <param name="rowPosition">
        /// The position of the row to check in the data grid view
        /// </param>
        /// <returns>
        /// If the line should stop the execution for debugging
        /// </returns>
        public bool IsLineDebugging(int rowPosition)
        {
            return
                new[] { BreakPointStates.Hit, BreakPointStates.Step, BreakPointStates.Set }.Contains(
                    this.GetBreakPointState(rowPosition));
        }

        /// <summary>
        /// Indicates if the grid view is empty
        /// </summary>
        /// <returns>
        /// Whether if the grid view is empty
        /// </returns>
        public bool IsOpcodeGridViewEmpty()
        {
            return this.dataGridViewOpcodes.DataSource == null;
        }

        /// <summary>
        /// Binds the list of opcodes to the data grid view
        /// </summary>
        /// <param name="machineState">
        /// The machine state
        /// </param>
        public void BindOpcodeList(IMachineState machineState)
        {
            this.opcodeList = this.GetDecodedInstructionsFromMemory(machineState);
            this.MemoryAddressColumn.DataPropertyName = "Item1";
            this.RawOpcodeColumn.DataPropertyName = "Item2";
            this.MnemonicColumn.DataPropertyName = "Item3";
            this.dataGridViewOpcodes.DataSource = this.opcodeList;
        }

        /// <summary>
        /// Cleans up a debugged line from hit state
        /// </summary>
        /// <param name="rowPosition">
        /// Row position to examine
        /// </param>
        public void CleanUpDebuggedLine(int rowPosition)
        {
            switch (this.GetBreakPointState(rowPosition))
            {
                case BreakPointStates.Hit:
                    this.SetBreakPointState(rowPosition, BreakPointStates.Set);
                    break;
                case BreakPointStates.Step:
                    this.SetBreakPointState(rowPosition, BreakPointStates.None);
                    break;
            }
        }

        /// <summary>
        /// Sets the state of step over on a row in the data grid
        /// </summary>
        /// <param name="rowPosition">
        /// The row position.
        /// </param>
        public void SetStepOverState(int rowPosition)
        {
            switch (this.GetBreakPointState(rowPosition))
            {
                case BreakPointStates.Set:
                    this.SetBreakPointState(rowPosition, BreakPointStates.Hit);
                    break;
                case BreakPointStates.None:
                    this.SetBreakPointState(rowPosition, BreakPointStates.Step);
                    break;
            }

            this.SetBreakPointFocus(rowPosition);
        }

        /// <summary>
        /// Determines if the following row position is going to 
        /// hit a breakpoint
        /// </summary>
        /// <param name="rowPosition">
        /// Row position
        /// </param>
        /// <returns>
        /// Whether if it is going to be a breakpoint hi
        /// </returns>
        public bool HasHitBreakPoint(int rowPosition)
        {
            return this.GetBreakPointState(rowPosition) == BreakPointStates.Set;
        }

        /// <summary>
        /// Sets the state of step over on a row in the data grid
        /// </summary>
        /// <param name="rowPosition">
        /// The row position.
        /// </param>
        public void SetHitState(int rowPosition)
        {
            this.SetBreakPointState(rowPosition, BreakPointStates.Hit);
            this.SetBreakPointFocus(rowPosition);
        }

        /// <summary>
        /// Sets the focus in the data grid view for a row position
        /// </summary>
        /// <param name="rowPosition">
        /// The row position.
        /// </param>
        private void SetBreakPointFocus(int rowPosition)
        {
            this.Invoke(
                new Action(
                    () =>
                    this.dataGridViewOpcodes.CurrentCell =
                    this.dataGridViewOpcodes.Rows[rowPosition].Cells[this.BreakPointColumn.Index]));
        }

        /// <summary>
        /// Sets the button status
        /// </summary>
        /// <param name="machineState">
        /// The machine state.
        /// </param>
        private void SetButtonStatus(IMachineState machineState)
        {
            for (int i = 0; i < this.buttonList.Count(); i++)
            {
                this.buttonList.ElementAt(i).BackColor = machineState.Keys[i] ? Color.Green : Color.Transparent;
            }
        }

        /// <summary>
        /// Sets the registry Values
        /// </summary>
        /// <param name="machineState">
        /// The machine state
        /// </param>
        private void SetRegisterValues(IMachineState machineState)
        {
            for (int i = 0; i < this.registerList.Count(); i++)
            {
                this.registerList.ElementAt(i).Text = machineState.VRegisters[i].ToString("X");
            }
        }

        /// <summary>
        /// Sets the stack Values
        /// </summary>
        /// <param name="machineState">
        /// The machine state
        /// </param>
        private void SetStackValues(IMachineState machineState)
        {
            this.stackList.ForEach(x => x.Text = @"0");

            for (int i = 0; i < machineState.Stack.Count; i++)
            {
                this.stackList.ElementAt(i).Text =
                    machineState.Stack.ElementAtOrDefault(i).ToString("X");
            }
        }

        /// <summary>
        /// Sets the time values
        /// </summary>
        /// <param name="machineState">
        /// The machine state.
        /// </param>
        public void SetTimerValues(IMachineState machineState)
        {
            this.textBoxDelayTimer.Text = machineState.DelayTimer.ToString("X");
            this.textBoxSoundTimer.Text = machineState.SoundTimer.ToString("X");
        }

        /// <summary>
        /// Sets the program counter
        /// </summary>
        /// <param name="machineState">
        /// The machine state.
        /// </param>
        private void SetProgramCounter(IMachineState machineState)
        {
            this.textBoxProgramCounter.Text = machineState.ProgramCounter.ToString("X");
        }

        /// <summary>
        /// Sets the draw flag
        /// </summary>
        /// <param name="machineState">
        /// The machine state.
        /// </param>
        private void SetDrawFlag(IMachineState machineState)
        {
            this.checkBoxDrawFlag.Checked = machineState.IsDrawFlagSet;
        }

        /// <summary>
        /// Gets a list of controls
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        /// <typeparam name="T">
        /// Type of controls to search in the container
        /// </typeparam>
        /// <returns>
        /// A list of found controls in container
        /// </returns>
        private IEnumerable<T> GetInnerControlsOfType<T>(Control container) where T : Control
        {
            return
                container.Controls.Cast<Control>()
                         .Where(x => x.GetType() == typeof(T))
                         .OrderBy(x => x.TabIndex)
                         .Cast<T>();
        }

        /// <summary>
        /// Gets a dictionary with the list of instructions
        /// </summary>
        /// <param name="machineState">
        /// A machine state
        /// </param>
        /// <returns>
        /// List of sorted opcodes and mnemonics
        /// </returns>
        private List<Tuple<string, string, string>> GetDecodedInstructionsFromMemory(IMachineState machineState)
        {
            var result = new List<Tuple<string, string, string>>();
            var numberOfIterations = C8Constants.StartRomAddress + machineState.NumberOfOpcodeBytes;

            for (var i = C8Constants.StartRomAddress; i < numberOfIterations; i++)
            {
                ushort opcode = machineState.Memory.ElementAt(i);
                opcode <<= 8;
                opcode |= machineState.Memory.ElementAt(i + 1);

                result.Add(
                    new Tuple<string, string, string>(
                        i.ToString("X"), opcode.ToString("X"), this.GetDecodedInstruction(opcode)));
            }

            return result;
        }

        /// <summary>
        /// The get decoded instruction.
        /// </summary>
        /// <param name="opcode">
        /// The operation code.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetDecodedInstruction(ushort opcode)
        {
            var firstnib = (byte)(opcode >> 12);

            switch (firstnib)
            {
                case 0x0:
                    switch (opcode & 0x00FF)
                    {
                        case 0xE0:
                            return string.Format("{0}", "CLS");
                        case 0xEE:
                            return string.Format("{0}", "RET");
                        default:
                            return string.Format("UNKNOWN 0");
                    }

                case 0x1:
                    return string.Format("{0} ${1:X}", "JP", opcode & 0x0FFF);
                case 0x2:
                    return string.Format("{0} ${1:X}", "CALL", opcode & 0x0FFF);
                case 0x3:
                    return string.Format("{0} V{1:X}, #${2:X}", "SE", (opcode & 0x0F00) >> 8, opcode & 0x00FF);
                case 0x4:
                    return string.Format("{0} V{1:X}, #${2:X}", "SNE", (opcode & 0x0F00) >> 8, opcode & 0x00FF);
                case 0x5:
                    return string.Format("{0} V{1:X}, V{2:X}", "SE", (opcode & 0x0F00) >> 8, (opcode & 0x00FF) >> 4);
                case 0x6:
                    return string.Format("{0} V{1:X}, #${2:X}", "LD", (opcode & 0x0F00) >> 8, opcode & 0x00FF);
                case 0x7:
                    return string.Format("{0} V{1:X}, #${2:X}", "ADD", (opcode & 0x0F00) >> 8, opcode & 0x00FF);
                case 0x8:
                    {
                        var lastnib = (byte)(opcode & 0x000F);

                        switch (lastnib)
                        {
                            case 0:
                                return string.Format(
                                    "{0} V{1:X}, V{2:X}", "LD", (opcode & 0x0F00) >> 8, (opcode & 0x00FF) >> 4);
                            case 1:
                                return string.Format(
                                    "{0} V{1:X}, V{2:X}", "OR", (opcode & 0x0F00) >> 8, (opcode & 0x00FF) >> 4);
                            case 2:
                                return string.Format(
                                    "{0} V{1:X}, V{2:X}", "AND", (opcode & 0x0F00) >> 8, (opcode & 0x00FF) >> 4);
                            case 3:
                                return string.Format(
                                    "{0} V{1:X}, V{2:X}", "XOR", (opcode & 0x0F00) >> 8, (opcode & 0x00FF) >> 4);
                            case 4:
                                return string.Format(
                                    "{0} V{1:X}, V{2:X}", "ADD", (opcode & 0x0F00) >> 8, (opcode & 0x00FF) >> 4);
                            case 5:
                                return string.Format(
                                    "{0} V{1:X}, V{2:X}", "SUB", (opcode & 0x0F00) >> 8, (opcode & 0x00F0) >> 4);
                            case 6:
                                return string.Format(
                                    "{0} V{1:X}, V{2:X}", "SHR", (opcode & 0x0F00) >> 8, (opcode & 0x00FF) >> 4);
                            case 7:
                                return string.Format(
                                    "{0} V{1:X}, V{2:X}", "SUBN", (opcode & 0x0F00) >> 8, (opcode & 0x00FF) >> 4);
                            case 0xe:
                                return string.Format(
                                    "{0} V{1:X}, V{2:X}", "SHL", (opcode & 0x0F00) >> 8, (opcode & 0x00FF) >> 4);
                            default:
                                return string.Format("UNKNOWN 8");
                        }
                    }

                case 0x9:
                    return string.Format("{0} V{1:X}, V{2:X}", "SNE", (opcode & 0x0F00) >> 8, (opcode & 0x00FF) >> 4);
                case 0xA:
                    return string.Format("{0} I, #${1:X}", "LD", opcode & 0x0FFF);
                case 0xB:
                    return string.Format("{0} ${1:X}(V0)", "JP", opcode & 0x0FFF);
                case 0xC:
                    return string.Format("{0} V{1:X}, #${2:X}", "RND", (opcode & 0x0F00) >> 8, opcode & 0x00FF);
                case 0xD:
                    return string.Format(
                        "{0} V{1:X}, V{2:X}, #${3:X}",
                        "DRW",
                        (opcode & 0x0F00) >> 8,
                        (opcode & 0x00FF) >> 4,
                        opcode & 0x00FF & 0xf);
                case 0xE:
                    switch (opcode & 0x00FF)
                    {
                        case 0x9E:
                            return string.Format("{0} V{1:X}", "SKP", (opcode & 0x0F00) >> 8);
                        case 0xA1:
                            return string.Format("{0} V{1:X}", "SKNP", (opcode & 0x0F00) >> 8);
                        default:
                            return string.Format("UNKNOWN E");
                    }

                case 0xF:
                    switch (opcode & 0x00FF)
                    {
                        case 0x07:
                            return string.Format("{0} V{1:X}, DT", "LD", (opcode & 0x0F00) >> 8);
                        case 0x0a:
                            return string.Format("{0} V{1:X} K", "LD", (opcode & 0x0F00) >> 8);
                        case 0x15:
                            return string.Format("{0} DT, V{1:X}", "LD", (opcode & 0x0F00) >> 8);
                        case 0x18:
                            return string.Format("{0} ST, V{1:X}", "LD", (opcode & 0x0F00) >> 8);
                        case 0x1e:
                            return string.Format("{0} I, V{1:X}", "ADD", (opcode & 0x0F00) >> 8);
                        case 0x29:
                            return string.Format("{0} F, V{1:X}", "LD", (opcode & 0x0F00) >> 8);
                        case 0x33:
                            return string.Format("{0} B, V{1:X}", "LD", (opcode & 0x0F00) >> 8);
                        case 0x55:
                            return string.Format("{0} [I], V{1:X}", "LD", (opcode & 0x0F00) >> 8);
                        case 0x65:
                            return string.Format("{0} V{1:X}, [I]", "LD", (opcode & 0x0F00) >> 8);
                        default:
                            return string.Format("UNKNOWN F");
                    }

                default:
                    return "Unknown opcode";
            }
        }

        /// <summary>
        /// Sets the null style of an empty image column to null
        /// Also sets the back selection color to white
        /// </summary>
        private void EmptyBreakPointColumn()
        {
            this.BreakPointColumn.DefaultCellStyle.NullValue = null;
            this.BreakPointColumn.DefaultCellStyle.SelectionBackColor = Color.White;
        }

        /// <summary>
        /// Double click datagridview cell does the break point stuff
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Opcode is a correct word")]
        private void DataGridViewOpcodesCellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            switch (this.GetBreakPointState(e.RowIndex))
            {
                case BreakPointStates.None:
                    this.SetBreakPointState(e.RowIndex, BreakPointStates.Set);
                    break;
                case BreakPointStates.Set:
                    this.SetBreakPointState(e.RowIndex, BreakPointStates.None);
                    break;
                case BreakPointStates.Hit:
                    this.SetBreakPointState(e.RowIndex, BreakPointStates.Step);
                    break;
                case BreakPointStates.Step:
                    this.SetBreakPointState(e.RowIndex, BreakPointStates.Hit);
                    break;
            }
        }

        /// <summary>
        /// Gets the breakpoint state of a given position
        /// </summary>
        /// <param name="rowPosition">
        /// The row position.
        /// </param>
        /// <returns>
        /// The break point state of the row
        /// </returns>
        private BreakPointStates GetBreakPointState(int rowPosition)
        {
            return this.dataGridViewOpcodes.Rows[rowPosition].Cells[this.BreakPointStateColumn.Index].Value == null
                       ? BreakPointStates.None
                       : (BreakPointStates)
                         this.dataGridViewOpcodes.Rows[rowPosition].Cells[this.BreakPointStateColumn.Index].Value;
        }

        /// <summary>
        /// Sets the breakpoint state
        /// </summary>
        /// <param name="rowPosition">
        /// Position in the grid
        /// </param>
        /// <param name="breakPointStates">
        /// The breakpoint state to set
        /// </param>
        private void SetBreakPointState(int rowPosition, BreakPointStates breakPointStates)
        {
            var rowToEdit = this.dataGridViewOpcodes.Rows[rowPosition];

            switch (breakPointStates)
            {
                case BreakPointStates.None:
                    rowToEdit.Cells[this.BreakPointStateColumn.Index].Value = BreakPointStates.None;
                    rowToEdit.Cells[this.BreakPointColumn.Index].Value = null;
                    rowToEdit.DefaultCellStyle.BackColor = Color.White;
                    rowToEdit.DefaultCellStyle.ForeColor = Color.Black;
                    rowToEdit.DefaultCellStyle.SelectionBackColor =
                        this.dataGridViewOpcodes.DefaultCellStyle.SelectionBackColor;
                    rowToEdit.DefaultCellStyle.SelectionForeColor =
                        this.dataGridViewOpcodes.DefaultCellStyle.SelectionForeColor;
                    this.SetValueOfBreakPointColumn(rowToEdit);
                    break;
                case BreakPointStates.Hit:
                    rowToEdit.Cells[this.BreakPointStateColumn.Index].Value = BreakPointStates.Hit;
                    rowToEdit.Cells[this.BreakPointColumn.Index].Value = Properties.Resources.BreakPointHit;
                    rowToEdit.DefaultCellStyle.BackColor = Color.Yellow;
                    rowToEdit.DefaultCellStyle.ForeColor = Color.Black;
                    rowToEdit.DefaultCellStyle.SelectionBackColor = Color.Yellow;
                    rowToEdit.DefaultCellStyle.SelectionForeColor = Color.Black;
                    this.SetValueOfBreakPointColumn(rowToEdit);
                    break;
                case BreakPointStates.Set:
                    rowToEdit.Cells[this.BreakPointStateColumn.Index].Value = BreakPointStates.Set;
                    rowToEdit.Cells[this.BreakPointColumn.Index].Value = Properties.Resources.BreakPointSet;
                    rowToEdit.DefaultCellStyle.BackColor = Color.IndianRed;
                    rowToEdit.DefaultCellStyle.ForeColor = Color.Black;
                    rowToEdit.DefaultCellStyle.SelectionBackColor = Color.IndianRed;
                    rowToEdit.DefaultCellStyle.SelectionForeColor = Color.White;
                    this.SetValueOfBreakPointColumn(rowToEdit);
                    break;
                case BreakPointStates.Step:
                    rowToEdit.Cells[this.BreakPointStateColumn.Index].Value = BreakPointStates.Step;
                    rowToEdit.Cells[this.BreakPointColumn.Index].Value = Properties.Resources.BreakPointStep;
                    rowToEdit.DefaultCellStyle.BackColor = Color.Yellow;
                    rowToEdit.DefaultCellStyle.ForeColor = Color.Black;
                    rowToEdit.DefaultCellStyle.SelectionBackColor = Color.Yellow;
                    rowToEdit.DefaultCellStyle.SelectionForeColor = Color.Black;
                    this.SetValueOfBreakPointColumn(rowToEdit);
                    break;
            }
        }

        /// <summary>
        /// Keeps the white value of the breakpoint icon cell
        /// </summary>
        /// <param name="rowToEdit">
        /// The row to edit.
        /// </param>
        private void SetValueOfBreakPointColumn(DataGridViewRow rowToEdit)
        {
            rowToEdit.Cells[this.BreakPointColumn.Index].Style.BackColor = Color.White;
            rowToEdit.Cells[this.BreakPointColumn.Index].Style.SelectionBackColor = Color.White;
        }

        /// <summary>
        /// Cleans the grid view containing opcodes
        /// </summary>
        public void CleanGridView()
        {
            this.dataGridViewOpcodes.DataSource = null;
        }
    }
}
