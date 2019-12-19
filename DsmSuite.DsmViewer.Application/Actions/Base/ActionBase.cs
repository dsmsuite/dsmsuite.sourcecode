using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;
using System.Collections.Generic;

namespace DsmSuite.DsmViewer.Application.Actions.Base
{
    public abstract class ActionBase : IAction
    {
        protected ActionBase(IDsmModel model)
        {
            Model = model;
        }

        protected IDsmModel Model { get; }

        public abstract void Do();

        public abstract void Undo();

        public string ClassName { get; protected set; }
        public string Title { get; protected set; }
        public string Details { get; protected set; }

        public string Description => $"{Title} : {Details}";

        public abstract IReadOnlyDictionary<string, string> Pack();
        public abstract IAction Unpack(IReadOnlyDictionary<string, string> data);

        protected void SetString(IDictionary<string, string> data, string key, string value)
        {
            data[key] = value;
        }

        protected string GetString(IReadOnlyDictionary<string, string> data, string key)
        {
            string value = "";

            if (data.ContainsKey(key))
            {
                value = data[key];
            }
            return value;
        }

        protected void SetInt(IDictionary<string, string> data, string key, int? value)
        {
            if (value.HasValue)
            {
                data[key] = value.Value.ToString();
            }
        }

        protected int? GetInt(IReadOnlyDictionary<string, string> data, string key)
        {
            int? value = null;

            if (data.ContainsKey(key))
            {
                int number;
                if (int.TryParse(data[key], out number))
                {
                    value = number;
                }
            }
            return value;
        }
    }
}
