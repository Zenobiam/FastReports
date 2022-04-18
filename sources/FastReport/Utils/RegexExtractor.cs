using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace FastReport.Utils
{
    public class RegexPatternExtractor
    {
        enum State { Raw, Range, EscapeChar, RoundBracket };
        StringBuilder pattern = new StringBuilder();
        State state = State.Raw;
        char prevCharacter = '\0';
        char startRange = '\0';
        Stack<State> stateStack = new Stack<State>();

        public string Pattern { get { return pattern.ToString(); } }

        public void Clear()
        {
            pattern = new StringBuilder();
            stateStack.Clear();
            state = State.Raw;
        }

        public void AddExpression(string expression)
        {
            foreach (char ch in expression)
            {
                switch (state)
                {
                    case State.Raw:
                        RawState(ch);
                        break;
                    case State.RoundBracket:
                        RoundBracketState(ch);
                        break;
                    case State.Range:
                        RangeState(ch);
                        break;
                    case State.EscapeChar:
                        ParseEscapeChar(ch);
                        break;
                    default:
                        throw new Exception("Expession extractor unknown state");
                }
            }
        }

        private void ParseEscapeChar(char ch)
        {
            switch(ch)
            {
                case '.':
                case '+':
                case '-':
                case '(':
                case ')':
                case '*':
                case '[':
                case ']':
                case '{':
                case '}':
                case '/':
                case '\\':
                    pattern.Append(ch);
                    state = stateStack.Pop();
                    break;
                default:
                    throw new NotImplementedException("Not implemented escape sequence");
            }
        }

        private void RangeState(char ch)
        {
            switch (ch)
            {
                case ']':
                    state = stateStack.Pop();
                    break;
                case '-':
                    startRange = prevCharacter;
                    break;
                case '\\':
                    stateStack.Push(state);
                    state = State.EscapeChar;
                    break;
                default:
                    if (startRange != '\0')
                    {
                        if (ch > startRange)
                        {
                            for (char c = (char)(startRange+1); c <= ch; c++)
                                pattern.Append(c);
                        }
#if IGNORE_ORDER
                        else
                        {
                            for (char c = ch; c <= startRange; c++)
                                pattern.Append(c);
                        }
#endif
                        startRange = '\0';
                    }
                    else
                    {
                        pattern.Append(ch);
                        prevCharacter = ch;
                    }
                    break;
            }
        }

        private void RoundBracketState(char ch)
        {
            switch(ch)
            {
                case ')':
                    state = stateStack.Pop();
                    break;
                case '[':
                    stateStack.Push(state);
                    state = State.Range;
                    break;
                case '|':
                    // Do nothing, just continue parsing
                    break;
                default:
                    pattern.Append(ch);
                    break;
            }
        }

        private void RawState(char ch)
        {
            switch (ch)
            {
                case '(':
                    stateStack.Push(state);
                    state = State.RoundBracket;
                    break;
                case '[':
                    stateStack.Push(state);
                    state = State.Range;
                    break;
                case '*':
                    // Ignore it in pattern extractor
                    break;
                default:
                    pattern.Append(ch);
                    break;
            }
        }
    }
}
