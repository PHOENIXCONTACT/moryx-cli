using Moryx.Cli.Template.Exceptions;
using Moryx.Cli.Template.Extensions;
using System.Text.RegularExpressions;

namespace Moryx.Cli.Template.StateBaseTemplate
{
    public partial class StateBaseTemplate
    {
        private readonly string _content;

        public StateBaseTemplate(string content)
        {
            _content = content;
            Parse(content);
        }

        public IEnumerable<Constructor> Constructors { get; private set; } = new List<Constructor>();
        public IEnumerable<StateDefinition> StateDefinitions { get; private set; } = new List<StateDefinition>();
        public string Content { get => _content;  }

        public static StateBaseTemplate FromFile(string fileName)
        {
            var content = File.ReadAllText(fileName);
            return new StateBaseTemplate(content);
        }

        private void Parse(string content)
        {
            var ctors = new List<Constructor>();
            var states = new Dictionary<int, string>();

            content.Split(Environment.NewLine).Each((line, i) =>
            {

                if (Regex.Match(line, @"public\s+\w+\(").Success)
                {
                    ctors.Add(new Constructor { Line = i + 1 });
                } 
                else {
                    var match = Regex.Match(line, @"\[\s*StateDefinition\s*\(\s*typeof\s*\(\s*(\w+)");
                    if (match.Success)
                    {
                        states.Add(i, match.Groups[1].Value);
                    }
                }
            });


            StateDefinitions = ExtractStateDefinitions(states, content);
            Constructors = ctors;
        }

        private IEnumerable<StateDefinition> ExtractStateDefinitions(Dictionary<int, string> states, string content)
        {
            var result = new List<StateDefinition>();
            var minifiedContent = content.Replace(Environment.NewLine, "");

            foreach (var state in states)
            {
                var start = minifiedContent.IndexOf($"[StateDefinition(typeof({state.Value}");
                var end = minifiedContent.IndexOf(";", start);
                var s = minifiedContent.Substring(start, end - start +1);
                var match = Regex.Match(s, @"(\w+)\s*=\s*(\d+)\s*");
                if (match.Success) {
                    result.Add(new StateDefinition
                    {
                        Name = match.Groups[1].Value,
                        Type = state.Value,
                        Value = Convert.ToInt32(match.Groups[2].Value),
                        IsInitial = Regex.Match(s, @"IsInitial\s*=\s*true").Success,
                        Line = state.Key + 1,
                    });
                }
            }
            return result;
        }

        public StateBaseTemplate AddState(string stateType)
        {
            if(StateDefinitions.Any(sd => sd.Type == stateType))
            {
                throw new StateAlreadyExistsException(stateType);
            }

            var lines = _content.Split(Environment.NewLine);
            var newLines = new List<string>();
            var ctorIndex = Constructors.First().Line - 1;
            newLines.AddRange(lines.Take(ctorIndex));
            var initial = StateDefinitions.Any(sd => sd.IsInitial)
                ? ""
                : ", IsInitial = true";
            int value = NextConst(StateDefinitions);

            newLines.Add($"        [StateDefinition(typeof({stateType}){initial})]");
            newLines.Add($"        protected const int {TypeToConst(stateType)} = {value};");
            newLines.Add("");

            newLines.AddRange(lines.TakeLast(lines.Count() - ctorIndex));

            return new StateBaseTemplate(string.Join(Environment.NewLine, newLines));
        }

        private int NextConst(IEnumerable<StateDefinition> stateDefinitions)
        {
            var result = StateDefinitions
                .OrderByDescending(sd => sd.Value)
                .FirstOrDefault()?
                .Value ?? 0;

            return result - (result % 10) + 10;
        }

        private string TypeToConst(string type)
            => "State" + type.Replace("State", "");

        public void SaveToFile(string filename)
        {
            File.WriteAllText(filename, Content);
        }
    }
}