using Game.Class;
using System;
using System.Collections;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static System.Formats.Asn1.AsnWriter;

public enum SubLocationType
{
    //general
    shop,
    marketplace,
    tavern,//no--
    blacksmith,
    bank,
    casino,
    wilderness,
    graveyard,
    pond,
    port,
    mine,

    //specific
    TrainingGrounds,
    crypt,
    swordInStone
}
public class SubLocation
{
    public SubLocationType type;
    public string name;


    public List<Item> shopItems = new List<Item>();

    public int casinoMaxBet = 0;

    public SubLocation() { } //Deserialize

    public SubLocation(string tName,SubLocationType tType)
    {
        name = tName;
        type = tType;
    }


    
    public void DoSubLocation()
    {
        MainUI.ClearMainArea();
        if (type == SubLocationType.shop) ShopLogic();
        
        if (type == SubLocationType.bank) BankLogic();

        if (type == SubLocationType.blacksmith) BlacksmithLogic();
        
        if (type == SubLocationType.casino)
        {
            MainUI.WriteInMainArea("what game do you want to play, \nBlackjack : 1 \nRoulette : 2 \nor leave : 0");
            int.TryParse(Console.ReadKey(true).KeyChar.ToString(), out int input);
            if (input == null || input > 2 || input < 0)
            {
                MainUI.ClearMainArea();
                MainUI.WriteInMainArea("sweetie you gotta type a number that we can use\n ");
                DoSubLocation();
                return;
            }
            else if (input == 0)
            {
                MainUI.ClearMainArea();
                Program.MainMenu();
                return;
            }
            else if (input == 1)
            {
                MainUI.ClearMainArea();
                BlackjackLogic();

            }
            else if (input == 2)
            {
                MainUI.ClearMainArea();
                RouletteLogic();
            }

        }

        if (type == SubLocationType.wilderness) WildernessLogic();
        
        if (type == SubLocationType.pond) FishingLogic();
        
        if (type == SubLocationType.graveyard) GraveyardLogic();
        
        if (type == SubLocationType.port) PortLogic();
        
        if(type == SubLocationType.marketplace) MarketplaceLogic();

        if (type == SubLocationType.TrainingGrounds) TrainingGroundsLogic();

        if (type == SubLocationType.mine) MineLogic();

        if (type == SubLocationType.crypt) CryptLogic();

        if(type == SubLocationType.swordInStone) SwordInStoneLogic();
             
        // not done---
        if (type == SubLocationType.tavern)
        {
            MainUI.WriteInMainArea("do you want to buy something : 1  \nor do you want to rent a room : 2  \nor leave : 0");
            int.TryParse(Console.ReadKey(true).KeyChar.ToString(), out int input);
            if (input == null || input > 2 || input < 0)
            {
                MainUI.ClearMainArea();
                MainUI.WriteInMainArea("sweetie you gotta type a number that we can use\n ");
                DoSubLocation();
                return;
            }
            else if(input == 1)
            {
                ShopLogic();
            }
            else if (input == 2)
            {

            }


        }




    }

    #region generalUse

    #region blacksmith / forge
    void BlacksmithLogic()
    {
        while (true)
        {
            MainUI.ClearMainArea();
            MainUI.WriteInMainArea("--- Blacksmith (Forge) ---\n");

            var currentLoc = LocationLibrary.Get(Program.player.currentLocation);
            string areaTag = currentLoc?.kingdom;

            var recipes = RecipeLibrary.GetRecipesFor(areaTag, CraftingStationType.Forge).ToList();

            if (recipes.Count == 0)
            {
                MainUI.WriteInMainArea("The blacksmith here doesn't know any recipes yet.\n");
                MainUI.WriteInMainArea("Press Enter to leave...");
                Console.ReadLine();
                Program.MainMenu();
                return;
            }

            MainUI.WriteInMainArea("nr  Recipe / Output                          Requirements");
            MainUI.WriteInMainArea("--------------------------------------------------------------------");

            for (int i = 0; i < recipes.Count; i++)
            {
                var r = recipes[i];


                string req = "   Requires: ";
                bool first = true;
                foreach (var mc in r.Materials)
                {
                    if (!first) req += ", ";
                    req += $"{mc.Quantity}x {mc.Material.name}";
                    first = false;
                }
                if (r.MoneyCost > 0)
                {
                    if (!first) req += ", ";
                    req += $"{r.MoneyCost} Rai";
                }
                string line = $"{i + 1,-3} {r.OutputItem.name,-30}";
                MainUI.WriteInMainArea(line);
                MainUI.WriteInMainArea($" -> {req}");
            }

            MainUI.WriteInMainArea("\n0 : Leave");
            MainUI.WriteInMainArea("\nType recipe number to craft, or 'D' + number for details (e.g., 'D1'): ");

            string input = Console.ReadLine() ?? "";
            
            bool wantsDetails = input.StartsWith("d", StringComparison.OrdinalIgnoreCase) || input.StartsWith("D");
            if (wantsDetails && input.Length > 1)
            {
                input = input.Substring(1);
            }

            if (!int.TryParse(input, out int choice) || choice < 0 || choice > recipes.Count)
            {
                MainUI.WriteInMainArea("Invalid choice. Press Enter to continue...");
                Console.ReadLine();
                continue;
            }

            if (choice == 0)
            {
                Program.MainMenu();
                return;
            }

            var selected = recipes[choice - 1];

            if (wantsDetails)
            {
                MainUI.ClearMainArea();
                MainUI.WriteInMainArea($"=== {selected.Name} ===\n");
                MainUI.WriteInMainArea($"Output: {selected.OutputQuantity}x {selected.OutputItem.name}");
                MainUI.WriteInMainArea($"Value: {selected.OutputItem.value} Rai each\n");
                MainUI.WriteInMainArea("Item Stats:");
                MainUI.WriteInMainArea(selected.OutputItem.GetDescription());
                
                MainUI.WriteInMainArea("Requirements:");
                foreach (var mc in selected.Materials)
                {
                    int have = 0;
                    foreach (var mat in Program.player.materialItems)
                    {
                        if (mat.name == mc.Material.name)
                        {
                            have += mat.amount;
                        }
                    }
                    foreach (var mat in Program.player.ownedItems)
                    {
                        if (mat.name == mc.Material.name)
                        {
                            have += mat.amount;
                        }
                    }
                    string status = have >= mc.Quantity ? "[✓]" : "[X]";
                    MainUI.WriteInMainArea($"  {status} {mc.Quantity}x {mc.Material.name} (You have: {have})");
                }
                if (selected.MoneyCost > 0)
                {
                    string status = Program.player.money >= selected.MoneyCost ? "[✓]" : "[X]";
                    MainUI.WriteInMainArea($"  {status} {selected.MoneyCost} Rai (You have: {Program.player.money})");
                }
                
                MainUI.WriteInMainArea("\nPress Enter to return...");
                Console.ReadLine();
                continue;
            }

            if (!CanCraft(selected))
            {
                MainUI.WriteInMainArea("\nYou lack the required materials or money.\nPress Enter to continue...");
                Console.ReadLine();
                continue;
            }

            CraftRecipe(selected);

            MainUI.WriteInMainArea($"\nCrafted {selected.OutputQuantity}x {selected.OutputItem.name}!\nPress Enter to continue...");
            Console.ReadLine();
        }
    }

    bool CanCraft(Recipe recipe)
    {
        if (Program.player.money < recipe.MoneyCost)
            return false;

        foreach (var cost in recipe.Materials)
        {
            int have = 0;
            foreach (var mat in Program.player.materialItems)
            {
                if (mat.name == cost.Material.name)
                {
                    have += mat.amount;
                }
            }
            foreach (var mat in Program.player.ownedItems)
            {
                if (mat.name == cost.Material.name)
                {
                    have += mat.amount;
                }
            }
            if (have < cost.Quantity) return false;
        }

        return true;
    }

    void CraftRecipe(Recipe recipe)
    {
        Program.player.money -= recipe.MoneyCost;

        foreach (var cost in recipe.Materials)
        {
            int remaining = cost.Quantity;
            for (int i = 0; i < Program.player.materialItems.Count && remaining > 0; i++)
            {
                var mat = Program.player.materialItems[i];
                if (mat.name != cost.Material.name) continue;

                int take = Math.Min(mat.amount, remaining);
                mat.amount -= take;
                remaining -= take;

                Program.player.currentMaterialLoad -= take;
                if (Program.player.currentMaterialLoad < 0) Program.player.currentMaterialLoad = 0;

                if (mat.amount <= 0)
                {
                    Program.player.materialItems.RemoveAt(i);
                    i--;
                }
            }
            for (int i = 0; i < Program.player.ownedItems.Count && remaining > 0; i++)
            {
                var mat = Program.player.ownedItems[i];
                if (mat.name != cost.Material.name) continue;

                int take = Math.Min(mat.amount, remaining);
                mat.amount -= take;
                remaining -= take;

                if (mat.amount <= 0)
                {
                    Program.player.ownedItems.RemoveAt(i);
                    i--;
                }
            }
        }

        for (int i = 0; i < recipe.OutputQuantity; i++)
        {
            Inventory.AddItem(recipe.OutputItem, 1);
        }

        Program.SavePlayer();
    }
    #endregion

