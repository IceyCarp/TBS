using Game.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

public class CombatManager
{
    private Player player;
    private List<Enemy> enemies;
    private List<Combatant> combatants;
    private const double ActionThreshold = 100.0;
    private readonly Random rng = new Random();
    private readonly Dictionary<Combatant, int> stunnedTurns = new();
    private CombatUI ui;
    private bool canFlee;
    private Location previousLocation;
    private bool playerFled = false;
    private bool enemyAttackedThisCombat = false;
    private bool playerHitLow = false;

    public static bool playerInCombat = false;
    public static bool playerFledLastCombat = false;

    public CombatManager(Player p, List<Enemy> initialEnemies, bool canFlee = true, Location previousLocation = null)
    {
        player = p;
        enemies = initialEnemies;
        this.canFlee = canFlee;
        this.previousLocation = previousLocation;

        player.IsPlayer = true;
        foreach (var e in enemies) e.IsPlayer = false;

        combatants = new List<Combatant>();
        combatants.Add(player);
        
        // Add companions from CompanionSystem
        var companions = CompanionSystem.GetCompanions(player);
        foreach (var companion in companions)
        {
            if (companion.IsAlive())
            {
                companion.IsAlly = true;
                companion.IsPlayer = false;
                combatants.Add(companion);
            }
        }
        
        combatants.AddRange(enemies);

        foreach (var combatant in combatants)
        {
            combatant.ActionGauge = 0;
            stunnedTurns[combatant] = 0;
            if (combatant.maxHP <= 0) combatant.maxHP = combatant.HP;
            if (combatant.maxHP > 0 && combatant.HP > combatant.maxHP) combatant.HP = combatant.maxHP;
        }

        ui = new CombatUI();
        ui.InitializeConsole();
    }

