using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace mkPoEParser
{
    class ModExtractor
    {
        public static List<string> ParseItem(List<string> modList, ItemData item)
        {
            var expression = @"[0-9]+(\.[0-9]{1,2})?";
            var itemTypeExpression = @"\/2DItems\/(.*)(:?\/[a-zA-Z0-9-_]*\.png)";

            string itemType = Regex.Match(item.Icon, itemTypeExpression).Groups[1].Value;

            if (itemType.Contains("Maps"))
            {
                itemType = "Maps";
            }
            else if (itemType == "Currency/Classic")
            {
                itemType = "Leaguestone";
            }
            else if (itemType == "Currency/Breach")
            {
                itemType = "Blessing";
            }

            if (itemType == "")
            {
                if (item.TypeLine.Contains("Flask"))
                {
                    itemType = "Flask";
                }
                else
                {
                    Debug.WriteLine(item.Icon);
                    itemType = "Misc.";
                }
            }

            if (item.ExplicitMods != null)
            {
                foreach (var mod in item.ExplicitMods)
                {
                    string toAdd = Regex.Replace(mod, expression, "#");
                    toAdd = String.Format("({0}) ExplicitMod: {1}", itemType, toAdd);

                    if (modList.Contains(toAdd) == false)
                    {
                        modList.Add(toAdd);
                    }
                }
            }

            if (item.ImplicitMods != null)
            {
                foreach (var mod in item.ImplicitMods)
                {
                    string toAdd = Regex.Replace(mod, expression, "#");
                    toAdd = String.Format("({0}) ImplicitMod: {1}", itemType, toAdd);

                    if (modList.Contains(toAdd) == false)
                    {
                        modList.Add(toAdd);
                    }
                }
            }

            if (item.CraftedMods != null)
            {
                foreach (var mod in item.CraftedMods)
                {
                    string toAdd = Regex.Replace(mod, expression, "#");
                    toAdd = String.Format("({0}) CraftedMod: {1}", itemType, toAdd);

                    if (modList.Contains(toAdd) == false)
                    {
                        modList.Add(toAdd);
                    }
                }
            }

            return modList;
        }
    }
}
