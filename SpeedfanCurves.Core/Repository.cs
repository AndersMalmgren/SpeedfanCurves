using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SpeedfanCurves.Core.Extensions;
using SpeedfanCurves.Core.Model;

namespace SpeedfanCurves.Core
{
    public class Repository : IRepository
    {
        private static Regex fansRegex = new Regex(
              @"xxx\ Pwm\ (?<id>.*)\r\n.*name=(?<name>.*)\r\n.*active=(?<active>.*)\r\n.*min=(?<min>.*)\r\n.*max=(?<max>.*)\r\n.*variate=(?<variate>.*)\r\n.*logged=(?<logged>.*)\r\n.*xxx\ end",
            RegexOptions.IgnoreCase
            | RegexOptions.IgnorePatternWhitespace
            | RegexOptions.Compiled
            );

        private static Regex tempRegex = new Regex(
              @"xxx\ Temp\ (?<id>.*)\r\n.*name=(?<name>.*)\r\n.*active=(?<active>.*)\r\n",
            RegexOptions.IgnoreCase
            | RegexOptions.IgnorePatternWhitespace
            | RegexOptions.Compiled
            );

        private static Regex fanControllerRegex = new Regex(
              @"xxx\ FanController\ (?<id>.*)\r\n.*name=(?<name>.*)\r\n.*pwm=(?<pwm>.*)\r\n.*method=(?<method>.*)\r\n.*enabled=(?<enabled>.*)\r\n",
            RegexOptions.IgnoreCase
            | RegexOptions.IgnorePatternWhitespace
            | RegexOptions.Compiled
            );

        private static Regex fanControllerTempRegex = new Regex(
              @"xxx\ FanControllerTemp\ from\ FanController\ (?<id>.*)\r\n.*temp=(?<temp>.*)\r\n.*MinTemp=(?<min>.*)\r\n.*MaxTemp=(?<max>.*)\r\n.*hysteresis=(?<hysteresis>.*)\r\n.*ControlPoints=(?<points>.*)\r\n",
            RegexOptions.IgnoreCase
            | RegexOptions.IgnorePatternWhitespace
            | RegexOptions.Compiled
            );

        public IEnumerable<Fan> Fans { get; private set; }

        public List<Curve> Curves { get; private set; }

        public List<FanControllerConfig> Config { get; private set; }

        public IEnumerable<Temp> Temps { get; private set; }

        private const string path = "speedfansens.cfg";

        public void Load()
        {
            var content = File.ReadAllText(path);

            var fans = fansRegex.Matches(content)
                .Cast<Match>()
                .Select(m => new Fan
                {
                    Id = m.Groups["id"].Value,
                    Name = m.Groups["name"].Value,
                    Active = bool.Parse(m.Groups["active"].Value),
                    Logged = bool.Parse(m.Groups["logged"].Value),
                    Variate = bool.Parse(m.Groups["variate"].Value),
                    Min = int.Parse(m.Groups["min"].Value),
                    Max = int.Parse(m.Groups["max"].Value),
                })
                .ToDictionary(f => f.Id);


            var temps = tempRegex.Matches(content)
                .Cast<Match>()
                .Select(m => new Temp
                {
                    Id = m.Groups["id"].Value,
                    Name = m.Groups["name"].Value,
                    Active = bool.Parse(m.Groups["active"].Value),
                })
                .Where(t => t.Active)
                .ToDictionary(t => t.Id);

            var controllers = fanControllerRegex.Matches(content)
                .Cast<Match>()
                .Select(m => new FanController
                {
                    Id = m.Groups["id"].Value,
                    Name = m.Groups["name"].Value,
                    Active = bool.Parse(m.Groups["enabled"].Value),
                    Fan = fans[m.Groups["pwm"].Value],
                    Method = (Methods)int.Parse(m.Groups["method"].Value)
                })
                .ToDictionary(fc => fc.Id);

            var controllerTempsCollection = controllers.Values.ToDictionary(c => c.Id, c => new List<FanControllerTemp>());

            var controllerTemps = fanControllerTempRegex.Matches(content)
                .Cast<Match>()
                .Select(m =>
                {
                    var temp = new FanControllerTemp(temps[m.Groups["temp"].Value])
                    {
                        MinTemp = int.Parse(m.Groups["min"].Value),
                        MaxTemp = int.Parse(m.Groups["max"].Value),
                        Hysteresis = int.Parse(m.Groups["hysteresis"].Value),
                        PointsRaw = m.Groups["points"].Value
                    };
                    controllerTempsCollection[m.Groups["id"].Value].Add(temp);
                    return temp;
                })
                .ToList();

            var curves = controllerTemps
                .GroupBy(ct => ct.PointsRaw)
                .Select(g => new Curve(g.Key, g.Key.Split(' ').Select(int.Parse).ToList()))
                .ToDictionary(c => c.Id);

            Fans = fans
                .Values;

            Curves = curves
                .Values
                .ToList();

            Temps = temps
                .Values;

            Config = controllers.Values.Select(c => new FanControllerConfig
            {
                Controller = c,
                Curves = controllerTempsCollection[c.Id]
                    .GroupBy(t => t.PointsRaw)
                    .Select(g => new CurveConfig(curves[g.Key], g.ToList()))
                    .ToList()
            })
            .ToList();
        }

