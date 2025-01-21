using System;
using System.Windows.Forms;
using BigInteger;


namespace SimpleNeuro
{
    public partial class frmNeuroCalc : Form
    {
        BigNumber lastValue;
        string lastOperation = "=";
        bool lastDigit = false;

        public frmNeuroCalc()
        {
            InitializeComponent();
        }

        private void frmNeuroCalc_Load(object sender, EventArgs e)
        {
            try
            {
                lastValue = new BigNumber(0);
            } catch (Exception ex)
            {
                MessageBox.Show(ex.InnerException.Message);
            }

            textBox_Result.Text = lastValue.ToString();
        }

        private void numbers_Click(object sender, EventArgs e)
        {
            if (sender is Button)
            {
                var text = (sender as Button).Text;

                if (!lastDigit)
                {
                    textBox_Result.Text = "";
                }
                lastDigit = true;

                if (textBox_Result.Text.Trim() == "0")
                    textBox_Result.Text = text;
                else
                    textBox_Result.AppendText(text);
            }
        }

        private void btnC_Click(object sender, EventArgs e)
        {
            //очищаем всё
            lastValue = new BigNumber(0);
            textBox_Result.Text = lastValue.ToString();
            lastOperation = "";
        }

        private void btnCE_Click(object sender, EventArgs e)
        {
            //очистка только текущего значения
            textBox_Result.Text = "0";
        }

        private void operation_Click(object sender, EventArgs e)
        {
            if (sender is Button)
            {
                var operation = (sender as Button).Text;
                var currentValue = textBox_Result.Text;
                var resultValue = new BigNumber(0);

                if (operation == "=")
                {
                    switch (lastOperation)
                    {
                        case "+":
                            resultValue = lastValue + new BigNumber(currentValue);
                            break;
                        case "-":
                            resultValue = lastValue - new BigNumber(currentValue);
                            break;
                        case "*":
                            resultValue = lastValue * new BigNumber(currentValue);
                            break;
                        case "/":
                            resultValue = lastValue / new BigNumber(currentValue);
                            break;
                    }
                    textBox_Result.Text = resultValue.ToString();

                } else
                {
                    lastOperation = operation;

                    lastValue = new BigNumber(currentValue);

                    lastDigit = false;

                }

            }
        }

        //игнорируем ввод всех символов кроме цифр
        private void textBox_Result_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }
    }


}
