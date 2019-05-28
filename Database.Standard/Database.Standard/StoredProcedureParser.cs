using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Text.RegularExpressions;

namespace Interstates.Control.Database
{
    public class StoredProcedureParser
    {
        private readonly ParameterParser _parameterParser;

        public StoredProcedureParser()
            : this(new ParameterParser())
        {
        }

        public StoredProcedureParser(ParameterParser parameterParser)
        {
            _parameterParser = parameterParser;
        }

        public ParameterParser Parser { get { return _parameterParser; } }

        public class Result
        {
            public Result(string storedProcedureName, string returnVariable, List<ParameterSlot> parameterSlots)
            {
                StoredProcedureName = storedProcedureName;
                ReturnVariable = returnVariable;
                ParameterSlots = parameterSlots.AsReadOnly();
            }

            public string StoredProcedureName { get; private set; }
            public string ReturnVariable { get; private set; }
            public ReadOnlyCollection<ParameterSlot> ParameterSlots { get; private set; }

            public virtual CommandHelper PrepareCommand(QueryBase query)
            {
                if (query == null) throw new ArgumentNullException("query");
                return new CommandHelper(this, query);
            }
        }

        /// <summary>
        /// This will build a command object that can run the commandText as a stored procedure.
        /// Find param values so that we can set the parameter value of the stored procedure.
        /// The following syntax is allowed for parameters. 
        /// Find:(@paramname)( = )( 'value' | N'value' | value )(, | end of line)
        /// You can pretty much copy and paste the Sql2005 managment studio Execute Stored procedure syntax and this will parse it.
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public virtual Result ParseStoredProcedureCall(string commandText)
        {
            string procedureName = null;
            string returnVariable = null;
            returnVariable = null;

            string[] commands = commandText.TrimStart(null).Split(new char[] { ' ', '\t' });
            

            // Parse the stored procedure, look for return value, then store procedure name
            foreach (string command in commands)
            {
                if (command.Length == 0)
                    continue;
                if (command.ToLower() == "exec")
                    continue;
                if (command == "=")
                    continue;

                // Is there a return value 
                if (command.StartsWith("@") || command.StartsWith("{@"))
                {
                    returnVariable = command;
                    continue;
                }

                // this must be the name of the stored procedure
                if (string.IsNullOrEmpty(procedureName))
                    procedureName = command;
                else
                    procedureName += " " + command;

                // This is a check to make sure the name contains all of the brackets([])
                int checkPos = procedureName.LastIndexOf('[');
                if (checkPos >= 0)
                {
                    if (checkPos < procedureName.LastIndexOf(']'))
                        break; // Name found
                }
                else
                    break; // Name found
            }
            
            // Find param values,
            // Find:(@paramname)( = )( 'value' | N'value' | value )(, | end of line)
            int intParams = commandText.IndexOf(procedureName);
            string strParams = commandText.Substring(intParams + procedureName.Length);

            List<ParameterSlot> parameterSlots = _parameterParser.ParseSlots(strParams);
            

            return new Result(procedureName.Trim(), returnVariable, parameterSlots);
        }        


        /// <summary>
        /// A class that parses tokens for the StoredProcedureParser.
        /// </summary>
        public class TokenParser
        {
            /// <summary>
            /// Internal state representing the starting state
            /// </summary>
            public const int DEFAULT = 0;
            /// <summary>
            /// Token is plain, unquoted text.
            /// </summary>
            public const int TEXT = 1;
            /// <summary>
            /// Token is quoted text
            /// </summary>
            public const int QUOTED = 2;
            /// <summary>
            /// Internal state when the tokenizer has seen the end single quote and needs to check whether the next character is a quote.
            /// </summary>
            public const int QUOTED_POTENTIALEND = 3;
            /// <summary>
            /// Token consists of all whitespace
            /// </summary>
            public const int WHITESPACE = 4;
            /// <summary>
            /// Token is a comma
            /// </summary>
            public const int COMMA = 5;
            /// <summary>
            /// Token is an equals sign
            /// </summary>
            public const int EQUALS = 6;
            /// <summary>
            /// Token is the end of the input.
            /// </summary>
            public const int EOF = 7;

