using IniParser;
using IniParser.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pulsarc.Utils
{
    static class Config
    {

        static public FileIniDataParser parser;
        static public IniData get;

        static public void Initialize()
        {
            parser = new FileIniDataParser();
            get = parser.ReadFile("config.ini");
        }
    }
}