    public void StartCombat()
    {
        playerFled = false;
        playerInCombat = true;
        player.SetStat("isInCombat", 1);
        Program.SavePlayer();

        ui.AddToLog("--- Combat Started! ---");
        ui.RenderCombatScreen(player, combatants);
        Thread.Sleep(1000);

        while (player.IsAlive() && enemies.Any(e => e.IsAlive()) && !playerFled)
        {
            var readyCombatants = GetReadyCombatants();

            if (readyCombatants.Count == 0)
            {
                AdvanceActionGauges();
                continue;
            }

            Combatant currentActor;
            var maxGauge = readyCombatants.Max(c => c.ActionGauge);
            var topCombatants = readyCombatants.Where(c => c.ActionGauge == maxGauge).ToList();

            if (topCombatants.Count == 1)
            {
                currentActor = topCombatants[0];
            }
            else
            {
                currentActor = topCombatants[rng.Next(topCombatants.Count)];
            }

            TakeTurn(currentActor);
        }

        if (playerFled)
        {
            if (player.activeEffects != null && player.activeEffects.Count > 0)
            {
                foreach (var ae in player.activeEffects)
                {
                    switch (ae.type)
                    {
                        case "dodge": player.dodge -= ae.value; break;
                        case "dodgeNegation": player.dodgeNegation -= ae.value; break;
                        case "critChance": player.critChance -= ae.value; break;
                        case "critDamage": player.critDamage -= ae.value; break;
                        case "armor": player.armor -= ae.value; break;
                        case "stun": player.stun -= ae.value; break;
                        case "stunNegation": player.stunNegation -= ae.value; break;
                        case "speed": player.speed -= ae.value; break;
                    }
                }
                player.activeEffects.Clear();
            }
            
            if (player.damageOverTimeEffects != null)
            {
                player.damageOverTimeEffects.Clear();
            }
            if (player.healOverTimeEffects != null)
            {
                player.healOverTimeEffects.Clear();
            }
            
            ui.AddToLog("--- You fled from combat! ---");
            ui.ClearMainArea();
            ui.RenderCombatScreen(player, combatants);
            ui.WriteInMainArea(1, "+----------------------------------------+");
            ui.WriteInMainArea(2, "�          FLED!                         �");
            ui.WriteInMainArea(3, "+----------------------------------------+");
            ui.WriteInMainArea(4, "You managed to escape from battle...");
            player.SetStat("isInCombat", 0);
            playerInCombat = false;
            Program.SavePlayer();
            if (previousLocation != null)
            {
                ui.WriteInMainArea(6, $"Returning to {previousLocation.name}...");
                player.currentLocation = previousLocation.name;
                Program.SavePlayer();
            }
            ui.WriteInMainArea(8, "Press Enter to continue...");
            Console.ReadLine();
        }
        else if (player.IsAlive())
        {
            // Track flawless victories (no enemy attacks)
            if (!enemyAttackedThisCombat)
            {
                player.IncrementStat("totalFlawlessVictories");
            }
            if (playerHitLow)
            {
                player.IncrementStat("totalGritVictories");
            }
            int totalMoney = enemies.Sum(e => e.money);
            int totalExp = enemies.Where(e => player.level - e.level < 5).Sum(e => e.exp);
            player.money += totalMoney;
            player.exp += totalExp;
            
            List<string> materialRewards = new List<string>();
            foreach (var enemy in enemies)
            {
                if (enemy.materialDrops != null && enemy.materialDrops.Count > 0)
                {
                    foreach (var drop in enemy.materialDrops)
                    {
                        float roll = (float)rng.NextDouble();
                        if (roll < drop.DropChance)
                        {
                            int qty = rng.Next(drop.MinQuantity, drop.MaxQuantity + 1);
                            if (qty > 0)
                            {
                                Inventory.AddItem(drop.Material, qty);
                                materialRewards.Add($"+{qty}x {drop.Material.name}");
                            }
                        }
                    }
                }
            }
            if (player.activeEffects != null && player.activeEffects.Count > 0)
            {
                foreach (var ae in player.activeEffects)
                {
                    switch (ae.type)
                    {
                        case "dodge": player.dodge -= ae.value; break;
                        case "dodgeNegation": player.dodgeNegation -= ae.value; break;
                        case "critChance": player.critChance -= ae.value; break;
                        case "critDamage": player.critDamage -= ae.value; break;
                        case "armor": player.armor -= ae.value; break;
                        case "stun": player.stun -= ae.value; break;
                        case "stunNegation": player.stunNegation -= ae.value; break;
                        case "speed": player.speed -= ae.value; break;
                    }
                }
                player.activeEffects.Clear();
            }
            
            if (player.damageOverTimeEffects != null)
            {
                player.damageOverTimeEffects.Clear();
            }
            if (player.healOverTimeEffects != null)
            {
                player.healOverTimeEffects.Clear();
            }
            ui.AddToLog("--- VICTORY! ---");
            string rewardText = $"Rewards: +{totalExp} EXP, +{totalMoney} Rai";
            if (materialRewards.Count > 0)
            {
                rewardText += ", " + string.Join(", ", materialRewards);
            }
            ui.AddToLog(rewardText);
            ui.ClearMainArea();
            ui.RenderCombatScreen(player, combatants);
            ui.WriteInMainArea(1, "+----------------------------------------+");
            ui.WriteInMainArea(2, "?          VICTORY!                      ?");
            ui.WriteInMainArea(3, "+----------------------------------------+");
            ui.WriteInMainArea(4, $"Rewards:");
            ui.WriteInMainArea(5, $"  +{totalExp} EXP");
            ui.WriteInMainArea(6, $"  +{totalMoney} Rai");
            ui.WriteInMainArea(8, "Press Enter to continue...");
            
            
            Program.SavePlayer();
            Console.ReadLine();
        }
        else
        {
            if (player.activeEffects != null && player.activeEffects.Count > 0)
            {
                foreach (var ae in player.activeEffects)
                {
                    switch (ae.type)
                    {
                        case "dodge": player.dodge -= ae.value; break;
                        case "dodgeNegation": player.dodgeNegation -= ae.value; break;
                        case "critChance": player.critChance -= ae.value; break;
                        case "critDamage": player.critDamage -= ae.value; break;
                        case "armor": player.armor -= ae.value; break;
                        case "stun": player.stun -= ae.value; break;
                        case "stunNegation": player.stunNegation -= ae.value; break;
                        case "speed": player.speed -= ae.value; break;
                    }
                }
                player.activeEffects.Clear();
            }
            
            if (player.damageOverTimeEffects != null)
            {
                player.damageOverTimeEffects.Clear();
            }
            if (player.healOverTimeEffects != null)
            {
                player.healOverTimeEffects.Clear();
            }
            
            ui.ClearMainArea();
            ui.RenderCombatScreen(player, combatants);
            ui.WriteInMainArea(1, "+----------------------------------------+");
            ui.WriteInMainArea(2, "?          DEFEAT...                     ?");
            ui.WriteInMainArea(3, "+----------------------------------------+");
            ui.WriteInMainArea(4, "You have been defeated in battle...");
            ui.WriteInMainArea(6, "Press Enter to continue...");

            
            Console.ReadLine();
        }

        Console.Clear();
        Console.CursorVisible = true;
        

        var companionsAfterCombat = combatants
            .Where(c => c is Enemy && ((Enemy)c).IsAlly)
            .Cast<Enemy>()
            .ToList();
        
        CompanionSystem.SaveCompanions(player, companionsAfterCombat);
        CompanionSystem.RemoveDeadCompanions(player);
        
        player.SetStat("isInCombat", 0);
        playerInCombat = false;
        Program.SavePlayer();
        
        // Check if player died
        Program.CheckPlayerDeath();

        MainUI.RenderMainMenuScreen(player);
        MainUI.LoopRenderMain();
    }

    private void AdvanceActionGauges()
    {
        double timeToNextTurn = double.MaxValue;
        foreach (var combatant in combatants.Where(c => c.IsAlive()))
        {
            if (combatant.speed <= 0) continue; // Avoid division by zero

            double timeNeeded = (ActionThreshold - combatant.ActionGauge) / combatant.speed;
            if (timeNeeded < timeToNextTurn)
            {
                timeToNextTurn = timeNeeded;
            }
        }

        // No inf loop ting ting (Failsafe)
        if (double.IsInfinity(timeToNextTurn) || timeToNextTurn <= 0)
        {
            timeToNextTurn = 1;
        }

        foreach (var combatant in combatants.Where(c => c.IsAlive()))
        {
            combatant.ActionGauge += combatant.speed * timeToNextTurn;
        }
    }

