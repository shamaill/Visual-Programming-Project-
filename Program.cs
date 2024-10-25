using System;
using System.Collections.Generic;
using System.Timers;

public class Product
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }

    public Product(string name, decimal price, int quantity = 1)
    {
        Name = name;
        Price = price;
        Quantity = quantity;
    }

    public decimal TotalPrice()
    {
        return Price * Quantity;
    }
}

public class ShoppingCart
{
    private Dictionary<string, Product> cart;
    private Dictionary<string, Dictionary<string, Product>> categories;
    private Dictionary<string, int> categoryProductCount;
    private const decimal SalesTax = 0.08m;
    private decimal discount = 0m;
    private System.Timers.Timer cartExpirationTimer;
    private const int ExpirationTimeInMinutes = 5;

    public ShoppingCart()
    {
        cart = new Dictionary<string, Product>();
        categories = new Dictionary<string, Dictionary<string, Product>>();
        categoryProductCount = new Dictionary<string, int>();
        InitializeCategories();
        InitializeCartExpiration();
    }

    private void InitializeCategories()
    {
        categories.Add("electronics", new Dictionary<string, Product>()
        {
            { "Laptop", new Product("Laptop", 1000m) },
            { "Phone", new Product("Phone", 500m) },
            { "Monitor", new Product("Monitor", 150m) },
            { "Tablet", new Product("Tablet", 300m) },
            { "Camera", new Product("Camera", 600m) }
        });

        categories.Add("fashion", new Dictionary<string, Product>()
        {
            { "Shoes", new Product("Shoes", 80m) },
            { "T-shirt", new Product("T-shirt", 25m) },
            { "Jeans", new Product("Jeans", 50m) },
            { "Jacket", new Product("Jacket", 120m) },
            { "Hat", new Product("Hat", 15m) }
        });

        categories.Add("groceries", new Dictionary<string, Product>()
        {
            { "Milk", new Product("Milk", 3m) },
            { "Bread", new Product("Bread", 2m) },
            { "Eggs", new Product("Eggs", 5m) },
            { "Cheese", new Product("Cheese", 7m) },
            { "Butter", new Product("Butter", 4m) }
        });

        categories.Add("home appliances", new Dictionary<string, Product>()
        {
            { "Vacuum", new Product("Vacuum", 200m) },
            { "Blender", new Product("Blender", 50m) },
            { "Microwave", new Product("Microwave", 120m) },
            { "Oven", new Product("Oven", 400m) },
            { "Toaster", new Product("Toaster", 30m) }
        });

        categories.Add("books", new Dictionary<string, Product>()
        {
            { "Fiction", new Product("Fiction", 20m) },
            { "Non-Fiction", new Product("Non-Fiction", 25m) },
            { "Comics", new Product("Comics", 15m) },
            { "Biography", new Product("Biography", 30m) },
            { "Science", new Product("Science", 35m) }
        });

        categories.Add("sports", new Dictionary<string, Product>()
        {
            { "Soccer Ball", new Product("Soccer Ball", 25m) },
            { "Basketball", new Product("Basketball", 30m) },
            { "Tennis Racket", new Product("Tennis Racket", 60m) },
            { "Golf Clubs", new Product("Golf Clubs", 200m) },
            { "Yoga Mat", new Product("Yoga Mat", 20m) }
        });

        categories.Add("toys", new Dictionary<string, Product>()
        {
            { "Action Figure", new Product("Action Figure", 15m) },
            { "Doll", new Product("Doll", 20m) },
            { "Puzzle", new Product("Puzzle", 10m) },
            { "Board Game", new Product("Board Game", 25m) },
            { "Remote Car", new Product("Remote Car", 40m) }
        });

        categories.Add("automotive", new Dictionary<string, Product>()
        {
            { "Tire", new Product("Tire", 100m) },
            { "Engine Oil", new Product("Engine Oil", 30m) },
            { "Brake Pads", new Product("Brake Pads", 60m) },
            { "Car Battery", new Product("Car Battery", 120m) },
            { "Headlights", new Product("Headlights", 50m) }
        });

        categories.Add("beauty", new Dictionary<string, Product>()
        {
            { "Lipstick", new Product("Lipstick", 20m) },
            { "Foundation", new Product("Foundation", 30m) },
            { "Mascara", new Product("Mascara", 15m) },
            { "Perfume", new Product("Perfume", 60m) },
            { "Face Cream", new Product("Face Cream", 25m) }
        });

        categories.Add("stationery", new Dictionary<string, Product>()
        {
            { "Notebook", new Product("Notebook", 5m) },
            { "Pen", new Product("Pen", 1m) },
            { "Pencil", new Product("Pencil", 0.5m) },
            { "Eraser", new Product("Eraser", 0.3m) },
            { "Marker", new Product("Marker", 2m) }
        });

        foreach (var category in categories.Keys)
        {
            categoryProductCount[category] = 0;
        }
    }

    private void InitializeCartExpiration()
    {
        cartExpirationTimer = new System.Timers.Timer(ExpirationTimeInMinutes * 60 * 1000);
        cartExpirationTimer.Elapsed += OnCartExpired;
        cartExpirationTimer.Start();
    }

    private void OnCartExpired(object sender, ElapsedEventArgs e)
    {
        cart.Clear();
        cartExpirationTimer.Stop();
        Console.WriteLine("\nYour cart has expired due to inactivity and has been cleared.");
    }

    public void ResetCartExpirationTimer()
    {
        cartExpirationTimer.Stop();
        cartExpirationTimer.Start();
    }

