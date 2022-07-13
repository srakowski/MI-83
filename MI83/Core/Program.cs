namespace MI83.Core;

using System;
using System.Collections.Generic;
using System.Linq;
using static Instruction;
using static Token;

class Program
{
    private readonly string _code;
    private Instruction[] _instructions;
    private int _instructionPointer;
    private Dictionary<string, int> _labelIndexes;
    private Dictionary<string, TypedValue> _variables;
    private Stack<TypedValue> _stack;

    public Program(string code)
    {
        _code = code;
    }

    public void ExecuteNextInstruction(Computer computer)
    {
        if (_instructions is null)
        {
            _instructions = Compile(_code);
            _instructionPointer = 0;
            _labelIndexes = _instructions
                .Select((l, i) => (l is Label lbl ? new { lbl.LabelToken, LabelIndex = i } : null))
                .Where(v => v is not null)
                .ToDictionary(
                    k => k.LabelToken,
                    v => v.LabelIndex
                );

            _variables = new Dictionary<string, TypedValue>();
            _stack = new Stack<TypedValue>();
        }

        if (_instructionPointer >= _instructions.Length)
        {
            computer.Shutdown = true;
            return;
        }

        var nextInstruction = _instructions[_instructionPointer];
        _instructionPointer++;

        if (nextInstruction is Label)
        {
            ExecuteNextInstruction(computer);
        }
        else
        {
            Interpret(nextInstruction, computer);
        }
    }

    private static Instruction[] Compile(string code)
    {
        var instructions = Parser.Parse(code).ToArray();
        //foreach (var instruction in instructions)
        //{
        //    Console.WriteLine($"{instruction.GetType().Name} {(instruction is Label l ? l.LabelIdentifier : "")}");
        //}
        //Console.ReadLine();
        return instructions.ToArray();
    }

    private void Interpret(Instruction instruction, Computer computer)
    {
        //Console.WriteLine($"Executing instruction: {instruction.GetType().Name}...");
        switch (instruction)
        {
            case RunPrgm runPrgm:
                computer.RunPrgm(runPrgm.PrgmName);
                break;

            case ThrowSyntaxError:
                computer.ExitWithSyntaxError();
                break;

            case ExitPrgm:
                computer.ExitPrgm();
                break;

            case EvaluateIdentifierAndPush i1:
                if (_variables.ContainsKey(i1.Value))
                {
                    _stack.Push(_variables[i1.Value]);
                }
                else if (computer.HasSysCommand(i1.Value))
                {
                    _stack.Push(computer.ExecuteSysCommand(i1.Value, null));
                }
                else if (_labelIndexes.ContainsKey(i1.Value))
                {
                    _stack.Push(new LabelValue(i1.Value));
                }
                else
                {
                    computer.ExitWithRuntimeError();
                }
                break;

            case EvaluateIdentifierCallAndPush i2:
                var poppedCallData = _stack.Pop();
                if (poppedCallData is not ListValue)
                {
                    computer.ExitWithRuntimeError();
                    return;
                }
                var callData = poppedCallData as ListValue;

                if (_variables.ContainsKey(i2.Value))
                {                    
                    var varValue = _variables[i2.Value];
                    if (varValue is ListValue listValue &&
                        callData.Values.SingleOrDefault() is NumericValue idx)
                    {
                        var idxValue = (int)idx.Value;
                        if (idxValue < 0 || idxValue >= listValue.Values.Count)
                        {
                            computer.ExitWithRuntimeError();
                            return;
                        }

                        _stack.Push(listValue.Values[idxValue]);
                    }
                    else
                    {
                        computer.ExitWithRuntimeError();
                    }
                }
                else if (computer.HasSysCommand(i2.Value))
                {
                    _stack.Push(computer.ExecuteSysCommand(i2.Value, callData));
                }
                else
                {
                    computer.ExitWithRuntimeError();
                }
                break;

            case PushStringLiteral sl:
                _stack.Push(new StringValue(sl.Value));
                break;

            case PushNumericLiteral nl:
                _stack.Push(new NumericValue(decimal.Parse(nl.Value)));
                break;

            case PopAssignPush pap:
                var dataToAssign = _stack.Peek();
                _variables[pap.VariableName] = dataToAssign;
                break;

            case PopAndGoto:
                var poppedLabel = _stack.Pop();
                if (poppedLabel is not LabelValue)
                {
                    computer.ExitWithRuntimeError();
                    return;
                }
                var labelToken = (poppedLabel as LabelValue).Value;
                if (!_labelIndexes.ContainsKey(labelToken))
                {
                    computer.ExitWithRuntimeError();
                    return;
                }
                _instructionPointer = _labelIndexes[labelToken];
                break;

            case BeginList:
                _stack.Push(new ListValue(new List<TypedValue>()));
                break;

            case PopAndAppendList:
                var valueToAdd = _stack.Pop();
                var list = _stack.Pop() as ListValue;
                list.Values.Add(valueToAdd);
                _stack.Push(list);
                break;

            case EndListAndPush:
                // list  is already on stack, do nothing.
                break;
        }
    }

