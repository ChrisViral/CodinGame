using System;
using System.Text;
using System.Collections.Generic;

/// <summary>
/// Solution class
/// </summary>
public class Solution
{
    #region Constants
    /// <summary>
    /// The CGX string delimiter
    /// </summary>
    public const char strDelim = '\'';
    /// <summary>
    /// Element separator character
    /// </summary>
    public const char separator = ';';
    /// <summary>
    /// Block start character
    /// </summary>
    public const char blockStart = '(';
    /// <summary>
    /// Block end character
    /// </summary>
    public const char blockEnd = ')';
    /// <summary>
    /// Key/Value separator
    /// </summary>
    public const char keyValueSep = '=';
    /// <summary>
    /// CGX ident (four spaces)
    /// </summary>
    public const string ident = "    ";
    #endregion

    #region Classes
    /// <summary>
    /// CGX Element interface
    /// </summary>
    public interface IElement
    {
        /// <summary>
        /// Prints the CGX element correctly
        /// </summary>
        /// <returns>The correctly printed element</returns>
        string Print();
    }

    /// <summary>
    /// Block element
    /// </summary>
    public class Block : IElement
    {
        #region Fields
        /// <summary>
        /// Child elements of the Block
        /// </summary>
        public readonly List<IElement> elements = null;
        /// <summary>
        /// Offset of the Block
        /// </summary>
        private readonly string _offset = string.Empty;
        #endregion