        public void RemoveCurve(Curve curve)
        {
            Curves.Remove(curve);
            Config.ForEach(fc => fc.Curves.RemoveAll(c => c.Curve == curve));
        }

        public FanControllerConfig AddNewFanController()
        {
            var controller = new FanController
            {
                Id = (Config.Max(c => int.Parse(c.Controller.Id)) + 1).ToString(),
                Fan =  Fans.First()
            
            };
            var config = new FanControllerConfig
            {
                Controller = controller,
            };
            Config.Add(config);
            return config;
        }

        private static readonly  IEnumerable<Regex> replacers = Replacers(
            @"xxx\ FanControllerTemp.*?xxx\ end", 
            @"xxx\ FanController\ \d{1,}.*?xxx\ end",
            @"xxx\ Pwm.*?xxx\ end\r\n\r\n"); 

        private static IEnumerable<Regex> Replacers(params string[] regexes)
        {
            return regexes.Select(regex => new Regex(regex,
                RegexOptions.IgnoreCase
                | RegexOptions.Singleline
                | RegexOptions.IgnorePatternWhitespace
                | RegexOptions.Compiled
                )).ToList();
        } 

        public void Save()
        {
            var content = File.ReadAllText(path);

            var stripped = replacers
                .CreateResult(content, (regex, result) => regex.Replace(result, string.Empty))
                .Trim(Environment.NewLine.ToCharArray());

            var fans = string.Join(Environment.NewLine, Fans.Select(f => $@"xxx Pwm {f.Id}
name={f.Name}
active={f.Active.ToString().ToLower()}
min={f.Min}
max={f.Max}
variate={f.Variate.ToString().ToLower()}
logged={f.Logged.ToString().ToLower()}
xxx end
"));

            var controllers = string.Join(Environment.NewLine, Config.Select(fc => fc.Controller).Select(c => $@"xxx FanController {c.Id}
name={c.Name}
pwm={c.Fan.Id}
method={(int)c.Method}
enabled={c.Active.ToString().ToLower()}
xxx end
"));

            var temps = string.Join(Environment.NewLine, Config.SelectMany(fc => fc.Curves.SelectMany(c => c.Temps.Select(t => $@"xxx FanControllerTemp from FanController {fc.Controller.Id}
temp={t.Temp.Id}
MinTemp={t.MinTemp}
MaxTemp={t.MaxTemp}
hysteresis={t.Hysteresis}
ControlPoints={c.Curve.RenderOutput()}
xxx end
")))
.ToList());

            File.WriteAllText(path, $@"{stripped}

{fans}
{controllers}
{temps}");
        }
    }
}