            /// <summary>
            /// Take an input string and split it up into relevant tokens.
            /// </summary>        
            /// <example>
            /// Input string: "@one = 24,   @two,{@three}"
            /// Tokens:
            /// "@one" (text)
            /// " " (whitespace)
            /// "=" (equals)
            /// "24" (text)
            /// "," (comma)
            /// "   " (whitespace)
            /// "@two" (text)
            /// "," (comma)
            /// "{@three}" (brackets)
            /// </example>
            /// <param name="input"></param>
            /// <returns></returns>
            public virtual List<Token> Tokenize(string input)
            {
                // Essentially, the tokenizer is a state machine.
                // The state machine takes one character at a time, looks at it,
                // and decides to do three things:
                //   1) transition to another state
                //   2) move to the next character
                //   3) add a token to the list of tokens
                // When there are no more characters in the input, the state
                // machine goes through one last time with an end-of-input character.
                // After that, it returns whatever tokens it has.

                // The list of tokens
                List<Token> tokens = new List<Token>();

                // The state machine's current state. We start at DEFAULT
                int state = DEFAULT;
                // Keep track of where a token started. This lets us get the full string for the token.
                int tokenStart = 0;
                // The location in the current string.
                int location = 0;

                // Loop until we're done with the input (and then do one more)
                while (location <= input.Length)
                {
                    char c = '\0'; // End of input
                    if (location < input.Length)
                        c = input[location];

                    // The main code of the state machine. Each
                    // case is a state which decides what to do with the
                    // current character.
                    switch (state)
                    {
                        case DEFAULT:
                            // All states return to the DEFAULT state after adding a token.

                            // Mark where the token starts.
                            tokenStart = location;
                            // The default state will always move to the next location.
                            location++;
                            switch (c)
                            {
                                case '\'':
                                    state = QUOTED;
                                    break;
                                case ',':
                                    state = COMMA;
                                    break;
                                case '=':
                                    state = EQUALS;
                                    break;
                                case '\0':
                                    // We're already in the default state, so nothing to do.
                                    state = DEFAULT;
                                    break;

                                default:
                                    //case ' ':
                                    if (Char.IsWhiteSpace(c))
                                    {
                                        state = WHITESPACE;
                                    }
                                    else
                                    {
                                        state = TEXT;
                                    }
                                    break;
                            }
                            break;
                        case TEXT:
                            // What we're currently eating up is characters in a bare text string.
                            // Any text at that doesn't start another token will be read as text.
                            // If we do get a character that starts another token, then add all of the
                            // text we've currently read as a TEXT token.
                            switch (c)
                            {
                                case '\'':
                                case ',':
                                case '=':
                                case '\0':
                                    tokens.Add(new Token { Type = TEXT, Value = input.Substring(tokenStart, location - tokenStart), StartLocation = tokenStart });
                                    state = DEFAULT;
                                    break;
                                default:
                                    if (Char.IsWhiteSpace(c))
                                    {
                                        tokens.Add(new Token { Type = TEXT, Value = input.Substring(tokenStart, location - tokenStart), StartLocation = tokenStart });
                                        state = DEFAULT;
                                    }
                                    else
                                    {
                                        // The character didn't start another token, so lets stay in the TEXT state and keep reading.
                                        location++;
                                    }
                                    break;
                            }
                            break;
                        case QUOTED:
                            // We're reading quoted text, which is everything between single quotes.
                            switch (c)
                            {
                                case '\'':
                                    // This could be the end of our text, or it could be two consecutive single quotes that is essentially an
                                    // escaped single quote. So, we need to transition to QUOTED_POTENTIALEND to determine which is the case.
                                    location++;
                                    state = QUOTED_POTENTIALEND;
                                    break;
                                case '\0':
                                    // Uh oh, we didn't see our closing quote, and there's nothing left!
                                    throw new ParameterTokenizerException("Unexpected end of input. Expecting a closing single quote.", location, state);
                                default:
                                    // This is just another character inside the quoted text. Keep reading character, then.
                                    location++;
                                    break;
                            }
                            break;
                        case QUOTED_POTENTIALEND:
                            // Our last character was a single quote, and our last state was QUOTED. So, we need to determine if that last
                            // single quote was just the first in two single quotes (representing an escaped single quote), or if it was
                            // really the end of the quoted string.
                            switch (c)
                            {
                                case '\'':
                                    // Not the end of the quoted string. So, keep reading.
                                    location++;
                                    state = QUOTED;
                                    break;
                                default:
                                    // Yep, it was the end of the quoted string, so lets add the quoted string to the tokens.
                                    // We have already read the next character, so don't increment location.
                                    state = DEFAULT;
                                    tokens.Add(new Token { Type = QUOTED, Value = input.Substring(tokenStart, location - tokenStart), StartLocation = tokenStart });
                                    break;
                            }
                            break;
                        case WHITESPACE:
                            // We're currently reading whitespace.
                            if (Char.IsWhiteSpace(c))
                            {
                                // Still whitespace, so keep reading.
                                location++;
                            }
                            else
                            {
                                // Not whitespace, so lets add our whitespace token.
                                // We have already read the next character, so don't increment location.
                                tokens.Add(new Token { Type = WHITESPACE, Value = input.Substring(tokenStart, location - tokenStart), StartLocation = tokenStart });
                                tokenStart = location;
                                state = DEFAULT;
                            }
                            break;
                        case COMMA:
                            // The last character we read was a comma. Since the comma token is ALWAYS 1 character long, we just need to add
                            // the token and go back to the default state.
                            tokens.Add(new Token { Type = COMMA, Value = input.Substring(tokenStart, 1), StartLocation = tokenStart });
                            state = DEFAULT;
                            break;
                        case EQUALS:
                            // The last character we read was an equals. Since the comma token is ALWAYS 1 character long, we just need to add
                            // the token and go back to the default state.
                            tokens.Add(new Token { Type = EQUALS, Value = input.Substring(tokenStart, 1), StartLocation = tokenStart });
                            state = DEFAULT;
                            break;
                    }
                }

                // It helps the parser incredibly if it can receive an end-of-input token at the end of the input, so add that on.
                tokens.Add(new Token { Type = EOF, Value = "[end of input]", StartLocation = input.Length });
                return tokens;
            }
        }