    public abstract record TypedValue;
    public record LabelValue(string Value) : TypedValue;
    public record ListValue(List<TypedValue> Values) : TypedValue;
    public record StringValue(string Value) : TypedValue;
    public record NumericValue(decimal Value) : TypedValue;
}

abstract record Token
{
    public string Value { get; set; }
    public int LineNumber { get; set; }
    public int LinePosition { get; set; }

    public record None : Token;
    public record EndOfFile : Token;
    public record InvalidToken : Token;

    public record Colon : Token;
    public record Identifier : Token;
    public record Arrow : Token;    
    public record LeftCurlyBrace : Token;
    public record RightCurlyBrace : Token;
    public record Comma : Token;
    public record LeftParen : Token;
    public record RightParen : Token;
    public record StringLiteral : Token;
    public record NumericLiteral : Token;

    public record Lbl : Token;
    public record Goto : Token;
}

internal static class Lexer
{
    private struct CodeCharacter
    {
        public CodeCharacter(char value, int lineNumber, int linePosition)
        {
            Value = value;
            LineNumber = lineNumber;
            LinePosition = linePosition;
        }

        public char Value { get; }

        public int LineNumber { get; }

        public int LinePosition { get; }

        public bool IsWhitespace => char.IsWhiteSpace(Value);

        public bool IsDigit => Value >= '0' && Value <= '9';

        public bool IsStartOfIdentifier => Value == '_' || (Value >= 'a' && Value <= 'z') || (Value >= 'A' && Value <= 'Z');

        public bool IsBodyOfIdentifier => IsStartOfIdentifier || IsDigit;

        public bool IsStartOfStringLiteral => Value == '\"';

        public bool IsStartOfArrow => Value == '-';
    }

    public static IEnumerable<Token> Analyze(string code) =>
        CodeCharacters(code)
            .Tokenize();

    private static IEnumerable<CodeCharacter> CodeCharacters(string code) =>
        code
            .Split('\n')
            .SelectMany((line, lineIndex) =>
                line
                    .ToCharArray()
                    .Select((character, linePositionIndex) =>
                        new CodeCharacter(character, lineIndex + 1, linePositionIndex + 1)
                    )
            );

    private static IEnumerable<Token> Tokenize(this IEnumerable<CodeCharacter> code)
    {
        var nextToken = ReadToken(code);
        while (!(nextToken.Value is EndOfFile || nextToken.Value is InvalidToken))
        {
            yield return nextToken.Value;
            nextToken = nextToken.Next();
        }
        yield return nextToken.Value;
    }

    private struct NextToken
    {
        public NextToken(Token token, Func<NextToken> next)
        {
            Value = token;
            Next = next;
        }
        public Token Value { get; }
        public Func<NextToken> Next { get; }
    }

