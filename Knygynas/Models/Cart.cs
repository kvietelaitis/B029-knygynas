using System.Collections.Generic;
using System.Linq;

namespace Knygynas.Models;

public class Cart
{
    public List<CartItem> Items { get; set; } = new List<CartItem>();

    public double TotalPrice => Items.Sum(i => i.Price * i.Quantity);

    public void AddItem(Book book, int quantity)
    {
        var item = Items.FirstOrDefault(i => i.ISBN == book.ISBN);
        if (item == null)
        {
            Items.Add(new CartItem
            {
                ISBN = book.ISBN,
                Title = book.Title,
                Price = book.Price,
                Quantity = quantity
            });
        }
        else
        {
            item.Quantity += quantity;
        }
    }

    public void RemoveItem(string isbn)
    {
        Items.RemoveAll(i => i.ISBN == isbn);
    }

    public void UpdateQuantity(string isbn, int quantity)
    {
        var item = Items.FirstOrDefault(i => i.ISBN == isbn);
        if (item != null)
        {
            item.Quantity = quantity;
            if (item.Quantity <= 0)
            {
                Items.Remove(item);
            }
        }
    }

    public void Clear()
    {
        Items.Clear();
    }
}
