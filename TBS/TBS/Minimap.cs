using Game.Class;
using System;

public static class Minimap
{
    // Kingdom color mapping
    private static Dictionary<string, ConsoleColor> kingdomColors = new Dictionary<string, ConsoleColor>
    {
        { "Coastal Alliance", ConsoleColor.Cyan },
        { "Greenwood Territories", ConsoleColor.Green },
        { "Fallen Kingdom", ConsoleColor.DarkRed },
        { "Frostborn Dominion", ConsoleColor.Blue },
        { "Rootbound Empire", ConsoleColor.Magenta },
        { "Aria", ConsoleColor.Red },
        { "Deep Dark", ConsoleColor.DarkMagenta }
    };

    public static void DisplayMinimap(int startX, int startY, int maxContentWidth)
    {
        List<int?> travelLocations = new List<int?> { null, null, null, null, null, null, null, null, null, null };

        for (int i = 0; i < LocationLibrary.locations.Count; i++)
        {
            if (LocationLibrary.Get(Program.player.currentLocation).location + new System.Numerics.Vector2(0, 1) == LocationLibrary.locations[i].location)
                travelLocations[2] = i;
            if (LocationLibrary.Get(Program.player.currentLocation).location + new System.Numerics.Vector2(0, -1) == LocationLibrary.locations[i].location)
                travelLocations[8] = i;
            if (LocationLibrary.Get(Program.player.currentLocation).location + new System.Numerics.Vector2(1, 0) == LocationLibrary.locations[i].location)
                travelLocations[6] = i;
            if (LocationLibrary.Get(Program.player.currentLocation).location + new System.Numerics.Vector2(-1, 0) == LocationLibrary.locations[i].location)
                travelLocations[4] = i;

            if (LocationLibrary.Get(Program.player.currentLocation).location + new System.Numerics.Vector2(1, 1) == LocationLibrary.locations[i].location)
                travelLocations[3] = i;
            if (LocationLibrary.Get(Program.player.currentLocation).location + new System.Numerics.Vector2(-1, -1) == LocationLibrary.locations[i].location)
                travelLocations[7] = i;
            if (LocationLibrary.Get(Program.player.currentLocation).location + new System.Numerics.Vector2(1, -1) == LocationLibrary.locations[i].location)
                travelLocations[9] = i;
            if (LocationLibrary.Get(Program.player.currentLocation).location + new System.Numerics.Vector2(-1, 1) == LocationLibrary.locations[i].location)
                travelLocations[1] = i;
        }


        int cellWidth = (maxContentWidth - 3) / 3; // -2 for the two separators "|"

        string Cell(string s, int maxLength)
        {
            if (s.Length > maxLength)
                return s.Substring(0, maxLength - 2) + "..";
            return s.PadRight(maxLength); // Use PadRight to ensure fixed width
        }

        int currentY = startY;

        // --- LINE 1 ---
        Console.SetCursorPosition(startX, currentY);
        WriteColoredCell(travelLocations, 1, cellWidth, startX, currentY);
        Console.Write(" | ");
        WriteColoredCell(travelLocations, 2, cellWidth, startX + cellWidth + 3, currentY);
        Console.Write(" | ");
        WriteColoredCell(travelLocations, 3, cellWidth, startX + (cellWidth + 3) * 2, currentY);
        currentY++;

        // --- SEPARATOR ---
        Console.SetCursorPosition(startX, currentY);
        Console.Write(new string('-', maxContentWidth + 2));
        currentY++;

        // --- LINE 2 (Current Location) ---
        Console.SetCursorPosition(startX, currentY);
        WriteColoredCell(travelLocations, 4, cellWidth, startX, currentY);
        Console.Write(" | ");
        Console.SetCursorPosition(startX + cellWidth + 3, currentY);
        WriteCurrentLocationColored(cellWidth);
        Console.Write(" | ");
        WriteColoredCell(travelLocations, 6, cellWidth, startX + (cellWidth + 3) * 2, currentY);
        currentY++;

        // --- SEPARATOR ---
        Console.SetCursorPosition(startX, currentY);
        Console.Write(new string('-', maxContentWidth + 2));
        currentY++;

        // --- LINE 3 ---
        Console.SetCursorPosition(startX, currentY);
        WriteColoredCell(travelLocations, 7, cellWidth, startX, currentY);
        Console.Write(" | ");
        WriteColoredCell(travelLocations, 8, cellWidth, startX + cellWidth + 3, currentY);
        Console.Write(" | ");
        WriteColoredCell(travelLocations, 9, cellWidth, startX + (cellWidth + 3) * 2, currentY);
        currentY++;

        void WriteColoredCell(List<int?> travelLocations, int travelIndex, int cellWidth, int x, int y)
        {
            Console.SetCursorPosition(x, y);

            string locationName = SafeLocationName(travelLocations, travelIndex);

            // Don't color ??? or ... locations
            if (locationName == "???" || locationName == "...")
            {
                Console.Write(Cell(locationName, cellWidth));
                return;
            }

            string kingdom = GetLocationKingdom(travelLocations, travelIndex);

            // Get color for this kingdom
            ConsoleColor color = ConsoleColor.Gray; // Default color
            if (kingdom != null && kingdomColors.ContainsKey(kingdom))
            {
                color = kingdomColors[kingdom];
            }

            // Write with color
            Console.ForegroundColor = color;
            Console.Write(Cell(locationName, cellWidth));
            Console.ResetColor();
        }

        void WriteCurrentLocationColored(int cellWidth)
        {
            var currentLocation = LocationLibrary.Get(Program.player.currentLocation);
            string kingdom = currentLocation?.kingdom;

            ConsoleColor color = ConsoleColor.Gray; // Default for no kingdom
            if (kingdom != null && kingdomColors.ContainsKey(kingdom))
            {
                color = kingdomColors[kingdom];
            }

            Console.ForegroundColor = color;
            Console.Write(Cell("current", cellWidth));
            Console.ResetColor();
        }

        static string GetLocationKingdom(List<int?> travelLocations, int travelIndex)
        {
            if (travelIndex < 0 || travelIndex >= travelLocations.Count)
                return null;

            int? locIndex = travelLocations[travelIndex];

            if (locIndex == null || locIndex < 0 || locIndex >= LocationLibrary.locations.Count)
                return null;

            var location = LocationLibrary.locations[locIndex.Value];
            return location?.kingdom;
        }

        static string SafeLocationName(List<int?> travelLocations, int travelIndex)
        {
            // Check if travelLocations has this
            if (travelIndex < 0 || travelIndex >= travelLocations.Count)
                return "...";

            int? locIndex = travelLocations[travelIndex];

            // Check if the location is valid
            if (locIndex == null || locIndex < 0 || locIndex >= LocationLibrary.locations.Count)
                return "...";

            var location = LocationLibrary.locations[locIndex.Value];

            // If the location exists but is not known
            if (!Program.player.knownLocationnames.Contains(location.name))
                return "???";

            // If location exists and is known, return its name
            return location?.name ?? "...";
        }



    }
    static int viewOffsetX = 0;  
    static int viewOffsetY = 0;
    static int maxViewOffsetXNeg = 6;
    static int maxViewOffsetYNeg = 5;
    static int maxViewOffsetXPos = 6;
    static int maxViewOffsetYPos = 5;
    public static void DisplayMainmap(int startX, int startY, int maxContentWidth)
    {

        int rows = 9;
        int cols = 5;

        int maxNegX = 0;
        int maxPosY = 0;
        int maxPosX = 0;
        int maxNegY = 0;

        var current = LocationLibrary.Get(Program.player.currentLocation).location;
        int centerCol = cols / 2; 
        int centerRow = rows / 2; 

        foreach (var loca in LocationLibrary.locations)
        {
            var loc = loca.location;
            
            int x = (int)(loc.X - current.X);
            int y = (int)(loc.Y - current.Y);

            if (x < 0) maxNegX = Math.Min(maxNegX, x);
            if (x > 0) maxPosX = Math.Max(maxPosX, x);
            if (y < 0) maxNegY = Math.Min(maxNegY, y);
            if (y > 0) maxPosY = Math.Max(maxPosY, y);
        }

        maxViewOffsetXNeg = Math.Abs(maxNegX) - centerCol;
        maxViewOffsetXPos = maxPosX - centerCol;
        maxViewOffsetYNeg = Math.Abs(maxNegY) - centerRow; 
        maxViewOffsetYPos = maxPosY - centerRow;

        maxViewOffsetXNeg = Math.Max(0, maxViewOffsetXNeg);
        maxViewOffsetXPos = Math.Max(0, maxViewOffsetXPos);
        maxViewOffsetYNeg = Math.Max(0, maxViewOffsetYNeg);
        maxViewOffsetYPos = Math.Max(0, maxViewOffsetYPos);

        viewOffsetX = Math.Clamp(viewOffsetX, -maxViewOffsetXNeg, maxViewOffsetXPos);
        viewOffsetY = Math.Clamp(viewOffsetY, -maxViewOffsetYNeg, maxViewOffsetYPos);
        while (true)
        {
            MainUI.ClearMainArea();
            MainUI.WriteInMainArea("Press enter to continue...");
            MainUI.WriteInMainArea("W A S D or the arrow keys, to move");


            int totalCells = rows * cols;
            List<int?> travelLocations = Enumerable.Repeat<int?>(null, totalCells).ToList();

            for (int i = 0; i < LocationLibrary.locations.Count; i++)
            {
                var loc = LocationLibrary.locations[i].location;

                int dx = (int)(loc.X - current.X) - viewOffsetX;
                int dy = (int)(loc.Y - current.Y) - viewOffsetY;

                // dx range is left/right, dy is up/down
                if (Math.Abs(dx) <= centerCol && Math.Abs(dy) <= centerRow)
                {
                    int row = centerRow - dy;
                    int col = centerCol + dx;

                    if (row >= 0 && row < rows && col >= 0 && col < cols)
                    {
                        int index = row * cols + col;
                        travelLocations[index] = i;
                    }
                }
            }

            int separatorWidth = 3;
            int cellWidth = Math.Max(1, (maxContentWidth - (cols - 1) * separatorWidth) / cols);

            int totalPrintedWidth = cols * cellWidth + (cols - 1) * separatorWidth;

            string Cell(string s, int maxLength)
            {
                if (s == null) s = "";
                if (s.Length > maxLength)
                    return s.Substring(0, Math.Max(0, maxLength - 2)) + "..";
                return s.PadRight(maxLength);
            }

            int currentY = startY;

            // DRAW rows × cols
            for (int row = 0; row < rows; row++)
            {
                Console.SetCursorPosition(startX, currentY);

                for (int col = 0; col < cols; col++)
                {
                    int index = row * cols + col;


                    WriteColoredCell(travelLocations, index, cellWidth);
                    

                    if (col < cols - 1)
                        Console.Write(" | ");
                }

                currentY++;

                // draw separator line between rows (not after last row)
                if (row < rows - 1)
                {
                    Console.SetCursorPosition(startX, currentY);
                    Console.Write(new string('-', totalPrintedWidth));
                    currentY++;
                }
            }


            void WriteColoredCell(List<int?> travelLocationsLocal, int travelIndex, int w)
            {
                Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop); // ensure pos is current

                string locationName = SafeLocationName(travelLocationsLocal, travelIndex);

                if (locationName == "???" || locationName == "...")
                {
                    Console.Write(Cell(locationName, w));
                    return;
                }

                string kingdom = GetLocationKingdom(travelLocationsLocal, travelIndex);

                ConsoleColor color = ConsoleColor.Gray;
                if (kingdom != null && kingdomColors.ContainsKey(kingdom))
                    color = kingdomColors[kingdom];

                Console.ForegroundColor = color;
                Console.Write(Cell(locationName, w));
                Console.ResetColor();
            }

            static string GetLocationKingdom(List<int?> travelLocations, int travelIndex)
            {
                if (travelIndex < 0 || travelIndex >= travelLocations.Count)
                    return null;

                int? locIndex = travelLocations[travelIndex];

                if (locIndex == null || locIndex < 0 || locIndex >= LocationLibrary.locations.Count)
                    return null;

                var location = LocationLibrary.locations[locIndex.Value];
                return location?.kingdom;
            }

            static string SafeLocationName(List<int?> travelLocations, int travelIndex)
            {
                // Check if travelLocations has this
                if (travelIndex < 0 || travelIndex >= travelLocations.Count)
                    return "...";

                int? locIndex = travelLocations[travelIndex];

                // Check if the location is valid
                if (locIndex == null || locIndex < 0 || locIndex >= LocationLibrary.locations.Count)
                    return "...";

                var location = LocationLibrary.locations[locIndex.Value];

                // If the location exists but is not known
                if (!Program.player.knownLocationnames.Contains(location.name))
                    return "???";

                // If location exists and is known, return its name
                return location?.name ?? "...";
            }

            var keyInfo = Console.ReadKey(true);

            int maxViewOffsetX = maxViewOffsetXPos;
            int minViewOffsetX = -maxViewOffsetXNeg;
            int maxViewOffsetY = maxViewOffsetYPos;
            int minViewOffsetY = -maxViewOffsetYNeg;

            switch (keyInfo.Key)
            {
                case ConsoleKey.LeftArrow:
                case ConsoleKey.A:
                    if (viewOffsetX > minViewOffsetX) viewOffsetX--;
                    break;

                case ConsoleKey.RightArrow:
                case ConsoleKey.D:
                    if (viewOffsetX < maxViewOffsetX) viewOffsetX++;
                    break;

                case ConsoleKey.UpArrow:
                case ConsoleKey.W:
                    if (viewOffsetY < maxViewOffsetY) viewOffsetY++;
                    break;

                case ConsoleKey.DownArrow:
                case ConsoleKey.S:
                    if (viewOffsetY > minViewOffsetY) viewOffsetY--;
                    break;

                case ConsoleKey.Enter:
                    Program.MainMenu();
                    return;

            }

            continue;
        }
    }

}