    private static NextToken ReadToken(this IEnumerable<CodeCharacter> code)
    {
        code = code.SkipWhile(c => c.IsWhitespace);

        if (!code.Any())
        {
            var token = new EndOfFile();
            return new NextToken(token, () => ReadToken(code));
        }

        var firstTokenChar = code.First();

        if (firstTokenChar.IsStartOfIdentifier)
        {
            var value = code
                .TakeWhile(c => c.IsBodyOfIdentifier)
                .Aggregate("", (s, c) => s + c.Value);

            code = code.Skip(value.Length);

            var token = KeywordTable.Contains(value)
                ? CreateKeywordToken(value)(code, firstTokenChar)
                : CreateToken<Identifier>(code, firstTokenChar, value);

            return token;
        }

        if (firstTokenChar.IsDigit)
        {
            var value = code
                .TakeWhile(c => c.IsDigit)
                .Aggregate("", (s, c) => s + c.Value);

            code = code.Skip(value.Length);
            return CreateToken<NumericLiteral>(code, firstTokenChar, value);
        }

        if (firstTokenChar.IsStartOfStringLiteral)
        {
            var value = code
                .Skip(1)
                .TakeWhile(c => c.Value != '"')
                .Aggregate("\"", (s, c) => s + c.Value)
                + '"';

            code = code.Skip(value.Length);
            return CreateToken<StringLiteral>(code, firstTokenChar, value);
        }

        if (firstTokenChar.IsStartOfArrow)
        {
            var value = code.Take(2)
                .Aggregate("", (s, c) => s + c.Value);

            if (value == "->")
            {
                code = code.Skip(value.Length);
                return CreateToken<Arrow>(code, firstTokenChar, value);
            }
        }

        var t = new InvalidToken() as Token;

        switch (firstTokenChar.Value)
        {
            case ':': t = new Colon(); break;
            case '(': t = new LeftParen(); break;
            case ')': t = new RightParen(); break;
            case '{': t = new LeftCurlyBrace(); break;
            case '}': t = new RightCurlyBrace(); break;
            case ',': t = new Comma(); break;
        }

        code = code.Skip(1);

        t.Value = firstTokenChar.Value.ToString();
        t.LineNumber = firstTokenChar.LineNumber;
        t.LinePosition = firstTokenChar.LinePosition;
        return new NextToken(t, () => ReadToken(code));
    }

    private readonly static HashSet<string> KeywordTable = new()
    {
        "Lbl",
        "Goto"
    };

    private static Func<IEnumerable<CodeCharacter>, CodeCharacter, NextToken> CreateKeywordToken(string keyword) =>
        (code, firstTokenChar) =>
            keyword switch
            { 
                "Lbl" => CreateToken<Lbl>(code, firstTokenChar, keyword),
                "Goto" => CreateToken<Goto>(code, firstTokenChar, keyword),
                _ => CreateToken<InvalidToken>(code, firstTokenChar, keyword)
            };

    private static NextToken CreateToken<T>(IEnumerable<CodeCharacter> code, CodeCharacter firstTokenChar, string value) where T : Token, new()
    {
        var token = new T
        {
            Value = value,
            LineNumber = firstTokenChar.LineNumber,
            LinePosition = firstTokenChar.LinePosition
        };

        return new NextToken(token, () => ReadToken(code));
    }
}