        /// <summary>
        /// A token from an input string.
        /// </summary>
        public struct Token
        {            
            /// <summary>
            /// The type of the token (see contant ints above)
            /// </summary>
            public int Type;
            /// <summary>
            /// The actual string in the token.
            /// </summary>
            public string Value;
            /// <summary>
            /// Where in the input the token starts.
            /// </summary>
            public int StartLocation;

            public override string ToString()
            {
                return Value;
            }
        }

        /// <summary>
        /// A class that parses parameters from a stream of tokens.
        /// </summary>
        public class ParameterParser
        {
            private readonly TokenParser _tokenParser;

            public ParameterParser()
                : this(new TokenParser())
            {
            }

            public ParameterParser(TokenParser tokenParser)
            {
                _tokenParser = tokenParser;
            }

            public TokenParser TokenParser { get { return _tokenParser; } }

            /// <summary>
            /// Parse the slots off of an input string. It's assumed that the stored procedure is already removed from the input.
            /// </summary>        
            public virtual List<ParameterSlot> ParseSlots(string input)
            {
                try
                {
                    return ParseSlots(_tokenParser.Tokenize(input));
                }
                catch (ParameterParserException exception)
                {
                    exception.Input = input;
                    throw;
                }
            }
            /// <summary>
            /// Parse the slots off of a tokenized string. It's assumed that the stored procedure is already removed from the input.
            /// </summary>
            public virtual List<ParameterSlot> ParseSlots(IEnumerable<Token> tokens)
            {
                // Essentially, the parser is a state machine.
                // The state machine takes one token at a time, looks at it,
                // and decides to do two things:
                //   1) transition to another state
                //   2) add a parameter slot to the list of parameter slots            
                // This state machine always goes on to the next token, i.e. a
                // state can't decide whether to move to the next token or not.

                // Start out with our possible states.

                // The state machine is starting a new parameter slot.
                const int START = 1;
                // The parameterslot is potentially a positional parameter slot.
                const int POSITIONAL = 2;
                // We stumbled on an equals sign, so this should be a named parameter,
                // but we still need the value to be sure.
                const int EXPECTVALUEFORNAMED = 3;
                // The parameter slot is a named parameter slot.
                const int NAMED = 4;
                // The parameter is a positional parameter slot, AND we've run across the OUTPUT keyword
                const int POSITIONALOUTPUT = 5;
                // The parameter is a named parameter slot, AND we've run across the OUTPUT keyword
                const int NAMEDOUTPUT = 6;

                // LEGAL FINAL STATES: Start

                // Our list of slots to return
                List<ParameterSlot> slots = new List<ParameterSlot>();

                // The current state of our state machine
                int state = START;
                // Keep track of the name and the value if available
                string name = "";
                string value = "";
                // Whether the current state has seen trailing whitespace (so that something like '{@Parameter}OUTPUT' isn't legal, but '{@Parameter} OUTPUT' is).
                bool hasTrailingWhitespace = false;

                Token lastToken = new Token();

                // So, go through each token.
                foreach (Token token in tokens)
                {
                    lastToken = token;
                    // The main state machine
                    switch (state)
                    {
                        case START:
                            // We're at the start of a new parameter slot.
                            hasTrailingWhitespace = false;
                            switch (token.Type)
                            {
                                case TokenParser.WHITESPACE:
                                    // We got meaningless whitespace. OK.
                                    break;
                                case TokenParser.EOF:
                                    // We're at the end of the input. This is quite alright, because we're not
                                    // in the middle of parsing a parameter slot.
                                    break;
                                case TokenParser.TEXT:
                                case TokenParser.QUOTED:
                                    // We got something that could be a value, so let's
                                    // make note that this is the value, and move to the POSITIONAL state.
                                    // If this is actually a named parameter, POSITIONAL will handle that.
                                    value = token.Value;
                                    state = POSITIONAL;
                                    break;
                                default:
                                    throw new ParameterParserException(String.Format("Error in parsing: unexpected '{0}'", token.Value), token, state);
                            }
                            break;
                        case POSITIONAL:
                            // What we've seen so far is some text that we assume is a value. [@value]
                            switch (token.Type)
                            {
                                case TokenParser.WHITESPACE:
                                    // We need to keep track of whether this state has trailing
                                    // whitespace, just in case we get an OUTPUT keyword.
                                    hasTrailingWhitespace = true;
                                    break;
                                case TokenParser.COMMA:
                                case TokenParser.EOF:
                                    // OK, a comma or an EOF means the end of this slot, so add it to the list.
                                    state = START;
                                    slots.Add(new ParameterSlot { Type = SlotType.Positional, IsOutput = false, Value = value });
                                    break;
                                case TokenParser.EQUALS:
                                    // Oh, OK, it turns out we don't have a positional parameter after all, because we just
                                    // got an equals sign. So, we probably have a named parameter. Since all we know is that there
                                    // is an equals sign (and not something meaningful after it), we need to transition to an
                                    // intermediate state that expects a value still.                                
                                    hasTrailingWhitespace = false;
                                    // That thing we thought was a value is actually the name, since it precedes the equals sign.
                                    name = value;
                                    state = EXPECTVALUEFORNAMED;
                                    break;
                                case TokenParser.TEXT:
                                    // Text? That's a little bit unexpected.
                                    // Well, any random text is unexpected, so we'll throw an error. However, if the text is
                                    // an important keyword, namely OUT or OUTPUT (essentially the same thing), then it isn't
                                    // unexpected.
                                    if (hasTrailingWhitespace && (token.Value.ToUpper() == "OUT" || token.Value.ToUpper() == "OUTPUT"))
                                        // This has the output keyword, so it must be a positional output.
                                        state = POSITIONALOUTPUT;
                                    else
                                        throw new ParameterParserException(String.Format("Error in parsing: unexpected '{0}'", token.Value), token, state);
                                    break;
                                default:
                                    throw new ParameterParserException(String.Format("Error in parsing: unexpected '{0}'", token.Value), token, state);
                            }
                            break;
                        case POSITIONALOUTPUT:
                            // What we've seen so far is some text that we assume is a value, and the keyword OUTPUT. [@value OUTPUT]
                            switch (token.Type)
                            {
                                case TokenParser.WHITESPACE:
                                    // Meaningless whitespace.
                                    hasTrailingWhitespace = true;
                                    break;
                                case TokenParser.COMMA:
                                case TokenParser.EOF:
                                    // OK, a comma or an EOF means the end of this slot, so add it to the list.
                                    state = START;
                                    slots.Add(new ParameterSlot { Type = SlotType.Positional, IsOutput = true, Value = value });
                                    break;
                                default:
                                    throw new ParameterParserException(String.Format("Error in parsing: unexpected '{0}'", token.Value), token, state);
                            }
                            break;
                        case EXPECTVALUEFORNAMED:
                            // What we've seen so far is some text we assume is a name, and an equals [@name =]
                            switch (token.Type)
                            {
                                case TokenParser.WHITESPACE:
                                    // Meaningless whitespace
                                    break;
                                case TokenParser.TEXT:
                                case TokenParser.QUOTED:
                                    // OK! We got what we now assume must be a value, since it comes after the equals sign. That means
                                    // we need to transition to the NAMED state for it to handle the rest of the named parameter
                                    // stuff.
                                    hasTrailingWhitespace = false;
                                    // Make note that this is our value.
                                    value = token.Value;
                                    state = NAMED;
                                    break;
                                default:
                                    throw new ParameterParserException(String.Format("Error in parsing: unexpected '{0}'", token.Value), token, state);
                            }
                            break;
                        case NAMED:
                            // What we've seen so far is some text we assume is a name, an equals, and some text we assume is a value [@name = @value]
                            switch (token.Type)
                            {
                                case TokenParser.WHITESPACE:
                                    // We need to keep track of whether this state has trailing
                                    // whitespace, just in case we get an OUTPUT keyword.
                                    hasTrailingWhitespace = true;
                                    break;
                                case TokenParser.COMMA:
                                case TokenParser.EOF:
                                    // OK, a comma or an EOF means the end of this slot, so add it to the list.
                                    state = START;
                                    slots.Add(new ParameterSlot { Type = SlotType.Named, IsOutput = false, Name = name, Value = value });
                                    break;
                                case TokenParser.TEXT:
                                    // Text? That's a little bit unexpected.
                                    // Well, any random text is unexpected, so we'll throw an error. However, if the text is
                                    // an important keyword, namely OUT or OUTPUT (essentially the same thing), then it isn't
                                    // unexpected.
                                    if (hasTrailingWhitespace && (token.Value.ToUpper() == "OUT" || token.Value.ToUpper() == "OUTPUT"))
                                        // This has the output keyword, so it must be a named output.
                                        state = NAMEDOUTPUT;
                                    else
                                        throw new ParameterParserException(String.Format("Error in parsing: unexpected '{0}'", token.Value), token, state);
                                    break;
                                default:
                                    throw new ParameterParserException(String.Format("Error in parsing: unexpected '{0}'", token.Value), token, state);
                            }
                            break;
                        case NAMEDOUTPUT:
                            // What we've seen so far is some text we assume is a name, an equals, some text we assume is a value, and the keyword OUTPUT [@name = @value OUTPUT]
                            switch (token.Type)
                            {
                                case TokenParser.WHITESPACE:
                                    // Meaningless whitespace
                                    hasTrailingWhitespace = true;
                                    break;
                                case TokenParser.COMMA:
                                case TokenParser.EOF:
                                    // OK, a comma or an EOF means the end of this slot, so add it to the list.
                                    state = START;
                                    slots.Add(new ParameterSlot { Type = SlotType.Named, IsOutput = true, Name = name, Value = value });
                                    break;
                                default:
                                    throw new ParameterParserException(String.Format("Error in parsing: unexpected '{0}'", token.Value), token, state);
                            }
                            break;
                    }
                }

                // We should be in the START state. If we're not, something went wrong.
                if (state == START)
                    return slots;
                else
                    throw new ParameterParserException(String.Format("Error in parsing: unexpected end of input"), lastToken, state);
            }
        }
        /// <summary>
        /// A specified parameter in a stored procedure call.
        /// </summary>
        /// <example>
        /// @return = Dummy_sp NULL, {@Value}, @p3 = @Other
        /// |---X1--| |--X2--| |-A|  |--B---|  |---C--|
        ///              
        /// A: A positional ParameterSlot with Value "NULL"
        /// B: A positional ParameterSlot with Value "{@Value}"
        /// C: A named ParameterSlot with Name "@p3" and Value "@Other"
        /// 
        /// X1: Not a ParameterSlot becasue it's the return code.
        /// X2: Not a ParameterSlot because it's just the stored procedure.
        /// </example>
        public class ParameterSlot
        {                        
            /// <summary>
            /// The type of the slot, whether it's named or positional.
            /// </summary>
            public SlotType Type;
            /// <summary>
            /// If the slot is named, then the name of the parameter. If the slot is not named, this is NULL.
            /// </summary>
            public string Name;
            /// <summary>
            /// The value of the slot.
            /// </summary>
            public string Value;
            /// <summary>
            /// Whether this slot is an OUTPUT parameter or not.
            /// </summary>
            public bool IsOutput;
            /// <summary>
            /// The DbParameter that corresponds to this slot. This is set when FindCorrespondingParameters is called.
            /// </summary>
            public DbParameter Parameter;

