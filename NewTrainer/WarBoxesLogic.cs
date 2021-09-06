using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewTrainer
{
    public enum WFCurrency
    {
        WB,
        Crowns,
        Credits
    }

    public class Warbox
    {
        public string title = "";
        public int rankAvailable = 2;
        public int firstBoxPrice = 425;
        public WFCurrency currency = WFCurrency.WB;
        public int numInShop = 1;
        public int discountForNextBox = 25;
    }

    abstract public class WarboxesBase
    {
        protected string filePath = "";
        protected List<Warbox> warboxes = null;
        protected int boxID = -1;
        bool notFoundNeedBox = false;
        public WarboxesBase()
        {

        }

        public void NotFoundNeedBox()
        {
            notFoundNeedBox = true;
        }

        public Warbox OptimalWarBox
        {
            get
            {
                return GetWarBoxForOpening();
            }
        }

        protected int GetMinNeededRank()
        {
            int minRank = 1000;
            foreach (var item in warboxes)
            {
                if (minRank > item.rankAvailable)
                    minRank = item.rankAvailable;
            }
            return minRank;
        }

        int CountBoxesCanBeOpened(int currency, int boxIter)
        {
            int cnt = 0;
            while (currency > 0)
            {
                for (int bx = 0; bx < 5; bx++)
                {
                    currency -= warboxes[boxIter].firstBoxPrice - warboxes[boxIter].discountForNextBox * bx;
                    if (currency >= 0)
                        cnt++;
                    else
                        break;
                }
            }
            return cnt;
        }

        protected int HowManyBoxesCanBeOpened()
        {
            boxID = -1;
            int maxBoxes = 0;
            for (int i = 0; i < warboxes.Count; i++)
            {
                int cnt = 0;
                switch (warboxes[i].currency)
                {
                    case WFCurrency.WB:
                        cnt = CountBoxesCanBeOpened(Global.userLogic.GetUserInfo().WB, i);
                        break;
                    case WFCurrency.Crowns:
                        cnt = CountBoxesCanBeOpened(Global.userLogic.GetUserInfo().Crowns, i);
                        break;
                    default:
                        break;
                }
                if (maxBoxes < cnt)
                {
                    maxBoxes = cnt;
                    boxID = i;
                }
            }
            return maxBoxes;
        }

        protected bool CanOpenSomething(out int counBoxesCanBeOpened)
        {
            counBoxesCanBeOpened = 0;
            if (notFoundNeedBox)
                return false;
            if (warboxes == null || warboxes.Count <= 0)
                return false;

            UserInfo uInfo = Global.userLogic.GetNoSetUserInfo();
            if (uInfo == null || GetMinNeededRank() > uInfo.InGameInfo.rank_id)
                return false;

            counBoxesCanBeOpened = HowManyBoxesCanBeOpened();
            if (counBoxesCanBeOpened <= 0)
                return false;
            return true;
        }

        public int GetMinimalPrice(WFCurrency currency)
        {
            if (warboxes == null)
                return 0;
            int minCost = 0;
            foreach (var item in warboxes)
            {
                if (item.currency == currency)
                {
                    if (minCost == 0 || item.firstBoxPrice < minCost)
                        minCost = item.firstBoxPrice;
                }
            }
            return minCost;
        }

        abstract protected Warbox GetWarBoxForOpening();
        abstract protected void LoadWarBoxes();
    }

    public class WarboxesForLevelUp : WarboxesBase
    {
        int minExperienceForBox = 50;
        public WarboxesForLevelUp()
        {
            filePath = "WarboxesForLevelUp.json";
            LoadWarBoxes();
        }

        override protected Warbox GetWarBoxForOpening()
        {
            if (Global.settingsMain.neededRank <= Global.userLogic.GetUserInfo().InGameInfo.rank_id)
                return null;
            int counBoxesCanBeOpened;
            if (!CanOpenSomething(out counBoxesCanBeOpened))
                return null;
            if (counBoxesCanBeOpened >= 5)
                return warboxes[boxID];
            int minExperience = minExperienceForBox * counBoxesCanBeOpened;
            if (Global.settingsMain.neededRankExperience - Global.userLogic.GetUserInfo().InGameInfo.experience <= minExperience)
                return warboxes[boxID];
            return null;
        }

        protected override void LoadWarBoxes()
        {


            if (File.Exists(filePath))
                warboxes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Warbox>>(File.ReadAllText(filePath), new CustomDateTimeConverter());
            if (warboxes != null)
                return;
            warboxes = new List<Warbox>();
            warboxes.Add(new Warbox()
            {
                currency = WFCurrency.WB,
                discountForNextBox = 25,
                firstBoxPrice = 425,
                numInShop = 1,
                rankAvailable = 2,
                title = "знаки"
            });
            File.WriteAllText(filePath, Newtonsoft.Json.JsonConvert.SerializeObject(warboxes, new CustomDateTimeConverter()));
        }
    }

    public class WarBoxesForDonate : WarboxesBase
    {
        public WarBoxesForDonate()
        {
            filePath = "WarBoxesForDonate.json";
            LoadWarBoxes();
        }

        override protected Warbox GetWarBoxForOpening()
        {
            int counBoxesCanBeOpened;
            if (!CanOpenSomething(out counBoxesCanBeOpened))
                return null;
            return warboxes[boxID];
        }

        protected override void LoadWarBoxes()
        {
            if (File.Exists(filePath))
                warboxes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Warbox>>(File.ReadAllText(filePath), new CustomDateTimeConverter());
            if (warboxes != null)
                return;
            warboxes = new List<Warbox>();
            warboxes.Add(new Warbox()
            {
                currency = WFCurrency.Crowns,
                discountForNextBox = 30,
                firstBoxPrice = 800,
                numInShop = 1,
                rankAvailable = 11,
                title = "Uzkon"
            });
            File.WriteAllText(filePath, Newtonsoft.Json.JsonConvert.SerializeObject(warboxes, new CustomDateTimeConverter()));
        }
    }
}