    private List<Combatant> GetReadyCombatants()
    {
        return combatants.Where(c => c.IsAlive() && c.ActionGauge >= ActionThreshold).ToList();
    }

    private int CalculateFleeChance()
    {
        // Get alive allies (player + companions)
        var aliveAllies = combatants.Where(c => c.IsAlive() && (c.IsPlayer || c.IsAlly)).ToList();
        var aliveEnemies = enemies.Where(e => e.IsAlive()).ToList();

        // Calculate total speed for allies and enemies
        int allySpeed = aliveAllies.Sum(a => a.speed);
        int enemySpeed = aliveEnemies.Sum(e => e.speed);

        // Base 50% + ally speed - enemy speed
        int fleeChance = 50 + allySpeed - enemySpeed;

        // Clamp between 5% and 95% to always give some chance
        return Math.Clamp(fleeChance, 5, 95);
    }

                
            private bool RollChance(int chance)
            {
            chance = Math.Clamp(chance, 0, 100);
            return rng.Next(0, 100) < chance;
            }
            
            private int ComputeDamage(Combatant attacker, Combatant defender, int baseValue, out bool dodged, out bool crit, out bool stunnedApplied, out int rawBeforeArmor, out int armorApplied, out double mult)
            {
                dodged = false; crit = false; stunnedApplied = false;
                rawBeforeArmor = 0; armorApplied = 0; mult = 1.0;
            
                int dodgeChance = Math.Clamp(defender.dodge - attacker.dodgeNegation, 0, 100);
                if (RollChance(dodgeChance))
                {
                    dodged = true;
                    return 0;
                }
            
                int critChance = Math.Clamp(attacker.critChance, 0, 100);
                if (RollChance(critChance))
                {
                    crit = true;
                    mult += attacker.critDamage / 100.0;
                }
            
                rawBeforeArmor = (int)Math.Round(baseValue * mult);
                armorApplied = Math.Max(0, defender.armor);
                int dmg = Math.Max(0, rawBeforeArmor - armorApplied);
            
                int stunChance = Math.Clamp(attacker.stun - defender.stunNegation, 0, 100);
                if (stunChance > 0 && RollChance(stunChance))
                {
                    stunnedApplied = true;
                }
            
            return dmg;
            }
            
            private void ExecuteAttackSingle(Combatant attacker, Attack attack, Combatant defender)
            {
            string attackMsg = $"{attacker.name} uses {attack.name} on {defender.name}!";
            ui.AddToLog(attackMsg);
            ui.RenderCombatScreen(player, combatants);
            Thread.Sleep(400);
            
            foreach (var effect in attack.effects)
            {
            if (effect.targetType == "allEnemies" || effect.targetType == "allAllies") continue;

            if (effect.type == "damage")
            {
                Combatant damageTarget = (effect.targetType == "self") ? attacker : defender;
                int before = damageTarget.HP;
                bool dodged, crit, stunInflicted;
                int dmg = ComputeDamage(attacker, damageTarget, effect.value, out dodged, out crit, out stunInflicted, out int rawBeforeArmor, out int armorApplied, out double mult);
                if (dodged && effect.targetType != "self")
                {
                    ui.AddToLog($"{damageTarget.name} dodged the attack!");
                    ui.RenderCombatScreen(player, combatants);
                    Thread.Sleep(350);
                    continue;
                }
                damageTarget.HP -= dmg;
                int after = Math.Max(damageTarget.HP, 0);
                ui.AddToLog($"{damageTarget.name} takes {dmg} damage{(crit ? " (CRIT!)" : "")} ({before} -> {after})");
                ui.RenderCombatScreen(player, combatants);
                Thread.Sleep(350);

                if (effect.duration > 0 && damageTarget.damageOverTimeEffects != null)
                {
                    damageTarget.damageOverTimeEffects.Add(new DamageOverTimeEffect(effect.value, effect.duration, attack.name));
                    ui.AddToLog($"{damageTarget.name} is afflicted with {attack.name} for {effect.duration} turns!");
                    ui.RenderCombatScreen(player, combatants);
                    Thread.Sleep(300);
                }

                if (stunInflicted)
                {
                    stunnedTurns[damageTarget] = Math.Max(stunnedTurns.ContainsKey(damageTarget) ? stunnedTurns[damageTarget] : 0, 1);
                    ui.AddToLog($"{damageTarget.name} is stunned!");
                    ui.RenderCombatScreen(player, combatants);
                    Thread.Sleep(350);
                }
            }
            else if (effect.type == "heal")
            {
            Combatant target;
            if (effect.targetType == "self" || effect.targetType == "ally")
            {
                target = attacker;
            }
            else
            {
                target = defender;
            }
            
            int before = target.HP;
            int max = target.maxHP > 0 ? target.maxHP : int.MaxValue;
            target.HP = Math.Min(before + effect.value, max);
            int healed = target.HP - before;
            ui.AddToLog($"{target.name} heals {healed} HP ({before} -> {target.HP})");
            ui.RenderCombatScreen(player, combatants);
            Thread.Sleep(350);

            // Check if this heal has a duration (HoT effect like regeneration)
            if (effect.duration > 0 && target.healOverTimeEffects != null)
            {
                target.healOverTimeEffects.Add(new HealOverTimeEffect(effect.value, effect.duration, attack.name));
                ui.AddToLog($"{target.name} gains {attack.name} regeneration for {effect.duration} turns!");
                ui.RenderCombatScreen(player, combatants);
                Thread.Sleep(300);
            }
            }
            else
            {
            Combatant target;
            if (effect.targetType == "self" || effect.targetType == "ally")
            {
                target = attacker;
            }
            else
            {
                target = defender;
            }
            
            effect.Apply(attacker, defender);

            if (effect.duration > 0 && target.activeEffects != null)
            {
                target.activeEffects.Add(new ActiveEffect(effect.type, effect.value, effect.duration));
            }

            string sign = effect.value >= 0 ? "+" : "";
            string dur = effect.duration > 0 ? $" for {effect.duration} turns" : "";
            ui.AddToLog($"{target.name}: {effect.type} {sign}{effect.value}{dur}");
            ui.RenderCombatScreen(player, combatants);
            Thread.Sleep(300);
            }
            }
            }
            