            public override string ToString()
            {
                if (Type == SlotType.Positional)
                    return Value;
                else
                    return String.Format("{0} = {1}", Name, Value);
            }
        }

        /// <summary>
        /// An exception thrown by the TokenParser.
        /// </summary>
        [Serializable]
        public class ParameterTokenizerException : ApplicationException
        {
            public ParameterTokenizerException(int location, int state)
            {
                Location = location;
                State = state;
            }
            public ParameterTokenizerException(string message, int location, int state)
                : base(message)
            {
                Location = location;
                State = state;
            }
            public ParameterTokenizerException(string message, int location, int state, Exception inner)
                : base(message, inner)
            {
                Location = location;
                State = state;
            }
            protected ParameterTokenizerException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context)
                : base(info, context) { }

            public int Location { get; private set; }
            public int State { get; private set; }
        }

        /// <summary>
        /// An exception thrown by the ParameterParser.
        /// </summary>
        [Serializable]
        public class ParameterParserException : DatabaseException
        {
            public ParameterParserException(Token token, int state)
            {
                Token = token;
                State = state;
            }
            public ParameterParserException(string message, Token token, int state)
                : base(message)
            {
                Token = token;
                State = state;
            }
            public ParameterParserException(string message, Token token, int state, Exception inner)
                : base(message, inner)
            {
                Token = token;
                State = state;
            }
            protected ParameterParserException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context)
                : base(info, context) { }

