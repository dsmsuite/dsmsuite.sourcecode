using System.Collections.Generic;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Actions.Base
{
    public class ActionReadOnlyAttributes
    {
        private readonly IDsmModel _model;
        private readonly IReadOnlyDictionary<string, string> _data;

        public ActionReadOnlyAttributes(IDsmModel model, IReadOnlyDictionary<string, string> data)
        {
            _data = data;
            _model = model;
        }

        public string GetString(string memberName)
        {
            return _data[RemoveUnderscore(memberName)];
        }

        public int GetInt(string memberName)
        {
            return int.Parse(_data[RemoveUnderscore(memberName)]);
        }

        public int? GetNullableInt(string memberName)
        {
            int? value = null;

            int number;
            if (_data.ContainsKey(RemoveUnderscore(memberName)) &&
                int.TryParse(_data[RemoveUnderscore(memberName)], out number))
            {
                value = number;
            }

            return value;
        }

        public IDsmElement GetElement(string memberName)
        {
            int id = GetInt(memberName);
            return _model.GetElementById(id) ?? 
                   _model.GetDeletedElementById(id);
        }

        public IDsmRelation GetRelation(string memberName)
        {
            int id = GetInt(memberName);
            return _model.GetRelationById(id) ?? 
                   _model.GetDeletedRelationById(id);
        }

        public IDsmElement GetRelationConsumer(string memberName)
        {
            IDsmElement consumer = null;
            IDsmRelation relation = GetRelation(memberName);
            if (relation != null)
            {
                consumer = _model.GetElementById(relation.Consumer.Id) ??
                           _model.GetDeletedElementById(relation.Consumer.Id);
            }
            return consumer;
        }

        public IDsmElement GetRelationProvider(string memberName)
        {
            IDsmElement provider = null;
            IDsmRelation relation = GetRelation(memberName);
            if (relation != null)
            {
                provider = _model.GetElementById(relation.Provider.Id) ??
                           _model.GetDeletedElementById(relation.Provider.Id);
            }
            return provider;
        }

        private static string RemoveUnderscore(string memberName)
        {
            return memberName.Substring(1);
        }
    }
}
