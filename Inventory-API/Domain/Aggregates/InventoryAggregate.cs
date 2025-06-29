using Domain.Entities.Inventory;
using Domain.Entities.Order;
using Domain.Events.Orders.Enum;
using Domain.Events.Orders;
using InventoryApi.Model.Events.Inventory;

namespace InventoryApi.Model.Action
{
    public class InventoryAggregate : Aggregate
    {
        public InventoryItem currentInventoryItem { get; set; } = new();

        public void AddInventoryItem(int eventVersion, InventoryItemIdentity inventoryItemIdentity, int Quantity)
        {
            if (Quantity < 0)
            {
                throw new InvalidOperationException("Quantity to remove must be a positive number.");
            }

            var inventoryItemAdded = new InventoryAdded(eventVersion, inventoryItemIdentity, Quantity);

            AddEvent(inventoryItemAdded);

            Apply(inventoryItemAdded);

            commit();
        }

        public void RemoveInventoryItem(int eventVersion, InventoryItemIdentity inventoryItemIdentity, int Quantity)
        {
            if(currentInventoryItem.Quantity < Quantity)
            {
                throw new InvalidOperationException("Insufficient inventory to remove the specified quantity.");
            }

            if(Quantity < 0)
            {
                throw new InvalidOperationException("Quantity to remove must be a positive number.");
            }

            var inventoryItemRemoved = new InventoryRemoved(eventVersion, inventoryItemIdentity, Quantity);

            AddEvent(inventoryItemRemoved);

            Apply(inventoryItemRemoved);

            commit();
        }


        public void Apply(InventoryAdded inventoryAdded)
        {
            currentInventoryItem.Quantity += inventoryAdded.Quantity;
        }

        public void Apply(InventoryRemoved inventoryRemoved)
        {
            currentInventoryItem.Quantity -= inventoryRemoved.Quantity;
        }
    }
}