            private void ExecuteAttackAoE(Combatant attacker, Attack attack, List<Combatant> defenders)
            {
            bool isAllyAoE = attack.effects.Any(e => e.targetType == "allAllies");
            string targetDescription = isAllyAoE ? "ALL allies" : "ALL enemies";
            ui.AddToLog($"{attacker.name} uses {attack.name} on {targetDescription}!");
            ui.RenderCombatScreen(player, combatants);
            Thread.Sleep(400);
            
            foreach (var effect in attack.effects)
            {
            if (effect.targetType != "allEnemies" && effect.targetType != "allAllies") continue;
            
            if (effect.type == "damage")
            {
            foreach (var d in defenders)
            {
            int before = d.HP;
            bool dodged, crit, stunInflicted;
            int dmg = ComputeDamage(attacker, d, effect.value, out dodged, out crit, out stunInflicted, out int rawBeforeArmor, out int armorApplied, out double mult);
            if (dodged)
            {
            ui.AddToLog($"{d.name} dodged the attack!");
            ui.RenderCombatScreen(player, combatants);
            Thread.Sleep(250);
            continue;
            }
            d.HP -= dmg;
            int after = Math.Max(d.HP, 0);
            ui.AddToLog($"{d.name} takes {dmg} damage{(crit ? " (CRIT!)" : "")} ({before} -> {after})");
            ui.RenderCombatScreen(player, combatants);
            Thread.Sleep(250);
            if (stunInflicted)
            {
            stunnedTurns[d] = Math.Max(stunnedTurns.ContainsKey(d) ? stunnedTurns[d] : 0, 1);
            ui.AddToLog($"{d.name} is stunned!");
            ui.RenderCombatScreen(player, combatants);
            Thread.Sleep(250);
            }
            }
            }
            else
            {
            foreach (var d in defenders)
            {
            if (effect.type == "heal")
            {
                int before = d.HP;
                int max = d.maxHP > 0 ? d.maxHP : int.MaxValue;
                d.HP = Math.Min(before + effect.value, max);
                int healed = d.HP - before;
                ui.AddToLog($"{d.name} heals {healed} HP ({before} -> {d.HP})");
                ui.RenderCombatScreen(player, combatants);
                Thread.Sleep(250);
            }
            else
            {
                effect.Apply(attacker, d);

                if (effect.duration > 0 && d.activeEffects != null)
                {
                    d.activeEffects.Add(new ActiveEffect(effect.type, effect.value, effect.duration));
                }

                string sign = effect.value >= 0 ? "+" : "";
                string dur = effect.duration > 0 ? $" for {effect.duration} turns" : "";
                ui.AddToLog($"{d.name}: {effect.type} {sign}{effect.value}{dur}");
                ui.RenderCombatScreen(player, combatants);
                Thread.Sleep(250);
            }
            }
            }
            }
            }
            
            
        private void UpdateTimedEffects(Combatant c)
        {
            // Update stat-based timed effects (buffs/debuffs)
            if (c.activeEffects != null && c.activeEffects.Count > 0)
            {
                var expired = new List<ActiveEffect>();
                foreach (var ae in c.activeEffects)
                {
                    ae.remainingTurns--;
                    if (ae.remainingTurns <= 0)
                    {
                        switch (ae.type)
                        {
                            case "dodge": c.dodge -= ae.value; break;
                            case "dodgeNegation": c.dodgeNegation -= ae.value; break;
                            case "critChance": c.critChance -= ae.value; break;
                            case "critDamage": c.critDamage -= ae.value; break;
                            case "armor": c.armor -= ae.value; break;
                            case "stun": c.stun -= ae.value; break;
                            case "stunNegation": c.stunNegation -= ae.value; break;
                            case "speed": c.speed -= ae.value; break;
                        }
                        expired.Add(ae);
                    }
                }

                foreach (var ae in expired)
                {
                    c.activeEffects.Remove(ae);
                }
            }

            // Apply and update damage over time effects (bleed, poison, burn, etc.)
            if (c.damageOverTimeEffects != null && c.damageOverTimeEffects.Count > 0)
            {
                for (int i = c.damageOverTimeEffects.Count - 1; i >= 0; i--)
                {
                    var dot = c.damageOverTimeEffects[i];
                    
                    // Apply damage with armor reduction
                    int rawDamage = dot.damagePerTurn;
                    int armorReduction = Math.Max(0, c.armor);
                    int actualDamage = Math.Max(0, rawDamage - armorReduction);
                    
                    c.HP -= actualDamage;
                    
                    if (armorReduction > 0 && rawDamage > armorReduction)
                    {
                        ui.AddToLog($"{c.name} takes {actualDamage} damage from {dot.sourceName} ({rawDamage} - {armorReduction} armor)!");
                    }
                    else if (armorReduction >= rawDamage)
                    {
                        ui.AddToLog($"{c.name} takes {actualDamage} damage from {dot.sourceName} (blocked by armor)!");
                    }
                    else
                    {
                        ui.AddToLog($"{c.name} takes {actualDamage} damage from {dot.sourceName}!");
                    }
                    
                    ui.RenderCombatScreen(player, combatants);
                    Thread.Sleep(300);

                    dot.remainingTurns--;
                    if (dot.remainingTurns <= 0)
                    {
                        c.damageOverTimeEffects.RemoveAt(i);
                        ui.AddToLog($"{c.name}'s {dot.sourceName} effect has worn off.");
                        ui.RenderCombatScreen(player, combatants);
                        Thread.Sleep(200);
                    }
                }
            }

            // Apply and update heal over time effects (regeneration, etc.)
            if (c.healOverTimeEffects != null && c.healOverTimeEffects.Count > 0)
            {
                for (int i = c.healOverTimeEffects.Count - 1; i >= 0; i--)
                {
                    var hot = c.healOverTimeEffects[i];
                    
                    // Apply healing
                    int oldHP = c.HP;
                    c.HP += hot.healPerTurn;
                    if (c.HP > c.maxHP) c.HP = c.maxHP;
                    int actualHeal = c.HP - oldHP;

                    if (actualHeal > 0)
                    {
                        ui.AddToLog($"{c.name} heals {actualHeal} HP from {hot.sourceName}!");
                        ui.RenderCombatScreen(player, combatants);
                        Thread.Sleep(300);
                    }

                    hot.remainingTurns--;
                    if (hot.remainingTurns <= 0)
                    {
                        c.healOverTimeEffects.RemoveAt(i);
                        ui.AddToLog($"{c.name}'s {hot.sourceName} effect has worn off.");
                        ui.RenderCombatScreen(player, combatants);
                        Thread.Sleep(200);
                    }
                }
            }
        }

