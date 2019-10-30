using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using RestSharp.Authenticators;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Text.RegularExpressions;
namespace text
{

    /// <summary>
    /// 自动理牌
    /// </summary>
    public class CardModel
    {
        public List<TypeCard> typeCardList;

        public CardModel()
        {
            typeCardList = new List<TypeCard>();
        }

    }

    /// <summary>
    /// 牌型
    /// </summary>
    public enum DeckTypeEnum : byte
    {
        /// <summary>
        /// 未识别
        /// </summary>
        Error = 255,
        /// <summary>
        /// 过牌为空
        /// </summary>
        None = 0,
        /// <summary>
        /// 高牌-散牌-乌龙1
        /// </summary>
        Single,
        /// <summary>
        /// 对子2
        /// </summary>
        Double,
        /// <summary>
        /// 两对3
        /// </summary>
        TwoDouble,
        /// <summary>
        /// 三张4
        /// </summary>
        Three,
        /// <summary>
        /// 顺子5
        /// </summary>
        ShunZi,
        /// <summary>
        /// 同花6
        /// </summary>
        TongHua,
        /// <summary>
        /// 葫芦7
        /// </summary>
        Gourd,
        /// <summary>
        /// 炸弹8
        /// </summary>
        Bomb,
        /// <summary>
        /// 同花顺9
        /// </summary>
        TongHuaShun,
        /// <summary>
        /// 五炸10
        /// </summary>
        FiveBomb
    }

    public class TypeCard
    {
        public DeckTypeEnum cardType;//牌型
        public List<int> cardList;//手牌
    }


}
namespace text
{
    class Program
    {

        /// <summary>0
        /// 牌面从大到小排序
        /// </summary>
        public static void SortCard(List<int> cards)
        {
            //牌面从大到小排序
            cards.Sort((b, a) =>
            {
                int result = (a % 100) - (b % 100);
                if (result == 0)
                {
                    result = (a / 100) - (b / 100);
                }
                return result;
            });
        }

        /// <summary>
        /// 牌面从小到大排序
        /// </summary>
        public static void SortCardMinToMax(List<int> cards)
        {
            //面大小排序
            cards.Sort((a, b) =>
            {
                int result = (a % 100) - (b % 100);
                if (result == 0)
                {
                    result = (b / 100) - (a / 100);
                }
                return result;
            });
        }


        /// <summary>
        /// 生成牌信息
        /// </summary>
        /// <param name="IsCheat">是否带大小王（癞子）</param>
        /// <param name="AddColor">加几色玩法</param>
        /// <returns></returns>
        public static List<int> RuffleCard(bool IsCheat, int AddColor)
        {
            int[] idArr;

            if (IsCheat)
            {
                //带王
                if (AddColor == 1)
                {
                    idArr = new int[] {
                    102,103,104,105,106,107,108,109,110,111,112,113,114,
                    202,203,204,205,206,207,208,209,210,211,212,213,214,
                    302,303,304,305,306,307,308,309,310,311,312,313,314,
                    402,403,404,405,406,407,408,409,410,411,412,413,414,
                    404,405,406,407,408,409,410,411,412,413,414,
                    616,
                    717
                    };
                }
                else if (AddColor == 2)
                {
                    idArr = new int[] {
                    102,103,104,105,106,107,108,109,110,111,112,113,114,
                    202,203,204,205,206,207,208,209,210,211,212,213,214,
                    302,303,304,305,306,307,308,309,310,311,312,313,314,
                    302,303,304,305,306,307,308,309,310,311,312,313,314,
                    402,403,404,405,406,407,408,409,410,411,412,413,414,
                    404,405,406,407,408,409,410,411,412,413,414,
                    616,
                    717
                    };
                }
                else
                {
                    idArr = new int[] {
                    102,103,104,105,106,107,108,109,110,111,112,113,114,
                    202,203,204,205,206,207,208,209,210,211,212,213,214,
                    302,303,304,305,306,307,308,309,310,311,312,313,314,
                    404,405,406,407,408,409,410,411,412,413,414,
                    616,
                    717
                    };
                }

            }
            else
            {
                if (AddColor == 1)
                {
                    idArr = new int[] {
                    102,103,104,105,106,107,108,109,110,111,112,113,114,
                    202,203,204,205,206,207,208,209,210,211,212,213,214,
                    302,303,304,305,306,307,308,309,310,311,312,313,314,
                    402,403,404,405,406,407,408,409,410,411,412,413,414,
                    402,403,404,405,406,407,408,409,410,411,412,413,414
                    };
                }
                else if (AddColor == 2)
                {
                    idArr = new int[] {
                    102,103,104,105,106,107,108,109,110,111,112,113,114,
                    202,203,204,205,206,207,208,209,210,211,212,213,214,
                    302,303,304,305,306,307,308,309,310,311,312,313,314,
                    302,303,304,305,306,307,308,309,310,311,312,313,314,
                    402,403,404,405,406,407,408,409,410,411,412,413,414,
                    402,403,404,405,406,407,408,409,410,411,412,413,414
                    };
                }
                else
                {
                    idArr = new int[] {
                    102,103,104,105,106,107,108,109,110,111,112,113,114,
                    202,203,204,205,206,207,208,209,210,211,212,213,214,
                    302,303,304,305,306,307,308,309,310,311,312,313,314,
                    402,403,404,405,406,407,408,409,410,411,412,413,414
                    };
                }

            }

            List<int> resCardIds = new List<int>();
            List<int> cardIds = idArr.ToList<int>();
            Random rand = new Random();
            while (cardIds.Count > 0)
            {
                int randNum = rand.Next(cardIds.Count);
                resCardIds.Add(cardIds[randNum]);
                cardIds.RemoveAt(randNum);
            }

            return resCardIds;
        }

