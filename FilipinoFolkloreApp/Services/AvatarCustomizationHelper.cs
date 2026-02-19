
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FilipinoFolkloreApp.Models;

namespace FilipinoFolkloreApp.Services
{
    public static class AvatarCustomizationHelper
    {
        public class CostumeInfo
        {
            public string ImagePath { get; set; } = "";
            public bool IsUnlocked { get; set; }
        }

        public class AvatarSet
        {
            public string AvatarIdSet { get; set; } = "";
            public List<string> CostumePaths { get; set; } = new();
            public List<string> TapisPaths { get; set; } = new();
        }

        // 🔹 Static base data (image locations)
        public static List<AvatarSet> Avatars { get; } = new()
        {
            new AvatarSet
            {
                AvatarIdSet = "avatar1",
                CostumePaths = new List<string>
                {
                    "avatarcustomization/avatar1/avatar1.png",
                    "avatarcustomization/avatar1/avatar1_blue.png",
                    "avatarcustomization/avatar1/avatar1_bluered.png",
                    "avatarcustomization/avatar1/avatar1_red.png",
                    "avatarcustomization/avatar1/avatar1_whiteblue.png",
                    "avatarcustomization/avatar1/avatar1_whitered.png"
                },
                TapisPaths = new List<string>
                {
                    "avatarcustomization/tapis/bblue.png",
                    "avatarcustomization/tapis/bbluered.png",
                    "avatarcustomization/tapis/bred.png",
                    "avatarcustomization/tapis/bwhiteblue.png",
                    "avatarcustomization/tapis/bwhitered.png"
                }

            },
            new AvatarSet
            {
                AvatarIdSet = "avatar2",
                CostumePaths = new List<string>
                {
                    "avatarcustomization/avatar2/avatar2.png",
                    "avatarcustomization/avatar2/avatar2_blue.png",
                    "avatarcustomization/avatar2/avatar2_bluered.png",
                    "avatarcustomization/avatar2/avatar2_red.png",
                    "avatarcustomization/avatar2/avatar2_whiteblue.png",
                    "avatarcustomization/avatar2/avatar2_whitered.png"
                },
                TapisPaths = new List<string>
                {
                    "avatarcustomization/tapis/bblue.png",
                    "avatarcustomization/tapis/bbluered.png",
                    "avatarcustomization/tapis/bred.png",
                    "avatarcustomization/tapis/bwhiteblue.png",
                    "avatarcustomization/tapis/bwhitered.png"
                }
            },
            new AvatarSet
            {
                AvatarIdSet = "avatar3",
                CostumePaths = new List<string>
                {
                    "avatarcustomization/avatar3/avatar3.png",
                    "avatarcustomization/avatar3/avatar3_white.png",
                    "avatarcustomization/avatar3/avatar3_whiteblack.png",
                    "avatarcustomization/avatar3/avatar3_whitegreen.png",
                    "avatarcustomization/avatar3/avatar3_whiteorange.png",
                    "avatarcustomization/avatar3/avatar3_whitered.png"
                },
                TapisPaths = new List<string>
                {
                    "avatarcustomization/tapis/gwhite.png",
                    "avatarcustomization/tapis/gwhiteblack.png",
                    "avatarcustomization/tapis/gwhitegreen.png",
                    "avatarcustomization/tapis/gwhiteorange.png",
                    "avatarcustomization/tapis/gwhitered.png"
                }
            },
            new AvatarSet
            {
                AvatarIdSet = "avatar4",
                CostumePaths = new List<string>
                {
                    "avatarcustomization/avatar4/avatar4.png",
                    "avatarcustomization/avatar4/avatar4_white.png",
                    "avatarcustomization/avatar4/avatar4_whiteblack.png",
                    "avatarcustomization/avatar4/avatar4_whitegreen.png",
                    "avatarcustomization/avatar4/avatar4_whiteorange.png",
                    "avatarcustomization/avatar4/avatar4_whitered.png"
                },
                TapisPaths = new List<string>
                {
                    "avatarcustomization/tapis/gwhite.png",
                    "avatarcustomization/tapis/gwhiteblack.png",
                    "avatarcustomization/tapis/gwhitegreen.png",
                    "avatarcustomization/tapis/gwhiteorange.png",
                    "avatarcustomization/tapis/gwhitered.png"
                }
            }
        };
        public static string SelectedAvatarSetId { get; set; } = "";
        public static AvatarSet CurrentAvatarSet(string requestedset) => Avatars.First(s => s.AvatarIdSet == requestedset);
        public static List<bool> purchasedCostumes = new List<bool>();
        public static async Task LoadPurchasedCostume()
        {

            var set = await App.Database.GetAllAvatarSetsAsync(); // or GetAvatarSetByAvatarIdAsync(...)
            var costume = set?.FirstOrDefault(); // get the single saved set (if any)

            if (costume == null)
            {
                // nothing saved yet -> defaults remain false
                purchasedCostumes = new List<bool> { false, false, false, false, false, false };
            }
            else
            {
                purchasedCostumes = new List<bool>
        {
            costume.avatarblueunlocked,     // index 0
            costume.avatarblueredunlocked,  // index 1
            costume.avatargreenunlocked,    // index 2
            costume.avatarpinkunlocked,     // index 3
            costume.avatarredunlocked
        };

            }
        }
        public static string? GetFirstCostumePath(string avatarSetId)
        {
            if (string.IsNullOrWhiteSpace(avatarSetId)) return null;

            var set = Avatars
                        .FirstOrDefault(a => a.AvatarIdSet == avatarSetId);

            if (set == null) return null;
            if (set.CostumePaths == null || set.CostumePaths.Count == 0) return null;

            return set.CostumePaths[0];
        }
        public static string GetFirstCostumePathOrDefault(string avatarSetId, string fallback = "avatarcustomization/default.png")
        {
            var first = GetFirstCostumePath(avatarSetId);
            return string.IsNullOrEmpty(first) ? fallback : first;
        }
    }
}