    public void AddToCart(string productName, int quantity)
    {
        ResetCartExpirationTimer();

        foreach (var category in categories)
        {
            if (category.Value.TryGetValue(productName, out var product))
            {
                if (cart.ContainsKey(product.Name))
                {
                    cart[product.Name].Quantity += quantity;
                }
                else
                {
                    cart[product.Name] = new Product(product.Name, product.Price, quantity);
                }
                Console.WriteLine($"{quantity}x {product.Name} added to the cart.");

                categoryProductCount[category.Key] += quantity;
                ApplyCategoryDiscount(category.Key);
                RecommendProducts(category.Key);
                return;
            }
        }
        Console.WriteLine("Product not found. Please check the available products.");
    }

    private void ApplyCategoryDiscount(string category)
    {
        int count = categoryProductCount[category];

        if (count > 2 && count < 4)
        {
            discount = 0.10m;
            Console.WriteLine("\nA 10% discount has been applied because you purchased more than 2 products from the category: " + category);
        }
        else if (count >= 4)
        {
            discount = 0.25m;
            Console.WriteLine("\nA 25% discount has been applied because you purchased 4 or more products from the category: " + category);
        }
        else
        {
            discount = 0m;
        }
    }

    private void RecommendProducts(string category)
    {
        Console.WriteLine($"\nYou may also like these products from {category}:");
        foreach (var product in categories[category].Values)
        {
            if (!cart.ContainsKey(product.Name))
            {
                Console.WriteLine($"- {product.Name}: ${product.Price}");
            }
        }
    }

    public void RemoveFromCart(string productName)
    {
        ResetCartExpirationTimer();

        if (cart.ContainsKey(productName))
        {
            Product removedProduct = cart[productName];
            cart.Remove(productName);

            foreach (var category in categories)
            {
                if (category.Value.ContainsKey(productName))
                {
                    categoryProductCount[category.Key] -= removedProduct.Quantity;
                    ApplyCategoryDiscount(category.Key);
                }
            }
            Console.WriteLine($"{productName} removed from the cart.");
        }
        else
        {
            Console.WriteLine("Product not found in the cart.");
        }
    }

    public void ViewCart()
    {
        ResetCartExpirationTimer();

        if (cart.Count == 0)
        {
            Console.WriteLine("The cart is empty.");
            return;
        }

        Console.WriteLine("\nYour Cart:");
        foreach (var item in cart.Values)
        {
            Console.WriteLine($"{item.Name} - ${item.Price} x {item.Quantity} = ${item.TotalPrice()}");
        }
    }

    public void ViewCategories()
    {
        Console.WriteLine("\nAvailable Categories and Products:");

        foreach (var category in categories)
        {
            Console.WriteLine($"\n{category.Key.ToUpper()}:");
            foreach (var product in category.Value.Values)
            {
                Console.WriteLine($"- {product.Name}: ${product.Price}");
            }
        }
    }

    public decimal GetTotalCost()
    {
        decimal total = 0m;

        foreach (var product in cart.Values)
        {
            total += product.TotalPrice();
        }

        total -= total * discount;
        total += total * SalesTax;

        return total;
    }

    public void Checkout()
    {
        if (cart.Count == 0)
        {
            Console.WriteLine("Cart is empty. Add items before checking out.");
            return;
        }

        decimal totalCost = GetTotalCost();
        Console.WriteLine($"\nYour total cost including sales tax is: ${totalCost}");
        cart.Clear();
        cartExpirationTimer.Stop();
        Console.WriteLine("Thank you for your purchase! The cart has been cleared.");
    }
}

class Program
{
    static void Main(string[] args)
{
    ShoppingCart cart = new ShoppingCart();
    bool shopping = true;

    while (shopping)
    {
        Console.Clear();
        Console.WriteLine("\n=====================================");
        Console.WriteLine("     Welcome to the Shopping Cart    ");
        Console.WriteLine("=====================================");
        Console.WriteLine("Please select an option from the menu");
        Console.WriteLine("-------------------------------------");
        Console.WriteLine("  1. View available products         ");
        Console.WriteLine("  2. Add a product to the cart       ");
        Console.WriteLine("  3. Remove a product from the cart  ");
        Console.WriteLine("  4. View your cart                  ");
        Console.WriteLine("  5. Checkout and pay                ");
        Console.WriteLine("  6. Quit                            ");
        Console.WriteLine("-------------------------------------");
        Console.WriteLine("Enter your choice (1-6): ");
        Console.WriteLine("=====================================");

        string action = Console.ReadLine().ToLower();

        switch (action)
        {
            case "1":
                cart.ViewCategories();
                break;

            case "2":
                Console.WriteLine("Enter product name:");
                string productName = Console.ReadLine();
                Console.WriteLine("Enter product quantity:");
                int quantity = Convert.ToInt32(Console.ReadLine());
                cart.AddToCart(productName, quantity);
                break;

            case "3":
                Console.WriteLine("Enter product name to remove:");
                string removeProductName = Console.ReadLine();
                cart.RemoveFromCart(removeProductName);
                break;

            case "4":
                cart.ViewCart();
                break;

            case "5":
                cart.Checkout();
                shopping = false;
                continue;

            case "6":
                shopping = false;
                Console.WriteLine("Thank you for visiting. Goodbye!");
                continue;

            default:
                Console.WriteLine("Invalid action. Please choose again.");
                break;
        }

        // Prompt user to press any key to return to the main menu
        Console.WriteLine("\nPress any key to return to the main menu...");
        Console.ReadKey();
    }
}

}