        /// <summary>
        /// 获取一个玩家的手牌 13张
        /// </summary>
        /// <returns></returns>
        public static List<int> GetOnePlayerCard()
        {
            List<int> CardList = RuffleCard(false, 0);

            return CardList.GetRange(0, 13);
        }

        /// <summary>
        /// 计算一副牌里面的所有可能牌型
        /// </summary>
        /// <param name="cardList"></param>
        /// <returns></returns>
        public static List<TypeCard> GetMaxCardType(List<int> cardList)
        {
            List<TypeCard> typeCardList = new List<TypeCard>();

            //从大到小排序
            SortCard(cardList);

            //复制一份从小到大的牌
            List<int> cardList2 = cardList.GetRange(0, cardList.Count);
            SortCardMinToMax(cardList2);

            //鬼牌数量
            int smallKingNum = 616;//小王
            int bigKingNum = 717;//大王
            int KingNum = 0;
            KingNum += cardList.Contains(smallKingNum) ? 1 : 0;
            KingNum += cardList.Contains(bigKingNum) ? 1 : 0;
            int king = cardList.Contains(smallKingNum) ? smallKingNum : bigKingNum;//单王

            List<int> FiveNum = cardList.GroupBy(p => p % 100).Where(p => p.Count() >= 5).Select(p => p.Key).ToList();
            List<int> FourNum = cardList.GroupBy(p => p % 100).Where(p => p.Count() >= 4).Select(p => p.Key).ToList();
            List<int> ThreeNum = cardList.GroupBy(p => p % 100).Where(p => p.Count() >= 3).Select(p => p.Key).ToList();
            List<int> TwoNum = cardList.GroupBy(p => p % 100).Where(p => p.Count() >= 2).Select(p => p.Key).ToList();

            #region 五炸
            foreach (var Fiveitem in FiveNum)
            {
                List<int> tempList = new List<int>();
                foreach (var item in cardList)
                {
                    if (item % 100 == Fiveitem)
                    {
                        tempList.Add(item);
                    }

                    if (tempList.Count >= 5)
                    {
                        TypeCard typeCard = new TypeCard() { cardList = tempList, cardType = DeckTypeEnum.FiveBomb };
                        typeCardList.Add(typeCard);
                        break;
                    }
                }

            }

            if (KingNum >= 1)
            {
                foreach (var FourItem in FourNum)
                {
                    List<int> tempList = new List<int>();
                    foreach (var item in cardList)
                    {
                        if (item % 100 == FourItem)
                        {
                            tempList.Add(item);
                        }

                        if (tempList.Count >= 4)
                        {
                            tempList.Add(king);
                            TypeCard typeCard = new TypeCard() { cardList = tempList, cardType = DeckTypeEnum.FiveBomb };
                            typeCardList.Add(typeCard);
                            break;
                        }
                    }

                }
            }

            if (KingNum == 2)
            {
                foreach (var ThreeItem in ThreeNum)
                {
                    List<int> tempList = new List<int>();
                    foreach (var item in cardList)
                    {
                        if (item % 100 == ThreeItem)
                        {
                            tempList.Add(item);
                        }

                        if (tempList.Count >= 3)
                        {
                            tempList.Add(smallKingNum);
                            tempList.Add(bigKingNum);

                            TypeCard typeCard = new TypeCard() { cardList = tempList, cardType = DeckTypeEnum.FiveBomb };
                            typeCardList.Add(typeCard);
                            break;
                        }
                    }

                }
            }
            #endregion

            #region 同花顺
            foreach (var item in cardList2)
            {
                if (item % 100 >= 11)
                {
                    break;
                }

                List<int> tempList = new List<int>();
                tempList.Add(item);

                foreach (var item2 in cardList2)
                {
                    if (item2 - 1 == tempList.Last())
                    {
                        tempList.Add(item2);
                    }

                    if (tempList.Count >= 5)
                    {
                        TypeCard typeCard = new TypeCard() { cardList = tempList, cardType = DeckTypeEnum.TongHuaShun };
                        typeCardList.Add(typeCard);
                        break;
                    }
                }
            }

            if (KingNum >= 1)
            {
                foreach (var item in cardList2)
                {
                    if (item % 100 >= 12)
                    {
                        break;
                    }

                    List<int> tempList = new List<int>();
                    tempList.Add(item);

                    foreach (var item2 in cardList2)
                    {
                        if (item2 - 1 == tempList.Last())
                        {
                            tempList.Add(item2);
                        }

                        if (tempList.Count >= 4)
                        {
                            tempList.Add(king);
                            TypeCard typeCard = new TypeCard() { cardList = tempList, cardType = DeckTypeEnum.TongHuaShun };
                            typeCardList.Add(typeCard);
                            break;
                        }
                    }
                }
            }

            if (KingNum == 2)
            {
                foreach (var item in cardList2)
                {
                    if (item % 100 >= 13)
                    {
                        break;
                    }

                    List<int> tempList = new List<int>();
                    tempList.Add(item);

                    foreach (var item2 in cardList2)
                    {
                        if (item2 - 1 == tempList.Last())
                        {
                            tempList.Add(item2);
                        }

                        if (tempList.Count >= 3)
                        {
                            tempList.Add(smallKingNum);
                            tempList.Add(bigKingNum);

                            TypeCard typeCard = new TypeCard() { cardList = tempList, cardType = DeckTypeEnum.TongHuaShun };
                            typeCardList.Add(typeCard);
                            break;
                        }
                    }
                }
            }
            #endregion

            #region 炸弹
            foreach (var FourItem in FourNum)
            {
                List<int> tempList = new List<int>();
                foreach (var item in cardList)
                {
                    if (item % 100 == FourItem)
                    {
                        tempList.Add(item);
                    }

                    if (tempList.Count >= 4)
                    {
                        TypeCard typeCard = new TypeCard() { cardList = tempList, cardType = DeckTypeEnum.Bomb };
                        typeCardList.Add(typeCard);
                        break;
                    }
                }

            }

            if (KingNum >= 1)
            {
                foreach (var ThreeItem in ThreeNum)
                {
                    List<int> tempList = new List<int>();
                    foreach (var item in cardList)
                    {
                        if (item % 100 == ThreeItem)
                        {
                            tempList.Add(item);
                        }

                        if (tempList.Count >= 3)
                        {

                            tempList.Add(king);
                            TypeCard typeCard = new TypeCard() { cardList = tempList, cardType = DeckTypeEnum.Bomb };
                            typeCardList.Add(typeCard);
                            break;
                        }
                    }

                }
            }

            if (KingNum == 2)
            {
                foreach (var TwoeeItem in TwoNum)
                {
                    List<int> tempList = new List<int>();
                    foreach (var item in cardList)
                    {
                        if (item % 100 == TwoeeItem)
                        {
                            tempList.Add(item);
                        }

                        if (tempList.Count >= 2)
                        {
                            tempList.Add(smallKingNum);
                            tempList.Add(bigKingNum);

                            TypeCard typeCard = new TypeCard() { cardList = tempList, cardType = DeckTypeEnum.Bomb };
                            typeCardList.Add(typeCard);
                            break;
                        }
                    }

                }
            }
            #endregion

            #region 葫芦
            foreach (var ThreeItem in ThreeNum)
            {
                List<int> tempList = new List<int>();
                foreach (var item in cardList)
                {
                    if (item % 100 == ThreeItem)
                    {
                        tempList.Add(item);
                    }

                    if (tempList.Count >= 3)
                    {
                        //找两对
                        foreach (var TwoItem in TwoNum)
                        {
                            if (ThreeItem != TwoItem)
                            {
                                foreach (var AllItem in cardList)
                                {
                                    if (AllItem % 100 != ThreeItem && AllItem % 100 == TwoItem)
                                    {
                                        tempList.Add(AllItem);
                                    }

                                    if (tempList.Count >= 5)
                                    {
                                        TypeCard typeCard = new TypeCard() { cardList = tempList, cardType = DeckTypeEnum.Gourd };
                                        typeCardList.Add(typeCard);
                                        tempList = typeCard.cardList.GetRange(0, 3);
                                        break;
                                    }
                                }


                            }

                        }
                        break;
                    }
                }
            }

            if (KingNum >= 1)
            {
                foreach (var ThreeItem in TwoNum)
                {
                    List<int> tempList = new List<int>();
                    foreach (var item in cardList)
                    {
                        if (item % 100 == ThreeItem)
                        {
                            tempList.Add(item);
                        }

                        if (tempList.Count >= 2)
                        {
                            //找两对
                            foreach (var TwoItem in TwoNum)
                            {
                                if (ThreeItem != TwoItem)
                                {
                                    foreach (var AllItem in cardList)
                                    {
                                        if (AllItem % 100 != ThreeItem && AllItem % 100 == TwoItem)
                                        {
                                            tempList.Add(AllItem);
                                        }

                                        if (tempList.Count >= 4)
                                        {
                                            tempList.Add(king);
                                            TypeCard typeCard = new TypeCard() { cardList = tempList, cardType = DeckTypeEnum.Gourd };
                                            typeCardList.Add(typeCard);
                                            tempList = typeCard.cardList.GetRange(0, 2);
                                            break;
                                        }
                                    }


                                }

                            }
                            break;
                        }
                    }
                }
            }

            if (KingNum == 2)
            {
                //不用算
            }

            #endregion

            #region 同花
            List<int> FiveColor = cardList.GroupBy(p => p / 100).Where(p => p.Count() >= 5).Select(p => p.Key).ToList();
            List<int> FourColor = cardList.GroupBy(p => p / 100).Where(p => p.Count() >= 4).Select(p => p.Key).ToList();
            List<int> ThreeColor = cardList.GroupBy(p => p / 100).Where(p => p.Count() >= 3).Select(p => p.Key).ToList();

            foreach (var Fiveitem in FiveColor)
            {
                List<int> tempList = new List<int>();
                foreach (var item in cardList)
                {
                    if (item / 100 == Fiveitem)
                    {
                        tempList.Add(item);
                    }
                }

                while (true)
                {
                    if (tempList.Count >= 5)
                    {
                        TypeCard typeCard = new TypeCard() { cardList = tempList.GetRange(0, 5), cardType = DeckTypeEnum.TongHua };
                        typeCardList.Add(typeCard);
                        tempList.RemoveAt(0);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (KingNum >= 1)
            {
                foreach (var Fouritem in FourColor)
                {
                    List<int> tempList = new List<int>();
                    foreach (var item in cardList)
                    {
                        if (item / 100 == Fouritem)
                        {
                            tempList.Add(item);
                        }
                    }

                    while (true)
                    {
                        if (tempList.Count >= 4)
                        {
                            TypeCard typeCard = new TypeCard() { cardList = tempList.GetRange(0, 4), cardType = DeckTypeEnum.TongHua };
                            typeCard.cardList.Add(king);
                            typeCardList.Add(typeCard);
                            tempList.RemoveAt(0);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            if (KingNum == 2)
            {
                foreach (var Threeitem in ThreeColor)
                {
                    List<int> tempList = new List<int>();
                    foreach (var item in cardList)
                    {
                        if (item / 100 == Threeitem)
                        {
                            tempList.Add(item);
                        }
                    }

                    while (true)
                    {
                        if (tempList.Count >= 3)
                        {
                            TypeCard typeCard = new TypeCard() { cardList = tempList.GetRange(0, 3), cardType = DeckTypeEnum.TongHua };
                            typeCard.cardList.Add(smallKingNum);
                            typeCard.cardList.Add(bigKingNum);
                            typeCardList.Add(typeCard);
                            tempList.RemoveAt(0);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            #endregion

            #region 顺子

            foreach (var MaxItem in cardList)
            {

                if (MaxItem % 100 <= 5)
                {
                    break;//从大到小，后面的也不需要循环了
                }

                List<int> tempList = new List<int>();
                tempList.Add(MaxItem);

                while (tempList.Count < 5)
                {

                    bool flag = false;

                    foreach (var item in cardList)
                    {
                        if (item % 100 + 1 == tempList.Last() % 100)
                        {
                            tempList.Add(item);
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        break;
                    }
                }

                if (tempList.Count >= 5)
                {
                    TypeCard typeCard = new TypeCard() { cardList = tempList, cardType = DeckTypeEnum.ShunZi };
                    typeCardList.Add(typeCard);
                }
            }

            if (KingNum >= 1)
            {
                foreach (var MaxItem in cardList)
                {

                    if (MaxItem % 100 <= 4)
                    {
                        break;//从大到小，后面的也不需要循环了
                    }

                    List<int> tempList = new List<int>();
                    tempList.Add(MaxItem);

                    while (tempList.Count < 4)
                    {

                        bool flag = false;

                        foreach (var item in cardList)
                        {
                            if (item % 100 + 1 == tempList.Last() % 100)
                            {
                                tempList.Add(item);
                                flag = true;
                                break;
                            }
                        }
                        if (!flag)
                        {
                            break;
                        }
                    }

                    tempList.Add(king);
                    if (tempList.Count >= 5)
                    {
                        TypeCard typeCard = new TypeCard() { cardList = tempList, cardType = DeckTypeEnum.ShunZi };
                        typeCardList.Add(typeCard);
                    }


                }
            }

            if (KingNum == 2)
            {
                foreach (var MaxItem in cardList)
                {

                    if (MaxItem % 100 <= 3)
                    {
                        break;//从大到小，后面的也不需要循环了
                    }

                    List<int> tempList = new List<int>();
                    tempList.Add(MaxItem);

                    while (tempList.Count < 3)
                    {

                        bool flag = false;

                        foreach (var item in cardList)
                        {
                            if (item % 100 + 1 == tempList.Last() % 100)
                            {
                                tempList.Add(item);
                                flag = true;
                                break;
                            }
                        }
                        if (!flag)
                        {
                            break;
                        }
                    }

                    tempList.Add(smallKingNum);
                    tempList.Add(bigKingNum);
                    if (tempList.Count >= 5)
                    {
                        TypeCard typeCard = new TypeCard() { cardList = tempList, cardType = DeckTypeEnum.ShunZi };
                        typeCardList.Add(typeCard);
                    }


                }
            }

            #endregion

            #region 三张
            foreach (var Threeitem in ThreeNum)
            {
                List<int> tempList = new List<int>();
                foreach (var item in cardList)
                {
                    if (item % 100 == Threeitem)
                    {
                        tempList.Add(item);
                    }

                    if (tempList.Count >= 3)
                    {
                        TypeCard typeCard = new TypeCard() { cardList = tempList, cardType = DeckTypeEnum.Three };
                        typeCardList.Add(typeCard);
                        break;
                    }
                }
            }

            if (KingNum >= 1)
            {
                foreach (var Twoitem in TwoNum)
                {
                    List<int> tempList = new List<int>();
                    foreach (var item in cardList)
                    {
                        if (item % 100 == Twoitem)
                        {
                            tempList.Add(item);
                        }

                        if (tempList.Count >= 2)
                        {
                            tempList.Add(king);
                            TypeCard typeCard = new TypeCard() { cardList = tempList, cardType = DeckTypeEnum.Three };
                            typeCardList.Add(typeCard);
                            break;
                        }
                    }
                }
            }

            if (KingNum == 2)
            {
                foreach (var item in cardList)
                {
                    List<int> tempList = new List<int>();
                    if (item != smallKingNum || item != bigKingNum)
                    {
                        tempList.Add(smallKingNum);
                        tempList.Add(bigKingNum);
                        TypeCard typeCard = new TypeCard() { cardList = tempList, cardType = DeckTypeEnum.Three };
                        typeCardList.Add(typeCard);
                    }
                }
            }

            #endregion

            #region 两对

            for (int i = 0; i < TwoNum.Count; i++)
            {
                List<int> tempList = new List<int>();

                foreach (var item in cardList)
                {
                    if (item % 100 == TwoNum[i])
                    {
                        tempList.Add(item);
                    }
                }

                while (tempList.Count >= 2)
                {

                    for (int j = i + 1; j < TwoNum.Count; j++)
                    {
                        List<int> tempList2 = new List<int>();
                        foreach (var item in cardList)
                        {
                            if (item % 100 == TwoNum[j])
                            {
                                tempList2.Add(item);
                            }
                        }

                        while ((tempList2.Count >= 2))
                        {
                            TypeCard typeCard = new TypeCard() { cardList = tempList.GetRange(0, 2), cardType = DeckTypeEnum.TwoDouble };
                            typeCard.cardList.AddRange(tempList2.GetRange(0, 2));
                            typeCardList.Add(typeCard);
                            tempList2.RemoveAt(0);
                        }

                    }

                    tempList.RemoveAt(0);

                }

            }

            if (KingNum >= 1)
            {
                //不需要
            }

            if (KingNum == 2)
            {
                //不需要
            }

            #endregion

            #region 对子

            foreach (var Twoitem in TwoNum)
            {
                List<int> tempList = new List<int>();

                foreach (var item in cardList)
                {
                    if (item % 100 == Twoitem)
                    {
                        tempList.Add(item);
                    }
                }

                while (tempList.Count >= 2)
                {
                    TypeCard typeCard = new TypeCard() { cardList = tempList.GetRange(0, 2), cardType = DeckTypeEnum.Double };
                    typeCardList.Add(typeCard);
                    tempList.RemoveAt(0);
                }
            }


            if (KingNum >= 1)
            {
                foreach (var item in cardList)
                {
                    List<int> tempList = new List<int>();
                    if (item != smallKingNum || item != bigKingNum)
                    {
                        tempList.Add(king);
                        TypeCard typeCard = new TypeCard() { cardList = tempList, cardType = DeckTypeEnum.Double };
                        typeCardList.Add(typeCard);
                    }
                }
            }

            if (KingNum == 2)
            {
                //不用算
            }

            #endregion


            //乌龙-只拿三张牌
            if (true)
            {
                if (cardList.Count < 3)
                {
                    Console.WriteLine("*****************************************************************最后剩牌不足三张");
                }
                else
                {
                    List<int> tempList = new List<int>();
                    tempList = cardList.GetRange(0, 3);
                    TypeCard typeCard = new TypeCard() { cardList = tempList, cardType = DeckTypeEnum.Single };
                    typeCardList.Add(typeCard);
                }


            }

            return typeCardList;
        }


        /// <summary>
        /// 删除元素
        /// </summary>
        /// <param name="list"></param>
        /// <param name="element"></param>
        public static void DeleteListElement(List<int> list, List<int> element)
        {
            foreach (var item in element)
            {
                list.Remove(item);
            }
        }


        /// <summary>
        /// 得到所有自动牌型
        /// </summary>
        /// <param name="cmlist"></param>
        /// <param name="CardList"></param>
        /// <returns></returns>
        public static List<CardModel> GetAllResult(List<CardModel> cmlist, List<int> CardList)
        {
            CardModel cm = new CardModel();
            List<TypeCard> typeCardList = new List<TypeCard>();

            //新建一个副本
            List<int> newCardlist = CardList.Select(p => p).ToList();

            //第一个
            if (cmlist.Count == 0)
            {
                while (true)
                {

                    typeCardList = GetMaxCardType(newCardlist);

                    //首道只能有三张牌
                    if (cm.typeCardList.Count == 2 && typeCardList[0].cardList.Count > 3)
                    {
                        typeCardList = GetMaxCardType(typeCardList[0].cardList.GetRange(0, 3));
                    }

                    cm.typeCardList.Add(typeCardList[0]);
                    DeleteListElement(newCardlist, typeCardList[0].cardList);

                    //补全少的牌
                    if (cm.typeCardList.Count >= 3)
                    {
                        for (int i = 0; i < cm.typeCardList.Count; i++)
                        {
                            if (i == 2)
                            {
                                while (cm.typeCardList[i].cardList.Count < 3)
                                {
                                    cm.typeCardList[i].cardList.Add(newCardlist[0]);
                                    newCardlist.RemoveAt(0);
                                }
                            }
                            else
                            {
                                while (cm.typeCardList[i].cardList.Count < 5)
                                {
                                    cm.typeCardList[i].cardList.Add(newCardlist[0]);
                                    newCardlist.RemoveAt(0);
                                }
                            }

                        }

                        break;
                    }
                }

                cmlist.Add(cm);

                GetAllResult(cmlist, CardList);
            }


            //其他自动
            if (cmlist.Count < 4)
            {
                int SingleCount = 0;//乌龙次数
                while (true)
                {
                    typeCardList = GetMaxCardType(newCardlist);
                    int index = cmlist.Count;//第几组自动
                    int typeCardListIndex = 0;

                    if (cm.typeCardList.Count == 3)
                    {

                    }
                    else if (cm.typeCardList.Count == 0)
                    {
                        if (typeCardList.Count < index)
                        {
                            return cmlist;
                        }

                        cm.typeCardList.Add(typeCardList[index]);
                        DeleteListElement(newCardlist, typeCardList[index].cardList);
                    }
                    else
                    {
                        //前道的牌不能大于后道
                        while (true)
                        {
                            if (typeCardList[typeCardListIndex].cardType > cm.typeCardList.Last().cardType)
                            {
                                typeCardListIndex++;
                                continue;
                            }
                            if (typeCardList[typeCardListIndex].cardType == cm.typeCardList.Last().cardType)
                            {
                                if (typeCardList[typeCardListIndex].cardList.Max() >= cm.typeCardList.Last().cardList.Max())
                                {
                                    return cmlist;
                                }
                            }

                            break;
                        }

                        //首道只能有三张牌
                        if (cm.typeCardList.Count == 2 && typeCardList[typeCardListIndex].cardList.Count > 3)
                        {
                            typeCardList = GetMaxCardType(typeCardList[typeCardListIndex].cardList.GetRange(0, 3));
                            cm.typeCardList.Add(typeCardList[0]);
                            DeleteListElement(newCardlist, typeCardList[0].cardList);
                        }
                        else
                        {
                            cm.typeCardList.Add(typeCardList[typeCardListIndex]);
                            DeleteListElement(newCardlist, typeCardList[typeCardListIndex].cardList);
                        }

                    }

                    //补全不够牌的
                    if (cm.typeCardList.Count >= 3)
                    {
                        for (int i = 0; i < cm.typeCardList.Count; i++)
                        {
                            if (i == 2)
                            {
                                while (cm.typeCardList[i].cardList.Count < 3)
                                {
                                    cm.typeCardList[i].cardList.Add(newCardlist[0]);
                                    newCardlist.RemoveAt(0);
                                }
                            }
                            else
                            {
                                while (cm.typeCardList[i].cardList.Count < 5)
                                {
                                    cm.typeCardList[i].cardList.Add(newCardlist[0]);
                                    newCardlist.RemoveAt(0);
                                }
                            }

                            if (cm.typeCardList[i].cardType == DeckTypeEnum.Single)
                            {
                                SingleCount++;
                            }

                        }

                        break;
                    }
                }

                cmlist.Add(cm);

                if (SingleCount >= 2)
                {
                    return cmlist;
                }

                GetAllResult(cmlist, CardList);

            }

            return cmlist;
        }
        static string Post1(string url,string postarray,string type,string k )
        {   
            var client = new RestClient(url);
            var request = new RestRequest(Method.POST);
            request.AddHeader(type, k);
            request.AddParameter("application/json",postarray, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            string right = response.Content;
            return right;
        }
        static string Post2(string url, string type, string k)
        {
            var client = new RestClient(url);
            var request = new RestRequest(Method.POST);
            request.AddHeader(type, k);
            request.AddParameter("application/json", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            string right = response.Content;
            return right;
        }
        static string Post3(string url, string type, string k,string type2,string k2,string postarray)
        {
            var client = new RestClient(url);
            var request = new RestRequest(Method.POST);
            request.AddHeader(type, k);
            request.AddHeader(type2, k2);
            request.AddParameter("application/json",postarray, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            string right = response.Content;
            return right;
        }

        static string Get(string url,string type,string k)
        {
            var client = new RestClient(url);
            var request = new RestRequest(Method.GET);
            request.AddHeader(type, k);
            IRestResponse response = client.Execute(request);
            string right = response.Content;
            return right;


        }
        static string Get2(string url)
        {
            var client = new RestClient(url);
            var request = new RestRequest(Method.GET);
       
            IRestResponse response = client.Execute(request);
            string right = response.Content;
            return right;


        }

        static void Main(string[] args)
        {
            int y = 0;
            int j = 0;
            string a = Post1("http://api.revth.com/auth/login", "{\"username\":\"jiage\",\"password\":\"123456\"}", "Content-Type", "application/json");
            Console.WriteLine(a);
            JObject right = JObject.Parse(a);
            string data = right["data"].ToString();
            JObject yes = JObject.Parse(data);
            string tokens = yes["token"].ToString();
            string b = Get("http://api.revth.com/auth/validate", "X-Auth-Token", tokens);
            Console.WriteLine(b);
            string c = Post2("http://api.revth.com/game/open", "X-Auth-Token", tokens);
            Console.WriteLine(c);
            JObject fight1 = JObject.Parse(c);
            string data2 = fight1["data"].ToString();
            JObject fight2 = JObject.Parse(data2);
            string cardlist1 = fight2["card"].ToString();
            Console.WriteLine(cardlist1);
            string id = fight2["id"].ToString();
            cardlist1 = cardlist1.Replace("2", "02");
            cardlist1 = cardlist1.Replace("3", "03");
            cardlist1 = cardlist1.Replace("4", "04");
            cardlist1 = cardlist1.Replace("5", "05");
            cardlist1 = cardlist1.Replace("6", "06");
            cardlist1 = cardlist1.Replace("7", "07");
            cardlist1 = cardlist1.Replace("8", "08");
            cardlist1 = cardlist1.Replace("9", "09");
            cardlist1 = cardlist1.Replace("$", "1");
            cardlist1 = cardlist1.Replace("&", "2");
            cardlist1 = cardlist1.Replace("*", "3");
            cardlist1 = cardlist1.Replace("#", "4");
            cardlist1 = cardlist1.Replace("J", "11");
            cardlist1 = cardlist1.Replace("Q", "12");
            cardlist1 = cardlist1.Replace("K", "13");
            cardlist1 = cardlist1.Replace("A", "14");
            string[] sArray = Regex.Split(cardlist1, " ", RegexOptions.IgnoreCase);

            foreach (var item in sArray)
            {
                Console.WriteLine(sArray[y++]);

            }
            y = 0;
            int[] cardl = new int[13];
            for (y = 0; y < 13; y++)
            {
                cardl[y] = int.Parse(sArray[y]);
            }
            for (y = 0; y < 13; y++)
            {
                Console.WriteLine(cardl[y]);
            }

            List<CardModel> cmlist = new List<CardModel>();
            List<int> CardList = new List<int>();
            for (y = 0; y < 13; y++)
            {
                CardList.Add(cardl[y]);
            }

            //13张手牌
            //List<int> CardList = new List<int>() { 102,203,105,104,306,211,312,413,208,204,311,106,313 };//13张手牌
            cmlist = GetAllResult(cmlist, CardList);

            Console.WriteLine("玩家手牌信息为：");

            foreach (var item in CardList)
            {
                Console.WriteLine(item);
            }

            Console.WriteLine("自动计算的牌型信息：");
            Console.WriteLine("");


            Console.WriteLine("按顺序输出：");
            string[] depaid = new string[3] { "", "", "" };


            foreach (var item2 in cmlist[0].typeCardList)
            { depaid[j] = string.Join(",", item2.cardList);

                int i;
                int n = depaid[j].Length;
                for (i = 0; i < n; i += 4)
                {
                    if (depaid[j][i] == '1')
                    {

                        depaid[j] = depaid[j].Remove(i, 1);
                        depaid[j] = depaid[j].Insert(i, "$");
                    }

                    else if (depaid[j][i] == '2')
                    {
                        depaid[j] = depaid[j].Remove(i, 1);
                        depaid[j] = depaid[j].Insert(i, "&");
                    }
                    else if (depaid[j][i] == '3')
                    {
                        depaid[j] = depaid[j].Remove(i, 1);
                        depaid[j] = depaid[j].Insert(i, "*");
                    }
                    else if (depaid[j][i] == '4')
                    {
                        depaid[j] = depaid[j].Remove(i, 1);
                        depaid[j] = depaid[j].Insert(i, "#");
                    }
                }
                depaid[j] = depaid[j].Replace("14", "A");
                depaid[j] = depaid[j].Replace("11", "J");
                depaid[j] = depaid[j].Replace("12", "Q");
                depaid[j] = depaid[j].Replace("13", "K");
                depaid[j] = depaid[j].Replace("02", "2");
                depaid[j] = depaid[j].Replace("03", "3");
                depaid[j] = depaid[j].Replace("04", "4");
                depaid[j] = depaid[j].Replace("05", "5");
                depaid[j] = depaid[j].Replace("06", "6");
                depaid[j] = depaid[j].Replace("07", "7");
                depaid[j] = depaid[j].Replace("08", "8");
                depaid[j] = depaid[j].Replace("09", "9");
                Console.WriteLine(item2.cardType);

                Console.WriteLine(depaid[j]);
                j++;
            } int id1;
            id1 = int.Parse(id);
            JObject pj = new JObject();
            pj.Add("id", id1);
            depaid[0] = depaid[0].Replace(",", " ");
            depaid[0].Insert(depaid[0].Length, ",");
            depaid[1] = depaid[1].Replace(",", " ");
            depaid[1].Insert(depaid[1].Length, ",");
            depaid[2] = depaid[2].Replace(",", " ");
            depaid[2].Insert(depaid[2].Length, ",");


            JArray jarray = new JArray();
            jarray.Add(depaid[2]);
            jarray.Add(depaid[1]);
            jarray.Add(depaid[0]);

     
            string car = depaid[2] + depaid[1] + depaid[0];
            pj.Add("card", jarray);
            
            string ps = pj.ToString(Newtonsoft.Json.Formatting.None,null);
            Console.WriteLine(ps);
            string d = Post3("http://api.revth.com/game/submit",  "Content-Type", "application/json","X-Auth-Token", tokens,ps);
            Console.WriteLine(d);
            
            string e = Get2("http://api.revth.com/rank");
            Console.WriteLine(e);
            Console.ReadLine();Console.WriteLine("");
        }
        

    }
}
