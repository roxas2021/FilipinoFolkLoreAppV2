using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilipinoFolkloreApp.Services
{
    public class HeartService
    {
        const int MaxHearts = 3;
        const int ResetMinutes = 1;
        const string LastHeartLostKey = "LastHeartLost";

        public int GetHearts()
        {
            RestoreIfNeeded();
            return FilipinoFolkloreApp.Services.AlamatContent.Hearts;
        }

        public void LoseHeart()
        {
            if (AlamatContent.Hearts <= 0)
                return;

            AlamatContent.Hearts--;

            // Save time ONLY when hearts are not full
            if (AlamatContent.Hearts < MaxHearts)
            {
                Preferences.Set(LastHeartLostKey, DateTime.UtcNow);
            }
        }

        public void RestoreIfNeeded()
        {
            if (AlamatContent.Hearts == MaxHearts)
                return;

            if (!Preferences.ContainsKey(LastHeartLostKey))
                return;

            var lastLost = Preferences.Get(LastHeartLostKey, DateTime.UtcNow);
            var elapsed = DateTime.UtcNow - lastLost;

            if (elapsed.TotalMinutes >= ResetMinutes)
            {
                AlamatContent.Hearts = MaxHearts;
                Preferences.Remove(LastHeartLostKey);
            }
        }
    }

}
