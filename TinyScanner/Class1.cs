using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TinyScanner
{
    public class Parser
    {
        private List<Token> tokens;
        private int index = 0;

        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
        }

        private Token Current()
        {
            if (index < tokens.Count)
                return tokens[index];
            return null;
        }

        private void Match(string expected)
        {
            if (Current() != null &&
               (Current().Value == expected || Current().Type == expected))
            {
                index++;
            }
            else
            {
                throw new Exception("Expected " + expected);
            }
        }
        public void Parse()
        {
            ParseStatement();
        }
        private void ParseStatement()
        {
            var token = Current();

            if (token == null) return;

            if (token.Value == "int" || token.Value == "float" || token.Value == "string")
            {
                Match(token.Value);
                Match("Identifier");
                Match(":=");
                ParseExpression();
                Match(";");
            }
            else if (token.Type == "Identifier")
            {
                Match("Identifier");
                Match(":=");
                ParseExpression();
                Match(";");
            }
            else
            {
                throw new Exception("Invalid start of statement: " + token.Value);
            }
        }
        private void ParseExpression()
        {
            ParseTerm();

            while (Current() != null &&
                  (Current().Value == "+" || Current().Value == "-"))
            {
                Match(Current().Value);
                ParseTerm();
            }
        }
        private void ParseTerm()
        {
            ParseFactor();

            while (Current() != null &&
                  (Current().Value == "*" || Current().Value == "/"))
            {
                Match(Current().Value);
                ParseFactor();
            }
        }
        private void ParseFactor()
        {
            var token = Current();

            if (token.Type == "Identifier")
                Match("Identifier");
            else if (token.Type == "Number")
                Match("Number");
            else
                throw new Exception("Invalid expression");
        }
    }
}