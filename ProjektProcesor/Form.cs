using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace ProjektProcesor
{
    public partial class Form : System.Windows.Forms.Form
    {
        public Form()
        {
            InitializeComponent();
        }

        static Dictionary<string, sRegister> sregDict = new()
        {
            { "AH", new sRegister() },
            { "AL", new sRegister() },
            { "BH", new sRegister() },
            { "BL", new sRegister() },
            { "CH", new sRegister() },
            { "CL", new sRegister() },
            { "DH", new sRegister() },
            { "DL", new sRegister() }
        };

        Dictionary<string, Register> regDict = new()
        {
            { "AX", new Register(sregDict["AH"], sregDict["AL"]) },
            { "BX", new Register(sregDict["BH"], sregDict["BL"]) },
            { "CX", new Register(sregDict["CH"], sregDict["CL"]) },
            { "DX", new Register(sregDict["DH"], sregDict["DL"]) },
            { "SI", new Register() },
            { "DI", new Register() },
            { "BP", new Register() },
            { "SP", new Register(0xFFFE) },
            { "DISP", new Register() }
        };

        static Memory memory = new();
        static Memory stack = new();

        bool DI_ON = false;
        bool SI_ON = false;
        bool BX_ON = false;
        bool BP_ON = false;
        bool DISP_ON = false;
        private void SI_Checked(object sender, EventArgs e)
        {
            if (SI_ON)
            {
                SI_ON = false;
                ((RadioButton)sender).Checked = false;
            }
            else
            {
                SI_ON = true;
                ((RadioButton)sender).Checked = true;
            }
        }
        private void DI_Checked(object sender, EventArgs e)
        {
            if (DI_ON)
            {
                DI_ON = false;
                ((RadioButton)sender).Checked = false;
            }
            else
            {
                DI_ON = true;
                ((RadioButton)sender).Checked = true;
            }
        }
        private void BX_Checked(object sender, EventArgs e)
        {
            if (BX_ON)
            {
                BX_ON = false;
                ((RadioButton)sender).Checked = false;
            }
            else
            {
                BX_ON = true;
                ((RadioButton)sender).Checked = true;
            }
        }
        private void BP_Checked(object sender, EventArgs e)
        {
            if (BP_ON)
            {
                BP_ON = false;
                ((RadioButton)sender).Checked = false;
            }
            else
            {
                BP_ON = true;
                ((RadioButton)sender).Checked = true;
            }
        }
        private void DISP_Checked(object sender, EventArgs e)
        {
            if (DISP_ON)
            {
                DISP_ON = false;
                ((RadioButton)sender).Checked = false;
            }
            else
            {
                DISP_ON = true;
                ((RadioButton)sender).Checked = true;
            }
        }
        private void MOV(object src, object dst)
        {
            if (src is Register && dst is Register)
            {
                ((Register)dst).setValue(((Register)src).getValue());
            }
            else if (src is sRegister && dst is sRegister)
            {
                ((sRegister)dst).Value = ((sRegister)src).Value;
            }
            else if (src is Register && dst is ushort)
            {
                memory.setBytes((ushort)dst, ((Register)src).getValue());
            }
            else if (src is sRegister && dst is ushort)
            {
                memory.setByte((ushort)dst, ((sRegister)src).Value);
            }
            else if (src is ushort && dst is Register)
            {
                ((Register)dst).setValue(memory.getBytes((ushort)src));
            }
            else if (src is ushort && dst is sRegister)
            {
                ((sRegister)dst).Value = memory.getByte((ushort)src);
            }
        }
        private void XCHG(object src, object dst)
        {
            if (src is Register && dst is Register)
            {
                ushort temp = ((Register)dst).getValue();
                ((Register)dst).setValue(((Register)src).getValue());
                ((Register)src).setValue(temp);
            }
            else if (src is sRegister && dst is sRegister)
            {
                byte temp = ((sRegister)dst).Value;
                ((sRegister)dst).Value = ((sRegister)src).Value;
                ((sRegister)src).Value = temp;
            }
            else if (src is Register && dst is ushort)
            {
                ushort temp = memory.getBytes((ushort)dst);
                memory.setBytes((ushort)dst, ((Register)src).getValue());
                ((Register)src).setValue(temp);
            }
            else if (src is sRegister && dst is ushort)
            {
                byte temp = memory.getByte((ushort)dst);
                memory.setByte((ushort)dst, ((sRegister)src).Value);
                ((sRegister)src).Value = temp;
            }
            else if (src is ushort && dst is Register)
            {
                ushort temp = ((Register)dst).getValue();
                ((Register)dst).setValue(memory.getBytes((ushort)src));
                memory.setBytes((ushort)src, temp);
            }
            else if (src is ushort && dst is sRegister)
            {
                byte temp = ((sRegister)dst).Value;
                ((sRegister)dst).Value = memory.getByte((ushort)src);
                memory.setByte((ushort)src, temp);
            }
        }
        private void PUSH(object src)
        {

            ushort sp = regDict["SP"].getValue();

            if (src is Register) stack.setBytes(sp, ((Register)src).getValue());
            if (src is ushort) stack.setBytes(sp, memory.getBytes((ushort)src));

            regDict["SP"].setValue((ushort)(sp - 2));
        }
        private void POP(object dst)
        {
            ushort sp = ((ushort)(regDict["SP"].getValue() + 2));

            if (dst is Register) ((Register)dst).setValue(stack.getBytes(sp));
            if (dst is ushort) memory.setBytes((ushort)dst, stack.getBytes(sp));

            regDict["SP"].setValue((ushort)(sp));
        }
        private void refresh()
        {
            valueAX.Text = regDict["AX"].getValue().ToString("X4");
            valueBX.Text = regDict["BX"].getValue().ToString("X4");
            valueCX.Text = regDict["CX"].getValue().ToString("X4");
            valueDX.Text = regDict["DX"].getValue().ToString("X4");

            valueSI.Text = regDict["SI"].getValue().ToString("X4");
            valueDI.Text = regDict["DI"].getValue().ToString("X4");
            valueBP.Text = regDict["BP"].getValue().ToString("X4");
            valueSP.Text = regDict["SP"].getValue().ToString("X4");
            valueDISP.Text = regDict["DISP"].getValue().ToString("X4");
        }
        private bool isInputValid(string input)
        {
            return (Regex.IsMatch(input, @"^[a-fA-F0-9]+$") && input.Length <= 4);
        }
        private ushort calculateAddress()
        {
            ushort result = 0;

            if (radioButtonSI.Checked) result += regDict["SI"].getValue();
            if (radioButtonDI.Checked) result += regDict["DI"].getValue();
            if (radioButtonBX.Checked) result += regDict["BX"].getValue();
            if (radioButtonBP.Checked) result += regDict["BP"].getValue();
            if (radioButtonDISP.Checked) result += regDict["DISP"].getValue();

            return result;
        }
        private object[] getOperandsFromText()
        {
            object[] operands = new object[] { comboBoxZrodlo.Text, comboBoxPrzezn.Text };

            for (int i = 0; i < 2; i++)
            {
                string operand = (string)operands[i];

                if (operand.StartsWith('[') && operand.EndsWith(']'))
                {
                    operand = operand[1..(operand.Length - 1)];
                    if (isInputValid(operand)) operands[i] = Convert.ToUInt16(operand, 16);

                }

                try
                {
                    operands[i] = regDict[operand];
                }
                catch (Exception) { }

                try
                {
                    operands[i] = sregDict[operand];
                }
                catch (Exception) { }

            }

            return operands;
        }
        private void buttonInsert_Click(object sender, EventArgs e)
        {
            if (isInputValid(boxAX.Text))
                regDict["AX"].setValue(Convert.ToUInt16(boxAX.Text, 16));
            if (isInputValid(boxBX.Text))
                regDict["BX"].setValue(Convert.ToUInt16(boxBX.Text, 16));
            if (isInputValid(boxCX.Text))
                regDict["CX"].setValue(Convert.ToUInt16(boxCX.Text, 16));
            if (isInputValid(boxDX.Text))
                regDict["DX"].setValue(Convert.ToUInt16(boxDX.Text, 16));
            if (isInputValid(boxSI.Text))
                regDict["SI"].setValue(Convert.ToUInt16(boxSI.Text, 16));
            if (isInputValid(boxDI.Text))
                regDict["DI"].setValue(Convert.ToUInt16(boxDI.Text, 16));
            if (isInputValid(boxBP.Text))
                regDict["BP"].setValue(Convert.ToUInt16(boxBP.Text, 16));
            if (isInputValid(boxSP.Text))
                regDict["SP"].setValue(Convert.ToUInt16(boxSP.Text, 16));
            if (isInputValid(boxDISP.Text))
                regDict["DISP"].setValue(Convert.ToUInt16(boxDISP.Text, 16));

            refresh();
        }
        private void buttonDoZrodla_Click(object sender, EventArgs e)
        {
            comboBoxZrodlo.Text = $"[{calculateAddress().ToString("X4")}]";
        }
        private void buttonDoPrzezn_Click(object sender, EventArgs e)
        {
            comboBoxPrzezn.Text = $"[{calculateAddress().ToString("X4")}]";
        }
        private void buttonMOV_Click(object sender, EventArgs e)
        {
            object[] operands = getOperandsFromText();
            MOV(operands[0], operands[1]);
            refresh();
        }
        private void buttonXCHG_Click(object sender, EventArgs e)
        {
            object[] operands = getOperandsFromText();
            XCHG(operands[0], operands[1]);
            refresh();
        }
        private void buttonPUSH_Click(object sender, EventArgs e)
        {
            object[] operands = getOperandsFromText();
            PUSH(operands[0]);
            refresh();
        }
        private void buttonPOP_Click(object sender, EventArgs e)
        {
            object[] operands = getOperandsFromText();
            POP(operands[1]);
            refresh();
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }
        private void label2_Click(object sender, EventArgs e)
        {

        }
        private void Form_Load(object sender, EventArgs e)
        {
            
        }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