    #region shop
    void ShopLogic()
    {


        MainUI.WriteInMainArea("Shop Items:");
        MainUI.WriteInMainArea("\n nr     Name                      Weight   Description        Price");
        MainUI.WriteInMainArea(" ----------------------------------------------------------------");
        int i = 0;
        foreach (var item in shopItems)
        {
            i++;
            MainUI.WriteInMainArea($" {i,-7}{item.name,-26} {item.weight,-7} {item.description,-20} {item.value}");
        }
        MainUI.WriteInMainArea("\nif you want to interact with anything type its corresponding number \nif not type 0");
        var n = int.TryParse(Console.ReadKey(true).KeyChar.ToString(), out int input);
        if (input == null || input > shopItems.Count || input < 0)
        {
            MainUI.ClearMainArea();
            MainUI.WriteInMainArea("sweetie you gotta type a number that we can use\n ");
            DoSubLocation();
            return;
        }
        else if (input == 0) { Program.MainMenu(); return; }
        input--;
        MainUI.ClearMainArea();
        MainUI.WriteInMainArea($"you've picked {shopItems[input].name}   it costs {shopItems[input].value}\n");

        MainUI.WriteInMainArea("0 : details");
        MainUI.WriteInMainArea("1 : buy");
        MainUI.WriteInMainArea("\ntype out the number next to the action you want to perform");

        var k = int.TryParse(Console.ReadKey(true).KeyChar.ToString(), out int ik);
        if (ik == null || ik < 0 || ik > 1)
        {
            MainUI.ClearMainArea();
            MainUI.WriteInMainArea("my love would you please type a functional number this time\n ");
            DoSubLocation();
            return;
        }
        else if (ik == 0)
        {
            MainUI.ClearMainArea();
            MainUI.WriteInMainArea($"you've picked {shopItems[input].name}\n");
            MainUI.WriteInMainArea($"{shopItems[input].GetDescription()}\n");

            MainUI.WriteInMainArea($"Press Enter to continue...");
            Console.ReadLine();
        }
        else if (ik == 1)
        {
            MainUI.WriteInMainArea($"how many would you like to buy?");
            var q = int.TryParse(Console.ReadLine(),out int quantity);
            if (quantity == null || quantity <= 0)
            {
                MainUI.ClearMainArea();
                MainUI.WriteInMainArea("sweetie you gotta type a number that we can use\n ");
                MainUI.WriteInMainArea("Press enter to continue...");
                Console.ReadLine();
                DoSubLocation();
                return;
            }
            if ((Program.player.money - shopItems[input].value * quantity) >= 0)
            {
                Inventory.AddItem(shopItems[input], quantity);
                Program.player.money -= shopItems[input].value * quantity;
            }
            else
            {
                MainUI.ClearMainArea();
                MainUI.WriteInMainArea("\nyou dont have enough Rai for that");
                Thread.Sleep(1000);
                DoSubLocation();
                return;
            }

        }
        Program.SavePlayer();
        DoSubLocation();
    }
    #endregion

    #region bank
    void BankLogic()
    {
        while (true)
        {
            MainUI.ClearMainArea();
            MainUI.WriteInMainArea("Welcome to the bank");
            MainUI.WriteInMainArea($"You have {Program.player.bankMoney} Rai in your account.");
            MainUI.WriteInMainArea("Would you like to:");
            MainUI.WriteInMainArea("1 : Deposit Items");
            MainUI.WriteInMainArea("2 : Withdraw Items");
            MainUI.WriteInMainArea("3 : Deposit Money");
            MainUI.WriteInMainArea("4 : Withdraw Money");
            MainUI.WriteInMainArea("0 : Leave");

            if (int.TryParse(Console.ReadKey(true).KeyChar.ToString(), out int input) == false || input > 4 || input < 0)
            {
                MainUI.WriteInMainArea(" \nyou gotta type a number from 0-4");
                Thread.Sleep(1000);
                continue;
            }

            switch (input)
            {
                case 0:
                    Program.MainMenu();
                    return;
                case 1:
                    DepositItems();
                    break;
                case 2:
                    WithdrawItems();
                    break;
                case 3:
                    DepositMoney();
                    break;
                case 4:
                    WithdrawMoney();
                    break;
            }
            Program.SavePlayer(); 
        }
    }

    private void DepositItems()
    {

        const float exponent = 1.5f;
        const float scale = 0.1f;

        while (true)
        {
            MainUI.ClearMainArea();
            MainUI.WriteInMainArea("Select an item to deposit:");
            MainUI.WriteInMainArea("");
            MainUI.WriteInMainArea("nr     Name                      Qty");
            MainUI.WriteInMainArea("--------------------------------------");

            if (Program.player.ownedItems.Count == 0)
            {
                MainUI.WriteInMainArea("You have no items to deposit.");
                MainUI.WriteInMainArea("");
                MainUI.WriteInMainArea("0 : Back");
            }
            else
            {
                for (int i = 0; i < Program.player.ownedItems.Count; i++)
                {
                    var item = Program.player.ownedItems[i];
                    MainUI.WriteInMainArea($"{i + 1,-7}{item.name,-25} {item.amount,-5}");
                }
                MainUI.WriteInMainArea("");
                MainUI.WriteInMainArea("0 : Back");
            }

            string inputString = Console.ReadLine() ?? "";
            var n = int.TryParse(inputString, out int input);

            if (input == 0)
            {
                return; 
            }

            if (!n || input < 1 || input > Program.player.ownedItems.Count)
            {
                MainUI.WriteInMainArea("\nInvalid selection. Please type a number from the list");
                Thread.Sleep(1000);
                continue;
            }

            Item selectedItem = Program.player.ownedItems[input - 1];
            int quantity = 1;

            // If stackable, ask how many
            if (selectedItem.type != ItemType.Equipment)
            {
                MainUI.WriteInMainArea($"How many {selectedItem.name} would you like to deposit? (Max: {selectedItem.amount})");
                string quantityString = Console.ReadLine() ?? "";
                var q = int.TryParse(quantityString, out quantity);

                if (!q || quantity < 1 || quantity > selectedItem.amount)
                {
                    MainUI.WriteInMainArea("\nInvalid amount");
                    Thread.Sleep(1000);
                    continue;
                }
            }

            // Check if item is equipped
            int equippedSlot = Program.player.equippedItems.FindIndex(item => item?.name == selectedItem?.name);
            if (equippedSlot >= 0)
            {
                MainUI.WriteInMainArea($"\nThis item is equipped. Unequipping {selectedItem.name}...");
                Inventory.UnequipItem(equippedSlot);
                Thread.Sleep(1000);
            }

            Item itemToBank = new Item(selectedItem); // Use copy constructor
            itemToBank.amount = quantity;

            // Add to bank list
            Program.player.bankItems.Add((Program.player.currentLocation, itemToBank));

            // Handle Stats & Effects
            if (selectedItem.type == ItemType.Artifact)
            {
                Inventory.RemoveEffects(selectedItem, null); // Remove stats for the whole stack
            }

            // Handle Item List
            if (selectedItem.type == ItemType.Equipment || selectedItem.amount <= quantity)
            {
                // Remove the item completely
                Program.player.ownedItems.Remove(selectedItem);
            }
            else
            {
                // Just subtract the amount
                selectedItem.amount -= quantity;
            }

            // Handle Weight & Speed 
            Inventory.UpdateWeight();

            // Re-apply artifact stats if some items are left
            if (selectedItem.type == ItemType.Artifact && Program.player.ownedItems.Contains(selectedItem))
            {
                Inventory.ApplyEffects(selectedItem, null); 
            }


            MainUI.WriteInMainArea($"\nDeposited {quantity}x {itemToBank.name}.");
            Thread.Sleep(1000);
        }
    }

    private void WithdrawItems()
    {

        while (true)
        {
            MainUI.ClearMainArea();
            MainUI.WriteInMainArea("Select an item to withdraw:");
            MainUI.WriteInMainArea("");
            MainUI.WriteInMainArea("nr     Name                      Qty   Location");
            MainUI.WriteInMainArea("--------------------------------------------------");

            if (Program.player.bankItems.Count == 0)
            {
                MainUI.WriteInMainArea("Your bank is empty.");
                MainUI.WriteInMainArea("");
                MainUI.WriteInMainArea("0 : Back");
            }
            else
            {
                for (int i = 0; i < Program.player.bankItems.Count; i++)
                {
                    var (location, item) = Program.player.bankItems[i];
                    MainUI.WriteInMainArea($"{i + 1,-7}{item.name,-25} {item.amount,-5} {location}");
                }
                MainUI.WriteInMainArea("");
                MainUI.WriteInMainArea("0 : Back");
            }

            string inputString = Console.ReadLine() ?? "";
            var n = int.TryParse(inputString, out int input);

            if (input == 0)
            {
                return; 
            }

            if (!n || input < 1 || input > Program.player.bankItems.Count)
            {
                MainUI.WriteInMainArea("\nInvalid selection. Please type a number from the list.");
                Thread.Sleep(1000);
                continue;
            }

            var (loc, selectedItem) = Program.player.bankItems[input - 1];
            int quantity = 1;

            // If stackable, ask how many
            if (selectedItem.type != ItemType.Equipment)
            {
                MainUI.ClearMainArea();
                MainUI.WriteInMainArea($"How many {selectedItem.name} would you like to withdraw? (Max: {selectedItem.amount})");
                string quantityString = Console.ReadLine() ?? "";
                var q = int.TryParse(quantityString, out quantity);

                if (!q || quantity < 1 || quantity > selectedItem.amount)
                {
                    MainUI.WriteInMainArea("\nInvalid amount.");
                    Thread.Sleep(1000);
                    continue;
                }
            }


            Inventory.AddItem(selectedItem, quantity);

            // --- Remove from bank ---
            if (selectedItem.type == ItemType.Equipment || selectedItem.amount == quantity)
            {
                // Remove the item completely from bank
                Program.player.bankItems.RemoveAt(input - 1);
            }
            else
            {
                // Just subtract the amount
                selectedItem.amount -= quantity;
                
            }

            MainUI.WriteInMainArea($"\nWithdrew {quantity}x {selectedItem.name}.");
            Thread.Sleep(1000);
        }
    }


    private void DepositMoney()
    {
        MainUI.ClearMainArea();
        MainUI.WriteInMainArea("Deposit Money");
        MainUI.WriteInMainArea($"Current Rai: {Program.player.money}");
        MainUI.WriteInMainArea($"Bank Account: {Program.player.bankMoney}");
        MainUI.WriteInMainArea("");
        MainUI.WriteInMainArea("How much would you like to deposit? (Type 0 to cancel)");

        string inputString = Console.ReadLine() ?? "";
        var n = int.TryParse(inputString, out int amount);

        if (!n || amount < 0)
        {
            MainUI.WriteInMainArea("\nInvalid amount");
            Thread.Sleep(1000);
            return;
        }

        if (amount == 0)
        {
            return;
        }

        if (amount > Program.player.money)
        {
            MainUI.WriteInMainArea("\nYou don't have that much Rai");
            Thread.Sleep(1000);
            return;
        }

        Program.player.money -= amount;
        Program.player.bankMoney += amount;

        MainUI.WriteInMainArea($"\nDeposited {amount} Rai");
        MainUI.WriteInMainArea($"New balance: {Program.player.bankMoney} Rai");
        Thread.Sleep(1500);
    }

