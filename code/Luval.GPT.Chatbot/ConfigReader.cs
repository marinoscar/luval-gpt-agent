﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Chatbot
{
    public static class ConfigReader
    {
        private static Dictionary<string, string> _data;
        public static void Initialize()
        {
            _data = new Dictionary<string, string>();
            LoadPrivate();
        }

        public static string? Get(string keyName, string? defaultValue = default(string))
        {
            if(_data == null) Initialize();
            if(_data == null || !_data.ContainsKey(keyName)) return defaultValue;
            return _data[keyName];
        }

        private static void LoadPrivate()
        {
            LoadFile("private.json");
        }

        private static void LoadFile(string filename)
        {
            var content = File.ReadAllText(filename);
            var items = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
            foreach (var item in items)
            {
                _data[item.Key] = item.Value;
            }
        }
    }
}