            public Token Token { get; private set; }
            public int State { get; private set; }
            public string Input { get; set; }

            public override string Message
            {
                get
                {
                    if (String.IsNullOrEmpty(Input))
                        return base.Message;
                    else
                    {
                        StringBuilder builder = new StringBuilder(base.Message);
                        string initialPart = Input.Substring(0, Token.StartLocation);
                        string value = Token.Value;
                        string finalPart = Input.Substring(Token.StartLocation + Token.Value.Length);
                        builder.AppendLine(String.Format(" at position {0} (bracketed with » and « below)", Token.StartLocation));
                        builder.Append(String.Format("{0}»{1}«{2}", initialPart, value, finalPart));

                        return builder.ToString();
                    }
                }
            }
        }

        /// <summary>
        /// A class that helps set parameters using the parser result.
        /// </summary>
        public class CommandHelper : IDisposable
        {
            private readonly Result _result;
            private readonly QueryBase _query;
            private readonly DbCommand _command;
            private readonly PositionalParameterCollection _positionalParameters;
            private readonly NamedParameterCollection _namedParameters;
            private readonly VariableCollection _variables;

            public CommandHelper(Result result, QueryBase query)
            {
                if (result == null) throw new ArgumentNullException("result");
                if (query == null) throw new ArgumentNullException("query");
                _result = result;
                _query = query;
                _command = _query.CreateStoredProcedureCommand(result.StoredProcedureName);
                //_query.DiscoverParameters(_command);
                PrepareParameters(_command, _result, out _positionalParameters, out _namedParameters, out _variables);
                foreach (ParameterSlot parameterSlot in result.ParameterSlots)
                    if (!IsParameter(parameterSlot.Value))
                        parameterSlot.Parameter.Value = EvaluateParameterValue(parameterSlot.Value);
            }

            protected virtual bool IsParameter(string value)
            {
                return value.StartsWith("@");
            }

            protected void PrepareParameters(DbCommand command, Result result, out PositionalParameterCollection positionalParameters, out NamedParameterCollection namedParameters, out VariableCollection variables)
            {
                Dictionary<DbParameter, bool> usedList = new Dictionary<DbParameter, bool>();
                foreach (DbParameter parameter in command.Parameters)
                    usedList[parameter] = false;

                List<ParameterSlot> namedParameterSlots = new List<ParameterSlot>();
                List<ParameterSlot> positionalParameterSlots = new List<ParameterSlot>();

                List<DbParameter> positionalParameterList = new List<DbParameter>();
                Dictionary<string, DbParameter> namedParameterDict = new Dictionary<string, DbParameter>();
                Dictionary<string, DbParameter> variableDict = new Dictionary<string, DbParameter>();
                
                foreach (ParameterSlot parameterSlot in result.ParameterSlots)
                    if (parameterSlot.Type == SlotType.Named)
                        namedParameterSlots.Add(parameterSlot);
                    else
                        positionalParameterSlots.Add(parameterSlot);

                for (int ni = 0; ni < namedParameterSlots.Count; ni++)
                {
                    ParameterSlot slot = namedParameterSlots[ni];
                    if (command.Parameters.Contains(slot.Name))
                    {
                        DbParameter parameter = command.Parameters[slot.Name];
                        if (usedList[parameter])
                            throw new DatabaseException(String.Format("Parameter with name '{0}' is already specified", slot.Name));
                        slot.Parameter = parameter;
                        usedList[parameter] = true;
                        if (IsParameter(slot.Value))
                            variableDict[slot.Value] = parameter;
                    }
                    else
                    {
                        throw new DatabaseException(String.Format("Parameter with name '{0}' was not found in the stored procedure", slot.Name));
                    }
                }
                int i = 0;
                int j = 0;
                while (i < positionalParameterSlots.Count)
                {
                    DbParameter parameter = null;
                    while (true)
                    {
                        if (command.Parameters.Count <= j)
                            throw new DatabaseException(String.Format("More parameters are specified than the stored procedure has parameters"));
                        parameter = command.Parameters[j];
                        j++;
                        if (parameter.Direction == ParameterDirection.ReturnValue)
                            continue;
                        if (!usedList[parameter])
                            break;
                    }

                    ParameterSlot slot = positionalParameterSlots[i];
                    slot.Parameter = parameter;
                    usedList[parameter] = true;
                    positionalParameterList.Add(parameter);
                    
                    if (IsParameter(slot.Value))
                        variableDict[slot.Value] = parameter;

                    i++;
                }

                // Set up the parameters.
                foreach (DbParameter parameter in command.Parameters)
                    namedParameterDict[parameter.ParameterName] = parameter;

                positionalParameters = new PositionalParameterCollection(positionalParameterList);
                namedParameters = new NamedParameterCollection(namedParameterDict);
                variables = new VariableCollection(variableDict);
            }

            protected Result Result { get { return _result; } }
            protected QueryBase Query { get { return _query; } }
            /// <summary>
            /// The command that was created.
            /// </summary>
            public DbCommand Command { get { return _command; } }
            /// <summary>
            /// All of the positional parameters in the result.
            /// </summary>
            public PositionalParameterCollection PositionalParameters { get { return _positionalParameters; } }
            /// <summary>
            /// All of the parameters in the command, available by name.
            /// </summary>
            public NamedParameterCollection NamedParameters { get { return _namedParameters; } }
            /// <summary>
            /// All of the variables in the result.
            /// </summary>
            public VariableCollection Variables { get { return _variables; } }

            protected static object EvaluateParameterValue(string stringRepresentation)
            {
                if (stringRepresentation.StartsWith("'") && stringRepresentation.EndsWith("'"))
                {
                    return stringRepresentation.Substring(1, stringRepresentation.Length - 2).Replace("''", "'");
                }
                else if (Regex.IsMatch(stringRepresentation, @"^\d+$"))
                {
                    return Convert.ToInt64(stringRepresentation);
                }
                else if (Regex.IsMatch(stringRepresentation, @"^\d*\.\d+$"))
                {
                    return Convert.ToDouble(stringRepresentation);
                }                
                else if (stringRepresentation.ToUpper().Equals("NULL"))
                {
                    return DBNull.Value;
                }
                throw new DatabaseException(String.Format("Could not convert string to a value: '{0}'", stringRepresentation));
            }

            public class PositionalParameterCollection : ICollection<DbParameter>
            {
                private readonly List<DbParameter> _parameters;

                public PositionalParameterCollection(ICollection<DbParameter> parameters)
                {
                    _parameters = new List<DbParameter>(parameters);
                }

                public DbParameter this[int index]
                {
                    get { return _parameters[index]; }
                }

                public void Add(DbParameter item)
                {
                    throw new NotSupportedException();
                }

                public void Clear()
                {
                    throw new NotSupportedException();
                }

                public bool Contains(DbParameter item)
                {
                    return _parameters.Contains(item);
                }

                public void CopyTo(DbParameter[] array, int arrayIndex)
                {
                    _parameters.CopyTo(array, arrayIndex);
                }

                public int Count
                {
                    get { return _parameters.Count; }
                }

                public bool IsReadOnly
                {
                    get { return true; }
                }

                public bool Remove(DbParameter item)
                {
                    throw new NotSupportedException();
                }

                public IEnumerator<DbParameter> GetEnumerator()
                {
                    return _parameters.GetEnumerator();
                }

                System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
                {
                    return ((System.Collections.IEnumerable)_parameters).GetEnumerator();
                }
            }

            public class NamedParameterCollection : IDictionary<string, DbParameter>
            {
                private readonly Dictionary<string, DbParameter> _dictionary;

                public NamedParameterCollection(Dictionary<string, DbParameter> dictionary)
                {
                    if (dictionary == null) throw new ArgumentNullException("dictionary");

                    _dictionary = dictionary;
                }

                public void Add(string key, DbParameter value)
                {
                    throw new NotSupportedException();
                }

                public bool ContainsKey(string key)
                {
                    return _dictionary.ContainsKey(key);
                }

                public ICollection<string> Keys
                {
                    get { return _dictionary.Keys; }
                }

                public bool Remove(string key)
                {
                    throw new NotSupportedException();
                }

                public bool TryGetValue(string key, out DbParameter value)
                {
                    return _dictionary.TryGetValue(key, out value);
                }

                public ICollection<DbParameter> Values
                {
                    get { return _dictionary.Values; }
                }

                public DbParameter this[string key]
                {
                    get
                    {
                        return _dictionary[key];
                    }
                    set
                    {
                        throw new NotSupportedException();
                    }
                }

                public void Add(KeyValuePair<string, DbParameter> item)
                {
                    throw new NotSupportedException();
                }

                public void Clear()
                {
                    throw new NotSupportedException();
                }

                public bool Contains(KeyValuePair<string, DbParameter> item)
                {
                    return ((IDictionary<string, DbParameter>)_dictionary).Contains(item);
                }

                public void CopyTo(KeyValuePair<string, DbParameter>[] array, int arrayIndex)
                {
                    ((IDictionary<string, DbParameter>)_dictionary).CopyTo(array, arrayIndex);
                }

                public int Count
                {
                    get { return _dictionary.Count; }
                }

                public bool IsReadOnly
                {
                    get { return true; }
                }

                public bool Remove(KeyValuePair<string, DbParameter> item)
                {
                    throw new NotSupportedException();
                }

                public IEnumerator<KeyValuePair<string, DbParameter>> GetEnumerator()
                {
                    return _dictionary.GetEnumerator();
                }

                System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
                {
                    return ((System.Collections.IEnumerable)_dictionary).GetEnumerator();
                }
            }
           
            public class VariableCollection : IDictionary<string, DbParameter>
            {
                private readonly Dictionary<string, DbParameter> _dictionary;

                public VariableCollection(Dictionary<string, DbParameter> dictionary)
                {
                    if (dictionary == null) throw new ArgumentNullException("dictionary");

                    _dictionary = dictionary;
                }

                public void Add(string key, DbParameter value)
                {
                    throw new NotSupportedException();
                }

                public bool ContainsKey(string key)
                {
                    return _dictionary.ContainsKey(key);
                }

                public ICollection<string> Keys
                {
                    get { return _dictionary.Keys; }
                }

                public bool Remove(string key)
                {
                    throw new NotSupportedException();
                }

                public bool TryGetValue(string key, out DbParameter value)
                {
                    return _dictionary.TryGetValue(key, out value);
                }

                public ICollection<DbParameter> Values
                {
                    get { return _dictionary.Values; }
                }

                public DbParameter this[string key]
                {
                    get
                    {
                        return _dictionary[key];
                    }
                    set
                    {
                        throw new NotSupportedException();
                    }
                }

                public void Add(KeyValuePair<string, DbParameter> item)
                {
                    throw new NotSupportedException();
                }

                public void Clear()
                {
                    throw new NotSupportedException();
                }

                public bool Contains(KeyValuePair<string, DbParameter> item)
                {
                    return ((IDictionary<string, DbParameter>)_dictionary).Contains(item);
                }

                public void CopyTo(KeyValuePair<string, DbParameter>[] array, int arrayIndex)
                {
                    ((IDictionary<string, DbParameter>)_dictionary).CopyTo(array, arrayIndex);
                }

                public int Count
                {
                    get { return _dictionary.Count; }
                }

                public bool IsReadOnly
                {
                    get { return true; }
                }

                public bool Remove(KeyValuePair<string, DbParameter> item)
                {
                    throw new NotSupportedException();
                }

                public IEnumerator<KeyValuePair<string, DbParameter>> GetEnumerator()
                {
                    return _dictionary.GetEnumerator();
                }

                System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
                {
                    return ((System.Collections.IEnumerable)_dictionary).GetEnumerator();
                }
            }

            void IDisposable.Dispose()
            {
                _command.Dispose();
            }
        }
    }
}
