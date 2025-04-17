using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting;
using System.Text;

namespace SpartaTextRpg
{
    public enum Job
    {
        전사 = 1,
        법사,
        궁수
    }

    public enum Menu
    {
        상태보기 = 1,
        인벤토리,
        상점
    }

    public struct ItemStat
    {
        private int attack;
        public int ATTACK
        {
            get { return attack; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Attack cannot be negative.");
                }
                attack = value;
            }
        }

        private int defense;
        public int DEFENSE
        {
            get { return defense; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Defense cannot be negative.");
                }
                defense = value;
            }
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            if (attack > 0)
            {
                str.Append($"공격력 +{attack}");
            }
            if (attack > 0 && defense > 0)
            {
                str.Append(" , ");
            }
            if (defense > 0)
            {
                str.Append($"방어력 +{defense}");
            }
            return str.ToString();
        }
        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (!(obj is ItemStat)) return false;
            ItemStat its = (ItemStat)obj;
            return (its.ATTACK == this.ATTACK) && (its.DEFENSE == this.DEFENSE);

        }

    }
    internal class Program
    {
        static void Main(string[] args)
        {
            GameManager gm = new GameManager();
            gm.StartGame();
        }
    }

    class GameManager
    {
        private string? playerInput;
        private Player player;
        private ItemDB db;
        private Market mk;
        public void StartGame()
        {
            Init();

        }
        private void SelectJob()
        {
            bool isJobNull = true;
            Job job;
            while (isJobNull)
            {
                Console.WriteLine("원하시는 직업을 선택해 주세요.");
                Console.WriteLine("1.전사\t2.법사\t3.궁수\n");
                playerInput = Console.ReadLine();
                Console.Write("\n");
                if (playerInput != null)
                {
                    try
                    {
                        job = (Job)int.Parse(playerInput);
                    }
                    catch (FormatException e)
                    {
                        Console.WriteLine("잘못된 입력입니다. 다시 입력해 주세요.\n");
                        continue;
                    }
                    switch (job)
                    {
                        case Job.전사:
                            player.JOB = Job.전사;
                            break;
                        case Job.법사:
                            player.JOB = Job.법사;
                            break;
                        case Job.궁수:
                            player.JOB = Job.궁수;
                            break;
                        default:
                            Console.WriteLine("잘못된 입력입니다. 다시 입력해 주세요.\n");
                            continue;
                    }
                    Console.Clear();
                    Console.WriteLine($"{job}직업을 선택하셨습니다.\n");
                    isJobNull = false;
                }
                else
                {
                    Console.WriteLine("잘못된 입력입니다. 다시 입력해 주세요.\n");
                    continue;
                }
            }
        }

        private void InputName()
        {
            bool isNameNull = true;
            string? name = null;
            while (isNameNull)
            {

                if (string.IsNullOrEmpty(name))
                {
                    Console.WriteLine("이름을 입력해 주세요.\n");
                    name = Console.ReadLine();
                }
                if (!string.IsNullOrEmpty(name))
                {
                    Console.WriteLine();
                    Console.WriteLine($"{name}으(로) 이름을 하시겠습니까?\n1.저장\t2.취소");
                    Console.WriteLine();
                    try
                    {
                        playerInput = Console.ReadLine();
                        Console.WriteLine();
                        int k = (int.Parse(playerInput));
                    }
                    catch (FormatException e)
                    {
                        Console.WriteLine("잘못된 입력입니다. 다시 입력해 주세요.");
                        continue;
                    }

                    if (playerInput != null)
                    {

                        if (int.Parse(playerInput) == 2)
                        {
                            Console.WriteLine("이름을 다시 입력해 주세요.");
                            name = null;
                            continue;
                        }
                        else if (int.Parse(playerInput) == 1)
                        {
                            Console.Clear();
                            Console.WriteLine($"당신의 이름은 {name}으(로) 저장되었습니다.");
                            player.NAME = name;
                            isNameNull = false;

                        }
                        else
                        {
                            Console.WriteLine("잘못된 입력입니다. 다시 입력해 주세요.\n");
                            continue;

                        }


                    }



                }
                else
                {
                    Console.WriteLine("잘못된 입력입니다. 다시 입력해 주세요.");
                    continue;
                }
            }
        }

        private void WatchState()
        {
            while (true)
            {

                Console.WriteLine("상태보기");
                Console.WriteLine("캐릭터의 정보가 표시됩니다.\n");
                string _addA = "";
                string _addD = "";
                if (player.additionalAttack > 0)
                {
                    _addA = "(+ " + player.additionalAttack.ToString() + ")";
                }
                else if (player.additionalDefense > 0)
                {
                    _addD = "(+ " + player.additionalDefense.ToString() + ")";
                }
                Console.WriteLine($"Lv. {player.LEVEL}      \r\n{player.NAME} ({player.JOB})\r\n공격력 : {player.ATTACK} {_addA}\r\n방어력 : {player.DEFENSE} {_addD}\r\n체 력 : {player.HEALTH}\r\nGold : {player.GOLD} G\r\n");
                Console.WriteLine("0.나가기\n");
                Console.Write("원하시는 행동을 입력해 주세요.>> ");
                playerInput = Console.ReadLine();
                if (!string.IsNullOrEmpty(playerInput))
                {
                    try
                    {
                        int parseInput = int.Parse(playerInput);

                    }
                    catch (FormatException e)
                    {
                        Console.Clear();
                        Console.WriteLine("잘못된 입력입니다. 다시 입력해 주세요.\n");
                        continue;
                    }
                    if (int.Parse(playerInput) == 0)
                    {
                        Console.Clear();
                        Console.WriteLine("상태보기를 종료합니다.\n");
                        return;
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("잘못된 입력입니다. 다시 입력해 주세요.\n");
                        continue;
                    }
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("잘못된 입력입니다. 다시 입력해 주세요.\n");
                    continue;
                }
            }
        }

        private void WatchInventory()
        {
            while (true)
            {
                Console.Clear();
                Console.Write("인벤토리\r\n보유 중인 아이템을 관리할 수 있습니다.\r\n\r\n[아이템 목록]\r\n\r\n1. 장착 관리\r\n0. 나가기\r\n\r\n원하시는 행동을 입력해주세요.\r\n>>");
                playerInput = Console.ReadLine();
                if (!string.IsNullOrEmpty(playerInput))
                {
                    try
                    {
                        int parseInput = int.Parse(playerInput);

                    }
                    catch (FormatException e)
                    {
                        Console.Clear();
                        Console.WriteLine("잘못된 입력입니다. 다시 입력해 주세요.\n");
                        continue;
                    }
                    if (int.Parse(playerInput) == 0)
                    {
                        Console.Clear();
                        Console.WriteLine("인벤토리를 종료합니다.\n");
                        return;
                    }
                    else if (int.Parse(playerInput) == 1)
                    {
                        Console.Clear();
                        CheckEquipment();
                        return;
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("잘못된 입력입니다. 다시 입력해 주세요.\n");
                        continue;
                    }
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("잘못된 입력입니다. 다시 입력해 주세요.\n");
                    continue;
                }
            }
        }

        private void CheckEquipment()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"인벤토리 - 장착 관리\r\n보유 중인 아이템을 관리할 수 있습니다.\r\n\r\n[아이템 목록]\n");
                player.inventory.ShowItems();
                Console.Write("\n0. 나가기\r\n\r\n원하시는 행동을 입력해주세요.\r\n>>");
                playerInput = Console.ReadLine();
                if (!string.IsNullOrEmpty(playerInput))
                {
                    try
                    {
                        int parseInput = int.Parse(playerInput);

                    }
                    catch (FormatException e)
                    {
                        Console.Clear();
                        Console.WriteLine("잘못된 입력입니다. 다시 입력해 주세요.\n");
                        return;
                    }
                    if (int.Parse(playerInput) > 0 && int.Parse(playerInput) <= player.inventory.GetItemCount())
                    {
                        int selectedItem = int.Parse(playerInput) - 1;

                        if (player.inventory.GetItem(selectedItem) is EquipmentItem)
                        {
                            EquipmentItem selectedEquipment = (EquipmentItem)player.inventory.GetItem(selectedItem);
                            if (!selectedEquipment.isEquip)
                            {
                                player.EquipItem(selectedEquipment);
                            }
                            else
                            {
                                player.EquipItem(selectedEquipment);
                            }
                        }
                        else
                        {
                            Console.WriteLine("장착할 수 없는 아이템입니다.\n");
                            continue;
                        }


                    }
                    else if (int.Parse(playerInput) == 0)
                    {
                        Console.Clear();
                        Console.WriteLine("인벤토리를 종료합니다.\n");
                        return;
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("잘못된 입력입니다. 다시 입력해 주세요.\n");
                        continue;
                    }
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("잘못된 입력입니다. 다시 입력해 주세요.\n");
                    continue;
                }
            }

        }

        public void MarketMenu()
        {
            while (true)
            {
                
                Console.WriteLine($"상점\r\n필요한 아이템을 얻을 수 있는 상점입니다.\r\n\n[보유 골드]\r\n{player.GOLD} G\r\n\n[아이템 목록]");
                Console.WriteLine();
                mk.mkInven.ShowItemsInMarket();
                Console.WriteLine();
                Console.Write("1. 아이템 구매\r\n0. 나가기\r\n\r\n원하시는 행동을 입력해주세요.\r\n>>");
                playerInput = Console.ReadLine();
                if (playerInput == null) continue;
                try
                {
                    int parseInput = int.Parse(playerInput);

                }
                catch (FormatException e)
                {
                    Console.Clear();
                    Console.WriteLine("잘못된 입력입니다. 다시 입력해 주세요.\n");
                    continue;
                }
                if (int.Parse(playerInput) == 1)
                {
                    Console.Clear();
                    BuyFromMarket();
                }
                else if (int.Parse(playerInput) == 0)
                {
                    Console.Clear();
                    return;
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("잘못된 입력입니다. 다시 입력해 주세요.\n");
                }


            }


        }

        public void BuyFromMarket()
        {
            while (true)
            {
                
                Console.WriteLine($"상점 - 아이템 구매\r\n필요한 아이템을 얻을 수 있는 상점입니다.\r\n\n[보유 골드]\r\n{player.GOLD} G\r\n\n[아이템 목록]");
                Console.WriteLine();
                mk.mkInven.ShowItemsInMarket();
                Console.WriteLine();
                Console.Write("\r\n0. 나가기\r\n\r\n원하시는 행동을 입력해주세요.\r\n>>");
                playerInput = Console.ReadLine();
                if (playerInput == null) continue;
                try
                {
                    int parseInput = int.Parse(playerInput);

                }
                catch (FormatException e)
                {
                    Console.Clear();
                    Console.WriteLine("잘못된 입력입니다. 다시 입력해 주세요.\n");
                    continue;
                }
                if (int.Parse(playerInput) == 0)
                {
                    Console.Clear();
                    return;
                }
                else if(int.Parse(playerInput)>0 && int.Parse(playerInput)<=mk.mkInven.GetItemCount())
                {
                    int selectedItem = int.Parse(playerInput) - 1;

                    if (player.GOLD >= mk.mkInven.GetItem(selectedItem).PRICE )
                    {
                        if (!mk.mkInven.GetItem(selectedItem).HASITEM)
                        {

                        player.GOLD -= mk.mkInven.GetItem(selectedItem).PRICE;
                        player.inventory.AddItem(mk.mkInven.GetItem(selectedItem).CloneItem());
                        mk.mkInven.GetItem(selectedItem).HASITEM = true;
                            Console.Clear();
                            Console.WriteLine("구매를 완료했습니다.\n");
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine("이미 구매한 아이템입니다\n");
                        }

                        
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("Gold 가 부족합니다. .\n");
                        continue;
                    }

                }
                else
                {
                    Console.WriteLine("잘못된 입력입니다. 다시 입력해 주세요.\n");
                }


            }



        }

        private void SelectMenu()
        {
            Menu menu;
            Console.Write("스파르타 마을에 오신 여러분 환영합니다.\r\n이곳에서 던전으로 들어가기전 활동을 할 수 있습니다.\r\n\r\n1. 상태 보기\r\n2. 인벤토리\r\n3. 상점\r\n\r\n원하시는 행동을 입력해주세요.>> ");
            playerInput = Console.ReadLine();
            if (playerInput == null)
            {
                Console.Clear();
                Console.WriteLine("잘못된 입력입니다. 다시 입력해 주세요.\n");
                return;
            }
            else
            {
                try
                {
                    int parseInput = int.Parse(playerInput);

                }
                catch (FormatException e)
                {
                    Console.Clear();
                    Console.WriteLine("잘못된 입력입니다. 다시 입력해 주세요.\n");
                    return;
                }
                menu = (Menu)int.Parse(playerInput);
                switch (menu)
                {
                    case Menu.상점:
                        Console.Clear();
                        MarketMenu();
                        break;
                    case Menu.인벤토리:
                        WatchInventory();
                        break;
                    case Menu.상태보기:
                        Console.Clear();
                        WatchState();
                        break;
                    default:
                        Console.Clear();
                        Console.WriteLine("잘못된 입력입니다. 다시 입력해 주세요.\n");
                        break;
                }   //메뉴 선택에 따라서 행동

            }




        }

        private void Init()
        {
            player = new Player();
            db = new ItemDB();
            mk = new Market(db);

            SelectJob();
            InputName();
            while (true)
            {
                //playerInput = null;
                SelectMenu();

            }

        }
    }

    class Player
    {
        private string name;
        public string NAME
        {
            get { return name; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("Name cannot be null or empty.");
                }
                name = value;
            }
        }

        private int level = 1;
        public int LEVEL
        {
            get { return level; }
            set
            {
                level = value;
            }
        }


        private int health = 100;
        public int HEALTH
        {
            get { return health; }
            set
            {
                health = value;
            }
        }


        private Job job;
        public Job JOB
        {
            get { return job; }
            set
            {
                job = value;
            }
        }

        public Inventory inventory;

        public Player()
        {
            inventory = new Inventory();
        }

        private int attack = 10;
        public int ATTACK
        {
            get { return attack; }
            set
            {
                attack = value;
            }
        }

        private int defense = 5;
        public int DEFENSE
        {
            get { return defense; }
            set
            {
                defense = value;
            }
        }



        private int gold = 1500;
        public int GOLD
        {
            get { return gold; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Gold cannot be negative.");
                }
                gold = value;
            }
        }

        public int additionalAttack { get; set; } = 0;
        public int additionalDefense { get; set; } = 0;

        public void EquipItem(EquipmentItem item)
        {
            if (!item.isEquip)
            {
                if (item.stat.ATTACK > 0)
                {
                    ATTACK += item.stat.ATTACK;
                    additionalAttack += item.stat.ATTACK;

                }
                if (item.stat.DEFENSE > 0)
                {
                    DEFENSE += item.stat.DEFENSE;
                    additionalDefense += item.stat.DEFENSE;
                }
                item.Equip();

            }
            else
            {
                if (item.stat.ATTACK > 0)
                {
                    ATTACK -= item.stat.ATTACK;
                    additionalAttack -= item.stat.ATTACK;

                }
                if (item.stat.DEFENSE > 0)
                {
                    DEFENSE -= item.stat.DEFENSE;
                    additionalDefense -= item.stat.DEFENSE;
                }
                item.UnEquip();
            }


        }




    }

    class Market
    {
        public Inventory mkInven;

        public Market(ItemDB db)
        {
            mkInven = new Inventory();
            List<Item> _buyItem = new List<Item>();
            mkInven.AddItem(db.CloneItemFromDB("낡은 검"));
            mkInven.AddItem(db.CloneItemFromDB("평범한 철 검"));
            mkInven.AddItem(db.CloneItemFromDB("스파르타 검"));
            mkInven.AddItem(db.CloneItemFromDB("천 갑옷"));
            mkInven.AddItem(db.CloneItemFromDB("가죽 갑옷"));
            mkInven.AddItem(db.CloneItemFromDB("판금 갑옷"));
        }





    }

    class ItemDB
    {
        List<Item> DBitems;  //중복방지를 위한 HashSet형식의 ItemDataBase

        public ItemDB()
        {
            DBitems = new List<Item>() {
            new EquipmentItem() { NAME = "낡은 검", PRICE = 100, DESCRIPTION = "낡은 검입니다. 날이 무디고, 곳곳에 녹이 슬어있습니다.", stat = new ItemStat() { ATTACK = 3 } },
            new EquipmentItem() { NAME = "평범한 철 검", PRICE = 500, DESCRIPTION = "평범한 검입니다. 날이 잘 서있고, 균형이 잘 잡혀있어 휘두르기 편합니다.", stat = new ItemStat() { ATTACK = 10 } },
            new EquipmentItem() { NAME = "스파르타 검", PRICE = 1000, DESCRIPTION = "스파르타의 정예군이 사용하던 양날 검입니다. 날이 날카롭게 서있습니다.", stat = new ItemStat() { ATTACK = 20 } },
            new EquipmentItem() { NAME = "천 갑옷", PRICE = 100, DESCRIPTION = "모직으로 이루어진 천 갑옷입니다. 갑옷으로써 사용하기엔 빈약합니다.", stat = new ItemStat() { DEFENSE = 3 } },
            new EquipmentItem() { NAME = "천 갑옷", PRICE = 100, DESCRIPTION = "더미데이터 입니다. 삭제될 예정입니다.", stat = new ItemStat() { DEFENSE = 3 } }, // 중복데이터제거로 사라질 데이터
            new EquipmentItem() { NAME = "가죽 갑옷", PRICE = 500, DESCRIPTION = "어떤 동물의 가죽으로 만들어진 갑옷입니다. ", stat = new ItemStat() { DEFENSE = 10 } },
            new EquipmentItem() { NAME = "판금 갑옷", PRICE = 1000, DESCRIPTION = "이름없는 장인의 노력으로 완전한 방어력을 갖춘 판금갑옷 입니다. ", stat = new ItemStat() { DEFENSE = 20 } }
            };
            DBitems = DBitems.Distinct().ToList(); // 중복데이터 제거 - 아이템 이름과 stat이 같을 경우 
        }

        public EquipmentItem CloneItemFromDB(string itemName)
        {
            foreach (var item in DBitems)
            {
                if (item.NAME.Equals(itemName))
                {
                    return (EquipmentItem)item.CloneItem();
                }
            }
            return null;
        }





    }


    class Inventory
    {
        private List<Item> items;
        public Inventory()
        {
            items = new List<Item>();

        }

        public void AddItem(Item item)
        {
            items.Add(item);
        }

        public void RemoveItem(Item item)
        {
            items.Remove(item);
        }

        public void ShowItems()
        {
            foreach (var item in items.Select((value, Index) => (value, Index)))
            {
                Console.WriteLine($"- {1 + item.Index} {item.value.ToString()}");
            }
        }
        public void ShowItemsInMarket()
        {
            foreach (var item in items.Select((value, Index) => (value, Index)))
            {
                if (!item.value.HASITEM)
                {

                Console.WriteLine($"- {1 + item.Index} {item.value.ToString()} | {item.value.PRICE} G");

                }
                else
                {
                Console.WriteLine($"- {1 + item.Index} {item.value.ToString()} | 구매완료");

                }
            }


        }

        public int GetItemCount()
        {
            return items.Count;
        }

        public Item GetItem(int i)
        {
            return items[i];

        }

        //public void CloneInventoryList(List<Item> items)
        //{
        //    foreach(var item in items)
        //    {
        //        items.Add(item.CloneItem());
        //    }  

        //}
    }

    class Item
    {
        private bool hasItem  = false;
        public bool HASITEM
        {
            get { return hasItem; }
            set { hasItem = value; }

        }
        private string name;
        public string NAME
        {
            get { return name; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("Name cannot be null or empty.");
                }
                name = value;
            }
        }

        private int price;
        public int PRICE
        {
            get { return price; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Price cannot be negative.");
                }
                price = value;
            }
        }

        private string description;
        public string DESCRIPTION
        {
            get { return description; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("Description cannot be null or empty.");
                }
                description = value;
            }
        }

        public virtual string ToString()
        {
            return $"이름 : {NAME}\t가격 : {PRICE}";
        }

        public virtual Item CloneItem()
        {
            Item item = new Item();
            item.name = name;
            item.price = price;
            item.description = description;
            return item;
        }
    }

    class EquipmentItem : Item, IEquipable
    {
        public ItemStat stat;
        public bool isEquip = false;

        public override bool Equals(object? obj)
        {
            return this.NAME == ((EquipmentItem)obj).NAME && stat.Equals(((EquipmentItem)obj).stat);
        }


        public override int GetHashCode()
        {
            var hashCode = this.NAME.GetHashCode();
            if (this.stat.ATTACK > 0)
            {

                hashCode = hashCode * 397 * this.stat.ATTACK.GetHashCode();
            }
            if (this.stat.DEFENSE > 0)
            {

                hashCode = hashCode * 397 * this.stat.DEFENSE.GetHashCode();
            }
            return hashCode;
        }


        public void Equip()
        {
            //Console.WriteLine($"{NAME}을(를) 장착했습니다.");
            if (isEquip) return;
            isEquip = true;
            NAME = "[E]" + NAME;
        }
        public void UnEquip()
        {
            //Console.WriteLine($"{NAME}을(를) 해제했습니다.");
            if (!isEquip) return;
            isEquip = false;
            NAME = NAME.Substring(3);
        }

        public override string ToString()
        {
            return $"{NAME,-8}| {stat.ToString(),-3} | {DESCRIPTION,-10}";
        }

        public override EquipmentItem CloneItem()
        {
            EquipmentItem item = new EquipmentItem();
            item.NAME = NAME;
            item.stat = new ItemStat() { ATTACK = this.stat.ATTACK, DEFENSE = this.stat.DEFENSE };
            item.DESCRIPTION = DESCRIPTION;
            item.PRICE = PRICE;

            return item;
        }

    }


    interface IEquipable
    {
        void Equip();
        void UnEquip();
    }




}

