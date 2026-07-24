using System;

// ============================================================
//   FINITE STATE MACHINE (FSM) PROJECT
//   DAA Project - C# Console Application
//
//   Four FSM Applications:
//     1. Traffic Light Simulation
//     2. String Pattern Validator  (pattern: a b+ c)
//     3. Vending Machine Simulation
//     4. Elevator Simulation
// ============================================================

namespace FSM_Project
{

    // ============================================================
    //   FSM 1: TRAFFIC LIGHT
    //   States: Red (0) -> Green (1) -> Yellow (2) -> Red ...
    // ============================================================

    class TrafficLightFSM
    {
        private int currentState; // 0=Red, 1=Green, 2=Yellow

        public TrafficLightFSM()
        {
            currentState = 0; // Always start at Red
        }

        public void NextState()
        {
            if (currentState == 0)
                currentState = 1;      // Red -> Green
            else if (currentState == 1)
                currentState = 2;      // Green -> Yellow
            else
                currentState = 0;      // Yellow -> Red
        }

        public void ShowState()
        {
            if (currentState == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("  *** STATE: RED ***");
                Console.WriteLine("      Action : STOP! Do not cross.");
            }
            else if (currentState == 1)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("  *** STATE: GREEN ***");
                Console.WriteLine("      Action : GO! You may proceed.");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("  *** STATE: YELLOW ***");
                Console.WriteLine("      Action : CAUTION! Slow down.");
            }
            Console.ResetColor();
        }
    }


    // ============================================================
    //   FSM 2: STRING PATTERN VALIDATOR
    //   Accepts strings matching:  a  b+  c
    //   e.g.  abc, abbc, abbbc  are VALID
    //         ac, bc, xyz       are INVALID
    //
    //   States:
    //     q0 = Start
    //     q1 = Got 'a'
    //     q2 = Got at least one 'b'
    //     q3 = Got 'c' after b  -> ACCEPT
    //     q4 = Dead/error       -> REJECT
    // ============================================================

    class StringValidatorFSM
    {
        private int currentState;

        public StringValidatorFSM()
        {
            currentState = 0;
        }

        public void Reset()
        {
            currentState = 0;
        }

        public void ProcessCharacter(char ch)
        {
            if (currentState == 4) return; // Already dead, do nothing

            if (currentState == 0)
            {
                if (ch == 'a') currentState = 1;
                else currentState = 4;
            }
            else if (currentState == 1)
            {
                if (ch == 'b') currentState = 2;
                else currentState = 4;
            }
            else if (currentState == 2)
            {
                if (ch == 'b') currentState = 2; // self-loop
                else if (ch == 'c') currentState = 3; // accept
                else currentState = 4;
            }
            else if (currentState == 3)
            {
                currentState = 4; // extra characters after 'c' = invalid
            }
        }

        public bool IsAccepted()
        {
            return currentState == 3;
        }

        public string GetStateName()
        {
            if (currentState == 0) return "q0 (Start)";
            if (currentState == 1) return "q1 (Got 'a')";
            if (currentState == 2) return "q2 (Got 'b')";
            if (currentState == 3) return "q3 --> ACCEPT";
            return "q4 --> REJECT (Dead)";
        }

        public void ValidateWithTrace(string input)
        {
            Reset();

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("  ----- TRACE TABLE -----");
            Console.WriteLine("  Step | Char | State After");
            Console.WriteLine("  -----|------|---------------------------");
            Console.ResetColor();

            Console.WriteLine("   0   |  -   | " + GetStateName());

            for (int i = 0; i < input.Length; i++)
            {
                char ch = input[i];
                ProcessCharacter(ch);
                Console.WriteLine("   " + (i + 1) + "   |  " + ch + "   | " + GetStateName());
            }

            Console.WriteLine("  ----- END OF TRACE -----");
            Console.WriteLine();

            if (IsAccepted())
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("  RESULT: '" + input + "' is VALID  (pattern: a b+ c matched!)");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("  RESULT: '" + input + "' is INVALID (pattern not matched)");
            }
            Console.ResetColor();
        }
    }


    // ============================================================
    //   FSM 3: VENDING MACHINE
    //
    //   How it works:
    //     - Machine sells one item (a cold drink) for Rs. 15
    //     - User inserts coins in steps of Rs. 5
    //     - When Rs. 15 or more is inserted, user can press 'D' to dispense
    //     - Any extra money is returned as change
    //     - User can press 'R' at any time to cancel and get a refund
    //
    //   States:
    //     0 = Idle        (no money inserted)
    //     1 = HasMoney    (some money inserted, not yet enough or not dispensed)
    //     2 = Dispensing  (drink dispensed, returning change)
    // ============================================================

    class VendingMachineFSM
    {
        private int currentState;  // 0=Idle, 1=HasMoney, 2=Dispensing
        private int balance;       // how much money has been inserted (in Rs.)

        private const int ITEM_PRICE = 15; // price of the drink in Rs.

        public VendingMachineFSM()
        {
            currentState = 0;
            balance = 0;
        }

        // Returns the name of the current state
        public string GetStateName()
        {
            if (currentState == 0) return "IDLE";
            if (currentState == 1) return "HAS MONEY (Rs. " + balance + " inserted)";
            return "DISPENSING";
        }

        // Shows a status line
        public void ShowStatus()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("  State   : " + GetStateName());
            Console.WriteLine("  Balance : Rs. " + balance);
            Console.WriteLine("  Price   : Rs. " + ITEM_PRICE);
            Console.ResetColor();
        }

        // The user inserts Rs. 5
        public void InsertCoin()
        {
            if (currentState == 2)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("  Machine is busy dispensing. Please wait.");
                Console.ResetColor();
                return;
            }

            balance += 5;
            currentState = 1; // Move to HasMoney state

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("  Rs. 5 inserted. Total balance: Rs. " + balance);
            Console.ResetColor();

            if (balance >= ITEM_PRICE)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("  You have enough credit! Press 'D' to get your drink.");
                Console.ResetColor();
            }
            else
            {
                int needed = ITEM_PRICE - balance;
                Console.WriteLine("  Need Rs. " + needed + " more to buy a drink.");
            }
        }

        // The user presses 'D' to dispense the drink
        public void DispenseDrink()
        {
            if (currentState == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("  No money inserted! Please insert coins first.");
                Console.ResetColor();
                return;
            }

            if (balance < ITEM_PRICE)
            {
                int needed = ITEM_PRICE - balance;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("  Not enough money. Need Rs. " + needed + " more.");
                Console.ResetColor();
                return;
            }

            // Enough money -> dispense
            currentState = 2;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("  *** DISPENSING YOUR COLD DRINK... ***");
            Console.WriteLine("  *   Enjoy your drink!               *");

            int change = balance - ITEM_PRICE;
            if (change > 0)
            {
                Console.WriteLine("  *   Change returned: Rs. " + change + "            *");
            }
            else
            {
                Console.WriteLine("  *   No change.                      *");
            }
            Console.ResetColor();

            // Reset the machine back to idle
            balance = 0;
            currentState = 0;
            Console.WriteLine("  Machine is now back to IDLE state.");
        }

        // The user presses 'R' to cancel and get a refund
        public void Refund()
        {
            if (currentState == 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("  No money to refund. Machine is already idle.");
                Console.ResetColor();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("  Cancelling... Refunding Rs. " + balance);
            Console.ResetColor();

            balance = 0;
            currentState = 0; // Back to Idle
            Console.WriteLine("  Machine reset to IDLE.");
        }
    }


    // ============================================================
    //   FSM 4: ELEVATOR SIMULATION
    //
    //   How it works:
    //     - Elevator starts at Floor 1 (ground floor)
    //     - Building has 4 floors (Floor 1 to Floor 4)
    //     - User presses a floor button to request that floor
    //     - Elevator moves one floor at a time (Up or Down)
    //     - When elevator reaches the requested floor, doors open
    //     - User must close doors before making a new request
    //
    //   States:
    //     0 = Idle        (stopped at a floor, doors closed)
    //     1 = MovingUp    (elevator going up toward target floor)
    //     2 = MovingDown  (elevator going down toward target floor)
    //     3 = DoorsOpen   (arrived at floor, doors are open)
    // ============================================================

    class ElevatorFSM
    {
        private int currentState;   // 0=Idle, 1=MovingUp, 2=MovingDown, 3=DoorsOpen
        private int currentFloor;   // which floor the elevator is currently on
        private int targetFloor;    // which floor the elevator is heading to

        private const int MIN_FLOOR = 1;
        private const int MAX_FLOOR = 4;

        public ElevatorFSM()
        {
            currentState = 0;   // Start Idle
            currentFloor = 1;   // Start at Floor 1
            targetFloor = 1;
        }

        // Returns the name of the current state
        public string GetStateName()
        {
            if (currentState == 0) return "IDLE";
            if (currentState == 1) return "MOVING UP";
            if (currentState == 2) return "MOVING DOWN";
            return "DOORS OPEN";
        }

        // Shows a status panel
        public void ShowStatus()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("  State         : " + GetStateName());
            Console.WriteLine("  Current Floor : " + currentFloor);
            if (currentState == 1 || currentState == 2)
                Console.WriteLine("  Target Floor  : " + targetFloor);
            Console.ResetColor();

            // Draw a simple floor diagram
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("  -- Building --");
            for (int f = MAX_FLOOR; f >= MIN_FLOOR; f--)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("  Floor " + f + " |");
                if (f == currentFloor)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    if (currentState == 3)
                        Console.Write(" [=OPEN=] ");   // doors open
                    else
                        Console.Write(" [ ELV ] ");    // elevator here
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write("         ");
                }
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("|");
            }
            Console.ResetColor();
        }

        // User presses a floor button
        public void RequestFloor(int floor)
        {
            // Cannot request while doors are open
            if (currentState == 3)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("  Doors are open! Please close doors first (press 'C').");
                Console.ResetColor();
                return;
            }

            // Validate floor number
            if (floor < MIN_FLOOR || floor > MAX_FLOOR)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("  Invalid floor! Please enter a floor between " + MIN_FLOOR + " and " + MAX_FLOOR + ".");
                Console.ResetColor();
                return;
            }

            // Already on that floor
            if (floor == currentFloor)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("  Elevator is already on Floor " + floor + "! Opening doors.");
                Console.ResetColor();
                currentState = 3; // Doors open
                return;
            }

            targetFloor = floor;

            // Decide direction
            if (targetFloor > currentFloor)
                currentState = 1; // MovingUp
            else
                currentState = 2; // MovingDown

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("  Request accepted. Heading to Floor " + targetFloor + "...");
            Console.ResetColor();

            // Move one floor at a time, printing each step
            while (currentFloor != targetFloor)
            {
                if (currentState == 1)
                    currentFloor++;    // go up
                else
                    currentFloor--;    // go down

                Console.ForegroundColor = ConsoleColor.DarkGray;

                if (currentFloor == targetFloor)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("  --> Arrived at Floor " + currentFloor + "!");
                }
                else
                {
                    Console.WriteLine("  --> Passing Floor " + currentFloor + "...");
                }
                Console.ResetColor();
            }

            // Reached target -> open doors
            currentState = 3;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("  *** DOORS OPENING on Floor " + currentFloor + " ***");
            Console.WriteLine("  Please enter / exit. Press 'C' to close doors.");
            Console.ResetColor();
        }

        // User presses 'C' to close doors
        public void CloseDoors()
        {
            if (currentState != 3)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("  Doors are already closed.");
                Console.ResetColor();
                return;
            }

            currentState = 0; // Back to Idle
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("  Doors closed. Elevator is IDLE on Floor " + currentFloor + ".");
            Console.ResetColor();
        }
    }


    // ============================================================
    //   MAIN PROGRAM  -  Menu System
    // ============================================================

    class Program
    {
        static void PrintHeader(string title)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("  ╔════════════════════════════════════════════════════╗");
            Console.WriteLine("  ║  " + title.PadRight(42) + "                        ║");
            Console.WriteLine("  ╚════════════════════════════════════════════════════╝");
            Console.ResetColor();
        }

        static void PrintLine()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("  ──────────────────────────────────────────");
            Console.ResetColor();
        }


        // ---- MENU 1: TRAFFIC LIGHT ----
        static void RunTrafficLight()
        {
            PrintHeader("FSM 1 - TRAFFIC LIGHT SIMULATION");

            Console.WriteLine();
            Console.WriteLine("  Cycle: RED -> GREEN -> YELLOW -> RED ...");
            Console.WriteLine("  Press ENTER to advance to next state.");
            Console.WriteLine("  Type 'q' + ENTER to go back to main menu.");
            Console.WriteLine();

            TrafficLightFSM light = new TrafficLightFSM();
            int step = 0;

            while (true)
            {
                PrintLine();
                Console.WriteLine("  Step " + step + ":");
                light.ShowState();
                Console.WriteLine();
                Console.Write("  Press ENTER for next state  (q to quit): ");

                string input = Console.ReadLine();
                if (input.ToLower() == "q") break;

                light.NextState();
                step++;
            }
        }


        // ---- MENU 2: STRING VALIDATOR ----
        static void RunStringValidator()
        {
            PrintHeader("FSM 2 - STRING PATTERN VALIDATOR");

            Console.WriteLine();
            Console.WriteLine("  Pattern accepted:  one 'a'  +  one or more 'b'  +  one 'c'");
            Console.WriteLine("  VALID:    abc, abbc, abbbc");
            Console.WriteLine("  INVALID:  ac, bc, ab, xyz, abcd");
            Console.WriteLine("  Type 'q' + ENTER to go back to main menu.");
            Console.WriteLine();

            StringValidatorFSM validator = new StringValidatorFSM();

            while (true)
            {
                PrintLine();
                Console.Write("  Enter a string to validate: ");
                string input = Console.ReadLine();

                if (input.ToLower() == "q") break;

                if (input.Length == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("  Please enter something.");
                    Console.ResetColor();
                    continue;
                }

                validator.ValidateWithTrace(input);
            }
        }


        // ---- MENU 3: VENDING MACHINE ----
        static void RunVendingMachine()
        {
            PrintHeader("FSM 3 - VENDING MACHINE SIMULATION");

            Console.WriteLine();
            Console.WriteLine("  Item available: Cold Drink  (Price: Rs. 15)");
            Console.WriteLine("  Coins accepted: Rs. 5 per insert");
            Console.WriteLine();
            Console.WriteLine("  Commands:");
            Console.WriteLine("    I  -->  Insert Rs. 5 coin");
            Console.WriteLine("    D  -->  Dispense drink");
            Console.WriteLine("    R  -->  Cancel and get refund");
            Console.WriteLine("    S  -->  Show current status");
            Console.WriteLine("    Q  -->  Back to main menu");
            Console.WriteLine();

            VendingMachineFSM machine = new VendingMachineFSM();

            while (true)
            {
                PrintLine();
                Console.Write("  Enter command (I / D / R / S / Q): ");
                string input = Console.ReadLine().ToUpper().Trim();

                if (input == "I")
                {
                    machine.InsertCoin();
                }
                else if (input == "D")
                {
                    machine.DispenseDrink();
                }
                else if (input == "R")
                {
                    machine.Refund();
                }
                else if (input == "S")
                {
                    Console.WriteLine();
                    machine.ShowStatus();
                }
                else if (input == "Q")
                {
                    Console.WriteLine("  Exiting Vending Machine...");
                    break;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("  Unknown command. Use I, D, R, S, or Q.");
                    Console.ResetColor();
                }

                Console.WriteLine();
            }
        }


        // ---- MENU 4: ELEVATOR ----
        static void RunElevator()
        {
            PrintHeader("FSM 4 - ELEVATOR SIMULATION");

            Console.WriteLine();
            Console.WriteLine("  Building has 4 floors  (Floor 1 = Ground)");
            Console.WriteLine("  Elevator starts at Floor 1.");
            Console.WriteLine();
            Console.WriteLine("  Commands:");
            Console.WriteLine("    1-4  -->  Request that floor");
            Console.WriteLine("    C    -->  Close doors");
            Console.WriteLine("    S    -->  Show current status");
            Console.WriteLine("    Q    -->  Back to main menu");
            Console.WriteLine();

            ElevatorFSM elevator = new ElevatorFSM();

            while (true)
            {
                PrintLine();
                Console.Write("  Enter command (1 / 2 / 3 / 4 / C / S / Q): ");
                string input = Console.ReadLine().ToUpper().Trim();

                if (input == "1" || input == "2" || input == "3" || input == "4")
                {
                    int floor = int.Parse(input);
                    Console.WriteLine();
                    elevator.RequestFloor(floor);
                }
                else if (input == "C")
                {
                    Console.WriteLine();
                    elevator.CloseDoors();
                }
                else if (input == "S")
                {
                    Console.WriteLine();
                    elevator.ShowStatus();
                }
                else if (input == "Q")
                {
                    Console.WriteLine("  Exiting Elevator Simulation...");
                    break;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("  Unknown command. Use 1, 2, 3, 4, C, S, or Q.");
                    Console.ResetColor();
                }

                Console.WriteLine();
            }
        }


        // ---- MAIN MENU ----
        static void Main(string[] args)
        {
            Console.Clear();
            PrintHeader("FSM PROJECT - Design & Analysis of Algorithms");

            Console.WriteLine();
            Console.WriteLine("  Course : Design & Analysis of Algorithms");
            Console.WriteLine("  Topic  : Finite State Machines (FSM)");
            Console.WriteLine();

            while (true)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("  ===== MAIN MENU =====");
                Console.ResetColor();
                Console.WriteLine("  [1] Traffic Light Simulation FSM");
                Console.WriteLine("  [2] String Pattern Validator FSM");
                Console.WriteLine("  [3] Vending Machine Simulation FSM");
                Console.WriteLine("  [4] Elevator Simulation FSM");
                Console.WriteLine("  [5] Exit");
                Console.WriteLine();
                Console.Write("  Enter your choice (1 / 2 / 3 / 4 / 5): ");

                string choice = Console.ReadLine();

                if (choice == "1") RunTrafficLight();
                else if (choice == "2") RunStringValidator();
                else if (choice == "3") RunVendingMachine();
                else if (choice == "4") RunElevator();
                else if (choice == "5")
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("\n  Thank you! Exiting program.");
                    Console.ResetColor();
                    break;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("  Invalid choice. Enter 1, 2, 3, 4, or 5.");
                    Console.ResetColor();
                }
            }
        }
    }
}