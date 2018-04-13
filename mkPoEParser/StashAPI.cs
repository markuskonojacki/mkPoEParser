using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mkPoEParser
{
    struct StashAPIData
    {
        public string Next_change_id { get; set; }
        public StashData[] Stashes { get; set; }
    }

    struct StashData
    {
        public string AccountName { get; set; }         // account name
        public string LastCharacterName { get; set; }   // name of the last logged in character name
        public string Id { get; set; }                  // When you update an item in a tab, or update the tab itself it's ID will take the next available one from the pool on that server shard (the old ID is discarded, and never reused).
        public string Stash { get; set; }               // stash name
        public string StashType { get; set; }           // eg.: PremiumStash or QuadStash

        public ItemData[] Items { get; set; }           // Array with all the changed items in the stash
        public Boolean Public { get; set; }             // True is stash is public. If false all the items in the stash could be removed
    }

    struct ItemData
    {
        public Boolean Verified { get; set; }                   // verified status
        public int W { get; set; }                              // width in slots
        public int H { get; set; }                              // height in slots
        public int Ilvl { get; set; }                           // itemlevel
        public string Icon { get; set; }                        // escaped poecdn.com url (eg.: https:\/\/web.poecdn.com\/image\/Art\/2DItems\/Weapons\/TwoHandWeapons\/Bows\/SarkhamsReach.png?scale=1&w=2&h=4&v=f333c2e4005ee20a84270731baa5fa6a3)
        public Boolean Support { get; set; }                    // true when support gem
        public string League { get; set; }                      // in what league is the item (eg.: HardCore, Softwore, Legacy, Legacy Hardcore)
        public string Id { get; set; }                          // When you update an item in a tab, or update the tab itself it's ID will take the next available one from the pool on that server shard (the old ID is discarded, and never reused).
        public Socket[] Sockets { get; set; }                   // group of sockets    
        public string Name { get; set; }                        // item name (eg.: "name": "<<set:MS>><<set:M>><<set:S>>Roth's Reach")
        public string TypeLine { get; set; }                    // item type (eg.: "typeLine": "<<set:MS>><<set:M>><<set:S>>Cautious Divine Life Flask of Warding", "typeLine": "Recurve Bow")
        public Boolean Identified { get; set; }                 // identified state of the item
        public Boolean Corrupted { get; set; }                  // corruption state of the item
        public Boolean LockedToCharacter { get; set; }          // eg.: quest items
        public string Note { get; set; }                        // note on the specific item (eg.: ~price 10 exa)

        public Propertie[] Properties { get; set; }             // properties of the item (eg.: gem tags or Quality, Critical Strike Chance or Attack Speed)
        public Propertie[] AdditionalProperties { get; set; }   // eg.: expirience on a gem
        public Requirement[] Requirements { get; set; }         // requirements of the item (eg.: Level or Dex)

        public string[] ExplicitMods { get; set; }              // eg.: ["68% increased Physical Damage", "+18 to Dexterity and Intelligence"]
        public string[] ImplicitMods { get; set; }              // ...
        public string[] CraftedMods { get; set; }               // ...

        public int FrameType { get; set; }                      // ?
        public string X { get; set; }                           // x position of the item in the stash
        public string Y { get; set; }                           // y position of the item in the stash
    }

    struct Propertie
    {
        /*{
            *    "name": "Experience",
            *    "values": [
            *        [
            *            "9569\/9569",
            *            0 
            *         ]
´        *    ],
            *    "displayMode": 2,
            *    "progress": 1
            *} 
            */
        public string Name { get; set; }        // name of the propertie (eg.: Experience)
        public object[] Values { get; set; }    // [string, int] (eg.: "10\/20", ?)
        public string DisplayMode { get; set; } // ?
        //public int Progress { get; set; }       // ?
    }

    struct Requirement
    {

    }

    struct Socket
    {
        public int Group { get; set; }      // 0..4
        public string Attr { get; set; }    // S(tr), D(ex), I(nt), G(? white)
    }
}
