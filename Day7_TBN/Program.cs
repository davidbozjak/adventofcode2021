using System.Text.RegularExpressions;

Regex numRegex = new(@"\d+");
Regex hexColorRegex = new(@"#[0-9a-z][0-9a-z][0-9a-z][0-9a-z][0-9a-z][0-9a-z]");

var parser = new SingleLineStringInputParser<int>(int.TryParse, str => str.ToCharArray().Select(w => w.ToString()).ToArray());
var input = new InputProvider<int>("Input.txt", parser.GetValue).ToList();

var wholeStringInput = new InputProvider<string>("Input.txt", GetString).ToList();

static bool GetString(string? input, out string? value)
{
    value = null;

    if (input == null) return false;

    value = input ?? string.Empty;

    return true;
}