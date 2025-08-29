using System;
using UnityEngine;
using PCG;

    public static class OrderGenerator
    {
        public static OrderNode GenerateTray(int difficulty)
        {
            int headCount = ProceduralRNG.Range(1, 5);  //CHANGE TO PARAMETER REMOVE
            bool largeBowlUnlocked = true; //CHANGE TO PARAMATER AND REMOVE
            int traySlots = 5; //CHANGE TO PARAMATER AND REMOVE
            int dishCount = Math.Clamp(headCount, 0, 3);
            int largeDishCount = 0;


            //Add Root Tray Node
            var root = new OrderNode {
                id = "TRAY",
                weight = 100,
            };


            // Dish Generation Rules:
            // Ensures that each customer head recieves a bowl;
            // Make sure that largeDishCount scales with difficulty;
            // Make sure that if there are two heads, it can either be one large bowl or two small/large bowls, the former for easier difficulty
            // 3 Difficulty Level 
            // Decide if its 3 or 5 people cause????
            // Base Tray = 3 slots, Upgraded Tray = 5 slots
            // Regular Dish = 1 slot, Large Dish = 2 slots

            if (headCount == 1)
            {
                if (largeBowlUnlocked && ProceduralRNG.Bool(0.1f + difficulty * 0.3f))
                {
                    largeDishCount = 1;
                }
                else
                {
                    largeDishCount = 0;
                }
            }
            else if (headCount == 2)    //could prolly use improvement
            {
                float sharedChance = 0.1f + difficulty * 0.2f; // increases with difficulty
                float twoLargeChance = 0.1f + difficulty * 0.3f;

                if (largeBowlUnlocked && ProceduralRNG.Bool(sharedChance))
                {
                    largeDishCount = 1;
                }
                else if (largeBowlUnlocked && ProceduralRNG.Bool(twoLargeChance) && traySlots >= 4)
                {
                    largeDishCount = 2;
                }
                else
                {
                    largeDishCount = 0;
                }
            }
            else if (headCount == 3)
            {
                float largeChance = 0.1f + difficulty * 0.3f;

                for (int i = 0; i < dishCount; i++)
                {
                    if (!largeBowlUnlocked) break;
                    if (traySlots > 3 && largeDishCount<2)
                    {
                        if (ProceduralRNG.Bool(largeChance))
                        {
                            largeDishCount++;
                        }
                    }
                    else
                    {
                        largeDishCount++;
                        break;
                    }
                }
            }
            else if (headCount>=4)
            {
                largeDishCount = 2;
            }

            //largeDishCount = 2;
            //for (int i = 0; i < dishCount; i++)
            //{
            //    if (largeDishCount < 2 && // cap at 2 large dishes
            //        largeBowlUnlocked &&
            //        ProceduralRNG.Bool(difficulty * 0.25f))
            //    {
            //        largeDishCount++;
            //    }
            //}

            if (Debug.isDebugBuild) Debug.Log("Dish Count for Tray:" + dishCount);
            if (Debug.isDebugBuild) Debug.Log("Large Dish Count for Tray:" + largeDishCount);

            int remainingLarge = largeDishCount;
            int totalSlotsUsed = 0;

            for (int i = 0; i < dishCount; i++)
            {
                bool shouldMakeLarge = remainingLarge > 0;

                if (shouldMakeLarge && totalSlotsUsed + 2 <= traySlots)
                {
                    root.children.Add(GenerateDish(difficulty, isLarge: true));
                    totalSlotsUsed += 2;
                    remainingLarge--;
                }
                else if (totalSlotsUsed + 1 <= traySlots)
                {
                    root.children.Add(GenerateDish(difficulty, isLarge: false));
                    totalSlotsUsed += 1;
                }
                else
                {
                    // No space left — skip or log warning
                    Debug.LogWarning("No slot available for dish " + i);
                }
            }

            // Add Beverage
            int beverageCount = ProceduralRNG.Range(1, 2);
            for (int i = 0; i <= beverageCount; i++)
            {
                root.children.Add(GenerateBeverage());
            }

            // Add Seasoning
            for (int i = 0; i <= beverageCount; i++)
            {
                root.children.Add(GenerateSeasoning());
            }

            if (Debug.isDebugBuild) Debug.Log("Created Tray: " + root.id);
            return root;
        }

        public static OrderNode GenerateDish(int difficulty, bool isLarge)
        {
            var dish = new DishNode("DISH_");
            dish.isLarge = isLarge;
            dish.id = isLarge ? "DISH_LARGE" : "DISH_REGULAR";
            dish.weight = isLarge ? 40 : 20;

            dish.children.Add(GeneratePot(isLarge));
            dish.children.Add(GeneratePan(isLarge));
            dish.children.Add(GenerateToppings(isLarge ? 4 : 2, difficulty));

            return dish;
        }

        public static OrderNode GeneratePot(bool isLarge)
        {
            var pot = new OrderNode() { id = "POT" };

            pot.children.Add(new BoilNode("BOIL"));

            pot.children.Add(new BonesNode("BONES")
            {
                id = "BONES",
            });

            return pot;
        }

        public static OrderNode GeneratePan(bool isLarge)
        {
            var pan = new OrderNode() { id = "PAN" };
            int ingredientCount = isLarge ? 7 : 3;

            for (int i = 0; i < ingredientCount; i++)
            {
                pan.children.Add(new ToppingNode($"INGREDIENT_{i}"));
            }

            return pan;
        }

        public static OrderNode GenerateToppings(int count, int difficulty)
        {
            var toppingsSection = new ToppingSectionNode("TOPPINGS_SECTION");

            string[] allToppings = {
                            "Pork Belly", "Egg", "Green Onion", "Corn", "Spicy Mayo", "Mushrooms"
                        };

            for (int i = 0; i < count; i++)
            {
                string name = allToppings[ProceduralRNG.Range(0, allToppings.Length)];
                int expectedCount = ProceduralRNG.Range(1, 3);

                toppingsSection.children.Add(new ToppingNode($"TOPPING_{i}")
                
           
                );
            }

            return toppingsSection;
        }

        private static string GetRandomIngredient()
        {
            string[] ingredients = { "Noodles", "Udon", "Soba", "Rice" };
            return ingredients[ProceduralRNG.Range(0, ingredients.Length)];
        }

        private static OrderNode GenerateBeverage()
        {
            return new BeverageNode
            {
                id = "BEVERAGE",
                weight = 10,
                size = 1
            };
        }

        private static OrderNode GenerateSeasoning()
        {
            return new SeasoningTrayNode
            {
                id = "SEASONING",
                weight = 10,
                trayCount = 1,  //edit to make it the number of people.
            };
        }

        }