        private void TakeTurn(Combatant actor)
        {
            if (!actor.IsAlive()) return;

            UpdateTimedEffects(actor);

            // Check if actor died from DOT effects
            if (!actor.IsAlive())
            {
                ui.AddToLog($"{actor.name} has been defeated by damage over time!");
                ui.RenderCombatScreen(player, combatants);
                Thread.Sleep(800);
                actor.ActionGauge -= ActionThreshold;
                if (actor.ActionGauge < 0) actor.ActionGauge = 0;
                return;
            }

            ui.RenderCombatScreen(player, combatants);
            ui.AddToLog("");
            // Stun check
            if (stunnedTurns.TryGetValue(actor, out int stunLeft) && stunLeft > 0)
            {
                ui.AddToLog($"{actor.name} is stunned and cannot act!");
                ui.RenderCombatScreen(player, combatants);
                stunnedTurns[actor] = stunLeft - 1;

                actor.ActionGauge -= ActionThreshold;
                if (actor.ActionGauge < 0) actor.ActionGauge = 0;

                Thread.Sleep(800);
                return;
            }

            if (actor == player)
            {
                player.UpdateActivity();
                
                Thread.Sleep(300);
                ui.RenderCombatScreen(player, combatants);
                ui.WriteInMainArea(12, "");
                ui.WriteInMainArea(13, "Press Enter to continue (← → to view party)...");
                ui.SetCursorInMainArea(22);
                
                while (true)
                {
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Enter)
                    {
                        break;
                    }
                    else if (key.Key == ConsoleKey.RightArrow)
                    {
                        ui.CyclePartyMember(true);
                        ui.RenderCombatScreen(player, combatants);
                        ui.WriteInMainArea(12, "");
                        ui.WriteInMainArea(13, "Press Enter to continue (← → to view party)...");
                        ui.SetCursorInMainArea(22);
                    }
                    else if (key.Key == ConsoleKey.LeftArrow)
                    {
                        ui.CyclePartyMember(false);
                        ui.RenderCombatScreen(player, combatants);
                        ui.WriteInMainArea(12, "");
                        ui.WriteInMainArea(13, "Press Enter to continue (← → to view party)...");
                        ui.SetCursorInMainArea(22);
                    }
                }
                
                ui.ResetPartyMemberIndex();
                player.UpdateActivity();

                var moves = player.equippedAttacks.Where(a => a != null).ToList();
                if (player.equippedWeapon != null && player.equippedWeapon.weaponAttack != null) moves.Add(player.equippedWeapon.weaponAttack);
                var consumables = player.ownedItems.Where(i => i.type == ItemType.Consumable && i.amount > 0).ToList();
                
                if (moves.Count == 0 && consumables.Count == 0 && !canFlee)
                {
                    ui.AddToLog($"{player.name} has no moves or items and skips turn.");
                    ui.RenderCombatScreen(player, combatants);
                    Thread.Sleep(800);
                    return;
                }

                // Display move and item selection in main area
                ui.ClearMainArea();
                ui.WriteInMainArea(0, "--- Your Turn ---");
                ui.WriteInMainArea(1, "");
                
                int lineNum = 2;
                
                // Show attacks
                if (moves.Count > 0)
                {
                    ui.WriteInMainArea(lineNum++, "ATTACKS:");
                    for (int i = 0; i < moves.Count; i++)
                    {
                        ui.WriteInMainArea(lineNum++, $"{i + 1}. {moves[i].name} - {moves[i].GetDescription()}");
                    }
                    lineNum++;
                }
                
                // Show consumables
                int itemStartIndex = moves.Count + 1;
                if (consumables.Count > 0)
                {
                    ui.WriteInMainArea(lineNum++, "ITEMS:");
                    for (int i = 0; i < consumables.Count; i++)
                    {
                        ui.WriteInMainArea(lineNum++, $"{itemStartIndex + i}. {consumables[i].name} x{consumables[i].amount} - {consumables[i].description}");
                    }
                    lineNum++;
                }
                
                // Show flee option
                int fleeOption = 0;
                if (canFlee)
                {
                    int displayFleeChance = CalculateFleeChance();
                    ui.WriteInMainArea(lineNum++, $"0. FLEE ({displayFleeChance}% chance)");
                    lineNum++;
                }
                
                ui.SetCursorInMainArea(lineNum);
                Console.Write("Choose action (← → to view party): ");

                int choice = -1;
                int totalOptions = moves.Count + consumables.Count;
                while (true)
                {
                    var key = Console.ReadKey(true);
                    
                    if (key.Key == ConsoleKey.RightArrow)
                    {
                        ui.CyclePartyMember(true);
                        ui.RenderCombatScreen(player, combatants);
                        
                        ui.ClearMainArea();
                        ui.WriteInMainArea(0, "--- Your Turn ---");
                        ui.WriteInMainArea(1, "");
                        
                        lineNum = 2;
                        if (moves.Count > 0)
                        {
                            ui.WriteInMainArea(lineNum++, "ATTACKS:");
                            for (int i = 0; i < moves.Count; i++)
                            {
                                ui.WriteInMainArea(lineNum++, $"{i + 1}. {moves[i].name} - {moves[i].GetDescription()}");
                            }
                            lineNum++;
                        }
                        
                        itemStartIndex = moves.Count + 1;
                        if (consumables.Count > 0)
                        {
                            ui.WriteInMainArea(lineNum++, "ITEMS:");
                            for (int i = 0; i < consumables.Count; i++)
                            {
                                ui.WriteInMainArea(lineNum++, $"{itemStartIndex + i}. {consumables[i].name} x{consumables[i].amount} - {consumables[i].description}");
                            }
                            lineNum++;
                        }
                        
                        if (canFlee)
                        {
                            int displayFleeChance = CalculateFleeChance();
                            ui.WriteInMainArea(lineNum++, $"0. FLEE ({displayFleeChance}% chance)");
                            lineNum++;
                        }
                        
                        ui.SetCursorInMainArea(lineNum);
                        Console.Write("Choose action (← → to view party): ");
                        continue;
                    }
                    else if (key.Key == ConsoleKey.LeftArrow)
                    {
                        ui.CyclePartyMember(false);
                        ui.RenderCombatScreen(player, combatants);
                        
                        ui.ClearMainArea();
                        ui.WriteInMainArea(0, "--- Your Turn ---");
                        ui.WriteInMainArea(1, "");
                        
                        lineNum = 2;
                        if (moves.Count > 0)
                        {
                            ui.WriteInMainArea(lineNum++, "ATTACKS:");
                            for (int i = 0; i < moves.Count; i++)
                            {
                                ui.WriteInMainArea(lineNum++, $"{i + 1}. {moves[i].name} - {moves[i].GetDescription()}");
                            }
                            lineNum++;
                        }
                        
                        itemStartIndex = moves.Count + 1;
                        if (consumables.Count > 0)
                        {
                            ui.WriteInMainArea(lineNum++, "ITEMS:");
                            for (int i = 0; i < consumables.Count; i++)
                            {
                                ui.WriteInMainArea(lineNum++, $"{itemStartIndex + i}. {consumables[i].name} x{consumables[i].amount} - {consumables[i].description}");
                            }
                            lineNum++;
                        }
                        
                        if (canFlee)
                        {
                            int displayFleeChance = CalculateFleeChance();
                            ui.WriteInMainArea(lineNum++, $"0. FLEE ({displayFleeChance}% chance)");
                            lineNum++;
                        }
                        
                        ui.SetCursorInMainArea(lineNum);
                        Console.Write("Choose action (← → to view party): ");
                        continue;
                    }
                    
                    if (int.TryParse(key.KeyChar.ToString(), out choice) && choice >= 0 && choice <= totalOptions)
                    {
                        player.UpdateActivity();
                        ui.ResetPartyMemberIndex();
                        break;
                    }
                    
                    ui.SetCursorInMainArea(lineNum + 1);
                    Console.Write("Invalid choice. Try again: ");
                }

                
                // Check if player chose to flee
                if (canFlee && choice == fleeOption)
                {
                    // Calculate flee chance based on speed
                    int fleeChance = CalculateFleeChance();
                    if (rng.Next(0, 100) < fleeChance)
                    {
                        ui.AddToLog($"{player.name} attempts to flee...");
                        ui.RenderCombatScreen(player, combatants);
                        Thread.Sleep(500);
                        ui.AddToLog("Flee successful!");
                        ui.RenderCombatScreen(player, combatants);
                        Thread.Sleep(500);
                        playerFled = true;                        playerFledLastCombat = true;
                        return;
                    }
                    else
                    {
                        ui.AddToLog($"{player.name} attempts to flee...");
                        ui.RenderCombatScreen(player, combatants);
                        Thread.Sleep(500);
                        ui.AddToLog("Failed to escape! The enemies block your path!");
                        ui.RenderCombatScreen(player, combatants);
                        Thread.Sleep(800);
                        // Turn is wasted, continue to gauge reduction
                        actor.ActionGauge -= ActionThreshold;
                        if (actor.ActionGauge < 0) actor.ActionGauge = 0;
                        return;
                    }
                }
                
                // Check if they chose an attack or item
                if (choice <= moves.Count)
                {
                    // They chose an attack
                    var chosen = moves[choice - 1];

                    if (chosen.requiredClass != null && player.playerClass.name != chosen.requiredClass.name)
                    {
                        ui.AddToLog($"{player.name} cannot use {chosen.name} - requires {chosen.requiredClass.name} class!");
                        ui.RenderCombatScreen(player, combatants);
                        Thread.Sleep(1000);
                        ui.AddToLog("Choose a different action...");
                        ui.RenderCombatScreen(player, combatants);
                        Thread.Sleep(500);
                        TakeTurn(actor);
                        return;
                    }

                    bool isAoEEnemies = chosen.effects.Any(e => e.targetType == "allEnemies");
                    bool isAoEAllies = chosen.effects.Any(e => e.targetType == "allAllies");
                    bool targetsSelfOrAlly = chosen.effects.All(e => e.targetType == "self" || e.targetType == "ally");
                    
                    if (isAoEEnemies)
                    {
                        var allEnemies = enemies.Cast<Combatant>().ToList();
                        ExecuteAttackAoE(player, chosen, allEnemies);
                    }
                    else if (isAoEAllies)
                    {
                        var allAllies = combatants.Where(c => c.IsAlive() && (c.IsPlayer || c.IsAlly)).ToList();
                        ExecuteAttackAoE(player, chosen, allAllies);
                    }
                    else if (targetsSelfOrAlly)
                    {
                        ExecuteAttackSingle(player, chosen, player);
                    }
                    else
                    {
                        // Target selection
                        ui.ClearMainArea();
                        ui.WriteInMainArea(0, "Choose a target:");
                        ui.WriteInMainArea(1, "");
                        for (int i = 0; i < enemies.Count; i++)
                        {
                            var name = enemies[i].name;
                            var sameNameCount = enemies.Count(e => e.name == name);
                            var indexAmongSame = sameNameCount > 1 ? $" #{enemies.Take(i + 1).Count(e => e.name == name)}" : "";
                            var tag = enemies[i].IsAlive() ? "" : " (dead)";
                            ui.WriteInMainArea(2 + i, $"{i + 1}. {name}{indexAmongSame}{tag} [{enemies[i].HP}/{enemies[i].maxHP}]");
                        }
                        ui.WriteInMainArea(2 + enemies.Count + 1, "");
                        ui.WriteInMainArea(2 + enemies.Count + 2, "0. Back (choose different action)");
                        ui.WriteInMainArea(2 + enemies.Count + 3, "");
                        ui.SetCursorInMainArea(2 + enemies.Count + 4);
                        Console.Write("Target #: ");

                        while (true)
                        {
                            if (!int.TryParse(Console.ReadKey(true).KeyChar.ToString(), out int t) || t < 0 || t > enemies.Count)
                            {
                                ui.SetCursorInMainArea(2 + enemies.Count + 5);
                                Console.Write("Invalid. Try again: ");
                                continue;
                            }
                            player.UpdateActivity();
                            if (t == 0)
                            {
                                TakeTurn(actor);
                                return;
                            }
                            var target = enemies[t - 1];
                            ExecuteAttackSingle(player, chosen, target);
                            break;
                        }
                    }
                }
                else
                {
                    // They chose an item
                    var chosenItem = consumables[choice - moves.Count - 1];
                    UseConsumableInCombat(player, chosenItem);
                }
            }
            else
            {
                var enemy = (Enemy)actor;
                ui.ClearMainArea();
                ui.WriteInMainArea(0, $"--- {enemy.name}'s Turn ---");
                ui.RenderCombatScreen(player, combatants);
                Thread.Sleep(500);

                if (enemy.attacks == null || enemy.attacks.Count == 0)
                {
                    ui.AddToLog($"{enemy.name} hesitates and does nothing.");
                    ui.RenderCombatScreen(player, combatants);
                    Thread.Sleep(800);
                    return;
                }
                var chosen = enemy.SelectWeightedAttack();
                
                bool isCompanion = enemy.IsAlly;
                
                if (!isCompanion)
                {
                    enemyAttackedThisCombat = true;
                    if (player.maxHP > 0 && player.HP <= player.maxHP * 0.3)
                        playerHitLow = true;
                }


                
                bool isAoEEnemies = chosen.effects.Any(e => e.targetType == "allEnemies");
                bool isAoEAllies = chosen.effects.Any(e => e.targetType == "allAllies");
                
                if (isAoEEnemies)
                {
                    var targets = isCompanion 
                        ? combatants.Where(c => c.IsAlive() && !c.IsPlayer && !c.IsAlly).ToList()
                        : combatants.Where(c => c.IsAlive() && (c.IsPlayer || c.IsAlly)).ToList();
                    ExecuteAttackAoE(enemy, chosen, targets);
                }
                else if (isAoEAllies)
                {
                    var targets = isCompanion
                        ? combatants.Where(c => c.IsAlive() && (c.IsPlayer || c.IsAlly)).ToList()
                        : combatants.Where(c => c.IsAlive() && !c.IsPlayer && !c.IsAlly).ToList();
                    ExecuteAttackAoE(enemy, chosen, targets);
                }
                else
                {
                    if (isCompanion)
                    {
                        var aliveEnemies = enemies.Where(e => e.IsAlive()).ToList();
                        if (aliveEnemies.Count > 0)
                        {
                            var target = aliveEnemies[rng.Next(aliveEnemies.Count)];
                            ExecuteAttackSingle(enemy, chosen, target);
                        }
                    }
                    else
                    {
                        var aliveAllies = combatants.Where(c => c.IsAlive() && (c.IsPlayer || c.IsAlly)).ToList();
                        if (aliveAllies.Count > 0)
                        {
                            var target = aliveAllies[rng.Next(aliveAllies.Count)];
                            ExecuteAttackSingle(enemy, chosen, target);
                        }
                    }
                }
            }
            actor.ActionGauge -= ActionThreshold;
            if (actor.ActionGauge < 0) actor.ActionGauge = 0;
            
        }

        private void UseConsumableInCombat(Player player, Item item)
        {
            ui.AddToLog($"{player.name} uses {item.name}!");
            ui.RenderCombatScreen(player, combatants);
            Thread.Sleep(400);

            // Apply item effects
            foreach (var stat in item.effects)
            {
                switch (stat.type)
                {
                    case "heal":
                        int before = player.HP;
                        player.HP = Math.Min(player.maxHP, player.HP + stat.value);
                        int healed = player.HP - before;
                        ui.AddToLog($"{player.name} heals {healed} HP ({before} -> {player.HP})");
                        break;
                    case "speed":
                        player.speed += stat.value;
                        ui.AddToLog($"{player.name} speed {(stat.value >= 0 ? "+" : "")}{stat.value}");
                        break;
                    case "armor":
                        player.armor += stat.value;
                        ui.AddToLog($"{player.name} armor {(stat.value >= 0 ? "+" : "")}{stat.value}");
                        break;
                    case "dodge":
                        player.dodge += stat.value;
                        ui.AddToLog($"{player.name} dodge {(stat.value >= 0 ? "+" : "")}{stat.value}");
                        break;
                    case "critChance":
                        player.critChance += stat.value;
                        ui.AddToLog($"{player.name} crit chance {(stat.value >= 0 ? "+" : "")}{stat.value}");
                        break;
                    case "critDamage":
                        player.critDamage += stat.value;
                        ui.AddToLog($"{player.name} crit damage {(stat.value >= 0 ? "+" : "")}{stat.value}");
                        break;
                }
                ui.RenderCombatScreen(player, combatants);
                Thread.Sleep(300);
            }

            // Consume the item
            item.amount--;
            if (item.amount <= 0)
            {
                player.ownedItems.Remove(item);
            }
            
            Program.SavePlayer();
        }
    }

