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
        }

        // 🔹 Static base data (image locations)
        public static List<AvatarSet> Avatars { get; } = new()
        {
            new AvatarSet
            {
                AvatarIdSet = "avatar1",
                CostumePaths = new List<string>
                {
                    "avatarcustomization/avatar1/defaultavatar1.png",
                    "avatarcustomization/avatar1/avatar_blue1.png",
                    "avatarcustomization/avatar1/avatar_bluered1.png",
                    "avatarcustomization/avatar1/avatar_green1.png",
                    "avatarcustomization/avatar1/avatar_pink1.png",
                    "avatarcustomization/avatar1/avatar_red1.png",
                    "avatarcustomization/avatar1/avatar_white1.png"
                }
            },
            new AvatarSet
            {
                AvatarIdSet = "avatar2",
                CostumePaths = new List<string>
                {
                    "avatarcustomization/avatar2/defaultavatar2.png",
                    "avatarcustomization/avatar2/avatar_blue2.png",
                    "avatarcustomization/avatar2/avatar_bluered2.png",
                    "avatarcustomization/avatar2/avatar_green2.png",
                    "avatarcustomization/avatar2/avatar_pink2.png",
                    "avatarcustomization/avatar2/avatar_red2.png",
                    "avatarcustomization/avatar2/avatar_white2.png"
                }
            },
            new AvatarSet
            {
                AvatarIdSet = "avatar3",
                CostumePaths = new List<string>
                {
                    "avatarcustomization/avatar3/defaultavatar3.png",
                    "avatarcustomization/avatar3/avatar_blue3.png",
                    "avatarcustomization/avatar3/avatar_bluered3.png",
                    "avatarcustomization/avatar3/avatar_green3.png",
                    "avatarcustomization/avatar3/avatar_pink3.png",
                    "avatarcustomization/avatar3/avatar_red3.png",
                    "avatarcustomization/avatar3/avatar_white3.png"
                }
            },
            new AvatarSet
            {
                AvatarIdSet = "avatar4",
                CostumePaths = new List<string>
                {
                    "avatarcustomization/avatar4/defaultavatar4.png",
                    "avatarcustomization/avatar4/avatar_blue4.png",
                    "avatarcustomization/avatar4/avatar_bluered4.png",
                    "avatarcustomization/avatar4/avatar_green4.png",
                    "avatarcustomization/avatar4/avatar_pink4.png",
                    "avatarcustomization/avatar4/avatar_red4.png",
                    "avatarcustomization/avatar4/avatar_white4.png"
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
            costume.avatarredunlocked,      // index 4
            costume.avatarwhiteunlocked     // index 5
        };
                
            }
        }
        /// <summary>
        /// Returns costume info (path + unlock status) for the given avatar.
        /// </summary>
    }
}
