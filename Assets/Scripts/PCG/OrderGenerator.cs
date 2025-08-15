using JetBrains.Annotations;
using System;
using Unity.VisualScripting;

namespace PCG
{
    public static class OrderGenerator
    {
        public static OrderNode GenerateTray(int difficulty)
        {
            //Tray has complete weight of 100.
            var root = new TrayRootNode();
            root.weight = 100;

            // Decide how many dishes to make
            bool wantsLargeDish = ProceduralRNG.Bool(0.4f + difficulty * 0.05f);

            if (wantsLargeDish)
            {
                root.children.Add(GenerateDish(difficulty, isLarge: true));
            }
            else
            {
                root.children.Add(GenerateDish(difficulty, isLarge: false));
            }

            // Add beverage and seasoning (optional)
            if (ProceduralRNG.Bool(0.6f))
            {
                root.children.Add(GenerateBeverage());
            }
            root.children.Add(GenerateSeasoning());

            return root;
        }

       public static OrderNode GenerateDish(int difficulty, bool isLarge)
        {
            var dish = new DishSectionNode();
            dish.id = isLarge ? "DISH_LARGE" : "DISH_REGULAR";
            dish.weight = isLarge ? 40 : 20;

            dish.children.Add(GeneratePot(isLarge));
            dish.children.Add(GeneratePan(isLarge));
            dish.children.Add(GenerateToppings(isLarge ? 4 : 2, difficulty));

            return dish;
        }

        public static OrderNode GeneratePot(bool large)
        {
            var pot = new PotNode() { id = "POT", weight = large ? 12 : 6 };

            pot.children.Add(new IntSubStep
            {
                id = "BOIL_TIME",
                name = "BoilTime",
                expectedValue = ProceduralRNG.Range(20, 41),
                weight = 5
            });

            pot.children.Add(new BooleanSubStep
            {
                id = "BONES",
                name = "Bones",
                expectedValue = ProceduralRNG.Bool(0.7f),
                weight = 4
            });

            return pot;
        }

        public static OrderNode GeneratePan(bool large)
        {
            var pan = new PanNode() { id = "PAN", weight = large ? 14 : 7 };
            int ingredientCount = large ? 7 : 3;

            for (int i = 0; i < ingredientCount; i++)
            {
                pan.children.Add(new ToppingNode
                {
                    id = $"INGREDIENT_{i}",
                    toppingName = GetRandomIngredient(),
                    expectedCount = 1,
                    weight = 2f
                });
            }

            return pan;
        }

        public static OrderNode GenerateToppings(int count, int difficulty)
        {
            var toppingsSection = new ToppingsSectionNode()
            {
                id = "TOPPINGS_SECTION",
                weight = 12 // was using isLarge — now fixed
            };

            string[] allToppings = {
                "Pork Belly", "Egg", "Green Onion", "Corn", "Spicy Mayo", "Mushrooms"
            };

            for (int i = 0; i < count; i++)
            {
                string name = allToppings[ProceduralRNG.Range(0, allToppings.Length)];
                int expectedCount = ProceduralRNG.Range(1, 3);

                toppingsSection.children.Add(new ToppingNode
                {
                    id = $"TOPPING_{i}",
                    toppingName = name,
                    expectedCount = expectedCount,
                    weight = 2f
                });
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
                drinkName = ProceduralRNG.Bool(0.5f) ? "Tea" : "Calamansi"
            };
        }

        private static OrderNode GenerateSeasoning()
        {
            return new SeasoningNode
            {
                id = "SEASONING",
                weight = 10,
                seasoningType = ProceduralRNG.Bool(0.5f) ? "Salt" : "Spice"
            };
        }
    }
}