internal static class Parser
{
    public static IEnumerable<Instruction> Parse(string code)
    {
        var tokens = new Queue<Token>(Lexer.Analyze(code));
        while (tokens.Count > 0)
        {
            var t = tokens.Peek();
            if (t is Colon)
            {
                tokens.Dequeue();
                continue;
            }

            if (t is Lbl)
            {
                tokens.Dequeue();
                t = tokens.Dequeue();
                if (t is Identifier lblIdentifier)
                {
                    yield return new Label(lblIdentifier.Value);
                }
                else
                {
                    yield return new ThrowSyntaxError();
                    break;
                }

                t = tokens.Dequeue();
                if (t is not Colon or EndOfFile)
                {
                    yield return new ThrowSyntaxError();
                    break;
                }

                continue;
            }

            if (t is Goto)
            {
                tokens.Dequeue();
                foreach (var instruction in ParseExpression(tokens))
                {
                    yield return instruction;
                    if (instruction is ThrowSyntaxError)
                    {
                        yield break;
                    }
                }
                yield return new PopAndGoto();
                continue;
            }

            if (t is Arrow)
            {
                tokens.Dequeue();
                t = tokens.Dequeue();
                if (t is Identifier assignIdentifier)
                {
                    yield return new PopAssignPush(assignIdentifier.Value);
                }
                else
                {
                    yield return new ThrowSyntaxError();
                    break;
                }

                t = tokens.Dequeue();
                if (t is not Colon or EndOfFile)
                {
                    yield return new ThrowSyntaxError();
                    break;
                }

                continue;
            }

            if (t is EndOfFile)
            {
                tokens.Dequeue();
                break;
            }

            foreach (var instruction in ParseExpression(tokens))
            {
                yield return instruction;
                if (instruction is ThrowSyntaxError)
                {
                    yield break;
                }
            }
        }
    }

    private static IEnumerable<Instruction> ParseExpression(Queue<Token> tokens)
    {
        var t = tokens.Dequeue();
        if (t is Identifier identifier)
        {
            t = tokens.Peek();
            if (t is LeftCurlyBrace)
            {
                tokens.Dequeue();
                foreach (var instruction in ParseList(tokens))
                {
                    yield return instruction;
                    if (instruction is ThrowSyntaxError)
                    {
                        yield break;
                    }
                }

                yield return new EvaluateIdentifierCallAndPush(identifier.Value);
            }
            else
            {
                yield return new EvaluateIdentifierAndPush(identifier.Value);
            }            
        }
        else if (t is StringLiteral stringLiteral)
        {
            yield return new PushStringLiteral(stringLiteral.Value);
        }
        else if (t is NumericLiteral numericLiteral)
        {
            yield return new PushNumericLiteral(numericLiteral.Value);
        }
        else if (t is LeftCurlyBrace)
        {
            foreach (var instruction in ParseList(tokens))
            {
                yield return instruction;
                if (instruction is ThrowSyntaxError)
                {
                    yield break;
                }
            }
        }
        else
        {
            yield return new ThrowSyntaxError();
            yield break;
        }

        t = tokens.Peek();
        if (t is Arrow)
        {
            tokens.Dequeue();
            t = tokens.Dequeue();
            if (t is Identifier assignIdentifier)
            {
                yield return new PopAssignPush(assignIdentifier.Value);
            }
            else
            {
                yield return new ThrowSyntaxError();
                yield break;
            }

            t = tokens.Dequeue();
            if (t is not Colon or EndOfFile)
            {
                yield return new ThrowSyntaxError();
                yield break;
            }
        }
        else if (t is Comma or RightCurlyBrace or Colon or EndOfFile)
        {
            yield break;
        }
        else
        {
            yield return new ThrowSyntaxError();
            yield break;
        }
    }

    private static IEnumerable<Instruction> ParseList(Queue<Token> tokens)
    {
        yield return new BeginList();
        while (true)
        {
            if (tokens.Peek() is RightCurlyBrace or Colon)
            {
                tokens.Dequeue();
                break;
            }

            foreach (var instruction in ParseExpression(tokens))
            {
                yield return instruction;
                if (instruction is ThrowSyntaxError)
                {
                    yield break;
                }
            }

            yield return new PopAndAppendList();

            var t = tokens.Peek();
            if (t is RightCurlyBrace)
            {
                tokens.Dequeue();
                break;
            }
            else if (t is Colon or EndOfFile)
            {
                break;
            }
            else if (t is Comma)
            {
                tokens.Dequeue();
            }
            else
            {
                yield return new ThrowSyntaxError();
                yield break;
            }
        }
        yield return new EndListAndPush();
    }
}