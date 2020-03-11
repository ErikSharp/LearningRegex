using System;
using static System.Diagnostics.Debug;
using System.Text.RegularExpressions;
using System.Linq;

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
            Examples();

            System.Console.WriteLine("All asserts have run");
        }

        private static void Examples()
        {
            var ukPostCodes = new Regex(@"[A-Z]{1,2}\d{1,2}\s\d[A-Z]{2}");
            Assert(ukPostCodes.IsMatch("RG22 5BY"));
            Assert(ukPostCodes.IsMatch("E14 9UY"));

            var reference = new Regex("^{.*}$");
            Assert(reference.IsMatch("{MyBid}"));

            var refMulInt = new Regex(@"^{[A-Za-z]\w*\s*\*\s*\d+}$");
            Assert(refMulInt.IsMatch("{MyBid * 55}"));
            Assert(refMulInt.IsMatch("{MyBid_5*55}"));

            var refMulDec = new Regex(@"^{(?<Ref>[A-Za-z]\w*)\s*\*\s*(?<Dec>\d+\.\d{1,3})}$");
            Assert(refMulDec.IsMatch("{MyBid * 55.555}"));
            Assert(refMulDec.IsMatch("{MyBid_5*55.5}"));
            Assert(refMulDec.IsMatch("{MyBid_5*55.}") == false);
            var matches = refMulDec.Matches("{MyBid_5*55.5}");
            if (matches.Any())
            {
                Assert(matches[0].Groups["Ref"].Value == "MyBid_5");
                Assert(matches[0].Groups["Dec"].Value == "55.5");
            }
            else
            {
                Assert(false);
            }
            matches = refMulDec.Matches("{3MyBid_5*55.5}");
            Assert(matches.Count == 0);

            var randomDec = new Regex(@"^{(?<middle>\d+\.\d{1,3})\.(?<operation>(random|step|chicken))\((?<deviate>\d+)\)}");
            matches = randomDec.Matches("{94.123.random(13)}");
            if (matches.Any())
            {
                Assert(matches[0].Groups["middle"].Value == "94.123");
                Assert(matches[0].Groups["operation"].Value == "random");
                Assert(matches[0].Groups["deviate"].Value == "13");
            }
            else
            {
                Assert(false);
            }
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
            rg = new Regex("[A-Za-z]+");
            Assert(rg.Matches("Asbe34c").Count == 2);
            rg = new Regex(@"[a-d]\d");
            Assert(rg.IsMatch("c9"));
            rg = new Regex(@"[^a-d]{3}\d{2}"); //negation
            Assert(rg.IsMatch("est44"));

            // () = Alternation
            rg = new Regex("th(e|is|at)");
            Assert(rg.IsMatch("the"));
            Assert(rg.IsMatch("this"));
            Assert(rg.IsMatch("that"));

            // {2} = matches 2 times
            rg = new Regex(@"\d{5}");
            Assert(rg.IsMatch("92104"));

            // {2,} = matches at least 2 times, but as few times possible
            rg = new Regex(@"\d{3,}");
            Assert(rg.Matches("92104").Count == 1);
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
