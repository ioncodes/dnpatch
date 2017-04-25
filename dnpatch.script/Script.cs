using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using dnlib.DotNet.Emit;
using Newtonsoft.Json.Linq;

namespace dnpatch.script
{
    public class Script
    {
        private string _scriptFile;                                                                     // Filepath
        private JObject _script;                                                                        // Script
        private Patcher _patcher;                                                                       // Patcher
        private readonly Dictionary<string, Target> _targets = new Dictionary<string, Target>();        // Target
        private string _optional;                                                                       // Optional arguments

        public Script(string path)
        {
            _scriptFile = path;
            _script = JObject.Parse(File.ReadAllText(path));
            BuildTarget();
            _patcher = new Patcher(_script.GetValue("target").ToString());
        }

        public void Patch()
        {
            foreach (var target in _targets)
            {
                if (target.Key == "empty")
                    _patcher.WriteEmptyBody(target.Value);
                else if (target.Key == "return")
                    _patcher.WriteReturnBody(target.Value, Convert.ToBoolean(_optional));
                else if (target.Key == "replace")
                    _patcher.ReplaceInstruction(target.Value);
                else if(target.Key == "remove")
                    _patcher.RemoveInstruction(target.Value);
            }

            if (_script["save"] != null)
            {
                if (_script.GetValue("save").ToString() == _script.GetValue("target").ToString())
                {
                    Save(true);
                }
                else
                {
                    Save(_script.GetValue("save").ToString());
                }
            }
        }

        public void Save(bool backup)
        {
            _patcher.Save(backup);
        }

        public void Save(string name)
        {
            _patcher.Save(name);
        }

        public void LoadScript(string path)
        {
            _scriptFile = path;
            _script = JObject.Parse(path);
            BuildTarget();
            _patcher = new Patcher(_script.GetValue("target").ToString());
        }

        private void BuildTarget()
        {
            JArray targets = (JArray) _script.GetValue("targets");
            foreach (var t in targets)
            {
                Target target = new Target
                {
                    Namespace = t["namespace"].ToString(),
                    Class = t["class"].ToString(),
                    Method = t["method"].ToString()
                };
                if (t["index"] != null)
                    target.Index = Convert.ToInt32(t["index"]);
                if (t["indices"] != null)
                    target.Indices = t["indices"].Values<int>().ToArray();
                if (t["optional"] != null)
                    _optional = t["optional"].ToString();
                if (t["instructions"] != null)
                {
                    JArray instructions = (JArray) t["instructions"];
                    if (instructions.Count == 1)
                    {
                        if (instructions[0]["opcode"] != null && instructions[0]["operand"] != null)
                        {
                            var operand = instructions[0].Last.Last;
                            if (operand.Type == JTokenType.Integer)
                            {
                                target.Instruction =
                                    Instruction.Create(
                                        (OpCode)
                                        GetInstructionField(instructions[0].First.First.ToString()).GetValue(this),
                                        operand.Value<int>());
                            }
                            else if (operand.Type == JTokenType.String)
                            {
                                target.Instruction =
                                    Instruction.Create(
                                        (OpCode)
                                        GetInstructionField(instructions[0].First.First.ToString()).GetValue(this),
                                        operand.Value<string>());
                            }
                        }
                        else if(instructions[0]["opcode"] != null)
                        {
                            target.Instruction =
                                Instruction.Create(
                                    (OpCode) GetInstructionField(instructions[0].First.First.ToString()).GetValue(this));
                        }
                    }
                    else
                    {
                        target.Instructions = new Instruction[instructions.Count];
                        for (int i = 0; i < instructions.Count; i++)
                        {
                            var instruction = instructions[i];
                            if (instruction["opcode"] != null && instruction["operand"] != null)
                                target.Instructions[i] =
                                    Instruction.Create((OpCode)GetInstructionField(instruction.First.First.ToString()).GetValue(this),
                                        instruction.Last.Last.Value<dynamic>());
                            else
                                target.Instructions[i] =
                                    Instruction.Create(
                                        (OpCode)GetInstructionField(instruction.First.First.ToString()).GetValue(this));
                        }
                    }
                }
                _targets.Add(t["action"].ToString(), target);
            }
        }

        private FieldInfo GetInstructionField(string name)
        {
            var type = typeof(OpCodes);
            var field = type.GetField(name, BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Static);
            return field;
        }
    }
}
