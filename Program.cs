using System;
using static System.Diagnostics.Debug;
using System.Text.RegularExpressions;

namespace LearningRegex
{
    class Program
    {
        static void Main(string[] args)
        {
            //a word that starts with letter "M"
            /*
                \b = The match must occur on a boundary between a \w (alphanumeric) and a \W (nonalphanumeric) character.
                [M] = Matches any single character in character_group. By default, the match is case-sensitive. [ae] gets: "a" in "gray" and "a", "e" in "lane"
                \w = Matches any word character
                + = Matches the previous element one or more times
            */
            var rg = new Regex(@"\b[M]\w+");
            var authors = "Mahesh Chand, Raj Kumar, Mike Gold, Allen O'Neill, Marshal Troll";
            MatchCollection matchedAuthors = rg.Matches(authors);

            Assert(matchedAuthors.Count == 3);
            Assert(matchedAuthors[0].Value == "Mahesh");
            Assert(matchedAuthors[1].Value == "Mike");
            Assert(matchedAuthors[2].Value == "Marshal");

            var text = "Here is a string with ton of white space";
            // \s = any whitespace character
            var replaced = Regex.Replace(text, @"\s+", "-");
            Assert(replaced == "Here-is-a-string-with-ton-of-white-space");

            var results = Regex.Split("aB19", "[a-z]", RegexOptions.IgnoreCase, matchTimeout: TimeSpan.FromSeconds(5));
            Assert(results.Length == 3);
            Assert(results[0] == "");
            Assert(results[1] == "");
            Assert(results[2] == "19");

            Quantifiers();
            SpecialCharacters();

            System.Console.WriteLine("All asserts have run");
        }

        private static void SpecialCharacters()
        {
            // ^ = the match must start at the beginning of the string
            var rg = new Regex("^My");
            Assert(rg.IsMatch("My dog Ada"));

            // $ = the match must occur at the end of the string or before \n at the end of the string
            rg = new Regex("da$");
            Assert(rg.IsMatch("My dog Ada"));

            // . = matches any character only once
            rg = new Regex("m.p");
            Assert(rg.IsMatch("map"));
            Assert(rg.IsMatch("mop"));
            Assert(rg.IsMatch("smopz"));
            Assert(rg.IsMatch("moap") == false);

            // \d = matches any digit characters
            rg = new Regex(@"\d");
            Assert(rg.IsMatch("asdfas4ss"));
            rg = new Regex("[0-9]"); //this is the same
            Assert(rg.IsMatch("asdfas4ss"));

            // \D = matches any non-digit
            rg = new Regex(@"\D");
            Assert(rg.IsMatch("1234123.234"));
            Assert(rg.Matches("1234123.234").Count == 1);
            Assert(rg.Matches("1234123.234")[0].Value == ".");
            Assert(rg.IsMatch("£123"));
            Assert(rg.Matches("£123").Count == 1);

            // \w = matches any alphanumeric character plus _
            rg = new Regex(@"\w");
            Assert(rg.IsMatch("1"));
            Assert(rg.IsMatch("W"));
            Assert(rg.IsMatch("_"));
            Assert(rg.IsMatch("& ") == false);

            // \W = matches anything that \w wont
            rg = new Regex(@"\W");
            Assert(rg.IsMatch("&"));
            Assert(rg.IsMatch("^a "));
            Assert(rg.IsMatch("..."));
            Assert(rg.Matches("ID B1.5").Count == 2);

            // \s = matches any whitespace character
            rg = new Regex(@"\s");
            Assert(rg.IsMatch(" "));

            // \S = matches non-whitespace
            rg = new Regex(@"\S\s");
            Assert(rg.Match("Erik Sharp").Value == "k ");

            // [] = matches any one character in the group
            rg = new Regex("[Ekr]");
            Assert(rg.Matches("Erik Sharp").Count == 4);
            rg = new Regex(@"[a-d]\d");
            Assert(rg.IsMatch("c9"));
            rg = new Regex(@"[^a-d]\d"); //negation
            Assert(rg.IsMatch("e4"));

        }

        private static void Quantifiers()
        {
            // zero or more
            var rg = new Regex("a*b");
            Assert(rg.IsMatch("b"));
            Assert(rg.IsMatch("ab"));
            Assert(rg.IsMatch("aab"));

            // one or more
            rg = new Regex("a+b");
            Assert(rg.IsMatch("b") == false);
            Assert(rg.IsMatch("ab"));
            Assert(rg.IsMatch("aab"));

            // zero or one
            rg = new Regex("a?b");
            Assert(rg.IsMatch("b"));
            Assert(rg.IsMatch("ab"));
            Assert(rg.IsMatch("aab"));
        }
    }
}
