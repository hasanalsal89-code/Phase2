using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace TinyScanner
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            string code = textBox1.Text;

            string pattern =
              @"(/\*.*?\*/)|" +
              @"(\bint\b|\bfloat\b|\bstring\b|\bread\b|\bwrite\b|\brepeat\b|\buntil\b|\bif\b|\belseif\b|\belse\b|\bthen\b|\breturn\b|\bendl\b)|" +
              @"(""[^""]*"")|" +
              @"([a-zA-Z][a-zA-Z0-9]*)|" +
              @"([0-9]+(\.[0-9]+)?)|" +
              @"(:=)|" +
              @"(\+|\-|\*|\/)|" +
              @"(<>|<|>|=)|" +
              @"(&&|\|\|)|" +
              @"(;|,|\(|\)|\{|\})";

            MatchCollection matches = Regex.Matches(code, pattern, RegexOptions.Singleline);

            List<Token> tokens = new List<Token>();

            foreach (Match m in matches)
            {
                string token = m.Value;
                string type = "";

                if (Regex.IsMatch(token, @"\b(int|float|string|read|write|repeat|until|if|elseif|else|then|return|endl)\b"))
                    type = "Keyword";
                else if (Regex.IsMatch(token, @"^[a-zA-Z][a-zA-Z0-9]*$"))
                    type = "Identifier";
                else if (Regex.IsMatch(token, @"^[0-9]+(\.[0-9]+)?$"))
                    type = "Number";
                else if (Regex.IsMatch(token, "^\"[^\"]*\"$"))
                    type = "String";
                else if (Regex.IsMatch(token, "^:=$"))
                    type = "Assignment";
                else if (Regex.IsMatch(token, @"^[+\-*/]$"))
                    type = "Arithmetic Operator";
                else if (Regex.IsMatch(token, @"^(<>|<|>|=)$"))
                    type = "Condition Operator";
                else if (Regex.IsMatch(token, @"^(&&|\|\|)$"))
                    type = "Boolean Operator";
                else if (token == ";")
                    type = "Semicolon";
                else if (Regex.IsMatch(token, @"^[,\(\)\{\}]$"))
                    type = "Symbol";
                else if (Regex.IsMatch(token, @"^/\*.*\*/$"))
                    type = "Comment";

                dataGridView1.Rows.Add(token, type);
                tokens.Add(new Token { Value = token, Type = type });
            }

            string[] lines = code.Split('\n');

            foreach (string line in lines)
            {
                string trimmed = line.Trim();

                if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("/*"))
                    continue;

                if ((trimmed.Contains(":=") ||
                     trimmed.StartsWith("read") ||
                     trimmed.StartsWith("write") ||
                     trimmed.StartsWith("return")) &&
                     !trimmed.EndsWith(";"))
                {
                    MessageBox.Show("Syntax Error: Missing semicolon (;) in line:\n" + trimmed);
                    return;
                }
            }
            string cleanedCode = Regex.Replace(code, pattern, "").Trim();

            if (!string.IsNullOrEmpty(cleanedCode))
            {
                MessageBox.Show("Lexical Error: Invalid token found -> " + cleanedCode);
                return;
            }

            try
            {
                Parser parser = new Parser(tokens);
                parser.Parse();

                MessageBox.Show("Code is VALID ✅");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Syntax Error ❌: " + ex.Message);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
