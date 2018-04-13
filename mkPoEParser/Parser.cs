using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
//using mkPoEParser.

namespace mkPoEParser
{
    class JsonParser
    {
        public mkPoEParser.MainWindow window;
        public string next_id;
        Thread workerThread;

        public Stopwatch watch;
        public long[,] average;
        public long BytesTransferred;

        List<string> ModList = new List<string>();

        private string SearchLeague { get; set; }
        private string SearchItemName { get; set; }
        private string SearchAccountName { get; set; }

        string[] currency_abbreviated = { "alt", "fuse", "alch", "chaos", "gcp", "exa", "chrom", "jew", "chance", "chisel", "scour", "blessed", "regret", "regal", "divine", "vaal" };
        string[] currency_singular = { "Orb of Alteration", "Orb of Fusing", "Orb of Alchemy", "Chaos Orb", "Gemcutter\"s Prism", "Exalted Orb", "Chromatic Orb", "Jeweller\"s Orb", "Orb of Chance", "Cartographer\"s Chisel", "Orb of Scouring", "Blessed Orb", "Orb of Regret", "Regal Orb", "Divine Orb", "Vaal Orb" };
        string[] currency_plural = { "Orbs of Alteration", "Orbs of Fusing", "Orbs of Alchemy", "Chaos Orbs", "Gemcutter\"s Prisms", "Exalted Orbs", "Chromatic Orbs", "Jeweller\"s Orbs", "Orbs of Chance", "Cartographer\"s Chisels", "Orbs of Scouring", "Blessed Orbs", "Orbs of Regret", "Regal Orbs", "Divine Orbs", "Vaal Orbs" };

        public JsonParser(mkPoEParser.MainWindow window, string searchLeague, string searchItemName, string searchAccountName)
        {
            this.window = window;
            this.SearchLeague = searchLeague;
            this.SearchItemName = searchItemName;
            this.SearchAccountName = searchAccountName;
        }

        public void Start()
        {
            workerThread = new Thread(RunParser);
            workerThread.Start();
        }

        public void Stop()
        {
            if (workerThread.IsAlive)
            {
                workerThread.Abort();
            }
        }

        public void RunParser()
        {
            average = new long[10, 2];
            watch = Stopwatch.StartNew();

            next_id = GetFirstId();

            while (true)
            {
                next_id = ParseStashData(next_id);
            }
        }

        public string GetFirstId()
        {
            string url = @"http://api.poe.ninja/api/Data/GetStats";

            var PoENinjaStats = (new WebClient()).DownloadString(url);
            PoENinjaStats stat = JsonConvert.DeserializeObject<PoENinjaStats>(PoENinjaStats);

            return stat.next_change_id;
        }

        public string ParsePrice(string to_parse)
        {
            Regex regex = new Regex("~(b/o|price) ([0-9]+) (alt|fuse|alch|chaos|gcp|exa|chrom|jew|chance|chisel|scour|blessed|regret|regal|divine|vaal)");
            Match match;
            decimal price = 0;
            string ret = "";

            if (to_parse != "" && to_parse != null)
            {
                match = regex.Match(to_parse);
                if (match.Success)
                {
                    price = decimal.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture);

                    if (price == 1)
                    {
                        // singular
                        string currency = currency_singular[Array.IndexOf(currency_abbreviated, match.Groups[3].Value)];
                        ret = String.Format("{0} {1}", price, currency);
                    }
                    else
                    {
                        // plural
                        string currency = currency_plural[Array.IndexOf(currency_abbreviated, match.Groups[3].Value)];
                        ret = String.Format("{0} {1}", price, currency);
                    }
                }
            }

            return ret;
        }
            