    private void WithdrawMoney()
    {
        MainUI.ClearMainArea();
        MainUI.WriteInMainArea("Withdraw Money");
        MainUI.WriteInMainArea($"Current Rai: {Program.player.money}");
        MainUI.WriteInMainArea($"Bank Account: {Program.player.bankMoney}");
        MainUI.WriteInMainArea("");
        MainUI.WriteInMainArea("How much would you like to withdraw? (Type 0 to cancel)");

        string inputString = Console.ReadLine() ?? "";
        var n = int.TryParse(inputString, out int amount);

        if (!n || amount < 0)
        {
            MainUI.WriteInMainArea("\nInvalid amount.");
            Thread.Sleep(1000);
            return;
        }

        if (amount == 0)
        {
            return;
        }

        if (amount > Program.player.bankMoney)
        {
            MainUI.WriteInMainArea("\nYou don't have that much Rai in your account.");
            Thread.Sleep(1000);
            return;
        }

        Program.player.bankMoney -= amount;
        Program.player.money += amount;

        MainUI.WriteInMainArea($"\nWithdrew {amount} Rai.");
        MainUI.WriteInMainArea($"New balance: {Program.player.bankMoney} Rai.");
        Thread.Sleep(1500);
    }
    #endregion

    #region casino
    void BlackjackLogic()
    {
        MainUI.ClearMainArea();
        MainUI.WriteInMainArea($"=== Blackjack ===");
        MainUI.WriteInMainArea($"Current Rai: {Program.player.money}");
        MainUI.WriteInMainArea($"Max bet: {casinoMaxBet}");
        MainUI.WriteInMainArea("");
        MainUI.WriteInMainArea("How much do you want to bet? (0 to leave)");

        if (!int.TryParse(Console.ReadLine(), out int bet) || bet < 0 || bet > casinoMaxBet)
        {
            MainUI.WriteInMainArea($"\nPlease enter a valid bet between 0 and {casinoMaxBet}.");
            Thread.Sleep(1500);
            DoSubLocation();
            return;
        }

        if (bet == 0)
        {
            MainUI.ClearMainArea();
            DoSubLocation();
            return;
        }

        if (bet > Program.player.money)
        {
            MainUI.WriteInMainArea("\nYou don't have enough Rai.");
            Thread.Sleep(1500);
            DoSubLocation();
            return;
        }

        Program.player.money -= bet;
        Program.SavePlayer();

        Random rand = new Random();
        List<(int rank, string suit)> playerCards = new List<(int, string)>();
        List<(int rank, string suit)> dealerCards = new List<(int, string)>();

        playerCards.Add(DrawRandomCard(rand));
        dealerCards.Add(DrawRandomCard(rand));
        playerCards.Add(DrawRandomCard(rand));
        dealerCards.Add(DrawRandomCard(rand));

        MainUI.ClearMainArea();
        MainUI.WriteInMainArea("=== Blackjack ===\n");
        MainUI.WriteInMainArea("Dealer's hand:");
        MainUI.WriteInMainArea($"  {FormatCard(dealerCards[0])}");
        MainUI.WriteInMainArea("  [Hidden Card]\n");

        MainUI.WriteInMainArea("Your hand:");
        foreach (var card in playerCards)
        {
            MainUI.WriteInMainArea($"  {FormatCard(card)}");
        }
        int playerValue = CalculateHandValue(playerCards);
        MainUI.WriteInMainArea($"Total: {playerValue}");

        bool playerBlackjack = playerValue == 21 && playerCards.Count == 2;
        if (playerBlackjack)
        {
            MainUI.WriteInMainArea("\nBLACKJACK!");
            Thread.Sleep(1500);
        }

        bool playerBust = false;
        if (!playerBlackjack)
        {
            while (true)
            {
                MainUI.WriteInMainArea("\n1 : Hit");
                MainUI.WriteInMainArea("2 : Stand");

                if (!int.TryParse(Console.ReadKey(true).KeyChar.ToString(), out int choice) || choice < 1 || choice > 2)
                {
                    MainUI.WriteInMainArea("\nPlease choose 1 or 2.");
                    Thread.Sleep(1000);
                    continue;
                }

                if (choice == 1)
                {
                    playerCards.Add(DrawRandomCard(rand));
                    MainUI.ClearMainArea();
                    MainUI.WriteInMainArea("=== Blackjack ===\n");
                    MainUI.WriteInMainArea("Dealer's hand:");
                    MainUI.WriteInMainArea($"  {FormatCard(dealerCards[0])}");
                    MainUI.WriteInMainArea("  [Hidden Card]\n");

                    MainUI.WriteInMainArea("Your hand:");
                    foreach (var card in playerCards)
                    {
                        MainUI.WriteInMainArea($"  {FormatCard(card)}");
                    }
                    playerValue = CalculateHandValue(playerCards);
                    MainUI.WriteInMainArea($"Total: {playerValue}");

                    if (playerValue > 21)
                    {
                        MainUI.WriteInMainArea("\nBUST! You went over 21.");
                        playerBust = true;
                        Thread.Sleep(1500);
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
        }

        int dealerValue = CalculateHandValue(dealerCards);
        if (!playerBust)
        {
            MainUI.ClearMainArea();
            MainUI.WriteInMainArea("=== Blackjack ===\n");
            MainUI.WriteInMainArea("Dealer reveals their hand...");
            Thread.Sleep(1000);

            MainUI.WriteInMainArea("\nDealer's hand:");
            foreach (var card in dealerCards)
            {
                MainUI.WriteInMainArea($"  {FormatCard(card)}");
            }
            dealerValue = CalculateHandValue(dealerCards);
            MainUI.WriteInMainArea($"Total: {dealerValue}");
            Thread.Sleep(1000);

            while (dealerValue < 17)
            {
                MainUI.WriteInMainArea("\nDealer hits...");
                Thread.Sleep(700);

                dealerCards.Add(DrawRandomCard(rand));
                MainUI.WriteInMainArea($"  {FormatCard(dealerCards[dealerCards.Count - 1])}");
                dealerValue = CalculateHandValue(dealerCards);
                MainUI.WriteInMainArea($"Total: {dealerValue}");
                Thread.Sleep(700);
            }

            if (dealerValue > 21)
            {
                MainUI.WriteInMainArea("\nDealer busts!");
                Thread.Sleep(1000);
            }
            else
            {
                MainUI.WriteInMainArea($"\nDealer stands at {dealerValue}.");
                Thread.Sleep(1000);
            }
        }

        MainUI.WriteInMainArea("\n" + new string('=', 40));
        
        if (playerBust)
        {
            MainUI.WriteInMainArea($"You lose! You lost {bet} Rai.");
        }
        else if (dealerValue > 21)
        {
            int winnings = playerBlackjack ? (int)(bet * 2.5) : bet * 2;
            Program.player.money += winnings;
            
            if (playerBlackjack)
            {
                MainUI.WriteInMainArea($"BLACKJACK! Dealer busts! You win {winnings} Rai!");
            }
            else
            {
                MainUI.WriteInMainArea($"Dealer busts! You win {bet} Rai (total: {winnings} Rai)");
            }
        }
        else if (playerValue > dealerValue)
        {
            int winnings = playerBlackjack ? (int)(bet * 2.5) : bet * 2;
            Program.player.money += winnings;
            
            if (playerBlackjack)
            {
                MainUI.WriteInMainArea($"BLACKJACK! You win {winnings} Rai!");
            }
            else
            {
                MainUI.WriteInMainArea($"You win! You gain {bet} Rai (total: {winnings} Rai)");
            }
        }
        else if (playerValue < dealerValue)
        {
            MainUI.WriteInMainArea($"You lose! You lost {bet} Rai.");
        }
        else
        {
            Program.player.money += bet;
            MainUI.WriteInMainArea($"Push! It's a tie. Your bet of {bet} Rai is returned.");
        }

        MainUI.WriteInMainArea($"Current Rai: {Program.player.money}");
        MainUI.WriteInMainArea(new string('=', 40));
        Program.SavePlayer();

        MainUI.WriteInMainArea("\n1 : Play again");
        MainUI.WriteInMainArea("0 : Leave");

        if (int.TryParse(Console.ReadKey(true).KeyChar.ToString(), out int playAgain) && playAgain == 1)
        {
            BlackjackLogic();
        }
        else
        {
            MainUI.ClearMainArea();
            DoSubLocation();
        }
    }

    private (int rank, string suit) DrawRandomCard(Random rand)
    {
        string[] suits = { "Hearts", "Diamonds", "Clubs", "Spades" };
        int rank = rand.Next(1, 14);
        string suit = suits[rand.Next(suits.Length)];
        return (rank, suit);
    }

    private string FormatCard((int rank, string suit) card)
    {
        string rankStr = card.rank switch
        {
            1 => "Ace",
            11 => "Jack",
            12 => "Queen",
            13 => "King",
            _ => card.rank.ToString()
        };
        return $"{rankStr} of {card.suit}";
    }

    private int CalculateHandValue(List<(int rank, string suit)> cards)
    {
        int value = 0;
        int aceCount = 0;

        foreach (var card in cards)
        {
            if (card.rank == 1)
            {
                aceCount++;
                value += 11;
            }
            else if (card.rank >= 11)
            {
                value += 10;
            }
            else
            {
                value += card.rank;
            }
        }

        while (value > 21 && aceCount > 0)
        {
            value -= 10;
            aceCount--;
        }

        return value;
    }
    void RouletteLogic()
    {
        MainUI.WriteInMainArea($"\nhow much do you want to bet?  current Rai: {Program.player.money} \nthe max bet is {casinoMaxBet}");
        int.TryParse(Console.ReadLine(), out int bet);
        if (bet == null || bet > casinoMaxBet || bet < 0)
        {
            MainUI.ClearMainArea();
            MainUI.WriteInMainArea("sweetie you gotta type a number that we can use\n ");
            DoSubLocation();
            return;
        }
        else if (bet > Program.player.money)
        {
            MainUI.WriteInMainArea("\nyou dont have that much Rai\n ");
            DoSubLocation();
            return;
        }
        MainUI.WriteInMainArea($"you bet: {bet}\n");
        Program.player.money -= bet;
        Program.SavePlayer();

        MainUI.WriteInMainArea($"where would you like to bet, \nblack : 1 \nred : 2 \neven : 3 \nodd : 4 \n1st 12 : 5 " +
            $"\n2nd 12 : 6 \n3rd 12 : 7 \n1 to 18(half) : 8 \n19 to 36(half) : 9 \nspecific number : 0\n");


        Random rand = new Random();
        int result = rand.Next(0, 35);

        HashSet<int> red = new HashSet<int>
                {
                    1, 3, 5, 7, 9, 12, 14, 16, 18,
                    19, 21, 23, 25, 27, 30, 32, 34, 36
                };

        int.TryParse(Console.ReadKey(true).KeyChar.ToString(), out int betPlace);
        switch (betPlace)
        {
            case 1:
                MainUI.WriteInMainArea($"you bet black \nthe number rolled is {result}");

                if (result == 0) ;
                else if (!red.Contains(result) && result != 0)
                {
                    MainUI.WriteInMainArea("you win!!!");
                    Program.player.money += bet * 2;
                }
                else
                {
                    MainUI.WriteInMainArea("you lose\n");
                }
                break;
            case 2:
                MainUI.WriteInMainArea($"you bet red \nthe number rolled is {result}");

                if (result == 0) ;
                else if (red.Contains(result))
                {
                    MainUI.WriteInMainArea("you win!!!");
                    Program.player.money += bet * 2;
                }
                else
                {
                    MainUI.WriteInMainArea("you lose\n");
                }
                break;
            case 3:
                MainUI.WriteInMainArea($"you bet even \nthe number rolled is {result}");
                if (result % 2 == 0 && result != 0)
                {
                    MainUI.WriteInMainArea("you win!!!");
                    Program.player.money += bet * 2;

                }
                else
                {
                    MainUI.WriteInMainArea("you lose\n");
                }
                break;
            case 4:
                MainUI.WriteInMainArea($"you bet odd \nthe number rolled is {result}");
                if (result % 2 == 1)
                {
                    MainUI.WriteInMainArea("you win!!!");
                    Program.player.money += bet * 2;

                }
                else
                {
                    MainUI.WriteInMainArea("you lose\n");
                }

                break;
            case 5:
                MainUI.WriteInMainArea($"you bet 1st 12 \nthe number rolled is {result}");
                if (result <= 12 && result != 0)
                {
                    MainUI.WriteInMainArea("you win!!!");
                    Program.player.money += bet * 3;
                }
                else
                {
                    MainUI.WriteInMainArea("you lose\n");
                }
                break;
            case 6:
                MainUI.WriteInMainArea($"you bet 2nd 12 \nthe number rolled is {result}");
                if (result > 12 && result <= 24)
                {
                    MainUI.WriteInMainArea("you win!!!");
                    Program.player.money += bet * 3;
                }
                else
                {
                    MainUI.WriteInMainArea("you lose\n");
                }
                break;
            case 7:
                MainUI.WriteInMainArea($"you bet 3rd 12 \nthe number rolled is {result}");
                if (result > 24)
                {
                    MainUI.WriteInMainArea("you win!!!");
                    Program.player.money += bet * 3;
                }
                else
                {
                    MainUI.WriteInMainArea("you lose\n");
                }
                break;
            case 8:
                MainUI.WriteInMainArea($"you bet 1st 18(half) \nthe number rolled is {result}");
                if (result <= 18 && result != 0)
                {
                    MainUI.WriteInMainArea("you win!!!");
                    Program.player.money += bet * 2;
                }
                else
                {
                    MainUI.WriteInMainArea("you lose\n");
                }
                break;
            case 9:
                MainUI.WriteInMainArea($"you bet 2nd 18(half) \nthe number rolled is {result}");
                if (result > 18)
                {
                    MainUI.WriteInMainArea("you win!!!");
                    Program.player.money += bet * 2;
                }
                else
                {
                    MainUI.WriteInMainArea("you lose\n");
                }
                break;
            case 0:
                int betNumber;
                while (true)
                {
                    MainUI.WriteInMainArea("what number?");
                    int.TryParse(Console.ReadLine(), out betNumber);
                    if (betNumber != null && betNumber < 36)
                    {
                        break;
                    }
                    MainUI.WriteInMainArea("type a number please");
                }

                MainUI.WriteInMainArea($"you bet {betNumber} \nthe number rolled is {result}");
                if (result == betNumber)
                {
                    MainUI.WriteInMainArea("you win!!!");
                    Program.player.money += bet * 34;
                }
                else
                {
                    MainUI.WriteInMainArea("you lose\n");
                }
                break;

        }

        MainUI.WriteInMainArea($"you have {Program.player.money} Rai");

        MainUI.WriteInMainArea("\nGamble some more!! : 1\nLeave : 0");
        int.TryParse(Console.ReadKey(true).KeyChar.ToString(), out int input3);
        if (input3 == null || input3 > 2 || input3 < 0)
        {
            MainUI.ClearMainArea();
            MainUI.WriteInMainArea("sweetie you gotta type a number that we can use\n ");
            DoSubLocation();
            return;
        }
        else if (input3 == 1)
        {
            MainUI.ClearMainArea();
            DoSubLocation();
            return;
        }
        else
        {
            MainUI.ClearMainArea();
            Program.MainMenu();
            return;
        }



    
    }
    #endregion

    #region wilderness
    void WildernessLogic()
    {
        EncounterManager encounterManager = new EncounterManager(Program.player);
        encounterManager.ProcessWildernessEncounters(LocationLibrary.Get(Program.player.currentLocation));

        Program.MainMenu();
    }


    #endregion

    #region fishingPond

    int fishingMeter=1;
    int fishingMeterTarget = 15;

    int position = 0;
    int direction = 1;
    int width = 10;         
    int target = 5;        
    bool fishing = true;   

    void FishingLogic()
    {
        fishingMeter = 1;

        MainUI.WriteInMainArea("You ready to start fishing?\nStart fishing : 1 \nGo back to menu : 0");
        string input = Console.ReadKey(true).KeyChar.ToString();
        int.TryParse(input, out int r);
        if (r==0)
        {
            MainUI.ClearMainArea();
            Program.MainMenu();
            return;
        }

        fishing = true;

        Random rand = new Random();
        fishingMeterTarget = rand.Next(15,26);

        while (fishing) 
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                {
                    if (position == target)
                    {
                        MainUI.WriteInMainArea("PERFECT!");

                        fishingMeter += 3;
                    }
                    else if (position >= target - 1 && position <= target + 1)
                    {
                        MainUI.WriteInMainArea("Good! ");

                        fishingMeter++;
                    }
                    else
                    {
                        MainUI.WriteInMainArea("Oh no... it got away.");

                        MainUI.WriteInMainArea("Press Enter to continue... ");
                        Console.ReadLine();
                        fishing = false;
                        break;
                    }
                    
                    Thread.Sleep(200);
                }
            }

            position += direction;

            if (position >= width)
            {
                position = width;
                direction = -1; 
            }
            else if (position <= 0)
            {
                position = 0;
                direction = 1;  
            }



            MainUI.ClearMainArea();

            DrawFishingBar(fishingMeter, fishingMeterTarget, 30);

            MainUI.WriteInMainArea("press Enter when the x is over the v");

            string targetLine = new string('_', target) + "V" + new string('_', target);
            MainUI.WriteInMainArea(targetLine);

            string movingLine = new string('_', position) + "X" + new string('_', width - position);
            MainUI.WriteInMainArea(movingLine);

            if (fishingMeter >= fishingMeterTarget)
            {
                MainUI.WriteInMainArea("\nYOU GOT FISH!!");


                Inventory.AddItem(ItemLibrary.fish,1);

                MainUI.WriteInMainArea("Press Enter to continue...");
                Console.ReadLine();
                fishing = false;
                break;
            }

            Thread.Sleep(100);

        }

        DoSubLocation();
    }
    private static void DrawFishingBar(int current, int max, int width)
    {
        int barWidth = width - 15;
        int filled = max > 0 ? (int)((double)current / max * barWidth) : 0;
        filled = Math.Max(0, Math.Min(filled, barWidth));

        string st = "Fish: "+ new string('█', filled)+ new string('░', barWidth - filled)+ $" {Math.Max(0, current)}/{max}";
        MainUI.WriteInMainArea(st);
    }

    #endregion

    #region graveyard
    void GraveyardLogic()
    {
        MainUI.ClearMainArea();
        MainUI.WriteInMainArea("Welcome to the Graveyard...");
        MainUI.WriteInMainArea("Here lie the spirits of fallen warriors who died in this town.");
        MainUI.WriteInMainArea("");
        MainUI.WriteInMainArea("Loading spirits...");
        Thread.Sleep(500);
        
        List<string> allDeadPlayerNames = Program.db.GetAllDeadPlayerNames();
        List<string> deadPlayerNames = new List<string>();
        
        foreach (var name in allDeadPlayerNames)
        {
            var deadPlayer = Program.db.LoadDeadPlayer(name);
            if (deadPlayer != null)
            {
                if (deadPlayer.currentLocation == Program.player.currentLocation)
                {
                    deadPlayerNames.Add(name);
                }
            }
        }
        
        MainUI.ClearMainArea();
        MainUI.WriteInMainArea("Welcome to the Graveyard...");
        MainUI.WriteInMainArea("Here lie the spirits of fallen warriors who died in this town.");
        MainUI.WriteInMainArea("");
        
        if (deadPlayerNames.Count == 0)
        {
            MainUI.WriteInMainArea("The graveyard is empty. No souls linger here...");
            MainUI.WriteInMainArea("");
            MainUI.WriteInMainArea("Press Enter to leave");
            Console.ReadLine();
            Program.MainMenu();
            return;
        }

        MainUI.WriteInMainArea($"You sense {deadPlayerNames.Count} restless spirit(s) here.");
        MainUI.WriteInMainArea("");
        MainUI.WriteInMainArea("Would you like to:");
        MainUI.WriteInMainArea("1. Challenge a spirit to combat");
        MainUI.WriteInMainArea("0. Leave");
        MainUI.WriteInMainArea("");

        int.TryParse(Console.ReadKey(true).KeyChar.ToString(), out int choice);
        
        if (choice == 0)
        {
            MainUI.ClearMainArea();
            Program.MainMenu();
            return;
        }
        else if (choice == 1)
        {
            ShowDeadPlayersList(deadPlayerNames);
        }
        else
        {
            MainUI.ClearMainArea();
            MainUI.WriteInMainArea("Invalid choice.");
            Thread.Sleep(1000);
            DoSubLocation();
        }
    }

    void ShowDeadPlayersList(List<string> deadPlayerNames)
    {
        int currentPage = 1;
        int itemsPerPage = 8;
        string searchTerm = "";
        
        while (true)
        {
            List<Player> deadPlayers = new List<Player>();
            foreach (var name in deadPlayerNames)
            {
                var p = Program.db.LoadDeadPlayer(name);
                if (p != null)
                {
                    deadPlayers.Add(p);
                }
            }
            
            List<Player> filteredPlayers;
            if (string.IsNullOrEmpty(searchTerm))
            {
                filteredPlayers = deadPlayers;
            }
            else
            {
                filteredPlayers = deadPlayers
                    .Where(p => p.name.ToLower().Contains(searchTerm.ToLower()) ||
                               p.playerClass.name.ToLower().Contains(searchTerm.ToLower()))
                    .ToList();
            }
            
            int totalItems = filteredPlayers.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / itemsPerPage);
            if (totalPages == 0) totalPages = 1;
            if (currentPage > totalPages) currentPage = totalPages;
            if (currentPage < 1) currentPage = 1;

            List<Player> pagePlayers = filteredPlayers
                .Skip((currentPage - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToList();

            MainUI.ClearMainArea();
            MainUI.WriteInMainArea("=== Restless Spirits ===");
            MainUI.WriteInMainArea("");
            
            if (!string.IsNullOrEmpty(searchTerm))
            {
                MainUI.WriteInMainArea($"Searching for: \"{searchTerm}\"");
                MainUI.WriteInMainArea("");
            }

            MainUI.WriteInMainArea("Nr     Name                 Level  Class            HP");
            MainUI.WriteInMainArea("----------------------------------------------------------------");
            
            for (int i = 0; i < pagePlayers.Count; i++)
            {
                var p = pagePlayers[i];
                MainUI.WriteInMainArea($"{i + 1,-7}{p.name,-20} {p.level,-7}{p.playerClass.name,-17}{p.HP}/{p.maxHP}");
            }
            
            MainUI.WriteInMainArea("");
            MainUI.WriteInMainArea($"--- Page {currentPage} of {totalPages} ---");
            MainUI.WriteInMainArea("");
            MainUI.WriteInMainArea("Type spirit number (1-8) to challenge, or:");
            MainUI.WriteInMainArea("[N] Next Page  [P] Prev Page  [S] Search  [0] Back");

            string input = Console.ReadKey(true).KeyChar.ToString().ToLower();

            if (input == "n")
            {
                if (currentPage < totalPages) currentPage++;
                continue;
            }
            if (input == "p")
            {
                if (currentPage > 1) currentPage--;
                continue;
            }
            if (input == "s")
            {
                MainUI.ClearMainArea();
                MainUI.WriteInMainArea("Enter search term (or leave empty to clear):");
                MainUI.WriteInMainArea("> ");
                searchTerm = Console.ReadLine()?.ToLower() ?? "";
                currentPage = 1;
                continue;
            }

            if (int.TryParse(input, out int selection))
            {
                if (selection == 0)
                {
                    DoSubLocation();
                    return;
                }
                
                if (selection >= 1 && selection <= pagePlayers.Count)
                {
                    Player selectedPlayer = pagePlayers[selection - 1];
                    ChallengeDeadPlayer(selectedPlayer.name);
                    return;
                }
            }

            MainUI.ClearMainArea();
            MainUI.WriteInMainArea("Invalid choice. Please try again.");
            Thread.Sleep(1000);
        }
    }

    void ChallengeDeadPlayer(string deadPlayerName)
    {
        MainUI.ClearMainArea();
        MainUI.WriteInMainArea($"Loading spirit of {deadPlayerName}...");
        
        Player deadPlayer = Program.db.LoadDeadPlayer(deadPlayerName);
        
        if (deadPlayer == null)
        {
            MainUI.WriteInMainArea($"The spirit of {deadPlayerName} has moved on...");
            MainUI.WriteInMainArea("They are no longer here.");
            Thread.Sleep(2000);
            DoSubLocation();
            return;
        }

        MainUI.WriteInMainArea($"You have summoned the spirit of {deadPlayer.name}!");
        MainUI.WriteInMainArea($"Level {deadPlayer.level} {deadPlayer.playerClass.name}");
        MainUI.WriteInMainArea($"HP: {deadPlayer.HP}/{deadPlayer.maxHP}");
        MainUI.WriteInMainArea("");
        MainUI.WriteInMainArea("Prepare for battle!");
        Thread.Sleep(2000);

        Enemy spiritEnemy = ConvertPlayerToEnemy(deadPlayer);
        
        Program.pendingDeadPlayerUpdate = deadPlayer;
        Program.pendingSpiritEnemy = spiritEnemy;
        
        List<Enemy> enemies = new List<Enemy> { spiritEnemy };
        CombatManager combat = new CombatManager(Program.player, enemies, false, null);
        combat.StartCombat();

        Program.pendingDeadPlayerUpdate = null;
        Program.pendingSpiritEnemy = null;

        if (Program.player.IsAlive())
        {
            MainUI.ClearMainArea();

            MainUI.RenderMainMenuScreen(Program.player);

            //CombatUI ui = new CombatUI();
            MainUI.WriteInMainArea("");
            MainUI.WriteInMainArea($"You have defeated the spirit of {deadPlayer.name}!");
            MainUI.WriteInMainArea("Their soul has been laid to rest...");

            Program.player.IncrementStat("totalCorpesKilled");

            if(Program.player.playerClass.name == ClassLibrary.Necromancer.name)
            {
                var companions = CompanionSystem.GetCompanions(Program.player);

                if (Program.player.GetStat("CommandDeadSlots") > companions.Count)
                {
                    MainUI.WriteInMainArea($"you've got space to clam anouther soul for your horde\ndo you wish to take {deadPlayer.name} as a soldier? (y/n)");
                    string choice = Console.ReadKey(true).KeyChar.ToString().ToLower();

                    if (choice == "y" || choice == "yes")
                    {
                        MainUI.WriteInMainArea($"You take {deadPlayer.name} into your army \nthey will be of great help in combat");

                        companions.Add(ConvertPlayerToEnemy(deadPlayer));
                        CompanionSystem.SaveCompanions(Program.player, companions);

                        MainUI.WriteInMainArea($"{deadPlayer.name} has joined your party!");

                    }
                    else if (choice == "n" || choice == "no")
                    {
                        MainUI.WriteInMainArea($"{deadPlayer.name} lets out a sigh and disspears clearly grateful for your decision");

                    }
                    else
                    {
                        MainUI.WriteInMainArea("the dead don't speak spanish, tell them yes or no next time");

                    }
                }
                else
                {
                    MainUI.WriteInMainArea("your army is full you cannot claim this soul");
                }
            }

            Program.db.DeleteDeadPlayer(deadPlayer.name);
            
            Thread.Sleep(2000);
            MainUI.WriteInMainArea("\nPress Enter to continue...");
            Console.ReadLine();
            
            Program.MainMenu();
        }
    }

    Enemy ConvertPlayerToEnemy(Player player)
    {
        int moneyDrop = (int)Math.Round(player.money * 0.1);
        
        Enemy enemy = new Enemy
        {
            name = player.name + " (Spirit)",
            level = player.level,
            HP = player.HP,
            maxHP = player.maxHP,
            speed = player.speed,
            armor = player.armor,
            dodge = player.dodge,
            dodgeNegation = player.dodgeNegation,
            critChance = player.critChance,
            critDamage = player.critDamage,
            stun = player.stun,
            stunNegation = player.stunNegation,
            money = moneyDrop,
            exp = player.level * 10,
            attacks = new List<Attack>()
        };

        foreach (var attack in player.equippedAttacks)
        {
            if (attack != null)
            {
                enemy.attacks.Add(attack);
            }
        }

        if (enemy.attacks.Count == 0)
        {
            enemy.attacks.Add(new Attack("Spirit Strike", new List<AttackEffect>
            {
                new AttackEffect("damage", 5, 0, "enemy")
            }));
        }

        return enemy;
    }
    #endregion

    #region port
    void PortLogic()
    {
        MainUI.ClearMainArea();
        MainUI.WriteInMainArea("=== Port Travel ===");
        MainUI.WriteInMainArea("You can travel to other ports you've discovered.");
        MainUI.WriteInMainArea("");

        // Get all port locations
        List<Location> portLocations = new List<Location>();
        foreach (var loc in LocationLibrary.locations)
        {
            bool hasPort = false;
            foreach (var subLoc in loc.subLocationsHere)
            {
                if (subLoc.type == SubLocationType.port)
                {
                    hasPort = true;
                    break;
                }
            }
            if (hasPort && loc.name != Program.player.currentLocation)
            {
                portLocations.Add(loc);
            }
        }

        if (portLocations.Count == 0)
        {
            MainUI.WriteInMainArea("No other ports are available for travel.");
            MainUI.WriteInMainArea("");
            MainUI.WriteInMainArea("Press Enter to leave");
            Console.ReadLine();
            Program.MainMenu();
            return;
        }

        MainUI.WriteInMainArea("Available Ports:");
        MainUI.WriteInMainArea("");
        MainUI.WriteInMainArea("Nr     Port Name            Status");
        MainUI.WriteInMainArea("----------------------------------------");

        int displayIndex = 1;
        List<Location> availablePorts = new List<Location>();
        
        foreach (var port in portLocations)
        {
            if (Program.player.knownLocationnames.Contains(port.name))
            {
                MainUI.WriteInMainArea($"{displayIndex,-7}{port.name,-20} Available");
                availablePorts.Add(port);
                displayIndex++;
            }
            else
            {
                MainUI.WriteInMainArea($"?      ???                  Undiscovered");
            }
        }

        MainUI.WriteInMainArea("");
        MainUI.WriteInMainArea("0. Leave");
        MainUI.WriteInMainArea("");
        MainUI.WriteInMainArea("Select a port to travel to:");

        string input = Console.ReadLine() ?? "";
        if (!int.TryParse(input, out int choice))
        {
            MainUI.WriteInMainArea("\nInvalid input.");
            Thread.Sleep(1000);
            DoSubLocation();
            return;
        }

        if (choice == 0)
        {
            MainUI.ClearMainArea();
            Program.MainMenu();
            return;
        }

        if (choice < 1 || choice > availablePorts.Count)
        {
            MainUI.WriteInMainArea("\nInvalid selection.");
            Thread.Sleep(1000);
            DoSubLocation();
            return;
        }

        Location selectedPort = availablePorts[choice - 1];
        
        MainUI.ClearMainArea();
        MainUI.WriteInMainArea($"Traveling to {selectedPort.name}...");
        Thread.Sleep(1500);
        
        Program.player.currentLocation = selectedPort.name;
        Program.SavePlayer();
        
        MainUI.WriteInMainArea($"You have arrived at {selectedPort.name}!");
        Thread.Sleep(1000);
        
        Program.MainMenu();
    }
    #endregion

    #region marketplace

    public void MarketplaceLogic()
    {
        while (true)
        {
            MainUI.ClearMainArea();
            MainUI.WriteInMainArea("Welcome to the market");
            MainUI.WriteInMainArea("Would you like to:");
            MainUI.WriteInMainArea("1 : Sell items");
            MainUI.WriteInMainArea("2 : Sell materials");
            MainUI.WriteInMainArea("0 : Leave");

            if (int.TryParse(Console.ReadKey(true).KeyChar.ToString(), out int input2) == false || input2 > 2 || input2 < 0)
            {
                MainUI.WriteInMainArea(" \nyou gotta type a number from 0-2");
                Thread.Sleep(1000);
                continue;
            }
            else if (input2 == 0)
            {
                Program.MainMenu();
                break;
            }
            else if (input2 == 1)
            {

                const float exponent = 1.5f;
                const float scale = 0.1f;

                while (true)
                {
                    MainUI.ClearMainArea();
                    MainUI.WriteInMainArea("Select an item to sell:");
                    MainUI.WriteInMainArea("");
                    MainUI.WriteInMainArea("nr     Name                      Qty     Value");
                    MainUI.WriteInMainArea("------------------------------------------------");

                    if (Program.player.ownedItems.Count == 0)
                    {
                        MainUI.WriteInMainArea("You have no items to sell.");
                        MainUI.WriteInMainArea("");
                        MainUI.WriteInMainArea("0 : Back");
                    }
                    else
                    {
                        for (int i = 0; i < Program.player.ownedItems.Count; i++)
                        {
                            var item = Program.player.ownedItems[i];
                            MainUI.WriteInMainArea($"{i + 1,-7}{item.name,-25} {item.amount,-7}  {item.value}");
                        }
                        MainUI.WriteInMainArea("");
                        MainUI.WriteInMainArea("0 : Back");
                    }

                    string inputString = Console.ReadLine() ?? "";
                    var n = int.TryParse(inputString, out int input);

                    if (input == 0)
                    {
                        Program.MainMenu();
                        return;
                    }

                    if (!n || input < 1 || input > Program.player.ownedItems.Count)
                    {
                        MainUI.WriteInMainArea("\nInvalid selection. Please type a number from the list");
                        Thread.Sleep(1000);
                        continue;
                    }

                    Item selectedItem = Program.player.ownedItems[input - 1];
                    int quantity = 1;

                    // If stackable, ask how many
                    if (selectedItem.type != ItemType.Equipment)
                    {
                        MainUI.WriteInMainArea($"How many {selectedItem.name} would you like to sell? (Max: {selectedItem.amount})");
                        string quantityString = Console.ReadLine() ?? "";
                        var q = int.TryParse(quantityString, out quantity);

                        if (!q || quantity < 1 || quantity > selectedItem.amount)
                        {
                            MainUI.WriteInMainArea("\nInvalid amount");
                            Thread.Sleep(1000);
                            continue;
                        }
                    }

                    // Check if item is equipped
                    int equippedSlot = Program.player.equippedItems.IndexOf(selectedItem);
                    if (equippedSlot >= 0)
                    {
                        MainUI.WriteInMainArea($"\nThis item is equipped. Unequipping {selectedItem.name}...");
                        Inventory.UnequipItem(equippedSlot);
                        Thread.Sleep(1000);
                    }

                    // Handle Stats & Effects
                    if (selectedItem.type == ItemType.Artifact)
                    {
                        Inventory.RemoveEffects(selectedItem, null); // Remove stats for the whole stack
                    }

                    // Handle Item List
                    if (selectedItem.type == ItemType.Equipment || selectedItem.amount <= quantity)
                    {
                        // Remove the item completely
                        Program.player.ownedItems.Remove(selectedItem);
                    }
                    else
                    {
                        // Just subtract the amount
                        selectedItem.amount -= quantity;
                    }

                    // Re-apply artifact stats if some items are left
                    if (selectedItem.type == ItemType.Artifact && Program.player.ownedItems.Contains(selectedItem))
                    {
                        Inventory.ApplyEffects(selectedItem, null);
                    }

                    Program.player.money += selectedItem.value * quantity;

                    // Handle Weight & Speed 
                    Inventory.UpdateWeight();

                    MainUI.WriteInMainArea($"\nSold {quantity}x {selectedItem.name} for {selectedItem.value * quantity} Rai");
                    Thread.Sleep(1000);
                }
            }
            else if (input2 == 2)
            {
                // Sell materials from material bag
                while (true)
                {
                    MainUI.ClearMainArea();
                    MainUI.WriteInMainArea("Select a material to sell:");
                    MainUI.WriteInMainArea("");
                    MainUI.WriteInMainArea("nr     Name                      Qty     Value");
                    MainUI.WriteInMainArea("------------------------------------------------");

                    if (Program.player.materialItems.Count == 0)
                    {
                        MainUI.WriteInMainArea("You have no materials to sell.");
                        MainUI.WriteInMainArea("");
                        MainUI.WriteInMainArea("0 : Back");
                    }
                    else
                    {
                        for (int i = 0; i < Program.player.materialItems.Count; i++)
                        {
                            var mat = Program.player.materialItems[i];
                            MainUI.WriteInMainArea($"{i + 1,-7}{mat.name,-25} {mat.amount,-7}  {mat.value}");
                        }
                        MainUI.WriteInMainArea("");
                        MainUI.WriteInMainArea("0 : Back");
                    }

                    string inputString = Console.ReadLine() ?? "";
                    var n = int.TryParse(inputString, out int input);

                    if (input == 0)
                    {
                        break;
                    }

                    if (!n || input < 1 || input > Program.player.materialItems.Count)
                    {
                        MainUI.WriteInMainArea("\nInvalid selection. Please type a number from the list");
                        Thread.Sleep(1000);
                        continue;
                    }

                    Item selectedMat = Program.player.materialItems[input - 1];
                    int quantity = 1;

                    MainUI.WriteInMainArea($"How many {selectedMat.name} would you like to sell? (Max: {selectedMat.amount})");
                    string quantityString = Console.ReadLine() ?? "";
                    var q = int.TryParse(quantityString, out quantity);

                    if (!q || quantity < 1 || quantity > selectedMat.amount)
                    {
                        MainUI.WriteInMainArea("\nInvalid amount");
                        Thread.Sleep(1000);
                        continue;
                    }

                    // Remove from material bag
                    selectedMat.amount -= quantity;
                    Program.player.currentMaterialLoad -= quantity;
                    if (Program.player.currentMaterialLoad < 0) Program.player.currentMaterialLoad = 0;

                    if (selectedMat.amount <= 0)
                    {
                        Program.player.materialItems.Remove(selectedMat);
                    }

                    // Add money
                    Program.player.money += selectedMat.value * quantity;

                    MainUI.WriteInMainArea($"\nSold {quantity}x {selectedMat.name} for {selectedMat.value * quantity} Rai");
                    Thread.Sleep(1000);
                }
            }

            Program.SavePlayer();
        }
    }

    #endregion

    #region mine

    void MineLogic()
    {
        MainUI.WriteInMainArea("You ready to start mining?\nStart mining : 1 \nGo back to menu : 0");

        string input = Console.ReadKey(true).KeyChar.ToString(); 
        int.TryParse(input, out int r);
        if (r == 0)
        {
            MainUI.ClearMainArea();
            Program.MainMenu();
            return;
        }

        Random rand = new Random();
        int veinStability = rand.Next(75, 101);
        int currentHaul = 0;
        bool collapsed = false;

        while (true)
        {
            MainUI.ClearMainArea();
            MainUI.WriteInMainArea($"You are chipping at a promising iron vein\n\nVein Stability: {veinStability}%\nYour Haul: {currentHaul} Iron\n\n1 : Mine deeper \n2 : Walk away with your haul");

            string choice = Console.ReadKey(true).KeyChar.ToString();

            if (choice == "2")
            {
                break; 
            }
            else if (choice == "1")
            {
                int failChance = 100 - veinStability;
                int roll = rand.Next(0, 100);

                if (roll < failChance)
                {
                    int dm = rand.Next(1, 36);
                    MainUI.ClearMainArea();
                    MainUI.WriteInMainArea($"The vein collapsed and covers you in rocks \nYou walk away with 0 Iron and a hurt head, {dm} damage\n");
                    Program.player.HP -= dm;
                    Program.CheckPlayerDeath();
                    collapsed = true;
                    currentHaul = 0;
                    MainUI.WriteInMainArea("press enter to continue...");
                    Console.ReadLine();
                    break; 
                }
                else
                {
                    int found = rand.Next(1, 3); 
                    int stabilityLost = rand.Next(15, 31);

                    currentHaul += found;
                    veinStability -= stabilityLost;

                    MainUI.WriteInMainArea($"\nSuccess\n You chip away and find {found} Iron");

                    if (veinStability <= 0)
                    {
                        MainUI.WriteInMainArea("");
                        MainUI.WriteInMainArea("You've mined all you can from this vein \nIt's too unstable to continue\n");
                        MainUI.WriteInMainArea("Press enter to continue...");
                        Console.ReadLine();
                        break; 
                    }
                }
            }
        }
        if (!collapsed)
        {
            MainUI.ClearMainArea();
            MainUI.WriteInMainArea($"You walk away from the mine.\nYou collected {currentHaul} Iron.");
            Inventory.AddItem(ItemLibrary.iron, currentHaul);
            MainUI.WriteInMainArea($"Press enter to continue...");
            Console.ReadLine();
        }

        MineLogic();
    }


    #endregion

    #endregion

    #region specificUse

    #region crypt

    void CryptLogic()
    {
        MainUI.ClearMainArea();
        int corpesKilled = Program.player.GetStat("totalCorpesKilled");

        if(corpesKilled < 5)
        {
            MainUI.WriteInMainArea("you stare upon the large stone door of the crypt \na feeling of dread filling every inch your body " +
                "\nyou swallow your emotion and try to open it \nit doesnt budge, you try everything but the door wont open for you\n" +
                "you turn around and leave\nyou didnt wanna enter anyway\n\nPress enter for main menu..");

            Console.ReadLine();
            Program.MainMenu();
            return;

        }
        else if (Program.player.playerClass.name != ClassLibrary.Necromancer.name)
        {
            MainUI.WriteInMainArea("you stare upon the large stone door of the crypt \nIt seems oddly familliar to you \nyou move in to open it...\n" +
                "upon touching it you immediately feel a sharp pain\nas your soul is ripped from your body \ndecending through the door into the crypt\n ");
            MainUI.WriteInMainArea("inside you see many horrors comparable to those of hell \none stands out to you, you approach it \n\n" +
                "-you have the power to become something great \nyou could command armies of us\nyou could wage war with empires for fun");
            MainUI.WriteInMainArea("\nDo you wish to optain this power? (y/n)");

            string choice = Console.ReadKey(true).KeyChar.ToString().ToLower();
            if (choice == "y" || choice == "yes")
            {
                MainUI.WriteInMainArea($"\nit seems pleased\n\n-good choice {Program.player.name} the dead shall answer your call");
                MainUI.WriteInMainArea("\nyou awaken outside the crypt \nupon looking down at yourself you realise you've changed " +
                    "\nyou've come to look a lot like the corpses from the graveyards");

                Program.player.playerClass = ClassLibrary.Necromancer;
                Program.player.RecalculateStats();

                Program.SavePlayer();

                MainUI.WriteInMainArea("Press enter for main menu..");
                Console.ReadLine();
                Program.MainMenu();
                return;

            }
            if (choice == "n" || choice == "no")
            {
                MainUI.WriteInMainArea("\nYou reject the offer \nyou suddenly feel dizzy as your soul is suddenly reunited with your body" +
                    "\nyou find yourself outside the crypt \npress enter for main menu...");

                Console.ReadLine();
                Program.MainMenu();
                return;
            }
            else 
            {
                MainUI.WriteInMainArea("\nyour utterance or lack thereoff is seen as rejection " +
                    "\nyou feel dizzy as your soul is suddenly reunited with your body\n" +
                    "you find yourself outside the crypt \npress enter for main menu...");

                Console.ReadLine();
                Program.MainMenu();
                return;
            }

        }
        else
        {
            MainUI.WriteInMainArea("you've done all you ought to do here");

            MainUI.WriteInMainArea("Press enter for main menu..");
            Console.ReadLine();
            Program.MainMenu();
            return;
        }



    }

    #endregion

    #region trainingGrounds

    void TrainingGroundsLogic()
    {
        MainUI.ClearMainArea();
        MainUI.WriteInMainArea("=== Training Grounds ===");
        MainUI.WriteInMainArea("");
        MainUI.WriteInMainArea("A weathered man sits on a crate at the edge of a dusty fighting pit.");
        MainUI.WriteInMainArea("He looks you over slowly.");
        MainUI.WriteInMainArea("");
        MainUI.WriteInMainArea("1 : Talk to him");
        MainUI.WriteInMainArea("0 : Leave");

        if (!int.TryParse(Console.ReadKey(true).KeyChar.ToString(), out int choice) || choice < 0 || choice > 1)
        {
            MainUI.ClearMainArea();
            TrainingGroundsLogic();
            return;
        }

        if (choice == 0)
        {
            MainUI.ClearMainArea();
            Program.MainMenu();
            return;
        }

        Player player = Program.player;
        int gritWins = player.GetStat("totalGritVictories");
        bool isBrawler = player.playerClass == ClassLibrary.Brawler;

        MainUI.ClearMainArea();
        MainUI.WriteInMainArea("=== Kael ===");
        MainUI.WriteInMainArea("");

        if (!isBrawler)
        {
            if (gritWins == 0)
            {
                MainUI.WriteInMainArea("\"You haven't taken a real hit in your life, have you.\"");
                MainUI.WriteInMainArea("\"Come back when the world's knocked you around a bit.\"");
                MainUI.WriteInMainArea("");
                MainUI.WriteInMainArea("Press Enter to leave...");
                Console.ReadLine();
                Program.MainMenu();
                return;
            }
            else if (gritWins < 5)
            {
                MainUI.WriteInMainArea("\"You're starting to get it. You've been in some real scraps.\"");
                MainUI.WriteInMainArea("\"But I need more from you before I waste my time.\"");
                MainUI.WriteInMainArea($"\"Come back when you've proven yourself further. ({gritWins}/5 grit victories)\"");
                MainUI.WriteInMainArea("");
                MainUI.WriteInMainArea("Press Enter to leave...");
                Console.ReadLine();
                Program.MainMenu();
                return;
            }
            else
            {
                MainUI.WriteInMainArea("\"Alright. You've taken your hits and kept moving.\"");
                MainUI.WriteInMainArea("\"That tells me something. Let's see what you're actually made of.\"");
                MainUI.WriteInMainArea("\"Step into the pit. I'll go easy on you. Mostly.\"");
                MainUI.WriteInMainArea("");
                MainUI.WriteInMainArea("1 : Fight Kael");
                MainUI.WriteInMainArea("0 : Not right now");

                if (!int.TryParse(Console.ReadKey(true).KeyChar.ToString(), out int fightChoice) || fightChoice == 0)
                {
                    MainUI.ClearMainArea();
                    Program.MainMenu();
                    return;
                }

                Enemy kael = BuildKael(0);
                CombatManager combat = new CombatManager(player, new List<Enemy> { kael }, false, null);
                combat.StartCombat();

                if (player.IsAlive())
                {
                    if (kael.HP <= 0)
                    {
                        MainUI.ClearMainArea();
                        MainUI.WriteInMainArea("Kael lets out a slow breath and nods.");
                        MainUI.WriteInMainArea("\"Not bad. You hit like you mean it.\"");
                        MainUI.WriteInMainArea("\"The Brawler's way isn't about armor or tricks.\"");
                        MainUI.WriteInMainArea("\"It's about taking everything they throw at you and hitting back harder.\"");
                        MainUI.WriteInMainArea("\"You've got that. Class is yours if you want it.\"");
                        MainUI.WriteInMainArea("");
                        MainUI.WriteInMainArea("Become a Brawler? (y/n): ");

                        string classChoice = Console.ReadKey(true).KeyChar.ToString().ToLower();
                        if (classChoice == "y")
                        {
                            player.playerClass = ClassLibrary.Brawler;
                            player.RecalculateStats();
                            MainUI.WriteInMainArea("");
                            MainUI.WriteInMainArea("\"Welcome to the pit. You're a Brawler now.\"");
                            Program.SavePlayer();
                        }
                        else
                        {
                            MainUI.WriteInMainArea("\"Suit yourself. I'll be here.\"");
                        }
                    }
                    else
                    {
                        MainUI.ClearMainArea();
                        MainUI.WriteInMainArea("Kael catches you with a heavy hand and holds you upright.");
                        MainUI.WriteInMainArea("\"You're tough. But not yet.\"");
                        MainUI.WriteInMainArea("\"Come back when you've hardened up more.\"");
                    }

                    MainUI.WriteInMainArea("");
                    MainUI.WriteInMainArea("Press Enter to continue...");
                    Console.ReadLine();
                }

                Program.MainMenu();
                return;
            }
        }
        else
        {
            bool knowsIronFist = player.ownedAttacks.Contains(AttackLibrary.IronFist);
            bool knowsHaymaker = player.ownedAttacks.Contains(AttackLibrary.Haymaker);

            if (!knowsIronFist)
            {
                if (gritWins < 10)
                {
                    MainUI.WriteInMainArea("\"You're back. Good.\"");
                    MainUI.WriteInMainArea("\"Get more real fights behind you and come find me again.\"");
                    MainUI.WriteInMainArea($"\"({gritWins}/10 grit victories)\"");
                    MainUI.WriteInMainArea("");
                    MainUI.WriteInMainArea("Press Enter to leave...");
                    Console.ReadLine();
                    Program.MainMenu();
                    return;
                }

                MainUI.WriteInMainArea("\"10 real fights. You've earned another round with me.\"");
                MainUI.WriteInMainArea("\"Win and I'll teach you something.\"");
                MainUI.WriteInMainArea("");
                MainUI.WriteInMainArea("1 : Fight Kael");
                MainUI.WriteInMainArea("0 : Not right now");

                if (!int.TryParse(Console.ReadKey(true).KeyChar.ToString(), out int fightChoice1) || fightChoice1 == 0)
                {
                    MainUI.ClearMainArea();
                    Program.MainMenu();
                    return;
                }

                Enemy kael1 = BuildKael(1);
                CombatManager combat1 = new CombatManager(player, new List<Enemy> { kael1 }, false, null);
                combat1.StartCombat();

                if (player.IsAlive())
                {
                    if (kael1.HP <= 0)
                    {
                        player.IncrementStat("kaelFightsWon");
                        MainUI.ClearMainArea();
                        MainUI.WriteInMainArea("Kael rolls his shoulder and grins.");
                        MainUI.WriteInMainArea("\"Alright. Here's what I know about breaking someone down.\"");
                        MainUI.WriteInMainArea("\"Iron Fist. Hit the same spot twice and the third time they won't get back up.\"");
                        AttackManager atkManager = new AttackManager(player);
                        atkManager.LearnAttack(AttackLibrary.IronFist);
                        Program.SavePlayer();
                    }
                    else
                    {
                        MainUI.ClearMainArea();
                        MainUI.WriteInMainArea("Kael shakes his head slowly.");
                        MainUI.WriteInMainArea("\"More work to do. Come back when you're ready.\"");
                    }

                    MainUI.WriteInMainArea("");
                    MainUI.WriteInMainArea("Press Enter to continue...");
                    Console.ReadLine();
                }

                Program.MainMenu();
                return;
            }
            else if (!knowsHaymaker)
            {
                if (gritWins < 20 || player.level < 10)
                {
                    MainUI.WriteInMainArea("\"You've come a long way.\"");
                    MainUI.WriteInMainArea("\"But I've still got one more thing to teach. You're not ready for it yet.\"");
                    MainUI.WriteInMainArea($"\"(Level {player.level}/10, {gritWins}/20 grit victories)\"");
                    MainUI.WriteInMainArea("");
                    MainUI.WriteInMainArea("Press Enter to leave...");
                    Console.ReadLine();
                    Program.MainMenu();
                    return;
                }

                MainUI.WriteInMainArea("\"You've grown. I can see it.\"");
                MainUI.WriteInMainArea("\"One more fight. This time I won't hold back much.\"");
                MainUI.WriteInMainArea("");
                MainUI.WriteInMainArea("1 : Fight Kael");
                MainUI.WriteInMainArea("0 : Not right now");

                if (!int.TryParse(Console.ReadKey(true).KeyChar.ToString(), out int fightChoice2) || fightChoice2 == 0)
                {
                    MainUI.ClearMainArea();
                    Program.MainMenu();
                    return;
                }

                Enemy kael2 = BuildKael(2);
                CombatManager combat2 = new CombatManager(player, new List<Enemy> { kael2 }, false, null);
                combat2.StartCombat();

                if (player.IsAlive())
                {
                    if (kael2.HP <= 0)
                    {
                        player.IncrementStat("kaelFightsWon");
                        MainUI.ClearMainArea();
                        MainUI.WriteInMainArea("Kael sits down hard, breathing heavy. First time you've seen him winded.");
                        MainUI.WriteInMainArea("\"That's it. That's the last thing I've got to teach you.\"");
                        MainUI.WriteInMainArea("\"The Haymaker. Everything you've got in one swing.\"");
                        MainUI.WriteInMainArea("\"You'll be slow after. Make it count.\"");
                        AttackManager atkManager = new AttackManager(player);
                        atkManager.LearnAttack(AttackLibrary.Haymaker);
                        Program.SavePlayer();
                    }
                    else
                    {
                        MainUI.ClearMainArea();
                        MainUI.WriteInMainArea("Kael dusts himself off.");
                        MainUI.WriteInMainArea("\"Not yet. Keep at it.\"");
                    }

                    MainUI.WriteInMainArea("");
                    MainUI.WriteInMainArea("Press Enter to continue...");
                    Console.ReadLine();
                }

                Program.MainMenu();
                return;
            }
            else
            {
                MainUI.WriteInMainArea("\"You've learned everything I know.\"");
                MainUI.WriteInMainArea("\"Keep swinging.\"");
                MainUI.WriteInMainArea("");
                MainUI.WriteInMainArea("Press Enter to leave...");
                Console.ReadLine();
                Program.MainMenu();
            }
        }
    }

    Enemy BuildKael(int tier)
    {
        int hp, armor, speed;
        List<Attack> attacks;

        switch (tier)
        {
            case 1:
                hp = 120; armor = 5; speed = 12;
                attacks = new List<Attack> { AttackLibrary.HeavySlam, AttackLibrary.ThrowHands };
                break;
            case 2:
                hp = 200; armor = 10; speed = 15;
                attacks = new List<Attack> { AttackLibrary.HeavySlam, AttackLibrary.RecklessSwing, AttackLibrary.ThrowHands };
                break;
            default:
                hp = 80; armor = 3; speed = 10;
                attacks = new List<Attack> { AttackLibrary.ThrowHands, AttackLibrary.HeavySlam };
                break;
        }

        return new Enemy
        {
            name = "Kael",
            level = tier == 0 ? 5 : tier == 1 ? 9 : 14,
            HP = hp,
            maxHP = hp,
            speed = speed,
            armor = armor,
            dodge = 0,
            dodgeNegation = 5,
            critChance = 5,
            critDamage = 120,
            stun = 0,
            stunNegation = 10,
            money = 0,
            exp = 0,
            attacks = attacks
        };
    }

    #endregion

    #region SwordInStone

    List<(string, int)> swordOptions = new List<(string, int)> { 
        ("The sword shifts slightly", 1),
        ("The blade vibrates in your hands", 2),
        ("A faint hum echoes from the stone", 3),
        ("Heat climbs up the hilt into your hands", 4),
        ("The sword suddenly resists.",4),
        ("Dust trickles from the crack", 1),
        ("The humming becomes uneven", 2),
        ("The blade feels strangely weightless", 1),
        ("A rhythm pulses deep within the stone",3)
    };

    async Task SwordInStoneLogic()
    {
        MainUI.ClearMainArea();
        if (Program.player.HasStat("PlayerHasGottenExcSword"))
        {
            MainUI.WriteInMainArea("you enter the clearing that once gifted you such great a gift \nthe empty stone forever a reminder of how grateful you are");

            MainUI.WriteInMainArea("Press enter when you're ready to leave");
            Console.ReadLine();

            Program.MainMenu();
            return;
        }
        MainUI.WriteInMainArea("upon entering the clearing you lay eyes upon a sword \nit's beautiful, an almost magical aura surrounding it \n" +
            "you walk up to it, its just as beautiful up close \nthe blade looks sharp, you find yourself wishing it was yours \n" +
            "all you need to do is pull it from the stone in which it resides");
        MainUI.WriteInMainArea("\nDo you attempt to pull it out? (y/n)");
        string choice = Console.ReadKey(true).KeyChar.ToString().ToLower();
        
        bool hasStat = false;
        if(Program.player.HasStat("PlayerWorthyOfSword")) hasStat = true;

        if (choice == "y" || choice == "yes")
        {
            if (!hasStat)
            {
                int rand = new Random().Next(1, 101);
                if (rand == 1) 
                {
                    Program.player.IncrementStat("PlayerWorthyOfSword");
                    Program.player.IncrementStat("PlayerWorthyOfSword",-1);
                    Program.SavePlayer();
                }
                
            }

            MainUI.WriteInMainArea("You grip the hilt of the sword firmly");

            if (Program.player.GetStat("PlayerWorthyOfSword") > 0)
            {
                MainUI.WriteInMainArea("it feels natural in your hand, as if it was destined to be yours");
                MainUI.WriteInMainArea("Press enter when you're ready to pull");
                Console.ReadLine();

                MainUI.ClearMainArea();

                int fails = 0;
                int successes = 0;

                while (fails < 3 && successes < 10)
                {

                    MainUI.ClearMainArea();
                    int swordChoice = new Random().Next(0,swordOptions.Count);
                    MainUI.WriteInMainArea($"{swordOptions[swordChoice].Item1}");
                    MainUI.WriteInMainArea("How do you react?");
                    MainUI.WriteInMainArea("[PULL] : 1 [STEADY] : 2 [Listen] : 3 [YIELD] : 4");

                    string? input = await ReadLineWithTimeout(3000);

                    if (input == null)
                    {
                        fails++;
                    }
                    else if (int.Parse(input) == swordOptions[swordChoice].Item2)
                    {
                        successes++;
                    }
                    else
                    {
                        fails++;
                    }
                }
                if(fails >= 3)
                {
                    Program.player.IncrementStat("PlayerWorthyOfSword", -2);
                    Program.SavePlayer();
                    MainUI.WriteInMainArea("you aren't worthy");
                    Thread.Sleep(3000);

                    Program.MainMenu();
                    return;
                }
                else
                {
                    MainUI.WriteInMainArea("you've acquired the legendary sword Excalibur");

                    Inventory.AddItem(ItemLibrary.Excalibur, 1);

                    Program.player.IncrementStat("PlayerHasGottenExcSword");
                    Program.SavePlayer();

                    MainUI.WriteInMainArea("Press enter when you're ready to leave");
                    Console.ReadLine();

                    Program.MainMenu();
                    return;
                }

            }
            else if (Program.player.GetStat("PlayerWorthyOfSword") < 0)
            {
                MainUI.WriteInMainArea("you aren't worthy");
                Thread.Sleep(3000);

                Program.MainMenu();
                return;
            }
            else
            {
                MainUI.WriteInMainArea("something broke:0");
                Thread.Sleep(5000);
                Program.MainMenu();
                return;
            }
        }
        else
        {
            MainUI.WriteInMainArea("you leave it be");
            Thread.Sleep(3000);

            Program.MainMenu();
            return;
        }
    }
    async Task<string?> ReadLineWithTimeout(int milliseconds)
    {
        Task<string?> readTask = Task.Run(() => Console.ReadLine());

        Task completedTask = await Task.WhenAny(readTask, Task.Delay(milliseconds));

        if (completedTask == readTask)
        {
            return readTask.Result;
        }

        return null;
    }

    #endregion

    #endregion



}


