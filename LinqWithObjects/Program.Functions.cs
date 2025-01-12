partial class Program
{
private static void DeferredExecution(string[] names)
{
SectionTitle("Deferred execution");
// Question: Which names end with an M?
// (using a LINQ extension method)
var query1 = names.Where(name => name.EndsWith("m"));
// Question: Which names end with an M?
// (using LINQ query comprehension syntax)
var query2 = from name in names where name.EndsWith("m") select name;
// Answer returned as an array of strings containing Pam and Jim.
string[] result1 = query1.ToArray();

// Answer returned as a list of strings containing Pam and Jim.
List<string> result2 = query2.ToList();
// Answer returned as we enumerate over the results.
foreach (string name in query1)
{
WriteLine(name); // outputs Pam
names[2] = "Jimmy"; // Change Jim to Jimmy.
// On the second iteration Jimmy does not
// end with an "m" so it does not get output.
}
}
static bool NameLongerThanFour(string name)
{
// Returns true for a name longer than four characters.
return name.Length > 4;
}
private static void FilteringUsingWhere(string[] names)
{
   
//     var query = names.Where(
// new Func<string, bool>(NameLongerThanFour));
IOrderedEnumerable<string> query = names.
Where(name => name.Length > 4)
.OrderBy(name => name.Length)
.ThenBy(name => name);
foreach (string item in query)
{
WriteLine(item);
}
}
}