        public string ParseStashData(string next_id)
        {
            string APIData = GetAPIData();

            if (APIData != "")
            {
                StashAPIData jsonString = JsonConvert.DeserializeObject<StashAPIData>(APIData);

                var ByteCount = ASCIIEncoding.ASCII.GetByteCount(APIData);
                BytesTransferred += ByteCount;

                window.AddToLog(String.Format("Parsing Id: {0} (this: {1} kB, total: {2} MB, average: {3} kB/s)", next_id, ByteCount / 1024, BytesTransferred / 1024 / 1024, Math.Round(BytesTransferred / watch.Elapsed.TotalSeconds / 1024, 2)));

                foreach (StashData stash in jsonString.Stashes)
                {
                    if (SearchAccountName == "" || (stash.AccountName != null && stash.AccountName.ToLower() == SearchAccountName.ToLower()))
                    {
                        foreach (ItemData item in stash.Items)
                        {
                            ModList = ModExtractor.ParseItem(ModList, item);
                            Debug.Print(ModList.Count.ToString());

                            if (item.League == SearchLeague &&
                                (SearchItemName != "" &&
                                    (item.Name.ToLower().Contains(SearchItemName.ToLower()) ||
                                     item.TypeLine.ToLower().Contains(SearchItemName.ToLower()))))
                            {
                                string price = ParsePrice(item.Note);

                                if (price == "")
                                {
                                    price = ParsePrice(stash.Stash);
                                }

                                var itemName = item.Name;

                                if (itemName != null && itemName != "")
                                {
                                    itemName = itemName.Substring(28, itemName.Length - 28);
                                }

                                if (itemName == null || itemName == "")
                                {
                                    itemName = item.TypeLine;
                                }

                                window.AddToLog("Item found:");

                                window.AddToLog(String.Format("@{0} Hi, I would like to buy your {1} listed for {2} in {3} (stash tab \"{4}\"; position: left {5}, top {6})",
                                                                stash.AccountName,
                                                                itemName,
                                                                price,
                                                                SearchLeague,
                                                                stash.Stash,
                                                                item.X,
                                                                item.Y));

                                if (item.Corrupted == true)
                                {
                                    window.AddToLog("### CORRUPTED ###");
                                }

                                string sockets = "";
                                int lastGroup = 0;
                                if (item.Sockets != null)
                                {
                                    foreach (Socket socket in item.Sockets)
                                    {
                                        if (socket.Group != lastGroup)
                                        {
                                            sockets += " | ";
                                        }

                                        switch (socket.Attr)
                                        {
                                            case "S":
                                                sockets += "R";
                                                break;

                                            case "I":
                                                sockets += "B";
                                                break;

                                            case "D":
                                                sockets += "G";
                                                break;

                                            case "G":
                                                sockets += "W";
                                                break;
                                        }
                                    }
                                }

                                if (sockets != "")
                                {
                                    window.AddToLog(String.Format("Sockets: {0}", sockets));
                                }

                                if (item.ImplicitMods != null && item.ImplicitMods[0] != null)
                                {
                                    window.AddToLog(String.Format("implicitMod: {0}", item.ImplicitMods[0]));
                                }

                                if (item.ExplicitMods != null)
                                {
                                    foreach (var mod in item.ExplicitMods)
                                    {
                                        window.AddToLog(String.Format("explicitMod: {0}", mod));
                                    }
                                }
                            }
                        }
                    }
                }

                if (ByteCount < 1024 * 1024)
                {
                    window.AddToLog("Throttle protection. File Size < 1MB. Waiting 1 second...");
                    Thread.Sleep(1000);
                }

                next_id = jsonString.Next_change_id;
            }
            else
            {
                Thread.Sleep(10000);
            }

            return next_id;
        }

        private string GetAPIData()
        {
            string url = @"http://www.pathofexile.com/api/public-stash-tabs?id=" + next_id;
            string json = "";

            try
            {
                json = (new WebClient()).DownloadString(url);
            }
            catch (Exception ex)
            {
                window.AddToLog("Error while getting new stash API data: " + ex.Message + "\nWill wait 10 seconds and then try again.");
            }

            return json;
        }
    }
}