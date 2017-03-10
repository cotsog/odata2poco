﻿using System;
using System.Collections.Generic;
using System.Text;

namespace OData2Poco.Shared
{
    /// <summary>
    /// Manage assemplies which is needed by using statement 
    /// sources of assemplies are: Attributes , dataType of properties and may be from external via PocoSetting.ExternalAssemplie
    /// </summary>
    class AssemplyManager
    {
        public  List<string> AssemplyReference;
        private readonly PocoSetting _pocoSetting;
        private readonly IDictionary<string, ClassTemplate> _model;
        public AssemplyManager(PocoSetting pocoSetting, IDictionary<string, ClassTemplate> model)
        {
            _pocoSetting = pocoSetting;
            _model = model;
            AssemplyReference = new List<string>();
            AddAssemplyReferenceList();

        }

        //add all assemplies for using either for attribute or datatype 
        //TODO: load entries from configuration file
        //TODO: Entries may be passed to pocosetting for external references defined by user
        private Dictionary<string, string> assemplyDict = new Dictionary<string, string>
        {
            //assemplies for attributes
            {"key","System.ComponentModel.DataAnnotations"},
            {"required" ,"System.ComponentModel.DataAnnotations.Schema"},
            {"table" ,"System.ComponentModel.DataAnnotations.Schema"},
            {"json","Newtonsoft.Json"}, //extrnal type can be installed from nuget
            //assemplies for datatype
            {"geometry","Microsoft.Spatial"}, //extrnal type can be installed from nuget
            {"geography", "Microsoft.Spatial"} //extrnal type can be installed from nuget
        };


        private void AddAssemply(string name)
        {
            name = name.ToLower();
            //Console.WriteLine("try to add " + name);
            if (!assemplyDict.ContainsKey(name)) return;
            var entry = assemplyDict[name];
            if (!AssemplyReference.Exists(a => a.Contains(entry)))
            {
                AssemplyReference.Add(entry);
                //Console.WriteLine(entry);
            }
        }

        private void AddAssemplyReferenceList()
        {
            //Add required namespace for attributes
            if (_pocoSetting.AddKeyAttribute) AddAssemply("key");
            if (_pocoSetting.AddRequiredAttribute) AddAssemply("required");
            if (_pocoSetting.AddTableAttribute) AddAssemply("table");
            if (_pocoSetting.AddJsonAttribute) AddAssemply("json");
            AddAssempliesOfDataType();//add assemplies of datatype
        }

        /// <summary>
        /// Scan the model to find all uniqe dataTypes which may have referenced or external assemply
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private void AddAssempliesOfDataType()
        {
            foreach (var entry in _model)
            {
                var properties = entry.Value.Properties;
                foreach (var property in properties)
                {
                    var type = property.PropType;
                    if (Helper.NullableDataTypes.ContainsKey(type)) continue; // skip simple data type
                    AddAssemply(type);
                }
            }
        }
    }
}
