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
        public abstract string ActionName { get; }
        public abstract string Title { get; }
        public abstract string Description { get; }

        public abstract void Do();

        public abstract void Undo();

        public abstract IReadOnlyDictionary<string, string> Pack();

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

        protected void SetInt(IDictionary<string, string> data, string key, int value)
        {
            data[key] = value.ToString();
        }

        protected int GetInt(IReadOnlyDictionary<string, string> data, string key)
        {
            return int.Parse(data[key]);
        }

        protected void SetNullableInt(IDictionary<string, string> data, string key, int? value)
        {
            if (value.HasValue)
            {
                data[key] = value.Value.ToString();
            }
        }

        protected int? GetNullableInt(IReadOnlyDictionary<string, string> data, string key)
        {
            int? value = null;

            int number;
            if (int.TryParse(data[key], out number))
            {
                value = number;
            }

            return value;
        }
    }
}