        #region Properties
        /// <summary>
        /// Offset passed to children objects
        /// </summary>
        protected virtual string offset
        {
            get { return this._offset + ident; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new Block
        /// </summary>
        /// <param name="body">String of the element of this block</param>
        /// <param name="offset">Ident offset of the block</param>
        public Block(string body, string offset)
        {
            this._offset = offset;
            this.elements = new List<IElement>(Parse(body));
        }
        #endregion

        #region Methods
        /// <summary>
        /// Parses the whole block body code into a list of CGX element
        /// </summary>
        /// <param name="body">The blocks body code</param>
        /// <returns>Enumerable of all the elements</returns>
        private IEnumerable<IElement> Parse(string body)
        {
            //If empty, no child elements
            if (body.Length == 0) { yield break; }

            bool isStr = false;         //String flag
            int ident = 0, index = 0;   //Ident depth, index of last parsed element
            for (int i = 0; i < body.Length; i++)
            {
                switch(body[i])
                {
                    //Increment ident depth if not in string
                    case blockStart:
                        if (!isStr) { ident++; } break;

                    //Decrement ident depth if not in string
                    case blockEnd:
                        if (!isStr) { ident--; } break;

                    //Toggle string flag
                    case strDelim:
                        isStr = !isStr; break;

                    //Parse new element if ident depth is zero and not in string
                    case separator:
                        {
                            if (ident == 0 && !isStr)
                            {
                                yield return GetElement(body.Sub(index, i - 1));
                                index = i + 1;
                            }
                            break;
                        }
                }
            }

            //Parse final element (no separator at the end)
            yield return GetElement(body.Substring(index));
        }
        
        /// <summary>
        /// Creates a new correct IElement of the given CGX code
        /// </summary>
        /// <param name="element">String of the element</param>
        /// <returns>The new IElement created</returns>
        private IElement GetElement(string element)
        {
            element = element.Trim();   //Trim all trailling and leading white spaces
            switch(element[0])
            {
                //If the first character is a block start, create new block
                case blockStart:
                    return new Block(element.Sub(1, element.Length - 2).Trim(), offset);
                
                //If the first element is a string delimiter, verify if next string delimiter is followed by key/value pair separator
                case strDelim:
                    {
                        for (int i = 1; i < element.Length - 1; i++)
                        {
                            if (element[i] == strDelim)
                            {
                                //If following element is a Key/Value pair separator, create new KeyValue
                                int j = i + 1;
                                while (j < element.Length && element[j] == ' ') { j++; }
                                if (element[j] == keyValueSep)
                                {
                                    return new KeyValue(element, offset, j);
                                }
                                break;
                            }
                        }
                        break;
                    }
            }

            //If not, it's a Primitive
            return new Primitive(element, offset);
        }
        
        /// <summary>
        /// Prints this block and all it's elements
        /// </summary>
        /// <returns>String representation of the block</returns>
        public virtual string Print()
        {
            //Append block offset and starting character
            StringBuilder b = new StringBuilder(this._offset).AppendLine(blockStart);
            if (elements.Count > 0)
            {
                //Append first element
                b.Append(elements[0].Print());
                for (int i = 1; i < elements.Count; i++)
                {
                    //Append element separator and next element
                    b.AppendLine(separator).Append(elements[i].Print());
                }
                //Linebreak
                b.AppendLine();
            }
            //Append ending character
            return b.Append(this._offset).Append(blockEnd).ToString();
        }
        #endregion
    }

    /// <summary>
    /// Global block containing all the elements
    /// </summary>
    public class Global : Block
    {
        #region Fields
        /// <summary>
        /// Overrides the Block passed offset, Global has none
        /// </summary>
        protected override string offset
        {
            get { return string.Empty; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new Global block
        /// </summary>
        /// <param name="body">Complete CGX body code</param>
        public Global(string body) : base(body, string.Empty) { }
        #endregion

        #region Methods
        /// <summary>
        /// Prints all the elements in the Global block
        /// </summary>
        /// <returns>The whole body of CGX code correctly formatted</returns>
        public override string Print()
        {
            //Initiates the builder
            StringBuilder b = new StringBuilder();
            if (elements.Count > 0)
            {
                //Appends the first element
                b.Append(elements[0].Print());
                for (int i = 1; i < elements.Count; i++)
                {
                    //Appends the following elements  with the separator
                    b.AppendLine(separator).Append(elements[i].Print());
                }
            }
            return b.ToString();
        }
        #endregion
    }

    /// <summary>
    /// Key/Value pair element
    /// </summary>
    public class KeyValue : IElement
    {
        #region Fields
        /// <summary>
        /// The string Key
        /// </summary>
        public string key = string.Empty;
        /// <summary>
        /// The value (Primitive or Block)
        /// </summary>
        public IElement value = null;
        /// <summary>
        /// Ident offset of the Key/Value pair
        /// </summary>
        private readonly string offset = string.Empty;
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new KeyValue pair
        /// </summary>
        /// <param name="body">CGX body code of the pair</param>
        /// <param name="offset">Ident offset of the pair</param>
        /// <param name="sepIndex">Index of the separator character</param>
        public KeyValue(string body, string offset, int sepIndex)
        {
            this.offset = offset;
            this.key = body.Sub(0, sepIndex - 1).Trim();    //Sets string key
            int i = sepIndex + 1;
            while (body[i] == ' ') { i++; }     //Skips white spaces before value
            //If the first character of the value is a block start, create a new Block
            if (body[i] == blockStart)
            {
                this.value = new Block(body.Sub(i + 1, body.Length - 2).Trim(), offset);
            }
            //Else it's a Primitive
            else { value = new Primitive(body.Substring(i).Trim(), string.Empty); }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Prints the Key/Value pair correctly
        /// </summary>
        /// <returns>Correctly idented string representation</returns>
        public string Print()
        {
            //If value is a block, insert a linebreak
            return offset + key + keyValueSep + ((value is Block) ? "\n" + value.Print() : value.Print()); 
        }
        #endregion
    }

    /// <summary>
    /// Primitive type element
    /// </summary>
    public class Primitive : IElement
    {
        #region Fields
        /// <summary>
        /// String value of the primitive
        /// </summary>
        public string value = string.Empty;
        /// <summary>
        /// Ident offset of the primitive
        /// </summary>
        private readonly string offset = string.Empty;
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new Primitive from the given value
        /// </summary>
        /// <param name="value">String value of the primitive</param>
        /// <param name="offset">Ident offset of the primitive</param>
        public Primitive(string value, string offset)
        {
            this.value = value;
            this.offset = offset;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Prints the Primitive correctly idented
        /// </summary>
        /// <returns>The primitive value idented</returns>
        public string Print()
        {
            return offset + value;
        }
        #endregion
    }
    #endregion

    #region Main
    /// <summary>
    /// Program
    /// </summary>
    private static void Main()
    {
        int N = int.Parse(Console.ReadLine());  //Number of lines
        //If there is no text
        if (N == 0) { Console.WriteLine(string.Empty); return; }
        //First line, creating builder
        StringBuilder b = new StringBuilder(Console.ReadLine());
        for (int i = 1; i < N; i++)
        {
            //Appending following lines
            b.Append(Console.ReadLine());
        }
        string code = b.ToString().Trim();  //Resulting CGX code line

        Global global = new Global(code);   //Creating the global environment
        Console.WriteLine(global.Print());  //Printing the formatted CGX
    }
    #endregion
}

/// <summary>
/// String extensions
/// </summary>
public static class Extensions
{
    #region Methods
    /// <summary>
    /// Appends a given character followed by the linebreak
    /// </summary>
    /// <param name="builder">StringBuilder instance</param>
    /// <param name="value">Char value to append</param>
    /// <returns>The resulting StringBuilder instance</returns>
    public static StringBuilder AppendLine(this StringBuilder builder, char value)
    {
        return builder.Append(value).AppendLine();
    }

    /// <summary>
    /// A substring method that takes a starting index and ending index
    /// </summary>
    /// <param name="str">String instance</param>
    /// <param name="startIndex">Starting index</param>
    /// <param name="endIndex">Ending index</param>
    /// <returns>The cut string</returns>
    public static string Sub(this string str, int startIndex, int endIndex)
    {
        return str.Substring(startIndex, endIndex - startIndex + 1);
    }
    #endregion
}